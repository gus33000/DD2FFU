// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BlockIoIdentifierFactory
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class BlockIoIdentifierFactory
    {
         public static readonly uint SizeOnDisk = 40;

        
        public static IBlockIoIdentifier CreateFromStream(BinaryReader reader)
        {
            switch (reader.ReadUInt32())
            {
                case 0:
                    return new HardDiskIdentifier();
                case 1:
                    return new RemovableDiskIdentifier();
                case 2:
                    return new CdRomIdentifier();
                case 3:
                    return new RamDiskIdentifier("", BcdElementBootDevice.CreateBaseBootDevice());
                case 5:
                    return new FileIdentifier("", BcdElementBootDevice.CreateBaseBootDevice());
                case 6:
                    return new VirtualDiskIdentifier();
                default:
                    throw new ImageStorageException("The block IO type is unrecognized.");
            }
        }
    }
}