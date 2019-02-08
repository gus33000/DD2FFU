// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDeviceGptInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementDeviceGptInput
    {
        public string DiskId { get; set; }

        public GptPartitionInput Partition { get; set; }

        public static BcdElementDevice CreateGptBootDevice(BcdElementDeviceGptInput inputValue)
        {
            var baseBootDevice = BcdElementDevice.CreateBaseBootDevice();
            var empty1 = Guid.Empty;
            var empty2 = Guid.Empty;
            Guid diskId;
            Guid partitionId;
            try
            {
                diskId = new Guid(inputValue.DiskId);
                partitionId = inputValue.Partition.PartitionId;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException("Unable to parse the GPTDevice value.", ex);
            }

            var simpleGpt = PartitionIdentifierEx.CreateSimpleGpt(diskId, partitionId);
            baseBootDevice.ReplaceBootDeviceIdentifier(simpleGpt);
            return baseBootDevice;
        }
    }
}