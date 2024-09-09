// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.SafeVolumeHandle
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public sealed class SafeVolumeHandle : SafeHandle
    {
        private bool _disposed;
        private ImageStorage _storage;

        public SafeVolumeHandle(ImageStorage storage, string partitionName)
            : base(nint.Zero, true)
        {
            _storage = storage;
            VolumeHandle = storage.OpenVolumeHandle(partitionName);
        }

        public SafeFileHandle VolumeHandle
        {
            get; private set;
        }

        public override bool IsInvalid => _disposed;

        public static implicit operator nint(SafeVolumeHandle safeVolumeHandle)
        {
            return safeVolumeHandle.VolumeHandle.DangerousGetHandle();
        }

        protected override bool ReleaseHandle()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
                if (VolumeHandle != null)
                {
                    VolumeHandle.Close();
                    VolumeHandle = null;
                }

                _storage = null;
            }

            return true;
        }
    }
}