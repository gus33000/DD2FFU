// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementObject
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementObject : BcdElement
    {
        public BcdElementObject(string value, BcdElementDataType dataType)
            : base(dataType)
        {
            StringData = value;
        }

        public Guid ElementObject
        {
            get
            {
                var result = Guid.Empty;
                if (!Guid.TryParse(StringData, out result))
                    throw new ImageStorageException(string.Format("{0}: The string data isn't a valid Guid.",
                        MethodBase.GetCurrentMethod().Name));
                return result;
            }
            set => StringData = value.ToString();
        }

        
        public override void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            logger.LogInfo(str + "Object ID: {{{0}}}", (object) ElementObject);
        }
    }
}