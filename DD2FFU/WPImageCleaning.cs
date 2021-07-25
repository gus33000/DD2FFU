using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DD2FFU
{
    internal class WPImageCleaning
    {
        private static readonly string[] filedeletionEFIESP =
        {
            @"\Resetlog.txt",
            @"\FveEnableSvc.log",
            @"\Cbs_Pbr.log",
            @"\ResetRemaininglog.txt"
        };

        private static readonly string[] filedeletionDATA =
        {
            @"\SystemData\NonEtwLogs\ImgUpd.log.cbs.log",
            @"\SystemData\NonEtwLogs\ImgUpd.log",
            @"\Users\Public\Documents\Non-Production errors.txt"
        };

        private static readonly string[] filedeletionCrashDump =
        {
            @"\DedicatedDump.sys",
            @"\DumpStack.log.tmp"
        };

        private static readonly string[] filedeletionMainOS =
        {
            @"\Windows\INF\setupapi.dev.log",
            @"\Windows\system32\offlineCrash.log"
        };

        private static readonly string[] folderdeletionEFIESP =
        {
            @"\efi\Microsoft\Logs"
        };

        private static readonly string[] filepurgeEFIESP =
        {
            @"\Windows\System32\CHARGELOG.txt",
            @"\Logs\wpdmp.log"
        };

        private static void CleanFS(string letter)
        {
            var process = new Process();
            process.StartInfo.FileName = "chkdsk.exe";
            process.StartInfo.Arguments = "/f /x " + letter + ":";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;

            process.Start();

            process.WaitForExit();
        }

        private static void CleanBin(string letter)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c rmdir /Q /S " + letter.Substring(0, 1) + @":\$RECYCLE.BIN";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.UseShellExecute = false;

            process.Start();

            process.WaitForExit();
        }

        public static void Clean(string physicalharddisk, List<ImageUtils.GPTPartition> partitions, string letter,
            string[] excluded)
        {
            var mainosid = "";

            foreach (var partition in partitions)
            {
                if (excluded.Any(x => x.ToLower() == partition.Name.ToLower()))
                    continue;

                Logging.Log("Processing " + partition.Name + "...");

                switch (partition.Name.ToLower())
                {
                    case "mainos":
                        {
                            mainosid = partition.id.ToString();
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                            foreach (var file in filedeletionMainOS)
                                try
                                {
                                    if (File.Exists(letter + @":" + file))
                                    {
                                        var attributes = File.GetAttributes(letter + @":" + file);
                                        if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
                                        {
                                            File.SetAttributes(letter + @":" + file, FileAttributes.Normal);
                                        }

                                        File.Delete(letter + @":" + file);
                                        Logging.Log("Removed " + file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            var hivetamperLogs = Directory.GetFiles(letter + @":" + @"\Windows\System32\Config", "*.TM*",
                                SearchOption.TopDirectoryOnly);
                            foreach (var file in hivetamperLogs)
                            {
                                Logging.Log("Removing " + file + "...");
                                File.Delete(file);
                            }

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            break;
                        }
                    case "logfs":
                        {
                            ImageUtils.SetDiskType(physicalharddisk, partition.id.ToString(),
                                "ebd0a0a2-b9e5-4433-87c0-68b6b72699c7");
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                            foreach (var file in Directory.EnumerateFiles(letter + @":\", "*.txt",
                                SearchOption.TopDirectoryOnly))
                                try
                                {
                                    Console.WriteLine(file);
                                    if (File.Exists(file))
                                    {
                                        var attributes = File.GetAttributes(file);
                                        if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
                                        {
                                            File.SetAttributes(file, FileAttributes.Normal);
                                        }

                                        File.Delete(file);
                                        Logging.Log("Removed " + file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            try
                            {
                                CleanBin(letter);
                                Logging.Log("Removed " + letter + @":\$RECYCLE.BIN");
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                            }

                            CleanFS(letter);

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            ImageUtils.SetDiskType(physicalharddisk, partition.id.ToString(), partition.Type);
                            break;
                        }
                    case "plat":
                        {
                            ImageUtils.SetDiskType(physicalharddisk, partition.id.ToString(),
                                "ebd0a0a2-b9e5-4433-87c0-68b6b72699c7");
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                            try
                            {
                                CleanBin(letter);
                                Logging.Log("Removed " + letter + @":\$RECYCLE.BIN");
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                            }

                            CleanFS(letter);

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            ImageUtils.SetDiskType(physicalharddisk, partition.id.ToString(), partition.Type);
                            break;
                        }
                    case "efiesp":
                        {
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);


                            foreach (var file in filedeletionEFIESP)
                                try
                                {
                                    if (File.Exists(letter + @":" + file))
                                    {
                                        var attributes = File.GetAttributes(letter + @":" + file);
                                        if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
                                        {
                                            File.SetAttributes(letter + @":" + file, FileAttributes.Normal);
                                        }

                                        File.Delete(letter + @":" + file);
                                        Logging.Log("Removed " + file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            foreach (var file in filepurgeEFIESP)
                                try
                                {
                                    var things = File.ReadAllText(letter + @":" + file).ToCharArray();
                                    for (var i = 0; i < things.Length; i++) things[i] = ' ';
                                    File.WriteAllText(letter + @":" + file, new string(things));
                                    Logging.Log("Purged " + file);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            foreach (var folder in folderdeletionEFIESP)
                                try
                                {
                                    Directory.Delete(letter + @":" + folder, true);
                                    Logging.Log("Removed " + folder);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            break;
                        }
                    case "data":
                        {
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                            foreach (var file in filedeletionDATA)
                                try
                                {
                                    if (File.Exists(letter + @":" + file))
                                    {
                                        var attributes = File.GetAttributes(letter + @":" + file);
                                        if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
                                        {
                                            File.SetAttributes(letter + @":" + file, FileAttributes.Normal);
                                        }

                                        File.Delete(letter + @":" + file);
                                        Logging.Log("Removed " + file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            try
                            {
                                foreach (var item in Directory.EnumerateFileSystemEntries(
                                    letter.Substring(0, 1) + @":\SystemData\Telemetry", "*", SearchOption.AllDirectories))
                                    try
                                    {
                                        if (item != letter.Substring(0, 1) + @":\SystemData\Telemetry\Archive" &&
                                            item != letter.Substring(0, 1) +
                                            @":\SystemData\Telemetry\KernelDumps\LiveDumps" && item !=
                                            letter.Substring(0, 1) + @":\SystemData\Telemetry\KernelDumps")
                                        {
                                            if (File.Exists(item))
                                                File.Delete(item);
                                            else
                                                Directory.Delete(item, true);

                                            Logging.Log("Removed " + item);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                    }
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                            }

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            break;
                        }
                    case "crashdump":
                        {
                            ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                            foreach (var file in filedeletionCrashDump)
                                try
                                {
                                    if (File.Exists(letter + @":" + file))
                                    {
                                        var attributes = File.GetAttributes(letter + @":" + file);
                                        if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
                                        {
                                            File.SetAttributes(letter + @":" + file, FileAttributes.Normal);
                                        }

                                        File.Delete(letter + @":" + file);
                                        Logging.Log("Removed " + file);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                                }

                            ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                            break;
                        }
                }

                if (partition.Type.ToLower() == "ebd0a0a2-b9e5-4433-87c0-68b6b72699c7")
                {
                    ImageUtils.MountDiskId(physicalharddisk, partition.id.ToString(), letter);

                    try
                    {
                        CleanBin(letter);
                        Logging.Log("Removed " + letter + @":\$RECYCLE.BIN");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex.ToString(), Logging.LoggingLevel.Error);
                    }

                    CleanFS(letter);

                    ImageUtils.UnMountDiskId(physicalharddisk, partition.id.ToString(), letter);
                }
            }

            Logging.Log("Remounting MainOS...");
            ImageUtils.MountDiskId(physicalharddisk, mainosid, letter);
        }
    }
}