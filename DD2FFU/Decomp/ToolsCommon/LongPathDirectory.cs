// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.LongPathDirectory
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class LongPathDirectory
    {
        public const string ALL_FILE_PATTERN = "*.*";

        public static void CreateDirectory(string path)
        {
            try
            {
                NativeMethods.IU_EnsureDirectoryExists(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Delete(string path)
        {
            var str = LongPathCommon.NormalizeLongPath(path);
            if (Exists(str) && !NativeMethods.RemoveDirectory(str))
                throw LongPathCommon.GetExceptionFromLastWin32Error();
        }

        public static void Delete(string path, bool recursive)
        {
            if (recursive)
                NativeMethods.IU_CleanDirectory(path, true);
            else
                Delete(path);
        }

        public static bool Exists(string path)
        {
            return NativeMethods.IU_DirectoryExists(path);
        }

        public static FileAttributes GetAttributes(string path)
        {
            var attributes = LongPathCommon.GetAttributes(path);
            if (!attributes.HasFlag(FileAttributes.Directory))
                throw LongPathCommon.GetExceptionFromWin32Error(267);
            return attributes;
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern,
            SearchOption searchOptions)
        {
            return GetDirectories(path, searchPattern, searchOptions);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            return EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOptions)
        {
            if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
                throw new NotImplementedException("Unknown search option: " + searchOptions);
            var fRecursive = searchOptions == SearchOption.AllDirectories;
            var rgDirectories = IntPtr.Zero;
            var cDirectories = 0;
            var allDirectories = NativeMethods.IU_GetAllDirectories(
                Path.Combine(path, Path.GetDirectoryName(searchPattern)), Path.GetFileName(searchPattern), fRecursive,
                out rgDirectories, out cDirectories);
            if (allDirectories != 0)
                throw LongPathCommon.GetExceptionFromWin32Error(allDirectories);
            try
            {
                return LongPathCommon.ConvertPtrArrayToStringArray(rgDirectories, cDirectories);
            }
            finally
            {
                var errorCode = NativeMethods.IU_FreeStringList(rgDirectories, cDirectories);
                if (errorCode != 0)
                    throw LongPathCommon.GetExceptionFromWin32Error(errorCode);
            }
        }

        public static string[] GetDirectories(string path, string searchPattern)
        {
            return GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static string[] GetDirectories(string path)
        {
            return GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOptions)
        {
            return GetFiles(path, searchPattern, searchOptions);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOptions)
        {
            if (searchOptions != SearchOption.AllDirectories && searchOptions != SearchOption.TopDirectoryOnly)
                throw new NotImplementedException("Unknown search option: " + searchOptions);
            var fRecursive = searchOptions == SearchOption.AllDirectories;
            var rgFiles = IntPtr.Zero;
            var cFiles = 0;
            var allFiles = NativeMethods.IU_GetAllFiles(Path.Combine(path, Path.GetDirectoryName(searchPattern)),
                Path.GetFileName(searchPattern), fRecursive, out rgFiles, out cFiles);
            if (allFiles != 0)
                throw LongPathCommon.GetExceptionFromWin32Error(allFiles);
            try
            {
                return LongPathCommon.ConvertPtrArrayToStringArray(rgFiles, cFiles);
            }
            finally
            {
                var errorCode = NativeMethods.IU_FreeStringList(rgFiles, cFiles);
                if (errorCode != 0)
                    throw LongPathCommon.GetExceptionFromWin32Error(errorCode);
            }
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static string[] GetFiles(string path)
        {
            return GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        }
    }
}