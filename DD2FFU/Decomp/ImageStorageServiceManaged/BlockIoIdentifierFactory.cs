// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BlockIoIdentifierFactory
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class BlockIoIdentifierFactory
    {
        public static readonly uint SizeOnDisk = 40;


        public static IBlockIoIdentifier CreateFromStream(BinaryReader reader)
        {
            return reader.ReadUInt32() switch
            {
                0 => new HardDiskIdentifier(),
                1 => new RemovableDiskIdentifier(),
                2 => new CdRomIdentifier(),
                3 => new RamDiskIdentifier("", BcdElementBootDevice.CreateBaseBootDevice()),
                5 => new FileIdentifier("", BcdElementBootDevice.CreateBaseBootDevice()),
                6 => new VirtualDiskIdentifier(),
                _ => throw new ImageStorageException("The block IO type is unrecognized."),
            };
        }
    }
}