// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VhdCommon
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Net;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal static class VhdCommon
    {
        public static uint VHDSectorSize = 512;
        public static uint DynamicVHDBlockSize = 2097152;

        public static uint Swap32(uint data)
        {
            return (uint)IPAddress.HostToNetworkOrder((int)data);
        }

        public static ulong Swap64(ulong data)
        {
            return (ulong)IPAddress.HostToNetworkOrder((long)data);
        }

        public static uint CalculateChecksum<T>(ref T type) where T : struct
        {
            uint num1 = 0;
            int length = Marshal.SizeOf(typeof(T));
            nint num2 = Marshal.AllocHGlobal(length);
            byte[] destination = new byte[length];
            try
            {
                Marshal.StructureToPtr((object)type, num2, false);
                Marshal.Copy(num2, destination, 0, length);
                foreach (byte num3 in destination)
                {
                    num1 += num3;
                }

                return ~num1;
            }
            finally
            {
                if (nint.Zero != num2)
                {
                    Marshal.FreeHGlobal(num2);
                }
            }
        }

        public static uint Round(uint number, uint roundTo)
        {
            return (uint)((int)number + (int)roundTo - 1) / roundTo * roundTo;
        }
    }
}