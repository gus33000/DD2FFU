// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.IntPtrExtensions
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class IntPtrExtensions
    {
        public static IntPtr Increment(this IntPtr ptr, int cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }

        public static IntPtr Increment<T>(this IntPtr ptr)
        {
            return ptr.Increment(Marshal.SizeOf(typeof(T)));
        }

        public static T ElementAt<T>(this IntPtr ptr, int index)
        {
            var cbSize = Marshal.SizeOf(typeof(T)) * index;
            return (T) Marshal.PtrToStructure(ptr.Increment(cbSize), typeof(T));
        }
    }
}