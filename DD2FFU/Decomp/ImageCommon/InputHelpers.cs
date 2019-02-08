// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.InputHelpers
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

using System;
using System.Globalization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class InputHelpers
    {
        public static bool StringToUint(string valueAsString, out uint value)
        {
            var flag = true;
            if (valueAsString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (!uint.TryParse(valueAsString.Substring(2, valueAsString.Length - 2), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out value))
                    flag = false;
            }
            else if (!uint.TryParse(valueAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                flag = false;
            }

            return flag;
        }

        public static bool StringToUint64(string valueAsString, out ulong value)
        {
            var flag = true;
            if (valueAsString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (!ulong.TryParse(valueAsString.Substring(2, valueAsString.Length - 2), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out value))
                    flag = false;
            }
            else if (!ulong.TryParse(valueAsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                flag = false;
            }

            return flag;
        }

        public static bool IsPowerOfTwo(uint value)
        {
            var num = (int) value;
            return (num & (num - 1)) == 0;
        }
    }
}