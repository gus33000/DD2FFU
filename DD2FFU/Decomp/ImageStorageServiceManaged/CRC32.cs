// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.CRC32
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Security.Cryptography;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class CRC32 : HashAlgorithm
    {
        private static readonly uint[] _crc32Table = new uint[256];
        private uint _crc32Value;
        private bool _hashCoreCalled;
        private bool _hashFinalCalled;

        static CRC32()
        {
            for (uint index1 = 0; index1 < 256U; ++index1)
            {
                uint num = index1;
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    if (((int)num & 1) != 0)
                    {
                        num = 3988292384U ^ (num >> 1);
                    }
                    else
                    {
                        num >>= 1;
                    }
                }

                _crc32Table[(int)index1] = num;
            }
        }

        public CRC32()
        {
            InitializeVariables();
        }

        public override byte[] Hash
        {
            get
            {
                if (!_hashCoreCalled)
                {
                    throw new NullReferenceException();
                }

                if (!_hashFinalCalled)
                {
                    throw new CryptographicException("Hash must be finalized before the hash value is retrieved.");
                }

                byte[] bytes = BitConverter.GetBytes(~_crc32Value);
                Array.Reverse(bytes);
                return bytes;
            }
        }

        public override int HashSize => 32;

        public override void Initialize()
        {
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            _crc32Value = uint.MaxValue;
            _hashCoreCalled = false;
            _hashFinalCalled = false;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (_hashFinalCalled)
            {
                throw new CryptographicException("Hash not valid for use in specified state.");
            }

            _hashCoreCalled = true;
            for (int index = ibStart; index < ibStart + cbSize; ++index)
            {
                byte num = (byte)(_crc32Value ^ array[index]);
                _crc32Value = _crc32Table[num] ^ ((_crc32Value >> 8) & 16777215U);
            }
        }

        protected override byte[] HashFinal()
        {
            _hashFinalCalled = true;
            return Hash;
        }
    }
}