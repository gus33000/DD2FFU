// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.OutputWrapper
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class OutputWrapper : IPayloadWrapper
    {
        private FileStream fileStream;
        private readonly string path;
        private readonly Queue<IAsyncResult> writes;

        public OutputWrapper(string path)
        {
            this.path = path;
            writes = new Queue<IAsyncResult>();
        }

        public void InitializeWrapper(long payloadSize)
        {
            fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1048576, true);
            fileStream.SetLength(payloadSize);
        }

        public void ResetPosition()
        {
            fileStream.Seek(0L, SeekOrigin.Begin);
        }

        public void Write(byte[] data)
        {
            while (writes.Count > 0 && writes.Peek().IsCompleted)
                fileStream.EndWrite(writes.Dequeue());
            writes.Enqueue(fileStream.BeginWrite(data, 0, data.Length, null, null));
        }

        public void FinalizeWrapper()
        {
            while (writes.Count > 0)
                fileStream.EndWrite(writes.Dequeue());
            if (fileStream == null)
                return;
            fileStream.Close();
            fileStream = null;
        }
    }
}