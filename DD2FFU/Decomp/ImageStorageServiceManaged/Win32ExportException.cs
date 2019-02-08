// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.Win32ExportException
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [Serializable]
    public class Win32ExportException : Exception
    {
        public Win32ExportException()
        {
        }

        public Win32ExportException(string message)
            : base(message)
        {
        }

        public Win32ExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected Win32ExportException(SerializationInfo info, StreamingContext context)
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