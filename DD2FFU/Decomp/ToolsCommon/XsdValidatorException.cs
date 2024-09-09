// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.XsdValidatorException
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Runtime.Serialization;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    [Serializable]
    public class XsdValidatorException : Exception
    {
        public XsdValidatorException()
        {
        }

        public XsdValidatorException(string message)
            : base(message)
        {
        }

        public XsdValidatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected XsdValidatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            string message = Message;
            if (InnerException != null)
            {
                message += InnerException.ToString();
            }

            return message;
        }
    }
}