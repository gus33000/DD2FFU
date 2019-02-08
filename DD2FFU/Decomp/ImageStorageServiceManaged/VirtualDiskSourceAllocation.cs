// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualDiskSourceAllocation
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Reflection;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class VirtualDiskSourceAllocation : ISourceAllocation, IDisposable
    {
        private bool _alreadyDisposed;
        private readonly uint _sectorsPerVirtualBlock;
        private DynamicHardDisk _virtualDisk;
        private string _virtualDiskPath;

        public VirtualDiskSourceAllocation(string virtualDiskPath, uint alignmentSize)
        {
            _virtualDiskPath = virtualDiskPath;
            _virtualDisk = new DynamicHardDisk(virtualDiskPath, false);
            _sectorsPerVirtualBlock = _virtualDisk.BlockSize / _virtualDisk.SectorSize;
            if (_virtualDisk.BlockSize % alignmentSize != 0U)
                throw new ImageStorageException(string.Format(
                    "{0}: The virtual disk allocation size (0x{1:x}) is not a multiple of the given alignment size (0x{2:x}).",
                    MethodBase.GetCurrentMethod().Name, _virtualDisk.BlockSize, alignmentSize));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool BlockIsAllocated(ulong diskByteOffset)
        {
            return _virtualDisk.AllocationTable[
                       (uint) (diskByteOffset / _virtualDisk.SectorSize / _sectorsPerVirtualBlock)] != uint.MaxValue;
        }

        public uint GetAllocationSize()
        {
            return _virtualDisk.BlockSize;
        }

        ~VirtualDiskSourceAllocation()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
                return;
            if (isDisposing)
            {
                _virtualDiskPath = null;
                if (_virtualDisk != null)
                {
                    _virtualDisk.Dispose();
                    _virtualDisk = null;
                }
            }

            _alreadyDisposed = true;
        }
    }
}