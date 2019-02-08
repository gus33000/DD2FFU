// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementInteger
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementInteger : BcdElement
    {
        public BcdElementInteger(byte[] binaryData, BcdElementDataType dataType)
            : base(dataType)
        {
            SetBinaryData(binaryData);
        }

        
        public ulong Value
        {
            get
            {
                var binaryData = GetBinaryData();
                uint num1 = 0;
                for (var index = 4; index < Math.Min(binaryData.Length, 8); ++index)
                    num1 |= (uint) binaryData[index] << ((index - 4) * 8);
                uint num2 = 0;
                for (var index = 0; index < Math.Min(binaryData.Length, 4); ++index)
                    num2 |= (uint) binaryData[index] << (index * 8);
                return ((ulong) num1 << 32) | num2;
            }
        }

        
        public override void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            try
            {
                logger.LogInfo(str + "Value: 0x{0:x}", (object) Value);
            }
            catch (ImageStorageException ex)
            {
                logger.LogInfo(str + "Value: <invalid data>");
            }
        }
    }
}