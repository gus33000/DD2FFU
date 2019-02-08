// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DataBlockSource
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DataBlockSource
    {
        public enum DataSource
        {
            Zero,
            Disk,
            Memory
        }

        private byte[] _memoryData;

        public DataSource Source { get; set; }

        public ulong StorageOffset { get; set; }

        public void SetMemoryData(byte[] buffer, int bufferOffset, int blockSize)
        {
            _memoryData = new byte[blockSize];
            Array.Copy(buffer, bufferOffset, _memoryData, 0, blockSize);
        }

        public void SetMemoryData(FileStream stream, int blockSize)
        {
            _memoryData = new byte[blockSize];
            stream.Read(_memoryData, 0, blockSize);
        }

        public void CreateMemoryData(int blockSize)
        {
            _memoryData = new byte[blockSize];
        }

        public byte[] GetMemoryData()
        {
            return _memoryData;
        }

        public byte[] GetNewMemoryData(uint blockSize)
        {
            _memoryData = new byte[(int) blockSize];
            return _memoryData;
        }

        public void LogInfo(IULogger logger, ushort indentLevel = 0)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Source           : {0}", (object) Source);
            if (Source == DataSource.Disk)
                logger.LogInfo(str + "  : {0}", (object) StorageOffset);
            logger.LogInfo("");
        }
    }
}