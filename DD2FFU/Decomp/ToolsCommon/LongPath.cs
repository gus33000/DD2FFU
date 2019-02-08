// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.LongPath
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class LongPath
    {
        private const string UNC_PREFIX = "\\\\";
        private const string LONGPATH_PREFIX = "\\\\?\\";
        private const string LONGPATH_UNC_PREFIX = "\\\\?\\UNC\\";

        public static string GetDirectoryName(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "Path cannot be null.");
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("Path cannot contain invalid characters.", nameof(path));
            var length = path.LastIndexOfAny(new char[2]
            {
                Path.DirectorySeparatorChar,
                Path.VolumeSeparatorChar
            });
            if (length == -1)
                return null;
            return path.Substring(0, length);
        }

        public static string GetFullPath(string path)
        {
            var str = LongPathCommon.NormalizeLongPath(path);
            if (str.StartsWith("\\\\?\\UNC\\", StringComparison.OrdinalIgnoreCase))
                return "\\\\" + str.Substring("\\\\?\\UNC\\".Length);
            if (str.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase))
                return str.Substring("\\\\?\\".Length);
            return str;
        }

        public static string GetFullPathUNC(string path)
        {
            return LongPathCommon.NormalizeLongPath(path);
        }

        public static string GetPathRoot(string path)
        {
            if (path == null)
                return null;
            if (path == string.Empty || path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("Path cannot be empty or contain invalid characters.", nameof(path));
            if (!Path.IsPathRooted(path))
                return string.Empty;
            if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
            {
                var num = path.IndexOf(Path.DirectorySeparatorChar, "\\\\".Length);
                if (num == -1)
                    return path;
                var length = path.IndexOf(Path.DirectorySeparatorChar, num + 1);
                if (length == -1)
                    return path;
                return path.Substring(0, length);
            }

            if (path.IndexOf(Path.VolumeSeparatorChar) != 1)
                return string.Empty;
            if (path.Length <= 2 || path[2] != Path.DirectorySeparatorChar)
                return path.Substring(0, 2);
            return path.Substring(0, 3);
        }

        public static string Combine(string path, string file)
        {
            return string.Format("{0}\\{1}", path.TrimEnd('\\'), file.Trim('\\'));
        }

        public static string GetFileName(string path)
        {
            return Regex.Match(path, "\\\\[^\\\\]+$").Value.TrimStart('\\');
        }

        public static string GetExtension(string path)
        {
            if (path == null)
                return null;
            var str = Regex.Match(path.ToLowerInvariant(), "\\.[^\\.]+$").Value.TrimStart('.');
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return "." + str;
        }
    }
}