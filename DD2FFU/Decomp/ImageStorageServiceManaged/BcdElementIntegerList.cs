// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementIntegerList
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementIntegerList : BcdElement
    {
        public BcdElementIntegerList(byte[] binaryData, BcdElementDataType dataType)
            : base(dataType)
        {
            SetBinaryData(binaryData);
        }

        
        public List<ulong> Value
        {
            get
            {
                var binaryData = GetBinaryData();
                var length = binaryData.Length / 8;
                var numArray = new ulong[length];
                for (var index1 = 0; index1 < length; ++index1)
                {
                    var index2 = index1 * 8;
                    var num1 = (uint) ((binaryData[index2 + 7] << 24) | (binaryData[index2 + 6] << 16) |
                                       (binaryData[index2 + 5] << 8)) | binaryData[index2 + 4];
                    var num2 = (uint) ((binaryData[index2 + 3] << 24) | (binaryData[index2 + 2] << 16) |
                                       (binaryData[index2 + 1] << 8)) | binaryData[index2];
                    numArray[index1] = ((ulong) num1 << 32) | num2;
                }

                return new List<ulong>(numArray);
            }
        }

        
        public override void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            foreach (var num in Value)
                logger.LogInfo(str + "Value: 0x{0:x16}", (object) num);
        }
    }
}