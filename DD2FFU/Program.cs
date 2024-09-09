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
    internal static class AssemblyExtensions
    {
        public static T GetAssemblyAttribute<T>(this Assembly ass) where T : Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            return attributes == null || attributes.Length == 0 ? null : attributes.OfType<T>().SingleOrDefault();
        }
    }

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
                var ret = new VolumeDeviceClass().Volumes.First(x => x.DiskNumbers.Any(y => y == int.Parse(imgFile.ToLower().Replace(@"\\.\physicaldrive", "")))).Eject(false);
                if (!string.IsNullOrEmpty(ret))
                {
                    Logging.Log("We could not eject the Mass storage device from your computer. Please remove it and press any key once done.", severity: Logging.LoggingLevel.Warning);
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

            var partitionarray = ImageUtils.GetPartsFromGPT(diskid);
            var mainosid = partitionarray.First(x => x.Name.ToLower() == "mainos").id.ToString();

            Logging.Log("Found MainOS partition id: " + mainosid);

            Logging.Log("Remounting MainOS as drive " + drive);
            ImageUtils.MountDiskId(diskid, mainosid, drive);

            if (selfhost || headless)
                using (var softhive = File.OpenWrite(drive + @":\Windows\System32\Config\SOFTWARE"))
                using (var software = new RegistryHive(softhive))
                {
                    var oobe = software.Root.OpenSubKey("Microsoft\\Shell\\OOBE");
                    if (selfhost)
                    {
                        Logging.Log("Enabling Selfhost OOBE on MainOS...");
                        oobe.SetValue("IsSelfHost", 1);
                    }

                    if (headless)
                    {
                        Logging.Log("Disabling OOBE Headless on MainOS...");
                        oobe.DeleteValue("OobeHeadless", false);
                        var shell = software.Root.OpenSubKey("Microsoft\\Shell");
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
            var plat = GetXmlClass<OEMDevicePlatform>(drive + @":\Windows\ImageUpdate\OEMDevicePlatform.xml");

            var diff = Math.Round(
                (eMMCDumpSize - ulong.Parse(plat.MinSectorCount) * Constants.SectorSize) / (1024 * 1024d),
                MidpointRounding.AwayFromZero);
            var dataid = partitionarray.First(x => x.Name.ToLower() == "data").id.ToString();

            Logging.Log("Calculated target image boundaries. Current layout should be shrunk by " + diff + "MB.");

            Logging.Log("Shrinking Data partition...");
            ImageUtils.ShrinkPartition(diskid, dataid, diff.ToString());

            Logging.Log(
                "Please verify the data free'd is a correct amount, if not, you'll need to capture and apply or clean image further... (The Data partition must be shrunk by: " +
                diff + "MB) Press any key once checked.", Logging.LoggingLevel.Warning);
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

        internal class Options
        {
            [Option('i', "img-file", HelpText = @"A path to the img file to convert *OR* a PhysicalDisk path. i.e. \\.\PhysicalDrive1 (Specifying a PhysicalDrive will make the tool eject the drive)", Required = true)]
            public string ImgFile
            {
                get; set;
            }

            [Option('f', "ffu-file", HelpText = "A path to the FFU file to output", Required = true)]
            public string FfuFile
            {
                get; set;
            }

            [Option('t', "temp", HelpText = "A path to the tool temporary directory", Required = false)]
            public string Temp
            {
                get; set;
            }

            [Option('l', "drive-letter",
                HelpText = "A drive letter that will be used during FFU generation for clean up. ie: X", Default = "B")]
            public string Drive
            {
                get; set;
            }

            [Option('e', "exclude-list", Required = false,
                HelpText = "Path to an optional partition exclude text list to use instead of the builtin one.")]
            public string Excludelist
            {
                get; set;
            }

            [Option('a', "anti-theft-version", Required = false, HelpText = "Anti theft version.", Default = "1.1")]
            public string Antitheftver
            {
                get; set;
            }

            [Option('o', "os-version", Required = false, HelpText = "Operating system version.",
                Default = "10.0.11111.0")]
            public string Osversion
            {
                get; set;
            }

            [Option('s', "oobe-selfhost", Required = false, HelpText = "Enable OOBE selfhost.", Default = false)]
            public bool Selfhost
            {
                get; set;
            }

            [Option('d', "disable-headless", Required = false, HelpText = "Disable OOBE headless.", Default = false)]
            public bool Headless
            {
                get; set;
            }

            [Option('r', "enable-recoveryffu", Required = false, HelpText = "Generates a recovery ffu with no partition skipped. Useful for clean state restore for a SPECIFIC unique device.", Default = false)]
            public bool Recovery
            {
                get; set;
            }
        }
    }
}