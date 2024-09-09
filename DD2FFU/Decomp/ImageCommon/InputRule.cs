// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.InputRule
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public abstract class InputRule
    {
        public string Property
        {
            get; set;
        }

        public string Mode
        {
            get; set;
        }

        public char ModeCharacter => string.CompareOrdinal(Mode, "AFFIRMATIVE") == 0
                    ? 'A'
                    : string.CompareOrdinal(Mode, "NEGATIVE") == 0
                    ? 'N'
                    : string.CompareOrdinal(Mode, "OPTIONAL") == 0 ? 'O' : throw new ArgumentException("Mode");
    }
}