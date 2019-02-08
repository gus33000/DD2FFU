// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.InputStore
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class InputStore
    {
        [XmlArrayItem(ElementName = "Partition", IsNullable = false, Type = typeof(InputPartition))] [XmlArray]
        public InputPartition[] Partitions;

        public InputStore()
        {
        }

        public InputStore(string storeType)
        {
            StoreType = storeType;
        }

        public string Id { get; set; }

        public string StoreType { get; set; }

        public string DevicePath { get; set; }

         public uint SizeInSectors { get; set; }

        public bool OnlyAllocateDefinedGptEntries { get; set; }

        public bool IsMainOSStore()
        {
            return string.CompareOrdinal(StoreType, "MainOSStore") == 0;
        }
    }
}