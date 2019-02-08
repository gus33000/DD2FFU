// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DiskStream
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DiskStream : Stream, IDisposable
    {
        private const uint BUFFER_SIZE = 65536;
        private bool _alreadyDisposed;
        private VirtualMemoryPtr _buffer;
        private ulong _bufferOffsetOnDisk = ulong.MaxValue;
        private bool _canRead;
        private bool _canWrite;
        private SafeFileHandle _diskHandle;
        private bool _ownsDiskHandle;
        private long _position;
        private ulong _sectorCount;
        private uint _sectorSize;
        private ulong _sizeInBytes;

        public DiskStream(SafeFileHandle diskHandle, bool canRead, bool canWrite)
        {
            Initialize(diskHandle, false, canRead, canWrite);
        }

        public DiskStream(string diskPath, Win32Exports.DesiredAccess desiredAccess, Win32Exports.ShareMode shareMode)
        {
            Initialize(
                Win32Exports.CreateFile(diskPath, desiredAccess, shareMode,
                    Win32Exports.CreationDisposition.OPEN_EXISTING,
                    Win32Exports.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL), true,
                (desiredAccess | Win32Exports.DesiredAccess.GENERIC_READ) > 0,
                (desiredAccess | Win32Exports.DesiredAccess.GENERIC_WRITE) > 0);
        }

        public override bool CanRead => _canRead;

        public override bool CanWrite => _canWrite;

        public override bool CanSeek => false;

        public override long Length => (long) _sectorCount * _sectorSize;

        public override long Position
        {
            get => _position;
            set
            {
                if ((ulong) value > _sizeInBytes)
                    throw new ImageStorageException("The specified osition is beyond the end of the disk.");
                _position = value;
            }
        }

        ~DiskStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
                return;
            if (isDisposing)
            {
                if (_diskHandle != null)
                {
                    if (_ownsDiskHandle)
                        _diskHandle.Dispose();
                    _diskHandle = null;
                }

                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }
            }

            _alreadyDisposed = true;
        }

        private void Initialize(SafeFileHandle diskHandle, bool ownsDiskHandle, bool canRead, bool canWrite)
        {
            _ownsDiskHandle = ownsDiskHandle;
            _diskHandle = diskHandle;
            _canRead = canRead;
            _canWrite = canWrite;
            long newFileLocation = 0;
            Win32Exports.SetFilePointerEx(diskHandle, 0L, out newFileLocation, Win32Exports.MoveMethod.FILE_BEGIN);
            _sectorCount = NativeImaging.GetSectorCount(IntPtr.Zero, _diskHandle);
            _sectorSize = NativeImaging.GetSectorSize(IntPtr.Zero, _diskHandle);
            _sizeInBytes = _sectorCount * _sectorSize;
            _buffer = new VirtualMemoryPtr(65536U);
        }

        public override void Flush()
        {
            Win32Exports.FlushFileBuffers(_diskHandle);
        }

        private void FillBuffer(uint sectorIndex)
        {
            var num = (ulong) (sectorIndex * _sectorSize);
            uint bytesToRead = 65536;
            uint bytesRead = 0;
            if (_sizeInBytes - num < 65536UL)
            {
                Marshal.Copy(new byte[65536], 0, (IntPtr) _buffer, 65536);
                bytesToRead = (uint) (_sizeInBytes - num);
            }

            long newFileLocation;
            Win32Exports.SetFilePointerEx(_diskHandle, (long) num, out newFileLocation,
                Win32Exports.MoveMethod.FILE_BEGIN);
            Win32Exports.ReadFile(_diskHandle, (IntPtr) _buffer, bytesToRead, out bytesRead);
            _bufferOffsetOnDisk = num;
        }

        private bool OffsetIsInBuffer(ulong diskOffset)
        {
            return diskOffset >= _bufferOffsetOnDisk && diskOffset - _bufferOffsetOnDisk <= 65536UL;
        }

        private uint BytesInBuffer(ulong diskOffset)
        {
            if (!OffsetIsInBuffer(diskOffset))
                throw new ImageStorageException("Attempt to copy from outside the buffer range.");
            return (uint) (65536UL - (diskOffset - _bufferOffsetOnDisk));
        }

        private void CopyFromBuffer(byte[] destination, int destinationOffset, int count, ulong diskOffset,
            out uint bytesCopied)
        {
            if (!OffsetIsInBuffer(diskOffset))
                throw new ImageStorageException("Attempt to copy from outside the buffer range.");
            var num = Math.Min(BytesInBuffer(diskOffset), (uint) count);
            Marshal.Copy(((IntPtr) _buffer).Increment((int) (uint) (diskOffset - _bufferOffsetOnDisk)), destination,
                destinationOffset, (int) num);
            bytesCopied = num;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var num = 0;
            if (_sizeInBytes - (uint) count < (uint) offset)
                throw new ImageStorageException("Attempt to read beyond end of the disk.");
            while (count > 0)
            {
                if (!OffsetIsInBuffer((ulong) _position))
                    FillBuffer((uint) ((ulong) _position / _sectorSize));
                uint bytesCopied = 0;
                CopyFromBuffer(buffer, offset + num, count, (ulong) _position, out bytesCopied);
                num += (int) bytesCopied;
                count -= (int) bytesCopied;
                _position += bytesCopied;
            }

            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new ImageStorageException("This operation is not supported.");
        }

        public override void SetLength(long value)
        {
            throw new ImageStorageException("Cannot set the length of a disk stream.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new ImageStorageException("This operation is not implemented.");
        }
    }
}