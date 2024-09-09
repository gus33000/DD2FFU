// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BootIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BootIdentifier : BaseIdentifier, IDeviceIdentifier
    {
        public void ReadFromStream(BinaryReader reader)
        {
            _ = reader.ReadBytes((int)Size);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            byte[] buffer = new byte[(int)Size];
            for (int index = 0; index < buffer.Length; ++index)
            {
                buffer[index] = 0;
            }

            writer.Write(buffer, 0, buffer.Length);
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Boot Identifier");
        }


        public uint Size => BcdElementBootDevice.BaseBootDeviceSizeInBytes - BcdElementBootDevice.BaseSize;
    }
}