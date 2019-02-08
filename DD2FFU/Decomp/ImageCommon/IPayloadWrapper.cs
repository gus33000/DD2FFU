// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.IPayloadWrapper
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public interface IPayloadWrapper
    {
        void InitializeWrapper(long payloadSize);

        void ResetPosition();

        void Write(byte[] data);

        void FinalizeWrapper();
    }
}