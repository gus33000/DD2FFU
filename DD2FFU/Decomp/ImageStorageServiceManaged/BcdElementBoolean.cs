// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementBoolean
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementBoolean : BcdElement
    {
        public BcdElementBoolean(byte[] binaryData, BcdElementDataType dataType)
            : base(dataType)
        {
            SetBinaryData(binaryData);
        }

        public bool Value
        {
            get => GetBinaryData()[0] > 0;
            set
            {
                if (value)
                    GetBinaryData()[0] = 1;
                else
                    GetBinaryData()[0] = 0;
            }
        }

        
        public override void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            logger.LogInfo(str + "Value: {0}", (object) Value);
        }
    }
}