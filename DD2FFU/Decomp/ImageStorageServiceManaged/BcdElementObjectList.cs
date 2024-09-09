// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementObjectList
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementObjectList : BcdElement
    {
        public BcdElementObjectList(string[] multiStringData, BcdElementDataType dataType)
            : base(dataType)
        {
            _multiStringData = new List<string>(multiStringData);
        }

        public List<Guid> ObjectList
        {
            get
            {
                List<Guid> guidList = new(MultiStringData.Count);
                for (int index = 0; index < MultiStringData.Count; ++index)
                {
                    if (!Guid.TryParse(MultiStringData[index], out Guid result))
                    {
                        throw new ImageStorageException(string.Format("{0}: The string data isn't a valid Guid.",
                                                MethodBase.GetCurrentMethod().Name));
                    }

                    guidList.Add(result);
                }

                return guidList;
            }
        }


        public override void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            foreach (Guid guid in ObjectList)
            {
                logger.LogInfo(str + "Object ID: {{{0}}}", guid);
            }
        }
    }
}