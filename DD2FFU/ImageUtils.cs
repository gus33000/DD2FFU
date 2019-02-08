using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using DD2FFU.Streams;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Decomp.Microsoft.WindowsPhone.Imaging;
using DiscUtils;
using DiscUtils.Containers;
using DiscUtils.Raw;
using DiscUtils.Streams;

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
            var handle = IntPtr.Zero;

            // open disk handle
            var openParameters = new NativeMethods.OPEN_VIRTUAL_DISK_PARAMETERS();
            openParameters.Version = NativeMethods.OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1;
            openParameters.Version1.RWDepth = NativeMethods.OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

            var openStorageType = new NativeMethods.VIRTUAL_STORAGE_TYPE();
            openStorageType.DeviceId = NativeMethods.VIRTUAL_STORAGE_TYPE_DEVICE_VHD;
            openStorageType.VendorId = NativeMethods.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;

            var openResult = NativeMethods.OpenVirtualDisk(ref openStorageType, vhdfile,
                NativeMethods.VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL,
                NativeMethods.OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE, ref openParameters, ref handle);
            if (openResult != NativeMethods.ERROR_SUCCESS)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Native error {0}.",
                    openResult));

            // attach disk - permanently
            var attachParameters = new NativeMethods.ATTACH_VIRTUAL_DISK_PARAMETERS();
            attachParameters.Version = NativeMethods.ATTACH_VIRTUAL_DISK_VERSION.ATTACH_VIRTUAL_DISK_VERSION_1;
            var attachResult = NativeMethods.AttachVirtualDisk(handle, IntPtr.Zero,
                NativeMethods.ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME, 0,
                ref attachParameters, IntPtr.Zero);
            if (attachResult != NativeMethods.ERROR_SUCCESS)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Native error {0}.",
                    attachResult));

            // close handle to disk
            NativeMethods.CloseHandle(handle);

            return GetDriveId(vhdfile);
        }

        private static string GetDriveId(string vhdpath)
        {
            var scope = new ManagementScope(@"\\localhost\ROOT\Microsoft\Windows\Storage");
            var query = new ObjectQuery("SELECT * FROM MSFT_PhysicalDisk");
            var searcher = new ManagementObjectSearcher(scope, query);
            var allPDisks = searcher.Get();
            foreach (ManagementObject onePDisk in allPDisks)
                // Show physical disk information

                if (onePDisk["PhysicalLocation"].ToString().ToLower() == vhdpath.ToLower())
                    return @"\\.\PHYSICALDRIVE" + onePDisk["DeviceId"];

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
            var iulogger = new IULogger();

            /*iulogger.InformationLogger = NullLog;
            iulogger.ErrorLogger = NullLog;
            iulogger.DebugLogger = NullLog;*/

            iulogger.DebugLogger = iulogger.InformationLogger;

            var imageStorageManager = new ImageStorageManager(iulogger);
            var imageStorage = imageStorageManager.AttachToMountedVirtualHardDisk(physicalDiskPath, false, true);

            var fullFlashUpdateImage = imageStorageManager.CreateFullFlashObjectFromAttachedImage(imageStorage);
            imageStorageManager.VirtualHardDiskSectorSize = fullFlashUpdateImage.Stores[0].SectorSize;
            
            fullFlashUpdateImage.OSVersion = osversion;
            fullFlashUpdateImage.AntiTheftVersion = antitheftversion;

            var payloadWrapper = GetPayloadWrapper(fullFlashUpdateImage, imagepath);

            imageStorageManager.DismountFullFlashImage(true, payloadWrapper, false, 1u);
            fullFlashUpdateImage = null;
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
                strm = new ReadFromDevice.DeviceStream(ddfile);//DeviceFileStream.DeviceFileStream(ddfile);
            else
                strm = new FileStream(ddfile, FileMode.Open);

            Stream fstream;
            if (!Recovery)
                fstream = new EPartitionStream(strm, partitions);
            else
                fstream = strm;

            using (var inDisk = new Disk(fstream, Ownership.Dispose))
            {
                var diskParams = inDisk.Parameters;

                using (var outDisk = VirtualDisk.CreateDisk("VHD", "dynamic", vhdfile, diskParams, "", ""))
                {
                    var contentStream = inDisk.Content;

                    var pump = new StreamPump
                    {
                        InputStream = contentStream,
                        OutputStream = outDisk.Content,
                        SparseCopy = true,
                        SparseChunkSize = SectorSize,
                        BufferSize = SectorSize * 1024
                    };
                    
                    var totalBytes = contentStream.Length;

                    var now = DateTime.Now;
                    pump.ProgressEvent += (o, e) => { ShowProgress(totalBytes, now, o, e); };

                    Logging.Log("Converting RAW to VHD");
                    pump.Run();
                    Console.WriteLine();
                }
            }
        }

        public static void MountDiskId(string diskid, string partid, string driveletter)
        {
            var commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nassign letter=" +
                           driveletter.Substring(0, 1);
            File.WriteAllText("diskpartcommands.txt", commands);

            var process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static void UnMountDiskId(string diskid, string partid, string driveletter)
        {
            var commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nremove letter=" +
                           driveletter.Substring(0, 1);
            File.WriteAllText("diskpartcommands.txt", commands);

            var process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

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
            var outputWrapper = new OutputWrapper(imagePath);
            IPayloadWrapper innerWrapper = outputWrapper;
            var innerWrapper2 = new SecurityWrapper(image, innerWrapper);
            return new ManifestWrapper(image, innerWrapper2);
        }

        protected static void ShowProgress(long totalBytes, DateTime startTime, object sourceObject,
            PumpProgressEventArgs e)
        {
            var now = DateTime.Now;
            var timeSoFar = now - startTime;

            var remaining =
                TimeSpan.FromMilliseconds(timeSoFar.TotalMilliseconds / e.BytesRead * (totalBytes - e.BytesRead));

            var speed = Math.Round(e.SourcePosition / 1024L / 1024L / timeSoFar.TotalSeconds);

            Logging.Log(
                string.Format("{0} {1}MB/s {2:hh\\:mm\\:ss\\.f}", GetDismLikeProgBar((int)(e.BytesRead * 100 / totalBytes)), speed.ToString(),
                    remaining, remaining.TotalHours, remaining.Minutes, remaining.Seconds, remaining.Milliseconds),
                returnline: false);

        }

        private static string GetDismLikeProgBar(int perc)
        {
            var eqsLength = (int) ((double) perc / 100 * 55);
            var bases = new string('=', eqsLength) + new string(' ', 55 - eqsLength);
            bases = bases.Insert(28, perc + "%");
            if (perc == 100)
                bases = bases.Substring(1);
            else if (perc < 10)
                bases = bases.Insert(28, " ");
            return "[" + bases + "]";
        }

        public static void SetDiskType(string diskid, string partid, string type)
        {
            var commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nset id=" + type;
            File.WriteAllText("diskpartcommands.txt", commands);

            var process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static void ShrinkPartition(string diskid, string partid, string desired)
        {
            var commands = "SELECT DISK " + diskid.Last() + "\nSELECT PART " + partid + "\nshrink desired=" + desired;
            File.WriteAllText("diskpartcommands.txt", commands);

            var process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.Arguments = "/s diskpartcommands.txt";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;

            process.Start();
            process.WaitForExit();

            File.Delete("diskpartcommands.txt");
        }

        public static List<GPTPartition> GetPartsFromGPT(string PhysicalDiskId)
        {
            var GPTSignature = "EFI PART";
            byte[] partitionArray = null;
            DeviceFileStream.DeviceFileStream ds;
            try
            {
                ds = new DeviceFileStream.DeviceFileStream(PhysicalDiskId);
            }
            catch (Exception ex)
            {
                Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                Logging.Log("Failed to open disk " + PhysicalDiskId.Last(), Logging.LoggingLevel.Error);
                throw new Exception("Failed to open disk " + PhysicalDiskId.Last());
            }

            var sector = new byte[Constants.SectorSize]; // 512d, regular sector size
            var read = ds.Read(sector, 0, sector.Length);
            if (read == sector.Length && Encoding.ASCII.GetString(sector, 0, 8) != GPTSignature)
                read = ds.Read(sector, 0, sector.Length);
            if (read == sector.Length && Encoding.ASCII.GetString(sector, 0, 8) == GPTSignature)
            {
                var partitionSlotCount = BitConverter.ToUInt32(sector, 0x50); // partition count from header
                var partitionSlotSize = BitConverter.ToUInt32(sector, 0x54); // partition size from header
                var bytesToRead = (int) Math.Round(partitionSlotCount * partitionSlotSize / (double) sector.Length,
                                      MidpointRounding.AwayFromZero) * sector.Length;
                partitionArray = new byte[bytesToRead];
                ds.Read(partitionArray, 0, partitionArray.Length);
            }

            ds.Dispose();

            if (partitionArray == null)
            {
                Logging.Log("Failed to read partition array", Logging.LoggingLevel.Error);
                throw new Exception("Failed to read partition array");
            }

            var partitionarray = new List<GPTPartition>();

            using (var br = new BinaryReader(new MemoryStream(partitionArray)))
            {
                Logging.Log("Partitions on disk " + PhysicalDiskId.Last() + ":");
                var name = new byte[72]; // fixed name size
                var iterator = 0;
                while (true)
                {
                    var type = new Guid(br.ReadBytes(16));
                    if (type == Guid.Empty)
                        break;
                    br.BaseStream.Seek(0x28, SeekOrigin.Current); // 40d, offset to name in partition entry after type
                    name = br.ReadBytes(name.Length);
                    iterator++;
                    Logging.Log($"({type}) [{iterator}] {Encoding.Unicode.GetString(name).TrimEnd('\0')}");

                    partitionarray.Add(new GPTPartition
                        {Type = type.ToString(), id = iterator, Name = Encoding.Unicode.GetString(name).TrimEnd('\0')});
                }
            }

            return partitionarray;
        }

        internal class GPTPartition
        {
            public string Type { get; set; }

            public int id { get; set; }

            public string Name { get; set; }
        }
    }
}