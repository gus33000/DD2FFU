using CommandLine;
using CommandLine.Text;
using DD2FFU.DiskManagement;
using DiscUtils.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UsbEject;

namespace DD2FFU
{
    internal class Program
    {
        internal static string[] partitions = Constants.partitions;

        private static void Main(string[] args)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            HeadingInfo Heading = new(ass.GetAssemblyAttribute<AssemblyDescriptionAttribute>().Description, ass.GetName().Version.ToString());
            CopyrightInfo Copyright = new(ass.GetAssemblyAttribute<AssemblyCompanyAttribute>().Company, DateTime.Today.Year);

            _ = Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                Console.WriteLine(Heading.ToString());
                Console.WriteLine(Copyright.ToString());
                Console.WriteLine();

                string tmp = Directory.GetCurrentDirectory() + @"\temp\";
                if (!string.IsNullOrEmpty(o.Temp))
                {
                    tmp = o.Temp;
                }

                _ = Directory.CreateDirectory(tmp);

                if (!string.IsNullOrEmpty(o.Excludelist) && File.Exists(o.Excludelist))
                {
                    partitions = new List<string>(File.ReadAllLines(o.Excludelist)).ToArray();
                }

                Convert(o.ImgFile, o.FfuFile, tmp, o.Drive, partitions, o.Osversion, o.Antitheftver, o.Selfhost,
                    o.Headless, o.Recovery);
            });
        }

        private static void Convert(string imgFile, string ffuFile, string temp, string drive, string[] excludelist,
            string osversion, string antitheftversion, bool selfhost, bool headless, bool Recovery)
        {
            string tempvhd = temp + @"\tempdisk.vhd";
            tempvhd = tempvhd.Replace(@"\\", @"\");

            ulong eMMCDumpSize;
            ulong SectorSize = 0x200;

            if (imgFile.ToLower().Contains(@"\\.\physicaldrive"))
            {
                Logging.Log("Tool is running in Device Dump mode.");
                Logging.Log("Gathering disk geometry...");

                SectorSize = (ulong)GetDiskSize.GetDiskSectorSize(@"\\.\PhysicalDrive" + imgFile.ToLower().Replace(@"\\.\physicaldrive", ""));

                ImageUtils.ConvertDD2VHD(imgFile, tempvhd, excludelist, Recovery, (int)SectorSize);

                Logging.Log("Ejecting Mass Storage Device...");
                string ret = new VolumeDeviceClass().Volumes.First(x => x.DiskNumbers.Any(y => y == int.Parse(imgFile.ToLower().Replace(@"\\.\physicaldrive", "")))).Eject(false);
                if (!string.IsNullOrEmpty(ret))
                {
                    Logging.Log("We could not eject the Mass storage device from your computer. Please remove it and press any key once done.", severity: LoggingLevel.Warning);
                    Console.ReadKey();
                    Console.ReadKey();
                }
            }
            else if (imgFile.ToLower().EndsWith(@".vhd"))
            {
                Logging.Log("Tool is running in VHD mode.");

                //File.Copy(imgFile, tempvhd);
            }
            else
            {
                Logging.Log("Tool is running in Image Dump mode.");
                Logging.Log("Gathering disk image geometry...");

                ImageUtils.ConvertDD2VHD(imgFile, tempvhd, excludelist, Recovery, (int)SectorSize);
            }

            Logging.Log("Mounting VHD file...");

            string diskid = ImageUtils.MountVHD(tempvhd);

            eMMCDumpSize = (ulong)GetDiskSize.GetDiskLength($@"\\.\PhysicalDrive{diskid.ToLower().Replace(@"\\.\physicaldrive", "")}");
            SectorSize = (ulong)GetDiskSize.GetDiskSectorSize($@"\\.\PhysicalDrive{diskid.ToLower().Replace(@"\\.\physicaldrive", "")}");

            Logging.Log($"Reported source device eMMC size is: {eMMCDumpSize} bytes - {eMMCDumpSize / 1024 / 1024}MB - {eMMCDumpSize / 1024 / 1024 / 1024}GB.");
            Logging.Log($"Selected {SectorSize}B for the sector size");

            Logging.Log("Reading source device partition table...");

            List<ImageUtils.GPTPartition> partitionarray = ImageUtils.GetPartsFromGPT(diskid);
            string mainosid = partitionarray.First(x => x.Name.ToLower() == "mainos").id.ToString();

            Logging.Log("Found MainOS partition id: " + mainosid);

            Logging.Log("Remounting MainOS as drive " + drive);
            ImageUtils.MountDiskId(diskid, mainosid, drive);

            if (selfhost || headless)
                using (FileStream softhive = File.OpenWrite(drive + @":\Windows\System32\Config\SOFTWARE"))
                using (RegistryHive software = new RegistryHive(softhive))
                {
                    RegistryKey oobe = software.Root.OpenSubKey("Microsoft\\Shell\\OOBE");
                    if (selfhost)
                    {
                        Logging.Log("Enabling Selfhost OOBE on MainOS...");
                        oobe.SetValue("IsSelfHost", 1);
                    }

                    if (headless)
                    {
                        Logging.Log("Disabling OOBE Headless on MainOS...");
                        oobe.DeleteValue("OobeHeadless", false);
                        RegistryKey shell = software.Root.OpenSubKey("Microsoft\\Shell");
                        shell.DeleteValue("DockingControllerFirstRunDisabled", false);
                    }
                }

            Logging.Log("Unmounting MainOS as drive " + drive);
            ImageUtils.UnMountDiskId(diskid, mainosid, drive);

            Logging.Log("Beginning cleaning of source device partitions...");
            WPImageCleaning.Clean(diskid, partitionarray, drive, excludelist);

            Logging.Log("Remounting MainOS as drive " + drive);
            ImageUtils.MountDiskId(diskid, mainosid, drive);

            Logging.Log("Reading source device Platform information on MainOS...");
            OEMDevicePlatform plat = GetXmlClass<OEMDevicePlatform>(drive + @":\Windows\ImageUpdate\OEMDevicePlatform.xml");

            double diff = Math.Round(
                (eMMCDumpSize - ulong.Parse(plat.MinSectorCount) * Constants.SectorSize) / (1024 * 1024d),
                MidpointRounding.AwayFromZero);
            string dataid = partitionarray.First(x => x.Name.ToLower() == "data").id.ToString();

            Logging.Log("Calculated target image boundaries. Current layout should be shrunk by " + diff + "MB.");

            Logging.Log("Shrinking Data partition...");
            ImageUtils.ShrinkPartition(diskid, dataid, diff.ToString());

            Logging.Log(
                "Please verify the data free'd is a correct amount, if not, you'll need to capture and apply or clean image further... (The Data partition must be shrunk by: " +
                diff + "MB) Press any key once checked.", LoggingLevel.Warning);
            Console.ReadKey();
            Console.ReadKey();

            Logging.Log("Committing FFU image...");

            ImageUtils.CommitFFU(diskid, ffuFile, antitheftversion, osversion);
            Logging.Log("FFU file has been commited.");
            Logging.Log("Cleaning up temporary VHD file...");
            File.Delete(tempvhd);
        }

        public static T GetXmlClass<T>(string XmlFile)
        {
            T result;
            XmlSerializer serializerp = new(typeof(T));
            using (StreamReader readerp = new(XmlFile))
            {
                result = (T)serializerp.Deserialize(readerp);
            }

            return result;
        }
    }
}