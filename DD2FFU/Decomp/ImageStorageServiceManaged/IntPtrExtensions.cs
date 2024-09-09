// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.IntPtrExtensions
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class IntPtrExtensions
    {
        public static nint Increment(this nint ptr, int cbSize)
        {
            return new nint(ptr.ToInt64() + cbSize);
        }

        public static nint Increment<T>(this nint ptr)
        {
            return ptr.Increment(Marshal.SizeOf(typeof(T)));
        }

        public static T ElementAt<T>(this nint ptr, int index)
        {
            int cbSize = Marshal.SizeOf(typeof(T)) * index;
            return (T)Marshal.PtrToStructure(ptr.Increment(cbSize), typeof(T));
        }
    }
}