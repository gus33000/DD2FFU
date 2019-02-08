// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.FileUtils
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class FileUtils
    {
        public const int MAX_PATH = 260;

        public static string RerootPath(string path, string oldRoot, string newRoot)
        {
            if (oldRoot.Last() != '\\')
                oldRoot += "\\";
            if (newRoot.Last() != '\\')
                newRoot += "\\";
            return path.Replace(oldRoot, newRoot);
        }

        public static string GetTempFile()
        {
            return GetTempFile(Path.GetTempPath());
        }

        public static string GetTempFile(string dir)
        {
            return Path.Combine(dir, Path.GetRandomFileName());
        }

        public static void DeleteTree(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentException("Empty directory path");
            if (LongPathFile.Exists(dirPath))
                throw new IOException(string.Format("Cannot delete directory {0}, it's a file", dirPath));
            if (!LongPathDirectory.Exists(dirPath))
                return;
            LongPathDirectory.Delete(dirPath, true);
        }

        public static void DeleteFile(string filePath)
        {
            if (!LongPathFile.Exists(filePath))
                return;
            LongPathFile.SetAttributes(filePath, FileAttributes.Normal);
            LongPathFile.Delete(filePath);
        }

        public static void CleanDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentException("Empty directory path");
            if (LongPathFile.Exists(dirPath))
                throw new IOException(string.Format("Cannot create directory {0}, a file with same name exists",
                    dirPath));
            NativeMethods.IU_CleanDirectory(dirPath, false);
        }

        public static string GetTempDirectory()
        {
            string path;
            do
            {
                path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            } while (LongPathDirectory.Exists(path));

            LongPathDirectory.CreateDirectory(path);
            return path;
        }

        public static bool IsTargetUpToDate(string inputFile, string targetFile)
        {
            if (!LongPathFile.Exists(targetFile))
                return false;
            var lastWriteTimeUtc = new FileInfo(targetFile).LastWriteTimeUtc;
            return !(new FileInfo(inputFile).LastWriteTimeUtc > lastWriteTimeUtc);
        }

        public static string GetFileVersion(string filepath)
        {
            var str = string.Empty;
            if (LongPathFile.Exists(filepath))
                str = FileVersionInfo.GetVersionInfo(filepath).FileVersion;
            return str;
        }

        public static string GetCurrentAssemblyFileVersion()
        {
            return GetFileVersion(Process.GetCurrentProcess().MainModule.FileName);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath, uint cchBuffer);

        public static string GetShortPathName(string dirPath)
        {
            var lpszShortPath = new StringBuilder(260);
            if (GetShortPathName(dirPath, lpszShortPath, 260U) == 0U)
                return dirPath;
            return lpszShortPath.ToString();
        }

        public static void CopyDirectory(string source, string destination)
        {
            LongPathDirectory.CreateDirectory(destination);
            foreach (var file in LongPathDirectory.GetFiles(source))
                LongPathFile.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
            foreach (var directory in LongPathDirectory.GetDirectories(source))
                CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}