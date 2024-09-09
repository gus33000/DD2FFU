// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.SecurityWrapper
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class SecurityWrapper : IPayloadWrapper
    {
        private int bytesHashed;
        private readonly FullFlashUpdateImage ffuImage;
        private byte[] hashData;
        private int hashOffset;
        private Task hashTask;
        private readonly IPayloadWrapper innerWrapper;
        private SHA256 sha;

        public SecurityWrapper(FullFlashUpdateImage ffuImage, IPayloadWrapper innerWrapper)
        {
            this.ffuImage = ffuImage;
            this.innerWrapper = innerWrapper;
        }

        public byte[] CatalogData
        {
            get; private set;
        }

        public void InitializeWrapper(long payloadSize)
        {
            if (payloadSize % ffuImage.ChunkSizeInBytes != 0L)
            {
                throw new ImageCommonException("Data size not aligned with hash chunk size.");
            }

            sha = new SHA256CryptoServiceProvider();
            sha.Initialize();
            bytesHashed = 0;
            hashOffset = 0;
            hashData = new byte[(int)((uint)((ulong)payloadSize / ffuImage.ChunkSizeInBytes) *
                                       ((uint)sha.HashSize / 8U))];
            CatalogData = ImageSigner.GenerateCatalogFile(hashData);
            byte[] securityHeader = ffuImage.GetSecurityHeader(CatalogData, hashData);
            innerWrapper.InitializeWrapper(payloadSize + securityHeader.Length);
            innerWrapper.Write(securityHeader);
        }

        public void ResetPosition()
        {
            innerWrapper.ResetPosition();
        }

        public void Write(byte[] data)
        {
            HashBufferAsync(data);
            innerWrapper.Write(data);
        }

        public void FinalizeWrapper()
        {
            hashTask.Wait();
            hashTask = null;
            if (hashOffset != hashData.Length)
            {
                throw new ImageCommonException(string.Format(
                                "Failed to hash all data in the stream. hashOffset = {0}, hashData.Length = {1}, bytesHashed = {2}.",
                                hashOffset, hashData.Length, bytesHashed));
            }

            CatalogData = ImageSigner.GenerateCatalogFile(hashData);
            byte[] securityHeader = ffuImage.GetSecurityHeader(CatalogData, hashData);
            innerWrapper.ResetPosition();
            innerWrapper.Write(securityHeader);
            ffuImage.CatalogData = CatalogData;
            ffuImage.HashTableData = hashData;
            innerWrapper.FinalizeWrapper();
        }

        private void HashBufferAsync(byte[] data)
        {
            hashTask?.Wait();
            hashTask = Task.Factory.StartNew(() => HashBuffer(data));
        }

        private void HashBuffer(byte[] data)
        {
            int chunkSizeInBytes = (int)ffuImage.ChunkSizeInBytes;
            int num = chunkSizeInBytes - bytesHashed;
            int offset = 0;
            while (offset < data.Length)
            {
                int count = num;
                if (data.Length - offset < num)
                {
                    count = data.Length;
                }

                byte[] hash = sha.ComputeHash(data, offset, count);
                bytesHashed += count;
                bytesHashed %= chunkSizeInBytes;
                if (bytesHashed == 0)
                {
                    CommitHashToTable(hash);
                }

                num = chunkSizeInBytes;
                offset += chunkSizeInBytes;
            }
        }

        private void CommitHashToTable(byte[] hash)
        {
            hash.CopyTo(hashData, hashOffset);
            hashOffset += hash.Length;
        }
    }
}