// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VhdExtensions
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal static class VhdExtensions
    {
        public static void WriteStruct<T>(this FileStream writer, ref T structure) where T : struct
        {
            var length = Marshal.SizeOf(typeof(T));
            var num = Marshal.AllocHGlobal(length);
            var numArray = new byte[length];
            try
            {
                Marshal.StructureToPtr((object) structure, num, false);
                Marshal.Copy(num, numArray, 0, length);
                writer.Write(numArray, 0, length);
            }
            finally
            {
                if (IntPtr.Zero != num)
                    Marshal.FreeHGlobal(num);
            }
        }

        public static T ReadStruct<T>(this FileStream reader) where T : struct
        {
            var length = Marshal.SizeOf(typeof(T));
            var num = Marshal.AllocHGlobal(length);
            var numArray = new byte[length];
            try
            {
                reader.Read(numArray, 0, length);
                Marshal.Copy(numArray, 0, num, length);
                return (T) Marshal.PtrToStructure(num, typeof(T));
            }
            finally
            {
                if (IntPtr.Zero != num)
                    Marshal.FreeHGlobal(num);
            }
        }
    }
}