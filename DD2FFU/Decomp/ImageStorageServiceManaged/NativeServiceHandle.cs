// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.NativeServiceHandle
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public sealed class NativeServiceHandle : SafeHandle
    {
        private bool _disposed;

        public NativeServiceHandle(LogFunction logError)
            : base(IntPtr.Zero, true)
        {
            ServiceHandle = NativeImaging.CreateImageStorageService(logError);
            if (ServiceHandle == IntPtr.Zero)
                throw new ImageStorageException("Unable to create the image storage service.");
        }

        public IntPtr ServiceHandle { get; }

        public override bool IsInvalid => _disposed;

        public static implicit operator IntPtr(NativeServiceHandle virtualServiceHandle)
        {
            return virtualServiceHandle.ServiceHandle;
        }

        protected override bool ReleaseHandle()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
                if (ServiceHandle != IntPtr.Zero)
                    NativeImaging.CloseImageStorageService(ServiceHandle);
            }

            return true;
        }
    }
}