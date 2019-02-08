using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using DD2FFU.DiskManagement;
using Microsoft.Win32.SafeHandles;

namespace DeviceFileStream
{
    internal class DeviceFileStream : Stream
    {
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const uint READ = 0x00000001;
        private const uint WRITE = 0x00000002;
        private const uint DEVICE = 0x00000040;
        private const uint NOBUFFERING = 0x20000000;
        private const uint WRITE_THROUGH = 0x80000000;

        private readonly FileStream stream;

        private bool disposed;
        private SafeFileHandle handleValue;

        private string PhysicalDiskId;

        private long length;

        public DeviceFileStream(string PhysicalDiskId, FileAccess mode = FileAccess.Read)
        {
            this.PhysicalDiskId = PhysicalDiskId;

            var diskHandle = CreateFile(@"\\.\PhysicalDrive" + PhysicalDiskId.ToLower().Replace(@"\\.\physicaldrive", ""),
                GENERIC_READ | GENERIC_WRITE, READ | WRITE, IntPtr.Zero, OPEN_EXISTING,
                DEVICE | NOBUFFERING | WRITE_THROUGH, IntPtr.Zero);
            if (diskHandle.ToInt32() == -1)
            {
                PrintWin32Error("Failed to open disk handle", Marshal.GetHRForLastWin32Error());
                Dispose();
                return;
            }

            GetFileSize(diskHandle, out length);

            handleValue = new SafeFileHandle(diskHandle, true);
            stream = new FileStream(handleValue, mode, 1024*2014, false);
        }

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;
        
        public override long Length
        {
            get
            {
                return GetDiskSize.GetDiskLength(@"\\.\PhysicalDrive" + PhysicalDiskId.ToLower().Replace(@"\\.\physicaldrive", ""));
            }
        }

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern bool GetFileSize(IntPtr hFile, out long lpFileSize);

        private static void PrintWin32Error(string Message, int ErrorCode)
        {
            var lpMsgBuf = new Win32Exception(ErrorCode).Message;
            Console.WriteLine(Message + ": " + lpMsgBuf);
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.End)
                return stream.Seek(Length - offset, SeekOrigin.Begin);
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            // Length throws an exception
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            handleValue.Close();
            handleValue.Dispose();
            handleValue = null;
            base.Close();
        }

        new void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        new void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                if (disposing)
                    if (handleValue != null)
                    {
                        stream.Dispose();
                        handleValue.Close();
                        handleValue.Dispose();
                        handleValue = null;
                    }

                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}