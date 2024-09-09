// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DiskStreamSource
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DiskStreamSource : IBlockStreamSource, IDisposable
    {
        private bool _alreadyDisposed;
        private readonly uint _blockSize;
        private VirtualMemoryPtr _buffer;
        private SafeFileHandle _handle;

        public DiskStreamSource(SafeFileHandle diskHandle, uint blockSize)
        {
            _blockSize = blockSize;
            _handle = diskHandle;
            _buffer = new VirtualMemoryPtr(blockSize);
            Length = (long)NativeImaging.GetSectorCount(nint.Zero, _handle) *
                     NativeImaging.GetSectorSize(nint.Zero, _handle);
        }

        public void ReadBlock(uint blockIndex, byte[] buffer, int bufferIndex)
        {
            Win32Exports.SetFilePointerEx(_handle, blockIndex * (int)_blockSize, out _,
                Win32Exports.MoveMethod.FILE_BEGIN);

            Win32Exports.ReadFile(_handle, (nint)_buffer, _blockSize, out _);
            Marshal.Copy(_buffer.AllocatedPointer, buffer, bufferIndex, (int)_blockSize);
        }

        public long Length
        {
            get;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DiskStreamSource()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (_handle != null)
                {
                    _handle = null;
                }

                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }
            }

            _alreadyDisposed = true;
        }
    }
}