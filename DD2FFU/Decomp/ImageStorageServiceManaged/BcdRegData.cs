// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdRegData
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdRegData
    {
        private readonly Dictionary<string, List<BcdRegValue>> _regKeys =
            new(StringComparer.OrdinalIgnoreCase);

        public void AddRegKey(string regKey)
        {
            if (_regKeys.ContainsKey(regKey))
            {
                return;
            }

            _regKeys.Add(regKey, []);
        }

        public Dictionary<string, List<BcdRegValue>> RegKeys()
        {
            return _regKeys;
        }

        public void AddRegValue(string regKey, string name, string value, string type)
        {
            if (!_regKeys.ContainsKey(regKey))
            {
                AddRegKey(regKey);
            }

            BcdRegValue bcdRegValue = new(name, value, type);
            _regKeys[regKey].Add(bcdRegValue);
        }
    }
}