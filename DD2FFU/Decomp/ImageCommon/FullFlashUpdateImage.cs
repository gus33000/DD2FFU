// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.FullFlashUpdateImage
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class FullFlashUpdateImage
    {
        private const uint _OneKiloByte = 1024;
        private const string _version = "2.0";
        private const uint _DefaultPartitionByteAlignment = 65536;
        public static readonly uint PartitionTypeMbr = 0;
        public static readonly uint PartitionTypeGpt = 1;
        private uint _defaultPartititionByteAlignment = 65536;
        private ImageHeader _imageHeader;
        private string _imagePath;
        private long _payloadOffset;
        private SecurityHeader _securityHeader;

        public SecurityHeader GetSecureHeader => _securityHeader;

        public static int SecureHeaderSize => Marshal.SizeOf((object)new SecurityHeader());

        public byte[] CatalogData
        {
            get; set;
        }

        public byte[] HashTableData
        {
            get; set;
        }

        public ImageHeader GetImageHeader => _imageHeader;

        public static int ImageHeaderSize => Marshal.SizeOf((object)new ImageHeader());

        public uint ChunkSize
        {
            get => _imageHeader.ChunkSize;
            set => _imageHeader.ChunkSize = value;
        }

        public uint ChunkSizeInBytes => ChunkSize * 1024U;

        public uint HashAlgorithmID
        {
            get => _securityHeader.HashAlgorithmID;
            set => _securityHeader.HashAlgorithmID = value;
        }

        public uint ManifestLength
        {
            get => _imageHeader.ManifestLength;
            set => _imageHeader.ManifestLength = value;
        }

        public int StoreCount => Stores.Count;

        public List<FullFlashUpdateStore> Stores { get; } = [];

        public uint ImageStyle
        {
            get
            {
                bool flag = true;
                if (Stores[0].Partitions != null && Stores[0].Partitions.Count() > 0)
                {
                    flag = IsGPTPartitionType(Stores[0].Partitions[0].PartitionType);
                }

                return !flag ? PartitionTypeMbr : PartitionTypeGpt;
            }
        }

        public FullFlashUpdatePartition this[string name]
        {
            get
            {
                foreach (FullFlashUpdateStore store in Stores)
                {
                    foreach (FullFlashUpdatePartition partition in store.Partitions)
                    {
                        if (string.CompareOrdinal(partition.Name, name) == 0)
                        {
                            return partition;
                        }
                    }
                }

                return null;
            }
        }

        public uint StartOfImageHeader
        {
            get
            {
                uint num = 0;
                if (GetManifest != null)
                {
                    num = num + FullFlashUpdateHeaders.SecurityHeaderSize + _securityHeader.CatalogSize +
                                          _securityHeader.HashTableSize;
                }

                return num;
            }
        }

        public FullFlashUpdateManifest GetManifest
        {
            get; private set;
        }

        public uint DefaultPartitionAlignmentInBytes
        {
            get => _defaultPartititionByteAlignment;
            set
            {
                if (!InputHelpers.IsPowerOfTwo(value))
                {
                    return;
                }

                _defaultPartititionByteAlignment = value;
            }
        }

        public uint SecurityPadding
        {
            get
            {
                uint num = 1024;
                uint blockSize;
                if (_imageHeader.ChunkSize != 0U)
                {
                    blockSize = num * _imageHeader.ChunkSize;
                }
                else
                {
                    if (_securityHeader.ChunkSize == 0U)
                    {
                        throw new ImageCommonException(
                                                "ImageCommon!FullFlashUpdateImage::SecurityPadding: Neither the of the headers have been initialized with a chunk size.");
                    }

                    blockSize = num * _securityHeader.ChunkSize;
                }

                return CalculateAlignment(
                    (uint)((int)FullFlashUpdateHeaders.SecurityHeaderSize +
                            (CatalogData != null ? CatalogData.Length : 0) +
                            (HashTableData != null ? HashTableData.Length : 0)), blockSize);
            }
        }

        public string Description
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(Description)] != null
                    ? GetManifest["FullFlash"][nameof(Description)]
                    : "";
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(Description)] = value;
            }
        }

        public List<string> DevicePlatformIDs
        {
            get
            {
                List<string> stringList = [];
                if (GetManifest == null || GetManifest["FullFlash"] == null)
                {
                    return stringList;
                }

                int num = 0;
                while (GetManifest["FullFlash"][string.Format("DevicePlatformId{0}", num)] != null)
                {
                    ++num;
                }

                for (int index1 = 0; index1 < num; ++index1)
                {
                    string index2 = string.Format("DevicePlatformId{0}", index1);
                    stringList.Add(GetManifest["FullFlash"][index2]);
                }

                return stringList;
            }
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                for (int index = 0; index < value.Count; ++index)
                {
                    GetManifest["FullFlash"][string.Format("DevicePlatformId{0}", index)] = value[index];
                }
            }
        }

        public string Version
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(Version)] != null
                    ? GetManifest["FullFlash"][nameof(Version)]
                    : string.Empty;
            private set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(Version)] = value;
            }
        }

        public string OSVersion
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(OSVersion)] != null
                    ? GetManifest["FullFlash"][nameof(OSVersion)]
                    : string.Empty;
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(OSVersion)] = value;
            }
        }

        public string CanFlashToRemovableMedia
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(CanFlashToRemovableMedia)] != null
                    ? GetManifest["FullFlash"][nameof(CanFlashToRemovableMedia)]
                    : string.Empty;
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(CanFlashToRemovableMedia)] = value;
            }
        }

        public string AntiTheftVersion
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(AntiTheftVersion)] != null
                    ? GetManifest["FullFlash"][nameof(AntiTheftVersion)]
                    : string.Empty;
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(AntiTheftVersion)] = value;
            }
        }

        public string RulesVersion
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(RulesVersion)] != null
                    ? GetManifest["FullFlash"][nameof(RulesVersion)]
                    : string.Empty;
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(RulesVersion)] = value;
            }
        }

        public string RulesData
        {
            get => GetManifest != null && GetManifest["FullFlash"] != null &&
                    GetManifest["FullFlash"][nameof(RulesData)] != null
                    ? GetManifest["FullFlash"][nameof(RulesData)]
                    : string.Empty;
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(RulesData)] = value;
            }
        }

        public bool UEFI
        {
            get => GetManifest == null || GetManifest["FullFlash"] == null ||
                    GetManifest["FullFlash"][nameof(UEFI)] == null
|| GetManifest["FullFlash"][nameof(UEFI)].Equals("True", StringComparison.OrdinalIgnoreCase);
            set
            {
                if (GetManifest == null)
                {
                    return;
                }

                if (GetManifest["FullFlash"] == null)
                {
                    _ = GetManifest.AddCategory("FullFlash", "FullFlash");
                }

                GetManifest["FullFlash"][nameof(UEFI)] = value.ToString();
            }
        }

        public void Initialize(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::Initialize: The FFU file '" +
                                                           imagePath + "' does not exist.");
            }

            _imagePath = Path.GetFullPath(imagePath);
            using FileStream imageStream = GetImageStream();
            using BinaryReader binaryReader = new(imageStream);
            uint num1 = binaryReader.ReadUInt32();
            byte[] signature1 = binaryReader.ReadBytes(12);
            if ((int)num1 != (int)FullFlashUpdateHeaders.SecurityHeaderSize ||
                !SecurityHeader.ValidateSignature(signature1))
            {
                throw new ImageCommonException(
                                        "ImageCommon!FullFlashUpdateImage::Initialize: Unable to load image because the security header is invalid.");
            }

            _securityHeader.ByteCount = num1;
            _securityHeader.ChunkSize = binaryReader.ReadUInt32();
            _securityHeader.HashAlgorithmID = binaryReader.ReadUInt32();
            _securityHeader.CatalogSize = binaryReader.ReadUInt32();
            _securityHeader.HashTableSize = binaryReader.ReadUInt32();
            CatalogData = binaryReader.ReadBytes((int)_securityHeader.CatalogSize);
            HashTableData = binaryReader.ReadBytes((int)_securityHeader.HashTableSize);
            _ = binaryReader.ReadBytes((int)SecurityPadding);
            uint num2 = binaryReader.ReadUInt32();
            byte[] signature2 = binaryReader.ReadBytes(12);
            if ((int)num2 != (int)FullFlashUpdateHeaders.ImageHeaderSize ||
                !ImageHeader.ValidateSignature(signature2))
            {
                throw new ImageCommonException(
                                        "ImageCommon!FullFlashUpdateImage::Initialize: Unable to load image because the image header is invalid.");
            }

            _imageHeader.ByteCount = num2;
            _imageHeader.ManifestLength = binaryReader.ReadUInt32();
            _imageHeader.ChunkSize = binaryReader.ReadUInt32();
            StreamReader manifestStream =
                new(new MemoryStream(binaryReader.ReadBytes((int)_imageHeader.ManifestLength)),
                    Encoding.ASCII);
            try
            {
                GetManifest = new FullFlashUpdateManifest(this, manifestStream);
            }
            finally
            {
                manifestStream.Close();
            }

            ValidateManifest();
            if (_imageHeader.ChunkSize > 0U)
            {
                _ = binaryReader.ReadBytes((int)CalculateAlignment((uint)imageStream.Position,
                                        _imageHeader.ChunkSize * 1024U));
            }

            _payloadOffset = imageStream.Position;
        }

        public FileStream GetImageStream()
        {
            FileStream fileStream = File.OpenRead(_imagePath);
            fileStream.Position = _payloadOffset;
            return fileStream;
        }

        public void AddStore(FullFlashUpdateStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            Stores.Add(store);
        }

        private void AddStore(ManifestCategory category)
        {
            uint sectorSize = uint.Parse(category["SectorSize"], CultureInfo.InvariantCulture);
            uint minSectorCount = 0;
            if (category["MinSectorCount"] != null)
            {
                minSectorCount = uint.Parse(category["MinSectorCount"], CultureInfo.InvariantCulture);
            }

            string storeId = null;
            if (category["StoreId"] != null)
            {
                storeId = category["StoreId"];
            }

            bool isMainOSStore = true;
            if (category["IsMainOSStore"] != null)
            {
                isMainOSStore = bool.Parse(category["IsMainOSStore"]);
            }

            string devicePath = null;
            if (category["DevicePath"] != null)
            {
                devicePath = category["DevicePath"];
            }

            bool onlyAllocateDefinedGptEntries = false;
            if (category["OnlyAllocateDefinedGptEntries"] != null)
            {
                onlyAllocateDefinedGptEntries = bool.Parse(category["OnlyAllocateDefinedGptEntries"]);
            }

            FullFlashUpdateStore flashUpdateStore = new();
            flashUpdateStore.Initialize(this, storeId, isMainOSStore, devicePath, onlyAllocateDefinedGptEntries,
                minSectorCount, sectorSize);
            Stores.Add(flashUpdateStore);
        }

        public static bool IsGPTPartitionType(string partitionType)
        {
            return Guid.TryParse(partitionType, out _);
        }

        public void DisplayImageInformation(IULogger logger)
        {
            foreach (string devicePlatformId in DevicePlatformIDs)
            {
                logger.LogInfo("\tDevice Platform ID: {0}", devicePlatformId);
            }

            logger.LogInfo("\tChunk Size: 0x{0:X}", ChunkSize);
            logger.LogInfo(" ");
            foreach (FullFlashUpdateStore store in Stores)
            {
                logger.LogInfo("Store");
                logger.LogInfo("\tSector Size: 0x{0:X}", store.SectorSize);
                logger.LogInfo("\tID: {0}", store.Id);
                logger.LogInfo("\tDevice Path: {0}", store.DevicePath);
                logger.LogInfo("\tContains MainOS: {0}", store.IsMainOSStore);
                if (store.IsMainOSStore)
                {
                    logger.LogInfo("\tMinimum Sector Count: 0x{0:X}", store.SectorCount);
                }

                logger.LogInfo(" ");
                logger.LogInfo("There are {0} partitions in the store.", store.Partitions.Count);
                logger.LogInfo(" ");
                foreach (FullFlashUpdatePartition partition in store.Partitions)
                {
                    logger.LogInfo("\tPartition");
                    logger.LogInfo("\t\tName: {0}", partition.Name);
                    logger.LogInfo("\t\tPartition Type: {0}", partition.PartitionType);
                    logger.LogInfo("\t\tTotal Sectors: 0x{0:X}", partition.TotalSectors);
                    logger.LogInfo("\t\tSectors In Use: 0x{0:X}", partition.SectorsInUse);
                    logger.LogInfo(" ");
                }
            }
        }

        private uint CalculateAlignment(uint currentPosition, uint blockSize)
        {
            uint num1 = 0;
            uint num2 = currentPosition % blockSize;
            if (num2 > 0U)
            {
                num1 = blockSize - num2;
            }

            return num1;
        }

        public byte[] GetSecurityHeader(byte[] catalogData, byte[] hashData)
        {
            MemoryStream memoryStream1 = new();
            BinaryWriter binaryWriter = new(memoryStream1);
            binaryWriter.Write(FullFlashUpdateHeaders.SecurityHeaderSize);
            binaryWriter.Write(FullFlashUpdateHeaders.GetSecuritySignature());
            binaryWriter.Write(ChunkSize);
            binaryWriter.Write(HashAlgorithmID);
            binaryWriter.Write(catalogData.Length);
            binaryWriter.Write(hashData.Length);
            binaryWriter.Write(catalogData);
            binaryWriter.Write(hashData);
            binaryWriter.Flush();
            if (memoryStream1.Length % ChunkSizeInBytes != 0L)
            {
                long num = ChunkSizeInBytes - (memoryStream1.Length % ChunkSizeInBytes);
                MemoryStream memoryStream2 = memoryStream1;
                memoryStream2.SetLength(memoryStream2.Length + num);
            }

            return memoryStream1.ToArray();
        }

        public byte[] GetManifestRegion()
        {
            MemoryStream memoryStream1 = new();
            BinaryWriter binaryWriter = new(memoryStream1);
            binaryWriter.Write(FullFlashUpdateHeaders.ImageHeaderSize);
            binaryWriter.Write(FullFlashUpdateHeaders.GetImageSignature());
            binaryWriter.Write(GetManifest.Length);
            binaryWriter.Write(ChunkSize);
            binaryWriter.Flush();
            GetManifest.WriteToStream(memoryStream1);
            if (memoryStream1.Length % ChunkSizeInBytes != 0L)
            {
                long num = ChunkSizeInBytes - (memoryStream1.Length % ChunkSizeInBytes);
                MemoryStream memoryStream2 = memoryStream1;
                memoryStream2.SetLength(memoryStream2.Length + num);
            }

            return memoryStream1.ToArray();
        }

        public void WriteManifest(Stream stream)
        {
            GetManifest.WriteToStream(stream);
        }

        private void ValidateManifest()
        {
            if (GetManifest["FullFlash"] == null)
            {
                throw new ImageCommonException(
                                "ImageCommon!FullFlashUpdateImage::ValidateManifest: Missing 'FullFlash' or 'Image' category in the manifest");
            }

            string str = GetManifest["FullFlash"]["Version"];
            if (str == null)
            {
                throw new ImageCommonException(
                                "ImageCommon!FullFlashUpdateImage::ValidateManifest: Missing 'Version' name/value pair in the 'FullFlash' category.");
            }

            if (!str.Equals("2.0", StringComparison.OrdinalIgnoreCase))
            {
                throw new ImageCommonException("ImageCommon!FullFlashUpdateImage::ValidateManifest: 'Version' value (" +
                                                           str + ") does not match current version of 2.0.");
            }
        }

        public void Initialize()
        {
            GetManifest = new FullFlashUpdateManifest(this);
            Version = "2.0";
        }

        public struct SecurityHeader
        {
            public static bool ValidateSignature(byte[] signature)
            {
                byte[] securitySignature = FullFlashUpdateHeaders.GetSecuritySignature();
                for (int index = 0; index < securitySignature.Length; ++index)
                {
                    if (signature[index] != securitySignature[index])
                    {
                        return false;
                    }
                }

                return true;
            }

            public uint ByteCount
            {
                get; set;
            }

            public uint ChunkSize
            {
                get; set;
            }

            public uint HashAlgorithmID
            {
                get; set;
            }

            public uint CatalogSize
            {
                get; set;
            }

            public uint HashTableSize
            {
                get; set;
            }
        }

        public struct ImageHeader
        {
            public static bool ValidateSignature(byte[] signature)
            {
                byte[] imageSignature = FullFlashUpdateHeaders.GetImageSignature();
                for (int index = 0; index < imageSignature.Length; ++index)
                {
                    if (signature[index] != imageSignature[index])
                    {
                        return false;
                    }
                }

                return true;
            }

            public uint ByteCount
            {
                get; set;
            }

            public uint ManifestLength
            {
                get; set;
            }

            public uint ChunkSize
            {
                get; set;
            }
        }

        public class FullFlashUpdatePartition
        {
            private uint _byteAlignment;
            private uint _clusterSize;
            private FullFlashUpdateStore _store;

            public string Name
            {
                get; set;
            }

            public uint TotalSectors
            {
                get; set;
            }

            public string PartitionType
            {
                get; set;
            }

            public string PartitionId
            {
                get; set;
            }

            public bool Bootable
            {
                get; set;
            }

            public bool ReadOnly
            {
                get; set;
            }

            public bool Hidden
            {
                get; set;
            }

            public bool AttachDriveLetter
            {
                get; set;
            }

            public string PrimaryPartition
            {
                get; set;
            }

            public bool Contiguous => true;

            public string FileSystem
            {
                get; set;
            }

            public uint ByteAlignment
            {
                get => _byteAlignment;
                set
                {
                    if (!InputHelpers.IsPowerOfTwo(value))
                    {
                        return;
                    }

                    _byteAlignment = value;
                }
            }

            public uint ClusterSize
            {
                get => _clusterSize;
                set
                {
                    if (!InputHelpers.IsPowerOfTwo(value))
                    {
                        return;
                    }

                    _clusterSize = value;
                }
            }

            public uint LastUsedSector => SectorsInUse > 0U ? SectorsInUse - 1U : 0;

            public uint SectorsInUse
            {
                get; set;
            }

            public bool UseAllSpace
            {
                get; set;
            }

            public bool RequiredToFlash
            {
                get; set;
            }

            public uint SectorAlignment
            {
                get; set;
            }

            public ulong OffsetInSectors
            {
                get; set;
            }

            public void Initialize(uint usedSectors, uint totalSectors, string partitionType, string partitionId,
                string name, FullFlashUpdateStore store, bool useAllSpace)
            {
                SectorsInUse = usedSectors;
                TotalSectors = totalSectors;
                PartitionType = partitionType;
                PartitionId = partitionId;
                Name = name;
                _store = store;
                UseAllSpace = useAllSpace;
                if (UseAllSpace && !name.Equals("Data", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ImageCommonException(string.Format(
                                        "ImageCommon!FullFlashUpdatePartition::Initialize: Partition {0} cannot specify UseAllSpace.",
                                        Name));
                }

                if (TotalSectors == uint.MaxValue)
                {
                    throw new ImageCommonException("ImageCommon!FullFlashUpdatePartition::Initialize: Partition " +
                                                                   name + " is too large (" + TotalSectors + " sectors)");
                }

                ReadOnly = false;
                Bootable = false;
                Hidden = false;
                AttachDriveLetter = false;
                RequiredToFlash = false;
                SectorAlignment = 0U;
                OffsetInSectors = 0UL;
                FileSystem = string.Empty;
                _byteAlignment = 0U;
                _clusterSize = 0U;
            }

            public void ToCategory(ManifestCategory category)
            {
                category.Clean();
                category["Name"] = Name;
                category["Type"] = PartitionType;
                if (!string.IsNullOrEmpty(PartitionId))
                {
                    category["Id"] = PartitionId;
                }

                category["Primary"] = PrimaryPartition;
                if (!string.IsNullOrEmpty(FileSystem))
                {
                    category["FileSystem"] = FileSystem;
                }

                if (ReadOnly)
                {
                    category["ReadOnly"] = ReadOnly.ToString(CultureInfo.InvariantCulture);
                }

                if (Hidden)
                {
                    category["Hidden"] = Hidden.ToString(CultureInfo.InvariantCulture);
                }

                if (AttachDriveLetter)
                {
                    category["AttachDriveLetter"] = AttachDriveLetter.ToString(CultureInfo.InvariantCulture);
                }

                if (Bootable)
                {
                    category["Bootable"] = Bootable.ToString(CultureInfo.InvariantCulture);
                }

                if (UseAllSpace)
                {
                    category["UseAllSpace"] = "true";
                }
                else
                {
                    category["TotalSectors"] = TotalSectors.ToString(CultureInfo.InvariantCulture);
                    category["UsedSectors"] = SectorsInUse.ToString(CultureInfo.InvariantCulture);
                }

                if (_byteAlignment != 0U)
                {
                    category["ByteAlignment"] = _byteAlignment.ToString(CultureInfo.InvariantCulture);
                }

                if (_clusterSize != 0U)
                {
                    category["ClusterSize"] = _clusterSize.ToString(CultureInfo.InvariantCulture);
                }

                if (SectorAlignment != 0U)
                {
                    category["SectorAlignment"] = SectorAlignment.ToString(CultureInfo.InvariantCulture);
                }

                if (!RequiredToFlash)
                {
                    return;
                }

                category["RequiredToFlash"] = RequiredToFlash.ToString(CultureInfo.InvariantCulture);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public class FullFlashUpdateStore : IDisposable
        {
            private bool _alreadyDisposed;
            private uint _sectorsUsed;
            private string _tempBackingStorePath = string.Empty;

            public FullFlashUpdateImage Image
            {
                get; private set;
            }

            public string Id
            {
                get; private set;
            }

            public bool IsMainOSStore
            {
                get; private set;
            }

            public string DevicePath
            {
                get; private set;
            }

            public bool OnlyAllocateDefinedGptEntries
            {
                get; private set;
            }

            public uint SectorCount
            {
                get; set;
            }

            public uint MinSectorCount
            {
                get => SectorCount;
                set => SectorCount = value;
            }

            public uint SectorSize
            {
                get; set;
            }

            public int PartitionCount => Partitions.Count;

            public List<FullFlashUpdatePartition> Partitions
            {
                get; private set;
            } =
                [];

            public FullFlashUpdatePartition this[string name]
            {
                get
                {
                    foreach (FullFlashUpdatePartition partition in Partitions)
                    {
                        if (string.CompareOrdinal(partition.Name, name) == 0)
                        {
                            return partition;
                        }
                    }

                    return null;
                }
            }

            public string BackingFile { get; private set; } = string.Empty;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~FullFlashUpdateStore()
            {
                Dispose(false);
            }

            protected virtual void Dispose(bool isDisposing)
            {
                if (_alreadyDisposed)
                {
                    return;
                }

                if (isDisposing)
                {
                    Partitions = null;
                }

                if (File.Exists(BackingFile))
                {
                    try
                    {
                        File.Delete(BackingFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Warning: ImageCommon!Dispose: Failed to delete temporary backing store '" +
                                          BackingFile + "' with exception: " + ex.Message);
                    }
                }

                if (Directory.Exists(_tempBackingStorePath))
                {
                    try
                    {
                        Directory.Delete(_tempBackingStorePath, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            "Warning: ImageCommon!Dispose: Failed to delete temporary backing store directory '" +
                            _tempBackingStorePath + "' with exception: " + ex.Message);
                    }
                }

                _alreadyDisposed = true;
            }

            public void Initialize(FullFlashUpdateImage image, string storeId, bool isMainOSStore, string devicePath,
                bool onlyAllocateDefinedGptEntries, uint minSectorCount, uint sectorSize)
            {
                _tempBackingStorePath = BuildPaths.GetImagingTempPath(Directory.GetCurrentDirectory());
                _ = Directory.CreateDirectory(_tempBackingStorePath);
                BackingFile = FileUtils.GetTempFile(_tempBackingStorePath) + "FFUBackingStore";
                Image = image;
                Id = storeId;
                IsMainOSStore = isMainOSStore;
                DevicePath = devicePath;
                OnlyAllocateDefinedGptEntries = onlyAllocateDefinedGptEntries;
                SectorCount = minSectorCount;
                SectorSize = sectorSize;
                _sectorsUsed = 0U;
            }

            public void AddPartition(FullFlashUpdatePartition partition)
            {
                if (this[partition.Name] != null)
                {
                    throw new ImageCommonException(
                                        "ImageCommon!FullFlashUpdateStore::AddPartition: Two partitions in a store have the same name (" +
                                        partition.Name + ").");
                }

                if (IsMainOSStore)
                {
                    if (SectorCount != 0U && partition.TotalSectors > SectorCount)
                    {
                        throw new ImageCommonException(
                                                "ImageCommon!FullFlashUpdateStore::AddPartition: The partition " + partition.Name +
                                                " is too large for the store.");
                    }

                    if (partition.UseAllSpace)
                    {
                        foreach (FullFlashUpdatePartition partition1 in Partitions)
                        {
                            if (partition1.UseAllSpace)
                            {
                                throw new ImageCommonException(
                                                                "ImageCommon!FullFlashUpdateStore::AddPartition: Two partitions in the same store have the UseAllSpace flag set.");
                            }
                            else if (partition.SectorsInUse > partition.TotalSectors)
                            {
                                throw new ImageCommonException(
                                                                "ImageCommon!FullFlashUpdateStore::AddPartition: The partition data is invalid.  There are more used sectors (" +
                                                                partition.SectorsInUse + ") than total sectors (" + partition.TotalSectors +
                                                                ") for partition:" + partition.Name);
                            }
                        }
                    }

                    if (SectorCount != 0U)
                    {
                        if (partition.UseAllSpace)
                        {
                            ++_sectorsUsed;
                        }
                        else
                        {
                            _sectorsUsed += partition.TotalSectors;
                        }

                        if (_sectorsUsed > SectorCount)
                        {
                            throw new ImageCommonException(
                                                        "ImageCommon!FullFlashUpdateStore::AddPartition: Partition (" + partition.Name +
                                                        ") on the Store does not fit. SectorsUsed = " + _sectorsUsed + " > MinSectorCount = " +
                                                        SectorCount);
                        }
                    }
                }

                Partitions.Add(partition);
            }

            internal void AddPartition(ManifestCategory category)
            {
                uint usedSectors = 0;
                uint totalSectors = 0;
                string partitionType = category["Type"];
                string name = category["Name"];
                string partitionId = category["Id"];
                bool useAllSpace = false;
                if (IsMainOSStore)
                {
                    if (category["UsedSectors"] != null)
                    {
                        usedSectors = uint.Parse(category["UsedSectors"], CultureInfo.InvariantCulture);
                    }

                    if (category["TotalSectors"] != null)
                    {
                        totalSectors = uint.Parse(category["TotalSectors"], CultureInfo.InvariantCulture);
                    }

                    if (category["UseAllSpace"] != null)
                    {
                        useAllSpace = bool.Parse(category["UseAllSpace"]);
                    }

                    if (!useAllSpace && totalSectors == 0U)
                    {
                        throw new ImageCommonException(string.Format(
                                                "ImageCommon!FullFlashUpdateImage::AddPartition: The partition category for partition {0} must contain either a 'TotalSectors' or 'UseAllSpace' key/value pair.",
                                                name));
                    }

                    if (useAllSpace && totalSectors > 0U)
                    {
                        throw new ImageCommonException(string.Format(
                                                "ImageCommon!FullFlashUpdateImage::AddPartition: The partition category for partition {0} cannot contain both a 'TotalSectors' and a 'UseAllSpace' key/value pair.",
                                                name));
                    }
                }

                FullFlashUpdatePartition partition = new();
                partition.Initialize(usedSectors, totalSectors, partitionType, partitionId, name, this, useAllSpace);
                if (category["Hidden"] != null)
                {
                    partition.Hidden = bool.Parse(category["Hidden"]);
                }

                if (category["AttachDriveLetter"] != null)
                {
                    partition.AttachDriveLetter = bool.Parse(category["AttachDriveLetter"]);
                }

                if (category["ReadOnly"] != null)
                {
                    partition.ReadOnly = bool.Parse(category["ReadOnly"]);
                }

                if (category["Bootable"] != null)
                {
                    partition.Bootable = bool.Parse(category["Bootable"]);
                }

                if (category["FileSystem"] != null)
                {
                    partition.FileSystem = category["FileSystem"];
                }

                partition.PrimaryPartition = category["Primary"];
                if (category["ByteAlignment"] != null)
                {
                    partition.ByteAlignment = uint.Parse(category["ByteAlignment"], CultureInfo.InvariantCulture);
                }

                if (category["ClusterSize"] != null)
                {
                    partition.ClusterSize = uint.Parse(category["ClusterSize"], CultureInfo.InvariantCulture);
                }

                if (category["SectorAlignment"] != null)
                {
                    partition.SectorAlignment = uint.Parse(category["SectorAlignment"], CultureInfo.InvariantCulture);
                }

                if (category["RequiredToFlash"] != null)
                {
                    partition.RequiredToFlash = bool.Parse(category["RequiredToFlash"]);
                }

                AddPartition(partition);
            }

            public void TransferLocation(Stream sourceStream, Stream destinationStream)
            {
                byte[] buffer = new byte[1048576];
                long val1 = sourceStream.Length - sourceStream.Position;
                while (val1 > 0L)
                {
                    int count = (int)Math.Min(val1, buffer.Length);
                    _ = sourceStream.Read(buffer, 0, count);
                    destinationStream.Write(buffer, 0, count);
                    val1 -= count;
                }
            }

            public void ToCategory(ManifestCategory category)
            {
                category["SectorSize"] = SectorSize.ToString(CultureInfo.InvariantCulture);
                if (SectorCount != 0U)
                {
                    category["MinSectorCount"] = SectorCount.ToString(CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(Id))
                {
                    category["StoreId"] = Id;
                }

                category["IsMainOSStore"] = IsMainOSStore.ToString(CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(DevicePath))
                {
                    category["DevicePath"] = DevicePath;
                }

                category["OnlyAllocateDefinedGptEntries"] =
                                    OnlyAllocateDefinedGptEntries.ToString(CultureInfo.InvariantCulture);
            }
        }

        public class ManifestCategory
        {
            private readonly Hashtable _keyValues = [];
            private int _maxKeySize;

            public ManifestCategory(string name)
            {
                Name = name;
            }

            public ManifestCategory(string name, string categoryValue)
            {
                Name = name;
                Category = categoryValue;
            }

            public string this[string name]
            {
                get => (string)_keyValues[name];
                set
                {
                    if (_keyValues.ContainsKey(name))
                    {
                        _keyValues[name] = value;
                    }
                    else
                    {
                        if (name.Length > _maxKeySize)
                        {
                            _maxKeySize = name.Length;
                        }

                        _keyValues.Add(name, value);
                    }
                }
            }

            public string Category { get; set; } = string.Empty;

            public string Name { get; } = string.Empty;

            public void RemoveNameValue(string name)
            {
                if (!_keyValues.ContainsKey(name))
                {
                    return;
                }

                _keyValues.Remove(name);
            }

            public void WriteToStream(Stream targetStream)
            {
                ASCIIEncoding asciiEncoding = new();
                byte[] bytes1 = asciiEncoding.GetBytes("[" + Category + "]\r\n");
                targetStream.Write(bytes1, 0, bytes1.Count());
                foreach (DictionaryEntry keyValue in _keyValues)
                {
                    string key = keyValue.Key as string;
                    byte[] bytes2 = asciiEncoding.GetBytes(key);
                    targetStream.Write(bytes2, 0, bytes2.Count());
                    for (int index = 0; index < _maxKeySize + 1 - key.Length; ++index)
                    {
                        targetStream.Write(asciiEncoding.GetBytes(" "), 0, 1);
                    }

                    byte[] bytes3 = asciiEncoding.GetBytes("= " + _keyValues[key] + "\r\n");
                    targetStream.Write(bytes3, 0, bytes3.Count());
                }

                targetStream.Write(asciiEncoding.GetBytes("\r\n"), 0, 2);
            }

            public void WriteToFile(TextWriter targetStream)
            {
                targetStream.WriteLine("[{0}]", Category);
                foreach (DictionaryEntry keyValue in _keyValues)
                {
                    string key = keyValue.Key as string;
                    targetStream.Write("{0}", key);
                    for (int index = 0; index < _maxKeySize + 1 - key.Length; ++index)
                    {
                        targetStream.Write(" ");
                    }

                    targetStream.WriteLine("= {0}", _keyValues[key]);
                }

                targetStream.WriteLine("");
            }

            public void Clean()
            {
                _keyValues.Clear();
            }
        }

        public class FullFlashUpdateManifest
        {
            private readonly ArrayList _categories = new(20);
            private readonly FullFlashUpdateImage _image;

            public FullFlashUpdateManifest(FullFlashUpdateImage image)
            {
                _image = image;
            }

            public FullFlashUpdateManifest(FullFlashUpdateImage image, StreamReader manifestStream)
            {
                Regex regex1 = new("^\\s*\\[(?<category>[^\\]]+)\\]\\s*$");
                Regex regex2 = new("^\\s*(?<key>[^=\\s]+)\\s*=\\s*(?<value>.*)(\\s*$)");
                Match match1 = null;
                _image = image;
                ManifestCategory category = null;
                while (!manifestStream.EndOfStream)
                {
                    string input = manifestStream.ReadLine();
                    if (regex1.IsMatch(input))
                    {
                        match1 = null;
                        string strA = regex1.Match(input).Groups["category"].Value;
                        ProcessCategory(category);
                        if (string.Compare(strA, "Store", StringComparison.Ordinal) == 0)
                        {
                            category = new ManifestCategory("Store", "Store");
                        }
                        else if (string.Compare(strA, "Partition", StringComparison.Ordinal) == 0)
                        {
                            category = new ManifestCategory("Partition", "Partition");
                        }
                        else
                        {
                            string str = strA;
                            category = AddCategory(str, str);
                        }
                    }
                    else if (category != null && regex2.IsMatch(input))
                    {
                        match1 = null;
                        Match match2 = regex2.Match(input);
                        category[match2.Groups["key"].Value] = match2.Groups["value"].Value;
                        if (match2.Groups["key"].ToString() == "Description")
                        {
                            match1 = match2;
                        }
                    }
                    else if (match1 != null)
                    {
                        ManifestCategory manifestCategory2 = category;
                        string index = match1.Groups["key"].Value;
                        manifestCategory2[index] = manifestCategory2[index] + Environment.NewLine + input;
                    }
                }

                ProcessCategory(category);
            }

            public ManifestCategory this[string categoryName]
            {
                get
                {
                    foreach (ManifestCategory category in _categories)
                    {
                        if (string.Compare(category.Name, categoryName, StringComparison.Ordinal) == 0)
                        {
                            return category;
                        }
                    }

                    return null;
                }
            }

            public uint Length
            {
                get
                {
                    MemoryStream memoryStream = new();
                    WriteToStream(memoryStream);
                    return (uint)memoryStream.Position;
                }
            }

            private void ProcessCategory(ManifestCategory category)
            {
                if (category == null)
                {
                    return;
                }

                if (string.CompareOrdinal(category.Name, "Store") == 0)
                {
                    _image.AddStore(category);
                }
                else
                {
                    if (string.CompareOrdinal(category.Name, "Partition") != 0)
                    {
                        return;
                    }

                    _image.Stores.Last().AddPartition(category);
                }
            }

            public ManifestCategory AddCategory(string name, string categoryValue)
            {
                if (this[name] != null)
                {
                    throw new ImageCommonException(
                                        "ImageCommon!FullFlashUpdateManifest::AddCategory: Cannot add duplicate categories to a manifest.");
                }

                ManifestCategory manifestCategory = new(name, categoryValue);
                _ = _categories.Add(manifestCategory);
                return manifestCategory;
            }

            public void RemoveCategory(string name)
            {
                if (this[name] == null)
                {
                    return;
                }

                _categories.Remove(this[name]);
            }

            public void WriteToStream(Stream targetStream)
            {
                foreach (ManifestCategory category in _categories)
                {
                    category.WriteToStream(targetStream);
                }

                foreach (FullFlashUpdateStore store in _image.Stores)
                {
                    ManifestCategory category1 = new("Store", "Store");
                    store.ToCategory(category1);
                    category1.WriteToStream(targetStream);
                    foreach (FullFlashUpdatePartition partition in store.Partitions)
                    {
                        ManifestCategory manifestCategory = new("Partition", "Partition");
                        ManifestCategory category2 = manifestCategory;
                        partition.ToCategory(category2);
                        manifestCategory.WriteToStream(targetStream);
                    }
                }
            }

            public void WriteToFile(string fileName)
            {
                try
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
                catch (Exception ex)
                {
                    throw new ImageCommonException(
                        "ImageCommon!FullFlashUpdateManifest::WriteToFile: Unable to delete the existing image file",
                        ex);
                }

                StreamWriter text = File.CreateText(fileName);
                WriteToStream(text.BaseStream);
                text.Close();
            }
        }
    }
}