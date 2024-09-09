// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.LongPathCommon
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    internal static class LongPathCommon
    {
        private static readonly int MAX_LONG_PATH = 32000;

        internal static Exception GetExceptionFromLastWin32Error()
        {
            return GetExceptionFromLastWin32Error("path");
        }

        internal static Exception GetExceptionFromLastWin32Error(string parameterName)
        {
            return GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
        }

        internal static Exception GetExceptionFromWin32Error(int errorCode)
        {
            return GetExceptionFromWin32Error(errorCode, "path");
        }

        internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName)
        {
            string messageFromErrorCode = GetMessageFromErrorCode(errorCode);
            return errorCode switch
            {
                2 => new FileNotFoundException(messageFromErrorCode),
                3 => new DirectoryNotFoundException(messageFromErrorCode),
                5 => new UnauthorizedAccessException(messageFromErrorCode),
                15 => new DriveNotFoundException(messageFromErrorCode),
                123 => new ArgumentException(messageFromErrorCode, parameterName),
                206 => new PathTooLongException(messageFromErrorCode),
                995 => new OperationCanceledException(messageFromErrorCode),
                _ => new IOException(messageFromErrorCode, NativeMethods.MakeHRFromErrorCode(errorCode)),
            };
        }

        private static string GetMessageFromErrorCode(int errorCode)
        {
            StringBuilder stringBuilder = new(512);
            int dwFlags = 12800;
            nint zero1 = nint.Zero;
            int dwMessageId = errorCode;
            int dwLanguageId = 0;
            StringBuilder lpBuffer = stringBuilder;
            int capacity = lpBuffer.Capacity;
            nint zero2 = nint.Zero;
            _ = NativeMethods.FormatMessage(dwFlags, zero1, dwMessageId, dwLanguageId, lpBuffer, capacity, zero2);
            return stringBuilder.ToString();
        }

        internal static string[] ConvertPtrArrayToStringArray(nint strPtrArray, int cStrings)
        {
            nint[] destination = new nint[cStrings];
            if (strPtrArray != nint.Zero)
            {
                Marshal.Copy(strPtrArray, destination, 0, cStrings);
            }

            List<string> stringList = new(cStrings);
            for (int index = 0; index < cStrings; ++index)
            {
                stringList.Add(Marshal.PtrToStringUni(destination[index]));
            }

            return stringList.ToArray();
        }

        public static string NormalizeLongPath(string path)
        {
            StringBuilder stringBuilder = new(MAX_LONG_PATH);
            string strPath = path;
            StringBuilder pathBuffer = stringBuilder;
            int capacity = pathBuffer.Capacity;
            int canonicalUncPath = NativeMethods.IU_GetCanonicalUNCPath(strPath, pathBuffer, capacity);
            return canonicalUncPath != 0 ? throw GetExceptionFromWin32Error(canonicalUncPath) : stringBuilder.ToString();
        }

        public static FileAttributes GetAttributes(string path)
        {
            int fileAttributes = (int)NativeMethods.GetFileAttributes(NormalizeLongPath(path));
            return fileAttributes != -1 ? (FileAttributes)fileAttributes : throw GetExceptionFromLastWin32Error();
        }
    }
}