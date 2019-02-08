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
            var messageFromErrorCode = GetMessageFromErrorCode(errorCode);
            switch (errorCode)
            {
                case 2:
                    return new FileNotFoundException(messageFromErrorCode);
                case 3:
                    return new DirectoryNotFoundException(messageFromErrorCode);
                case 5:
                    return new UnauthorizedAccessException(messageFromErrorCode);
                case 15:
                    return new DriveNotFoundException(messageFromErrorCode);
                case 123:
                    return new ArgumentException(messageFromErrorCode, parameterName);
                case 206:
                    return new PathTooLongException(messageFromErrorCode);
                case 995:
                    return new OperationCanceledException(messageFromErrorCode);
                default:
                    return new IOException(messageFromErrorCode, NativeMethods.MakeHRFromErrorCode(errorCode));
            }
        }

        private static string GetMessageFromErrorCode(int errorCode)
        {
            var stringBuilder = new StringBuilder(512);
            var dwFlags = 12800;
            var zero1 = IntPtr.Zero;
            var dwMessageId = errorCode;
            var dwLanguageId = 0;
            var lpBuffer = stringBuilder;
            var capacity = lpBuffer.Capacity;
            var zero2 = IntPtr.Zero;
            NativeMethods.FormatMessage(dwFlags, zero1, dwMessageId, dwLanguageId, lpBuffer, capacity, zero2);
            return stringBuilder.ToString();
        }

        internal static string[] ConvertPtrArrayToStringArray(IntPtr strPtrArray, int cStrings)
        {
            var destination = new IntPtr[cStrings];
            if (strPtrArray != IntPtr.Zero)
                Marshal.Copy(strPtrArray, destination, 0, cStrings);
            var stringList = new List<string>(cStrings);
            for (var index = 0; index < cStrings; ++index)
                stringList.Add(Marshal.PtrToStringUni(destination[index]));
            return stringList.ToArray();
        }

        public static string NormalizeLongPath(string path)
        {
            var stringBuilder = new StringBuilder(MAX_LONG_PATH);
            var strPath = path;
            var pathBuffer = stringBuilder;
            var capacity = pathBuffer.Capacity;
            var canonicalUncPath = NativeMethods.IU_GetCanonicalUNCPath(strPath, pathBuffer, capacity);
            if (canonicalUncPath != 0)
                throw GetExceptionFromWin32Error(canonicalUncPath);
            return stringBuilder.ToString();
        }

        public static FileAttributes GetAttributes(string path)
        {
            var fileAttributes = (int) NativeMethods.GetFileAttributes(NormalizeLongPath(path));
            if (fileAttributes != -1)
                return (FileAttributes) fileAttributes;
            throw GetExceptionFromLastWin32Error();
        }
    }
}