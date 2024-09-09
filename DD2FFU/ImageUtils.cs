using DD2FFU.Streams;
using DiscUtils;
using DiscUtils.Containers;
using DiscUtils.Raw;
using DiscUtils.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Decomp.Microsoft.WindowsPhone.Imaging;

namespace DD2FFU
{
    internal class ImageUtils
    {
        /// <summary>
        ///     Mounts FFU or VHD files on a target system.
        /// </summary>
        /// <param name="vhdfile">A path as a string to a FFU or VHD file to mount on the current system.</param>
        /// <param name="flag">If set to true, the function will mount a FFU image and not a VHD image.</param>
        /// <returns>
        ///     A string array containing for the first parameter the full path to the junction linked to the image, and for
        ///     the second parameter the Physical disk id path.
        /// </returns>
        public static string MountVHD(string vhdfile)
        {
            nint handle = IntPtr.Zero;

            // open disk handle
            NativeMethods.OPEN_VIRTUAL_DISK_PARAMETERS openParameters = new()
            {
                Version = NativeMethods.OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1
            };
            openParameters.Version1.RWDepth = NativeMethods.OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

            NativeMethods.VIRTUAL_STORAGE_TYPE openStorageType = new()
            {
                DeviceId = NativeMethods.VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
                VendorId = NativeMethods.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
            };

            int openResult = NativeMethods.OpenVirtualDisk(ref openStorageType, vhdfile,
                NativeMethods.VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL,
                NativeMethods.OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE, ref openParameters, ref handle);
            if (openResult != NativeMethods.ERROR_SUCCESS)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Native error {0}.",
                    openResult));
            }

            // attach disk - permanently
            NativeMethods.ATTACH_VIRTUAL_DISK_PARAMETERS attachParameters = new()
            {
                Version = NativeMethods.ATTACH_VIRTUAL_DISK_VERSION.ATTACH_VIRTUAL_DISK_VERSION_1
            };
            int attachResult = NativeMethods.AttachVirtualDisk(handle, IntPtr.Zero,
                NativeMethods.ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME, 0,
                ref attachParameters, IntPtr.Zero);
            if (attachResult != NativeMethods.ERROR_SUCCESS)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Native error {0}.",
                    attachResult));
            }

            // close handle to disk
            _ = NativeMethods.CloseHandle(handle);

            return GetDriveId(vhdfile);
        }

        private static string GetDriveId(string vhdpath)
        {
            ManagementScope scope = new(@"\\localhost\ROOT\Microsoft\Windows\Storage");
            ObjectQuery query = new("SELECT * FROM MSFT_PhysicalDisk");
            ManagementObjectSearcher searcher = new(scope, query);
            ManagementObjectCollection allPDisks = searcher.Get();
            foreach (ManagementObject onePDisk in allPDisks)
            {
                // Show physical disk information
                Console.WriteLine("====");
                Console.WriteLine(onePDisk["PhysicalLocation"].ToString());
                Console.WriteLine(vhdpath.ToString());
                Console.WriteLine("====");

                if (onePDisk["PhysicalLocation"].ToString().Equals(vhdpath, StringComparison.CurrentCultureIgnoreCase))
                {
                    return $@"\\.\PHYSICALDRIVE{onePDisk["DeviceId"]}";
                }
            }

            throw new Exception();
        }

        /// <summary>
        ///     Commits a FFU image from a mounted VHD on a target system.
        /// </summary>
        /// <param name="physicalDiskPath">The physical disk path of a mounted VHD file.</param>
        /// <param name="imagepath">The path to the output FFU file.</param>
        /// <param name="antitheftversion">The antitheft version to use for the FFU image.</param>
        /// <param name="osversion">The OS version to use for the FFU file metadata.</param>
        /// <returns></returns>
        public static void CommitFFU(string physicalDiskPath, string imagepath, string antitheftversion,
            string osversion)
        {
            IULogger iulogger = new();

            /*iulogger.InformationLogger = NullLog;
            iulogger.ErrorLogger = NullLog;
            iulogger.DebugLogger = NullLog;*/

            iulogger.DebugLogger = iulogger.InformationLogger;

            ImageStorageManager imageStorageManager = new(iulogger);
            ImageStorage imageStorage = imageStorageManager.AttachToMountedVirtualHardDisk(physicalDiskPath, false, true);

            FullFlashUpdateImage fullFlashUpdateImage = imageStorageManager.CreateFullFlashObjectFromAttachedImage(imageStorage);
            imageStorageManager.VirtualHardDiskSectorSize = fullFlashUpdateImage.Stores[0].SectorSize;

            fullFlashUpdateImage.OSVersion = osversion;
            fullFlashUpdateImage.AntiTheftVersion = antitheftversion;

            IPayloadWrapper payloadWrapper = GetPayloadWrapper(fullFlashUpdateImage, imagepath);

            imageStorageManager.DismountFullFlashImage(true, payloadWrapper, false, 1u);
        }

        /// <summary>
        ///     Coverts a raw DD image into a VHD file suitable for FFU imaging.
        /// </summary>
        /// <param name="ddfile">The path to the DD file.</param>
        /// <param name="vhdfile">The path to the output VHD file.</param>
        /// <returns></returns>
        public static void ConvertDD2VHD(string ddfile, string vhdfile, string[] partitions, bool Recovery, int SectorSize)
        {
            SetupHelper.SetupContainers();
            Stream strm;

            if (ddfile.ToLower().Contains(@"\\.\physicaldrive"))
            {
                strm = new ReadFromDevice.DeviceStream(ddfile);//DeviceFileStream.DeviceFileStream(ddfile);
            }
            else
            {
                strm = new FileStream(ddfile, FileMode.Open);
            }

            Stream fstream = !Recovery ? new EPartitionStream(strm, partitions) : strm;

            using Disk inDisk = new(fstream, Ownership.Dispose);
            VirtualDiskParameters diskParams = inDisk.Parameters;

            using VirtualDisk outDisk = VirtualDisk.CreateDisk("VHD", "dynamic", vhdfile, diskParams, "", "");
            SparseStream contentStream = inDisk.Content;

            StreamPump pump = new()
            {
                InputStream = contentStream,
                OutputStream = outDisk.Content,
                SparseCopy = true,
                SparseChunkSize = SectorSize,
                BufferSize = SectorSize * 1024
            };

            long totalBytes = contentStream.Length;

            DateTime now = DateTime.Now;
            pump.ProgressEvent += (o, e) => { ShowProgress(totalBytes, now, o, e); };

            Logging.Log("Converting RAW to VHD");
            pump.Run();
            Console.WriteLine();
        }

        public static void MountDiskId(string diskid, string partid, string driveletter)
        {
            string commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nassign letter=" +
                           driveletter[..1];
            File.WriteAllText("diskpartcommands.txt", commands);

            Process process = new();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            _ = process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static void UnMountDiskId(string diskid, string partid, string driveletter)
        {
            string commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nremove letter=" +
                           driveletter[..1];
            File.WriteAllText("diskpartcommands.txt", commands);

            Process process = new();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            _ = process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        protected static void NullLog(string unused, object[] notused)
        {
        }

        protected static IPayloadWrapper GetPayloadWrapper(FullFlashUpdateImage image, string imagePath)
        {
            OutputWrapper outputWrapper = new(imagePath);
            IPayloadWrapper innerWrapper = outputWrapper;
            SecurityWrapper innerWrapper2 = new(image, innerWrapper);
            return new ManifestWrapper(image, innerWrapper2);
        }

        protected static void ShowProgress(long totalBytes, DateTime startTime, object sourceObject,
            PumpProgressEventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSoFar = now - startTime;

            TimeSpan remaining =
                TimeSpan.FromMilliseconds(timeSoFar.TotalMilliseconds / e.BytesRead * (totalBytes - e.BytesRead));

            double speed = Math.Round(e.SourcePosition / 1024L / 1024L / timeSoFar.TotalSeconds);

            Logging.Log(
                string.Format("{0} {1}MB/s {2:hh\\:mm\\:ss\\.f}", GetDismLikeProgBar((int)(e.BytesRead * 100 / totalBytes)), speed.ToString(),
                    remaining, remaining.TotalHours, remaining.Minutes, remaining.Seconds, remaining.Milliseconds),
                returnline: false);

        }

        private static string GetDismLikeProgBar(int perc)
        {
            int eqsLength = (int)((double)perc / 100 * 55);
            string bases = new string('=', eqsLength) + new string(' ', 55 - eqsLength);
            bases = bases.Insert(28, perc + "%");
            if (perc == 100)
            {
                bases = bases[1..];
            }
            else if (perc < 10)
            {
                bases = bases.Insert(28, " ");
            }

            return "[" + bases + "]";
        }

        public static void SetDiskType(string diskid, string partid, string type)
        {
            string commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nset id=" + type;
            File.WriteAllText("diskpartcommands.txt", commands);

            Process process = new();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            _ = process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static void ShrinkPartition(string diskid, string partid, string desired)
        {
            string commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nshrink desired=" + desired;
            File.WriteAllText("diskpartcommands.txt", commands);

            Process process = new();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;

            _ = process.Start();
            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static List<GPTPartition> GetPartsFromGPT(string PhysicalDiskId)
        {
            string GPTSignature = "EFI PART";
            byte[] partitionArray = null;
            DeviceFileStream.DeviceFileStream ds;
            try
            {
                ds = new DeviceFileStream.DeviceFileStream(PhysicalDiskId);
            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), LoggingLevel.Error);
                Logging.Log("Failed to open disk " + PhysicalDiskId.Last(), LoggingLevel.Error);
                throw new Exception("Failed to open disk " + PhysicalDiskId.Last());
            }

            byte[] sector = new byte[Constants.SectorSize]; // 512d, regular sector size
            int read = ds.Read(sector, 0, sector.Length);
            if (read == sector.Length && Encoding.ASCII.GetString(sector, 0, 8) != GPTSignature)
            {
                read = ds.Read(sector, 0, sector.Length);
            }

            if (read == sector.Length && Encoding.ASCII.GetString(sector, 0, 8) == GPTSignature)
            {
                uint partitionSlotCount = BitConverter.ToUInt32(sector, 0x50); // partition count from header
                uint partitionSlotSize = BitConverter.ToUInt32(sector, 0x54); // partition size from header
                int bytesToRead = (int)Math.Round(partitionSlotCount * partitionSlotSize / (double)sector.Length,
                                      MidpointRounding.AwayFromZero) * sector.Length;
                partitionArray = new byte[bytesToRead];
                _ = ds.Read(partitionArray, 0, partitionArray.Length);
            }

            ds.Dispose();

            if (partitionArray == null)
            {
                Logging.Log("Failed to read partition array", LoggingLevel.Error);
                throw new Exception("Failed to read partition array");
            }

            List<GPTPartition> partitionarray = [];

            using (BinaryReader br = new(new MemoryStream(partitionArray)))
            {
                Logging.Log("Partitions on disk " + PhysicalDiskId.Last() + ":");
                byte[] name = new byte[72]; // fixed name size
                int iterator = 0;
                while (true)
                {
                    Guid type = new(br.ReadBytes(16));
                    if (type == Guid.Empty)
                    {
                        break;
                    }

                    _ = br.BaseStream.Seek(0x28, SeekOrigin.Current); // 40d, offset to name in partition entry after type
                    name = br.ReadBytes(name.Length);
                    iterator++;
                    Logging.Log($"({type}) [{iterator}] {Encoding.Unicode.GetString(name).TrimEnd('\0')}");

                    partitionarray.Add(new GPTPartition
                    {
                        Type = type.ToString(),
                        id = iterator,
                        Name = Encoding.Unicode.GetString(name).TrimEnd('\0')
                    });
                }
            }

            return partitionarray;
        }

        internal class GPTPartition
        {
            public string Type
            {
                get; set;
            }

            public int id
            {
                get; set;
            }

            public string Name
            {
                get; set;
            }
        }
    }
}