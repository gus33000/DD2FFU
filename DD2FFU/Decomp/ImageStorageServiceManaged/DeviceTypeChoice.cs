// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DeviceTypeChoice
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType(IncludeInSchema = false)]
    public enum DeviceTypeChoice
    {
        GPTDevice,
        MBRDevice,
        RamdiskDevice
    }
}