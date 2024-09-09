// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageStorageException
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [Serializable]
    public class ImageStorageException : Exception
    {
        public ImageStorageException()
        {
        }

        public ImageStorageException(string message)
            : base(message)
        {
        }

        public ImageStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ImageStorageException(SerializationInfo info, StreamingContext context)
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