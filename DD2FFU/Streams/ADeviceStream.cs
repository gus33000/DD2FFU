﻿using DD2FFU.DiskManagement;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReadFromDevice
{
    public class DeviceStream : Stream
    {
        private const uint GENERIC_READ = 0x80000000;
        private const uint OPEN_EXISTING = 3;
        private const uint DEVICE = 0x00000040;
        private const uint NOBUFFERING = 0x20000000;

        private readonly string PhysicalDiskId;
        private long _Position = 0;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern nint CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, nint lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, nint hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            nint hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            ref int lpNumberOfBytesRead,
            nint lpOverlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            nint lpInBuffer,
            uint nInBufferSize,
            nint lpOutBuffer,
            int nOutBufferSize,
            ref uint lpBytesReturned,
            nint lpOverlapped
            );

        [DllImport("kernel32.dll")]
        private static extern bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, uint dwMoveMethod);

        private SafeFileHandle handleValue = null;
        private long _length;

        private static readonly uint FILE_DEVICE_FILE_SYSTEM = 9;
        private static readonly uint METHOD_BUFFERED = 0;
        private static readonly uint FILE_ANY_ACCESS = 0x00000000;

        private static readonly uint FSCTL_LOCK_VOLUME = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 6, METHOD_BUFFERED, FILE_ANY_ACCESS);
        private static readonly uint FSCTL_UNLOCK_VOLUME = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 7, METHOD_BUFFERED, FILE_ANY_ACCESS);

        private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return (DeviceType << 16) | (Access << 14) | (Function << 2) | Method;
        }

        public DeviceStream(string device)
        {
            PhysicalDiskId = device;
            Load(device);
        }

        private void Load(string Path)
        {
            if (string.IsNullOrEmpty(Path))
            {
                throw new ArgumentNullException("Path");
            }

            _length = GetDiskSize.GetDiskLength(@"\\.\PhysicalDrive" + PhysicalDiskId.ToLower().Replace(@"\\.\physicaldrive", ""));
            nint ptr = CreateFile(Path, GENERIC_READ, 0, nint.Zero, OPEN_EXISTING, DEVICE, nint.Zero);
            handleValue = new SafeFileHandle(ptr, true);

            if (handleValue.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            uint lpBytesReturned = 0;
            uint result = DeviceIoControl(handleValue, FSCTL_LOCK_VOLUME, nint.Zero, 0, nint.Zero, 0, ref lpBytesReturned, nint.Zero);

            if (0 == result)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override void Flush()
        {
            return;
        }

        public override long Length => _length;

        public override long Position
        {
            get => _Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and 
        /// (offset + count - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int BytesRead = 0;
            byte[] BufBytes = new byte[count];
            if (!ReadFile(handleValue.DangerousGetHandle(), BufBytes, count, ref BytesRead, nint.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            for (int i = 0; i < BytesRead; i++)
            {
                buffer[offset + i] = BufBytes[i];
            }

            _Position += count;

            return BytesRead;
        }

        public override int ReadByte()
        {
            int BytesRead = 0;
            byte[] lpBuffer = new byte[1];
            if (!ReadFile(
            handleValue.DangerousGetHandle(),                        // handle to file
            lpBuffer,                // data buffer
            1,        // number of bytes to read
            ref BytesRead,    // number of bytes read
            nint.Zero
            ))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                ;
            }

            _Position += 1;

            return lpBuffer[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long off = offset;

            switch (origin)
            {
                case SeekOrigin.Current:
                    off += _Position;
                    break;
                case SeekOrigin.End:
                    off += _length;
                    break;
            }

            if (!SetFilePointerEx(handleValue, off, out long ret, 0))
            {
                return _Position;
            }

            _Position = ret;

            return ret;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            uint lpBytesReturned = 0;
            uint result = DeviceIoControl(handleValue, FSCTL_UNLOCK_VOLUME, nint.Zero, 0, nint.Zero, 0, ref lpBytesReturned, nint.Zero);

            if (0 == result)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            handleValue.Close();
            handleValue.Dispose();
            handleValue = null;
            base.Close();
        }
        private bool disposed = false;

        private new void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private new void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                if (disposing)
                {
                    if (handleValue != null)
                    {
                        uint lpBytesReturned = 0;
                        uint result = DeviceIoControl(handleValue, FSCTL_UNLOCK_VOLUME, nint.Zero, 0, nint.Zero, 0, ref lpBytesReturned, nint.Zero);

                        if (0 == result)
                        {
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        }

                        handleValue.Close();
                        handleValue.Dispose();
                        handleValue = null;
                    }
                }
                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}