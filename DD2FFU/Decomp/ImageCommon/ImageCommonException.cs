// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageCommonException
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

using System;
using System.Runtime.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [Serializable]
    public class ImageCommonException : Exception
    {
        public ImageCommonException()
        {
        }

        public ImageCommonException(string message)
            : base(message)
        {
        }

        public ImageCommonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ImageCommonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            var message = Message;
            if (InnerException != null)
                message += InnerException.ToString();
            return message;
        }
    }
}