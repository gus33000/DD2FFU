// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualMemoryPtr
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public sealed class VirtualMemoryPtr : SafeHandle
    {
        private readonly UIntPtr _memorySize;
        private bool _disposed;

        
        public VirtualMemoryPtr(uint memorySize)
            : base(IntPtr.Zero, true)
        {
            _memorySize = (UIntPtr) memorySize;
            try
            {
                AllocatedPointer = Win32Exports.VirtualAlloc(_memorySize,
                    Win32Exports.AllocationType.MEM_COMMIT | Win32Exports.AllocationType.MEM_RESERVE,
                    Win32Exports.MemoryProtection.PAGE_READWRITE);
            }
            catch (Win32ExportException ex)
            {
                throw new ImageStorageException("Unable to create the virtual memory pointer.", ex);
            }
        }

        public IntPtr AllocatedPointer { get; }

         public uint MemorySize => (uint) _memorySize;

        public override bool IsInvalid => _disposed;

        public static implicit operator IntPtr(VirtualMemoryPtr virtualMemoryPointer)
        {
            return virtualMemoryPointer.AllocatedPointer;
        }

        protected override bool ReleaseHandle()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
                try
                {
                    Win32Exports.VirtualFree(AllocatedPointer, Win32Exports.FreeType.MEM_RELEASE);
                }
                catch (Win32ExportException ex)
                {
                    throw new ImageStorageException(string.Format("Unable to free the virtual memory pointer.", ex));
                }
            }

            return true;
        }
    }
}