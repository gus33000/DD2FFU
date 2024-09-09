// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.CommonUtils
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class CommonUtils
    {
        private const int S_OK = 0;
        private const int WimNoCommit = 0;
        private const int WimCommit = 1;
        private static readonly HashAlgorithm Sha256Algorithm = HashAlgorithm.Create("SHA256");

        public static nint MountVHD(string vhdPath, bool fReadOnly)
        {
            VIRTUAL_DISK_ACCESS_MASK accessMask = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL;
            if (fReadOnly)
            {
                accessMask = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_READ;
            }

            OPEN_VIRTUAL_DISK_FLAG openFlags = OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE;
            ATTACH_VIRTUAL_DISK_FLAG attachFlags = ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_NONE;
            if (fReadOnly)
            {
                attachFlags = ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_READ_ONLY;
            }

            return MountVHD(vhdPath, accessMask, openFlags, attachFlags);
        }

        public static nint MountVHD(string vhdPath, VIRTUAL_DISK_ACCESS_MASK accessMask,
            OPEN_VIRTUAL_DISK_FLAG openFlags, ATTACH_VIRTUAL_DISK_FLAG attachFlags)
        {
            nint zero = nint.Zero;
            VIRTUAL_STORAGE_TYPE vdisktype = new()
            {
                DeviceId = VHD_STORAGE_TYPE_DEVICE.VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
                VendorId = VIRTUAL_STORAGE_TYPE_VENDOR.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
            };
            OPEN_VIRTUAL_DISK_PARAMETERS vopenparam = new()
            {
                Version = OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1,
                RWDepth = 1U
            };
            int error1 = VirtualDiskLib.OpenVirtualDisk(ref vdisktype, vhdPath, accessMask, openFlags, ref vopenparam,
                ref zero);
            if (0 < error1)
            {
                throw new Win32Exception(error1);
            }

            ATTACH_VIRTUAL_DISK_PARAMETERS attachparam = new()
            {
                Version = ATTACH_VIRTUAL_DISK_VERSION.ATTACH_VIRTUAL_DISK_VERSION_1
            };
            int error2 =
                VirtualDiskLib.AttachVirtualDisk(zero, nint.Zero, attachFlags, 0U, ref attachparam, nint.Zero);
            return 0 < error2 ? throw new Win32Exception(error2) : zero;
        }

        public static void DismountVHD(nint hndlVirtDisk)
        {
            if (hndlVirtDisk == nint.Zero)
            {
                return;
            }

            if (0 < VirtualDiskLib.DetachVirtualDisk(hndlVirtDisk,
                                DETACH_VIRTUAL_DISK_FLAG.DETACH_VIRTUAL_DISK_FLAG_NONE, 0U))
            {
                throw new Win32Exception();
            }

            _ = VirtualDiskLib.CloseHandle(hndlVirtDisk);
        }

        [DllImport("UpdateDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IU_MountWim(string WimPath, string MountPath, string TemporaryPath);

        [DllImport("UpdateDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IU_DismountWim(string WimPath, string MountPath, int CommitMode);

        public static bool MountWIM(string wimPath, string mountPoint, string tmpDir)
        {
            return IU_MountWim(wimPath, mountPoint, tmpDir) == 0;
        }

        public static bool DismountWIM(string wimPath, string mountPoint, bool commit)
        {
            return IU_DismountWim(wimPath, mountPoint, commit ? 1 : 0) == 0;
        }

        public static string FindInPath(string filename)
        {
            string path1 = LongPathFile.Exists(Path.Combine(Environment.CurrentDirectory, filename))
                ? Environment.CurrentDirectory
                : Environment.GetEnvironmentVariable("PATH").Split(';')
                    .FirstOrDefault(x => LongPathFile.Exists(Path.Combine(x, filename)));
            return string.IsNullOrEmpty(path1)
                ? throw new FileNotFoundException(string.Format("Can't find file '{0}' anywhere in the %PATH%",
                    filename))
                : Path.Combine(path1, filename);
        }

        public static int RunProcess(string workingDir, string command, string args, bool hiddenWindow)
        {
            return RunProcess(workingDir, command, args, hiddenWindow, false, out _);
        }

        public static int RunProcess(string command, string args)
        {
            int num = RunProcess(null, command, args, true, true, out string processOutput);
            if (num == 0)
            {
                return num;
            }

            Console.WriteLine(processOutput);
            return num;
        }

        public static int RunProcessVerbose(string command, string args)
        {
            int num = RunProcess(null, command, args, true, true, out string processOutput);
            Console.WriteLine(processOutput);
            return num;
        }

        private static int RunProcess(string workingDir, string command, string args, bool hiddenWindow,
            bool captureOutput, out string processOutput)
        {
            int num = 0;
            processOutput = string.Empty;
            command = Environment.ExpandEnvironmentVariables(command);
            args = Environment.ExpandEnvironmentVariables(args);
            ProcessStartInfo startInfo = new()
            {
                CreateNoWindow = true
            };
            if (hiddenWindow)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            if (workingDir != null)
            {
                startInfo.WorkingDirectory = workingDir;
            }

            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = captureOutput;
            startInfo.UseShellExecute = !captureOutput;
            if (!string.IsNullOrEmpty(command) && !LongPathFile.Exists(command))
            {
                _ = FindInPath(command);
            }

            startInfo.FileName = command;
            startInfo.Arguments = args;
            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    if (captureOutput)
                    {
                        processOutput = process.StandardOutput.ReadToEnd();
                    }

                    process.WaitForExit();
                    if (!process.HasExited)
                    {
                        throw new IUException("Process <{0}> didn't exit correctly", command);
                    }

                    num = process.ExitCode;
                }
            }

            return num;
        }

        public static string BytesToHexicString(byte[] bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new(bytes.Length * 2);
            for (int index = 0; index < bytes.Length; ++index)
            {
                _ = stringBuilder.Append(bytes[index].ToString("X2", CultureInfo.InvariantCulture.NumberFormat));
            }

            return stringBuilder.ToString();
        }

        public static byte[] HexicStringToBytes(string text)
        {
            if (text == null)
            {
                return new byte[0];
            }

            if (text.Length % 2 != 0)
            {
                throw new IUException("Incorrect length of a hexic string:\"{0}\"", text);
            }

            List<byte> byteList = new(text.Length / 2);
            int startIndex = 0;
            while (startIndex < text.Length)
            {
                string s = text.Substring(startIndex, 2);
                if (!byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out byte result))
                {
                    throw new IUException("Failed to parse hexic string: \"{0}\"", s);
                }

                byteList.Add(result);
                startIndex += 2;
            }

            return byteList.ToArray();
        }

        public static bool ByteArrayCompare(byte[] array1, byte[] array2)
        {
            if (array1 == array2)
            {
                return true;
            }

            if (array1 == null || array2 == null || array1.Length != array2.Length)
            {
                return false;
            }

            for (int index = 0; index < array1.Length; ++index)
            {
                if (array1[index] != array2[index])
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetCopyrightString()
        {
            string format = "Microsoft (C) {0} {1}";
            string processName = Process.GetCurrentProcess().ProcessName;
            string assemblyFileVersion = FileUtils.GetCurrentAssemblyFileVersion();
            StringBuilder stringBuilder = new();
            _ = stringBuilder.AppendFormat(format, processName, assemblyFileVersion);
            _ = stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPrincipal)]
        public static bool IsCurrentUserAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole("BUILTIN\\\\Administrators");
        }

        public static string GetSha256Hash(byte[] buffer)
        {
            return BitConverter.ToString(Sha256Algorithm.ComputeHash(buffer)).Replace("-", string.Empty);
        }
    }
}