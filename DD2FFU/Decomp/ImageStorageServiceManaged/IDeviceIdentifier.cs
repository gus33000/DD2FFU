// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.IDeviceIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{

    public interface IDeviceIdentifier
    {
        uint Size
        {
            get;
        }

        BcdElementBootDevice Parent
        {
            get; set;
        }
        void ReadFromStream(BinaryReader reader);

        void WriteToStream(BinaryWriter writer);

        void LogInfo(IULogger logger, int indentLevel);
    }
}