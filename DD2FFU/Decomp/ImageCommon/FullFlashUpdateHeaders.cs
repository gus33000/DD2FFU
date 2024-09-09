// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.FullFlashUpdateHeaders
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

using System.Text;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class FullFlashUpdateHeaders
    {
        public static uint SecurityHeaderSize =>
            (uint)(FullFlashUpdateImage.SecureHeaderSize + GetSecuritySignature().Length);

        public static uint ImageHeaderSize =>
            (uint)(FullFlashUpdateImage.ImageHeaderSize + GetImageSignature().Length);

        public static byte[] GetSecuritySignature()
        {
            return Encoding.ASCII.GetBytes("SignedImage ");
        }

        public static byte[] GetImageSignature()
        {
            return Encoding.ASCII.GetBytes("ImageFlash  ");
        }
    }
}