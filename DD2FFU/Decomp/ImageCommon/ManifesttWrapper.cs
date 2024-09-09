// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ManifestWrapper
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ManifestWrapper : IPayloadWrapper
    {
        private readonly FullFlashUpdateImage ffuImage;
        private readonly IPayloadWrapper innerWrapper;

        public ManifestWrapper(FullFlashUpdateImage ffuImage, IPayloadWrapper innerWrapper)
        {
            this.ffuImage = ffuImage;
            this.innerWrapper = innerWrapper;
        }

        public void InitializeWrapper(long payloadSize)
        {
            byte[] manifestRegion = ffuImage.GetManifestRegion();
            innerWrapper.InitializeWrapper(payloadSize + manifestRegion.Length);
            innerWrapper.Write(manifestRegion);
        }

        public void ResetPosition()
        {
            innerWrapper.ResetPosition();
        }

        public void Write(byte[] data)
        {
            innerWrapper.Write(data);
        }

        public void FinalizeWrapper()
        {
            innerWrapper.FinalizeWrapper();
        }
    }
}