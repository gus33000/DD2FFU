// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.LongPathFile
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class LongPathFile
    {
        public static bool Exists(string path)
        {
            return NativeMethods.IU_FileExists(path);
        }

        public static FileAttributes GetAttributes(string path)
        {
            return LongPathCommon.GetAttributes(path);
        }

        public static void SetAttributes(string path, FileAttributes attributes)
        {
            if (!NativeMethods.SetFileAttributes(LongPathCommon.NormalizeLongPath(path), attributes))
            {
                throw LongPathCommon.GetExceptionFromLastWin32Error();
            }
        }

        public static void Delete(string path)
        {
            string lpFileName = LongPathCommon.NormalizeLongPath(path);
            if (!Exists(path) || NativeMethods.DeleteFile(lpFileName))
            {
                return;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 2)
            {
                throw LongPathCommon.GetExceptionFromWin32Error(lastWin32Error);
            }
        }

        public static void Move(string sourcePath, string destinationPath)
        {
            if (!NativeMethods.MoveFile(LongPathCommon.NormalizeLongPath(sourcePath),
                LongPathCommon.NormalizeLongPath(destinationPath)))
            {
                throw LongPathCommon.GetExceptionFromLastWin32Error();
            }
        }

        public static void Copy(string sourcePath, string destinationPath, bool overwrite)
        {
            if (!NativeMethods.CopyFile(LongPathCommon.NormalizeLongPath(sourcePath),
                LongPathCommon.NormalizeLongPath(destinationPath), !overwrite))
            {
                throw LongPathCommon.GetExceptionFromLastWin32Error();
            }
        }

        public static void Copy(string sourcePath, string destinationPath)
        {
            Copy(sourcePath, destinationPath, false);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access)
        {
            return Open(path, mode, access, FileShare.None, 0, FileOptions.None);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return Open(path, mode, access, share, 0, FileOptions.None);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize,
            FileOptions options)
        {
            if (bufferSize == 0)
            {
                bufferSize = 1024;
            }

            FileStream fileStream =
                            new(GetFileHandle(LongPathCommon.NormalizeLongPath(path), mode, access, share, options),
                                access, bufferSize, (options & FileOptions.Asynchronous) == FileOptions.Asynchronous);
            if (mode == FileMode.Append)
            {
                _ = fileStream.Seek(0L, SeekOrigin.End);
            }

            return fileStream;
        }

        public static FileStream OpenRead(string path)
        {
            return Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static FileStream OpenWrite(string path)
        {
            return Open(path, FileMode.Create, FileAccess.ReadWrite);
        }

        public static StreamWriter CreateText(string path)
        {
            return new StreamWriter(Open(path, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8);
        }

        public static byte[] ReadAllBytes(string path)
        {
            using FileStream fileStream = OpenRead(path);
            byte[] buffer = new byte[fileStream.Length];
            _ = fileStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public static void WriteAllBytes(string path, byte[] contents)
        {
            using FileStream fileStream = OpenWrite(path);
            fileStream.Write(contents, 0, contents.Length);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            using StreamReader streamReader = new(OpenRead(path), encoding, true);
            return streamReader.ReadToEnd();
        }

        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.Default);
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            using StreamWriter streamWriter = new(OpenWrite(path), encoding);
            streamWriter.Write(contents);
        }

        public static void WriteAllText(string path, string contents)
        {
            WriteAllText(path, contents, new UTF8Encoding(false));
        }

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            using StreamReader streamReader = new(OpenRead(path), encoding, true);
            List<string> stringList = [];
            while (!streamReader.EndOfStream)
            {
                stringList.Add(streamReader.ReadLine());
            }

            return stringList.ToArray();
        }

        public static string[] ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.Default);
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            using StreamWriter streamWriter = new(OpenWrite(path), encoding);
            foreach (string content in contents)
            {
                streamWriter.WriteLine(content);
            }
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            WriteAllLines(path, contents, new UTF8Encoding(false));
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            using StreamWriter streamWriter = new(Open(path, FileMode.Append, FileAccess.ReadWrite), encoding);
            streamWriter.Write(contents);
        }

        public static void AppendAllText(string path, string contents)
        {
            AppendAllText(path, contents, new UTF8Encoding(false));
        }

        private static SafeFileHandle GetFileHandle(string normalizedPath, FileMode mode, FileAccess access,
            FileShare share, FileOptions options)
        {
            NativeMethods.EFileAccess underlyingAccess = GetUnderlyingAccess(access);
            FileMode underlyingMode = GetUnderlyingMode(mode);
            SafeFileHandle file = NativeMethods.CreateFile(normalizedPath, underlyingAccess, (uint)share, nint.Zero,
                (uint)underlyingMode, (uint)options, nint.Zero);
            return !file.IsInvalid ? file : throw LongPathCommon.GetExceptionFromLastWin32Error();
        }

        private static FileMode GetUnderlyingMode(FileMode mode)
        {
            return mode == FileMode.Append ? FileMode.OpenOrCreate : mode;
        }

        private static NativeMethods.EFileAccess GetUnderlyingAccess(FileAccess access)
        {
            return access switch
            {
                FileAccess.Read => NativeMethods.EFileAccess.GenericRead,
                FileAccess.Write => NativeMethods.EFileAccess.GenericWrite,
                FileAccess.ReadWrite => NativeMethods.EFileAccess.GenericRead | NativeMethods.EFileAccess.GenericWrite,
                _ => throw new ArgumentOutOfRangeException(nameof(access)),
            };
        }
    }
}