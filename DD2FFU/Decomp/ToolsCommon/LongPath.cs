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
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null.");
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException("Path cannot contain invalid characters.", nameof(path));
            }

            int length = path.LastIndexOfAny(new char[2]
                        {
                Path.DirectorySeparatorChar,
                Path.VolumeSeparatorChar
                        });
            return length == -1 ? null : path[..length];
        }

        public static string GetFullPath(string path)
        {
            string str = LongPathCommon.NormalizeLongPath(path);
            return str.StartsWith("\\\\?\\UNC\\", StringComparison.OrdinalIgnoreCase)
                ? "\\\\" + str["\\\\?\\UNC\\".Length..]
                : str.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase) ? str["\\\\?\\".Length..] : str;
        }

        public static string GetFullPathUNC(string path)
        {
            return LongPathCommon.NormalizeLongPath(path);
        }

        public static string GetPathRoot(string path)
        {
            if (path == null)
            {
                return null;
            }

            if (path == string.Empty || path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException("Path cannot be empty or contain invalid characters.", nameof(path));
            }

            if (!Path.IsPathRooted(path))
            {
                return string.Empty;
            }

            if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
            {
                int num = path.IndexOf(Path.DirectorySeparatorChar, "\\\\".Length);
                if (num == -1)
                {
                    return path;
                }

                int length = path.IndexOf(Path.DirectorySeparatorChar, num + 1);
                return length == -1 ? path : path[..length];
            }

            return path.IndexOf(Path.VolumeSeparatorChar) != 1
                ? string.Empty
                : path.Length <= 2 || path[2] != Path.DirectorySeparatorChar ? path[..2] : path[..3];
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
            {
                return null;
            }

            string str = Regex.Match(path.ToLowerInvariant(), "\\.[^\\.]+$").Value.TrimStart('.');
            return string.IsNullOrEmpty(str) ? string.Empty : "." + str;
        }
    }
}