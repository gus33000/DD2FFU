// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageStoreHeader
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ImageStoreHeader
    {
        public static readonly int PlatformIdSizeInBytes = 192;
        private byte[] _devicePath;
        private ushort _devicePathLength;
        private ushort _numberOfStores = 1;
        private ushort _storeIndex = 1;
        private ulong _storePayloadSize;

        [StructVersion(Version = 1)] public FullFlashUpdateType UpdateType { get; set; }

        [StructVersion(Version = 1)] public ushort MajorVersion { get; set; }

        [StructVersion(Version = 1)] public ushort MinorVersion { get; set; }

        [StructVersion(Version = 1)] public ushort FullFlashMajorVersion { get; set; }

        [StructVersion(Version = 1)] public ushort FullFlashMinorVersion { get; set; }

        [StructVersion(Version = 1)] public byte[] PlatformIdentifier { get; set; } = new byte[PlatformIdSizeInBytes];

        [StructVersion(Version = 1)] public uint BytesPerBlock { get; set; }

        [StructVersion(Version = 1)] public uint StoreDataEntryCount { get; set; }

        [StructVersion(Version = 1)] public uint StoreDataSizeInBytes { get; set; }

        [StructVersion(Version = 1)] public uint ValidationEntryCount { get; set; }

        [StructVersion(Version = 1)] public uint ValidationDataSizeInBytes { get; set; }

        [StructVersion(Version = 1)] public uint InitialPartitionTableBlockIndex { get; set; }

        [StructVersion(Version = 1)] public uint InitialPartitionTableBlockCount { get; set; }

        [StructVersion(Version = 1)] public uint FlashOnlyPartitionTableBlockIndex { get; set; }

        [StructVersion(Version = 1)] public uint FlashOnlyPartitionTableBlockCount { get; set; }

        [StructVersion(Version = 1)] public uint FinalPartitionTableBlockIndex { get; set; }

        [StructVersion(Version = 1)] public uint FinalPartitionTableBlockCount { get; set; }

        [StructVersion(Version = 2)]
        public ushort NumberOfStores
        {
            get
            {
                if (MajorVersion < 2)
                    throw new NotImplementedException(nameof(NumberOfStores));
                return _numberOfStores;
            }
            set => _numberOfStores = value;
        }

        [StructVersion(Version = 2)]
        public ushort StoreIndex
        {
            get
            {
                if (MajorVersion < 2)
                    throw new NotImplementedException(nameof(StoreIndex));
                return _storeIndex;
            }
            set => _storeIndex = value;
        }

        [StructVersion(Version = 2)]
        public ulong StorePayloadSize
        {
            get
            {
                if (MajorVersion < 2)
                    throw new NotImplementedException(nameof(StorePayloadSize));
                return _storePayloadSize;
            }
            set => _storePayloadSize = value;
        }

        [StructVersion(Version = 2)]
        public ushort DevicePathLength
        {
            get
            {
                if (MajorVersion < 2)
                    throw new NotImplementedException(nameof(DevicePathLength));
                return _devicePathLength;
            }
            set => _devicePathLength = value;
        }

        [StructVersion(Version = 2)]
        public byte[] DevicePath
        {
            get
            {
                if (MajorVersion < 2)
                    throw new NotImplementedException(nameof(DevicePath));
                return _devicePath;
            }
            set => _devicePath = value;
        }

        private void SetFullFlashVersion(FullFlashUpdateImage fullFlashImage)
        {
            var regex = new Regex("(?<MajorVersion>\\d+)\\.(?<MinorVersion>\\d+)");
            if (!regex.IsMatch(fullFlashImage.Version))
                throw new ImageStorageException(string.Format("{0}: The full flash update version isn't valid. '{1}'",
                    MethodBase.GetCurrentMethod().Name, fullFlashImage.Version));
            var match = regex.Match(fullFlashImage.Version);
            try
            {
                var num1 = ushort.Parse(match.Groups["MajorVersion"].Value);
                var num2 = ushort.Parse(match.Groups["MinorVersion"].Value);
                FullFlashMajorVersion = num1;
                FullFlashMinorVersion = num2;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: The full flash image version number is invalid. '{1}'",
                        MethodBase.GetCurrentMethod().Name, fullFlashImage.Version), ex);
            }
        }

        public void Initialize(FullFlashUpdateType updateType, uint blockSizeInBytes,
            FullFlashUpdateImage fullFlashImage)
        {
            BytesPerBlock = blockSizeInBytes;
            UpdateType = updateType;
            MajorVersion = 1;
            var index = 0;
            foreach (var devicePlatformId in fullFlashImage.DevicePlatformIDs)
            {
                var bytes = Encoding.ASCII.GetBytes(devicePlatformId);
                var num = bytes.Length + 1;
                if (index + num > PlatformIdSizeInBytes - 1)
                    throw new ImageStorageException(string.Format("{0}: The platform ID group is too large.",
                        MethodBase.GetCurrentMethod().Name));
                bytes.CopyTo(PlatformIdentifier, index);
                index += num;
            }

            SetFullFlashVersion(fullFlashImage);
        }

        public void Initialize2(FullFlashUpdateType updateType, uint blockSizeInBytes,
            FullFlashUpdateImage fullFlashImage, ushort numberOfStores, ushort storeIndex, string devicePath)
        {
            Initialize(updateType, blockSizeInBytes, fullFlashImage);
            MajorVersion = 2;
            NumberOfStores = numberOfStores;
            StoreIndex = storeIndex;
            DevicePathLength = (ushort) devicePath.Length;
            DevicePath = new UnicodeEncoding().GetBytes(devicePath.ToCharArray());
        }

        public void WriteToStream(Stream stream)
        {
            var binaryWriter = new BinaryWriter(stream);
            var position = stream.Position;
            binaryWriter.Write((uint) UpdateType);
            binaryWriter.Write(MajorVersion);
            binaryWriter.Write(MinorVersion);
            binaryWriter.Write(FullFlashMajorVersion);
            binaryWriter.Write(FullFlashMinorVersion);
            binaryWriter.Write(PlatformIdentifier);
            binaryWriter.Write(BytesPerBlock);
            binaryWriter.Write(StoreDataEntryCount);
            binaryWriter.Write(StoreDataSizeInBytes);
            binaryWriter.Write(ValidationEntryCount);
            binaryWriter.Write(ValidationDataSizeInBytes);
            binaryWriter.Write(InitialPartitionTableBlockIndex);
            binaryWriter.Write(InitialPartitionTableBlockCount);
            binaryWriter.Write(FlashOnlyPartitionTableBlockIndex);
            binaryWriter.Write(FlashOnlyPartitionTableBlockCount);
            binaryWriter.Write(FinalPartitionTableBlockIndex);
            binaryWriter.Write(FinalPartitionTableBlockCount);
            if (MajorVersion >= 2)
            {
                binaryWriter.Write(NumberOfStores);
                binaryWriter.Write(StoreIndex);
                binaryWriter.Write(StorePayloadSize);
                binaryWriter.Write(DevicePathLength);
                binaryWriter.Write(DevicePath);
            }
        }

        public static ImageStoreHeader ReadFromStream(Stream stream)
        {
            var binaryReader = new BinaryReader(stream);
            var position = stream.Position;
            var imageStoreHeader = new ImageStoreHeader();
            imageStoreHeader.UpdateType = (FullFlashUpdateType) binaryReader.ReadUInt32();
            imageStoreHeader.MajorVersion = binaryReader.ReadUInt16();
            imageStoreHeader.MinorVersion = binaryReader.ReadUInt16();
            imageStoreHeader.FullFlashMajorVersion = binaryReader.ReadUInt16();
            imageStoreHeader.FullFlashMinorVersion = binaryReader.ReadUInt16();
            imageStoreHeader.PlatformIdentifier = binaryReader.ReadBytes(PlatformIdSizeInBytes);
            imageStoreHeader.BytesPerBlock = binaryReader.ReadUInt32();
            imageStoreHeader.StoreDataEntryCount = binaryReader.ReadUInt32();
            imageStoreHeader.StoreDataSizeInBytes = binaryReader.ReadUInt32();
            imageStoreHeader.ValidationEntryCount = binaryReader.ReadUInt32();
            imageStoreHeader.ValidationDataSizeInBytes = binaryReader.ReadUInt32();
            imageStoreHeader.InitialPartitionTableBlockIndex = binaryReader.ReadUInt32();
            imageStoreHeader.InitialPartitionTableBlockCount = binaryReader.ReadUInt32();
            imageStoreHeader.FlashOnlyPartitionTableBlockIndex = binaryReader.ReadUInt32();
            imageStoreHeader.FlashOnlyPartitionTableBlockCount = binaryReader.ReadUInt32();
            imageStoreHeader.FinalPartitionTableBlockIndex = binaryReader.ReadUInt32();
            imageStoreHeader.FinalPartitionTableBlockCount = binaryReader.ReadUInt32();
            if (imageStoreHeader.MajorVersion >= 2)
            {
                imageStoreHeader.NumberOfStores = binaryReader.ReadUInt16();
                imageStoreHeader.StoreIndex = binaryReader.ReadUInt16();
                imageStoreHeader.StorePayloadSize = binaryReader.ReadUInt64();
                imageStoreHeader.DevicePathLength = binaryReader.ReadUInt16();
                imageStoreHeader.DevicePath = binaryReader.ReadBytes(imageStoreHeader.DevicePathLength * 2);
            }

            return imageStoreHeader;
        }

        public void LogInfo(IULogger logger)
        {
            LogInfo(logger, 0);
        }

        public void LogInfo(IULogger logger, int indentLevel)
        {
            var str1 = Encoding.ASCII.GetString(PlatformIdentifier);
            var str2 = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str2 + "Image Store Payload Header");
            checked
            {
                indentLevel += 2;
            }

            var str3 = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str3 + "UpdateType                       : {0}", (object) UpdateType);
            logger.LogInfo(str3 + "MajorVersion                     : 0x{0:x}", (object) MajorVersion);
            logger.LogInfo(str3 + "MinorVersion                     : 0x{0:x}", (object) MinorVersion);
            logger.LogInfo(str3 + "FullFlashMajorVersion            : 0x{0:x}", (object) FullFlashMajorVersion);
            logger.LogInfo(str3 + "FullFlashMinorVersion            : 0x{0:x}", (object) FullFlashMinorVersion);
            logger.LogInfo(str3 + "PlatformIdentifier               : {0}",
                (object) str1.Substring(0, str1.IndexOf(char.MinValue)));
            logger.LogInfo(str3 + "BytesPerBlock                    : 0x{0:x}", (object) BytesPerBlock);
            logger.LogInfo(str3 + "StoreDataEntryCount              : 0x{0:x}", (object) StoreDataEntryCount);
            logger.LogInfo(str3 + "StoreDataSizeInBytes             : 0x{0:x}", (object) StoreDataSizeInBytes);
            logger.LogInfo(str3 + "ValidationEntryCount             : 0x{0:x}", (object) ValidationEntryCount);
            logger.LogInfo(str3 + "ValidationDataSizeInBytes        : 0x{0:x}", (object) ValidationDataSizeInBytes);
            logger.LogInfo(str3 + "InitialPartitionTableBlockIndex  : 0x{0:x}",
                (object) InitialPartitionTableBlockIndex);
            logger.LogInfo(str3 + "InitialPartitionTableBlockCount  : 0x{0:x}",
                (object) InitialPartitionTableBlockCount);
            logger.LogInfo(str3 + "FlashOnlyPartitionTableBlockIndex: 0x{0:x}",
                (object) FlashOnlyPartitionTableBlockIndex);
            logger.LogInfo(str3 + "FlashOnlyPartitionTableBlockCount: 0x{0:x}",
                (object) FlashOnlyPartitionTableBlockCount);
            logger.LogInfo(str3 + "FinalPartitionTableBlockIndex    : 0x{0:x}", (object) FinalPartitionTableBlockIndex);
            logger.LogInfo(str3 + "FinalPartitionTableBlockCount    : 0x{0:x}", (object) FinalPartitionTableBlockCount);
            if (MajorVersion < 2)
                return;
            var str4 = Encoding.ASCII.GetString(DevicePath);
            logger.LogInfo(str3 + "NumberOfStores                   : 0x{0:x}", (object) NumberOfStores);
            logger.LogInfo(str3 + "StoreIndex                       : 0x{0:x}", (object) StoreIndex);
            logger.LogInfo(str3 + "StorePayloadSize                 : 0x{0:x}", (object) StorePayloadSize);
            logger.LogInfo(str3 + "DevicePathLength                 : 0x{0:x}", (object) DevicePathLength);
            logger.LogInfo(str3 + "DevicePath                       : {0}",
                (object) str4.Substring(0, str4.IndexOf(char.MinValue)));
        }

        public int GetStructureSize()
        {
            var num1 = 0;
            foreach (var property in typeof(ImageStoreHeader).GetProperties())
            foreach (var accessor in property.GetAccessors())
                if (!accessor.IsStatic && !property.PropertyType.IsArray)
                {
                    if (accessor.IsPublic)
                    {
                        var customAttributes =
                            property.GetCustomAttributes(typeof(StructVersionAttribute), false) as
                                StructVersionAttribute[];
                        if (customAttributes != null && (MajorVersion >= 2 || !IsVersion2Field(customAttributes)))
                        {
                            if (property.PropertyType.IsEnum)
                            {
                                num1 += Marshal.SizeOf(Enum.GetUnderlyingType(property.PropertyType));
                                break;
                            }

                            num1 += Marshal.SizeOf(property.PropertyType);
                        }

                        break;
                    }
                }
                else
                {
                    break;
                }

            var num2 = num1 + PlatformIdSizeInBytes;
            if (MajorVersion >= 2)
                num2 += DevicePathLength;
            return num2;
        }

        private bool IsVersion2Field(StructVersionAttribute[] structVersions)
        {
            return structVersions[0].Version == 2;
        }
    }
}