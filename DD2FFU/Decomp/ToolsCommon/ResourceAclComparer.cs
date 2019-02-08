// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.ResourceAclComparer
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class ResourceAclComparer : IEqualityComparer<ResourceAcl>
    {
        public bool Equals(ResourceAcl x, ResourceAcl y)
        {
            var flag = false;
            if (!string.IsNullOrEmpty(x.Path) && !string.IsNullOrEmpty(y.Path))
                flag = x.Path.Equals(y.Path, StringComparison.OrdinalIgnoreCase);
            return flag;
        }

        public int GetHashCode(ResourceAcl obj)
        {
            var num = 0;
            if (!string.IsNullOrEmpty(obj.Path))
                num = obj.Path.GetHashCode();
            return num;
        }
    }
}