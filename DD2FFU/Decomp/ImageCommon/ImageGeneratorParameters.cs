// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageGeneratorParameters
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ImageGeneratorParameters
    {
        
        public enum FFUHashAlgorithm : uint
        {
            Ffuhsha256 = 32780 // 0x0000800C
        }

         public const uint DefaultChunkSize = 256;

         public const uint DevicePlatformIDSize = 192;

        private const uint _OneKiloBtye = 1024;
        private const uint _MinimumSectorSize = 512;
        private const uint _MinimumSectorFreeCount = 8192;
        private const uint ALG_CLASS_HASH = 32768;
        private const uint ALG_TYPE_ANY = 0;
        private const uint ALG_SID_SHA_256 = 12;
        private const uint CALG_SHA_256 = 32780;
        private uint _algid = 32780;
        private uint _chunkSize = 256;
        private IULogger _logger;
        public string[] DevicePlatformIDs;
        public List<InputStore> Stores;
        public bool UEFI;

        public ImageGeneratorParameters()
        {
            Stores = new List<InputStore>();
        }

        public string Description { get; set; }

        public InputStore MainOSStore
        {
            get { return Stores.FirstOrDefault(x => x.IsMainOSStore()); }
        }

        
        public uint ChunkSize
        {
            get => _chunkSize;
            set => _chunkSize = value != 0U ? value : 256U;
        }

         public uint DefaultPartitionByteAlignment { get; set; }

         public uint VirtualHardDiskSectorSize { get; set; }

         public uint SectorSize { get; set; }

         public uint MinSectorCount { get; set; }

        
        public uint Algid
        {
            get => _algid;
            set => _algid = value != 0U ? value : 32780U;
        }

        public uint DeviceLayoutVersion { get; private set; } = 1;

        public InputRules Rules { get; set; }

        public void Initialize(IULogger logger)
        {
            if (logger == null)
                _logger = new IULogger();
            else
                _logger = logger;
        }

        private bool VerifyPartitionSizes()
        {
            uint num1 = 0;
            if (Stores == null)
                return true;
            foreach (var partition in MainOSStore.Partitions)
                if (partition.UseAllSpace)
                    ++num1;
                else
                    num1 += partition.TotalSectors;
            if (num1 <= MinSectorCount)
                return true;
            var num2 = num1 * (ulong) SectorSize / 1024UL / 1024UL;
            var num3 = MinSectorCount * (ulong) SectorSize / 1024UL / 1024UL;
            _logger.LogError(string.Format(
                "ImageCommon!ImageGeneratorParameters::VerifyPartitionSizes: The total sectors used by all the partitions ({0} sectors/{1} MB) is larger than the MinSectorCount ({2} sectors/{3} MB). This means the image would not flash to a device with only {4} sectors/{5} MB. Either remove image content, or increase MinSectorCount.",
                (object) num1, (object) num2, (object) MinSectorCount, (object) num3, (object) MinSectorCount,
                (object) num3));
            return false;
        }

        public bool VerifyInputParameters()
        {
            var dictionary = new Dictionary<string, string>();
            if (uint.MaxValue / ChunkSize < 1024U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and the total size must be under 4GB.");
                return false;
            }

            if (SectorSize < 512U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The sector size must be at least 512 bytes: {0} bytes.",
                    (object) SectorSize);
                return false;
            }

            if (!InputHelpers.IsPowerOfTwo(SectorSize))
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The sector size must be a multiple of 2: {0} bytes.",
                    (object) SectorSize);
                return false;
            }

            if (ChunkSize * 1024U < SectorSize)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and the total size must be under larger the sector size: {0} bytes.",
                    (object) SectorSize);
                return false;
            }

            if (ChunkSize * 1024U % SectorSize != 0U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The chunk size is specified in Kilobytes and must be divisible by the sector size: {0}.",
                    (object) SectorSize);
                return false;
            }

            if (DefaultPartitionByteAlignment != 0U && !InputHelpers.IsPowerOfTwo(DefaultPartitionByteAlignment))
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The default partition byte alignment must be a multiple of 2: {0} bytes.",
                    (object) DefaultPartitionByteAlignment);
                return false;
            }

            if (Stores == null || Stores.Count == 0)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, at least one store must be specified.");
                return false;
            }

            if (Stores.Count(x => x.IsMainOSStore()) != 1)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, one and only one of the stores must be MainOS.");
                return false;
            }

            if (MainOSStore.Partitions == null || MainOSStore.Partitions.Count() == 0)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: For Generating a FFU image, at least one partition must be specified.");
                return false;
            }

            if (SectorSize == 0U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The SectorSize cannot be 0. Please provide a valid SectorSize.");
                return false;
            }

            if (ChunkSize == 0U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The ChunkSize cannot be 0. Please provide a valid ChunkSize between 1-1024.");
                return false;
            }

            if (ChunkSize < 1U || ChunkSize > 1024U)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The ChunkSize must between 1-1024.");
                return false;
            }

            var num = 0;
            if (DevicePlatformIDs != null)
                foreach (var devicePlatformId in DevicePlatformIDs)
                    num += devicePlatformId.Length + 1;
            if (num > 191)
            {
                _logger.LogError(
                    "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: parameter DevicePlatformID larger than {0}.",
                    (object) 192U.ToString());
                return false;
            }

            foreach (var store in Stores)
            foreach (var partition in store.Partitions)
            {
                if (dictionary.ContainsKey(partition.Name))
                {
                    _logger.LogError("ImageCommon!ImageGeneratorParameters::VerifyInputParameters: A partition '" +
                                     partition.Name + "' is defined twice in the DeviceLayout.");
                    return false;
                }

                dictionary.Add(partition.Name, "Partitions");
            }

            var inputPartition = (InputPartition) null;
            foreach (var partition in MainOSStore.Partitions)
            {
                if (inputPartition != null)
                {
                    _logger.LogError(
                        "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: Partitions that specify UseAllSpace must be at the end.  See partition '{0}' and '{1}' for conflict.",
                        (object) inputPartition.Name, (object) partition.Name);
                    return false;
                }

                if (partition.UseAllSpace)
                {
                    inputPartition = partition;
                    if (partition.TotalSectors != 0U)
                    {
                        _logger.LogError(
                            "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: A partition cannot use all available space and have total sectors set.  See partition " +
                            partition.Name);
                        return false;
                    }
                }

                if (partition.ByteAlignment != 0U)
                {
                    if (partition.SingleSectorAlignment && (int) partition.ByteAlignment != (int) SectorSize)
                    {
                        _logger.LogError(
                            "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: Partition '{0}' has both a byte alignment and SingleSectorAlignment set.",
                            (object) partition.Name);
                        return false;
                    }

                    if (!InputHelpers.IsPowerOfTwo(partition.ByteAlignment))
                    {
                        _logger.LogError(
                            "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The byte alignment for partition '{0}' must be a multiple of 2: {1} bytes.",
                            (object) partition.Name, (object) partition.ByteAlignment);
                        return false;
                    }
                }

                if (partition.SingleSectorAlignment)
                    partition.ByteAlignment = SectorSize;
                if (!string.IsNullOrEmpty(partition.PrimaryPartition) &&
                    FindPartition(partition.PrimaryPartition) == null)
                {
                    _logger.LogError(string.Format(
                        "ImageCommon!ImageGeneratorParameters::VerifyInputParameters: The primary partition for partition '{0}' is not found Primary: '{1}'.",
                        partition.Name, partition.PrimaryPartition));
                    return false;
                }
            }

            return VerifyPartitionSizes();
        }

        private InputPartition FindPartition(string PartitionName)
        {
            foreach (var store in Stores)
            {
                var source =
                    store.Partitions.Where(x => x.Name.Equals(PartitionName, StringComparison.OrdinalIgnoreCase));
                if (source.ToArray().Length != 0)
                    return source.First();
            }

            return null;
        }

        public static bool IsDeviceLayoutV2(string DeviceLayoutXMLFile)
        {
            var navigator = new XPathDocument(DeviceLayoutXMLFile).CreateNavigator();
            navigator.MoveToFollowing(XPathNodeType.Element);
            return navigator.GetNamespacesInScope(XmlNamespaceScope.All).Values.Any(x =>
                string.CompareOrdinal(x, "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2") == 0);
        }

        public static Stream GetDeviceLayoutXSD(string deviceLayoutXMLFile)
        {
            if (IsDeviceLayoutV2(deviceLayoutXMLFile))
                return GetXSDStream(DevicePaths.DeviceLayoutSchema2);
            return GetXSDStream(DevicePaths.DeviceLayoutSchema);
        }

        public static Stream GetOEMDevicePlatformXSD()
        {
            return GetXSDStream(DevicePaths.OEMDevicePlatformSchema);
        }

        public static Stream GetXSDStream(string xsdID)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var manifestResourceNames = executingAssembly.GetManifestResourceNames();
            var name = string.Empty;
            foreach (var str in manifestResourceNames)
                if (str.Contains(xsdID))
                {
                    name = str;
                    break;
                }

            if (string.IsNullOrEmpty(name))
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::GetXSDStream: XSD resource was not found: " + xsdID);
            return executingAssembly.GetManifestResourceStream(name);
        }

        public void ProcessInputXML(string deviceLayoutXMLFile, string oemDevicePlatformXMLFile)
        {
            var devicePlatformInput = (OEMDevicePlatformInput) null;
            var xsdValidator1 = new XsdValidator();
            try
            {
                using (var deviceLayoutXsd = GetDeviceLayoutXSD(deviceLayoutXMLFile))
                {
                    xsdValidator1.ValidateXsd(deviceLayoutXsd, deviceLayoutXMLFile, _logger);
                }
            }
            catch (XsdValidatorException ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to validate Device Layout XSD.", ex);
            }

            _logger.LogInfo("ImageCommon: Successfully validated the Device Layout XML");
            if (IsDeviceLayoutV2(deviceLayoutXMLFile))
                InitializeV2DeviceLayout(deviceLayoutXMLFile);
            else
                InitializeV1DeviceLayout(deviceLayoutXMLFile);
            var xsdValidator2 = new XsdValidator();
            try
            {
                using (var devicePlatformXsd = GetOEMDevicePlatformXSD())
                {
                    xsdValidator2.ValidateXsd(devicePlatformXsd, oemDevicePlatformXMLFile, _logger);
                }
            }
            catch (XsdValidatorException ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to validate OEM Device Platform XSD.",
                    ex);
            }

            _logger.LogInfo("ImageCommon: Successfully validated the OEM Device Platform XML");
            var xmlSerializer = new XmlSerializer(typeof(OEMDevicePlatformInput));
            using (var streamReader = new StreamReader(oemDevicePlatformXMLFile))
            {
                try
                {
                    devicePlatformInput = (OEMDevicePlatformInput) xmlSerializer.Deserialize(streamReader);
                }
                catch (Exception ex)
                {
                    throw new ImageCommonException(
                        "ImageCommon!ImageGeneratorParameters::ProcessInputXML: Unable to parse OEM Device Platform XML.",
                        ex);
                }
            }

            try
            {
                DevicePlatformIDs = devicePlatformInput.DevicePlatformIDs;
                MinSectorCount = devicePlatformInput.MinSectorCount;
                UEFI = devicePlatformInput.BIOSMode.Equals("UEFI", StringComparison.OrdinalIgnoreCase);
                foreach (var store in Stores)
                foreach (var inputPartition in store.Partitions.Where(x =>
                {
                    if (!string.IsNullOrEmpty(x.FileSystem))
                        return x.FileSystem.Equals("NTFS", StringComparison.OrdinalIgnoreCase);
                    return false;
                }))
                    inputPartition.Compressed = true;
                foreach (var PartitionName in devicePlatformInput.UncompressedPartitions ?? new string[0])
                {
                    var partition = FindPartition(PartitionName);
                    if (partition == null)
                        throw new ImageCommonException("Partition " + PartitionName +
                                                       " was marked in the OEMDevicePlatform as uncompressed, but the partition doesn't exist in the device layout. Please ensure the spelling of the partition is correct in OEMDevicePlatform and that the partition is defined in the OEMDeviceLayout.");
                    _logger.LogInfo("ImageCommon: Marking partition " + PartitionName +
                                    " uncompressed as requested by device plaform.");
                    partition.Compressed = false;
                }

                AddSectorsToMainOs(devicePlatformInput.AdditionalMainOSFreeSectorsRequest,
                    devicePlatformInput.MainOSRTCDataReservedSectors);
                if (devicePlatformInput.MMOSPartitionTotalSectorsOverride != 0U)
                {
                    var partition = FindPartition("MMOS");
                    if (partition == null)
                        throw new ImageCommonException(
                            "ImageCommon!ImageGeneratorParameters::ProcessInputXML: The OEM Device Platform XML specifies that the MMOS should have total sectors set but no MMOS partition was found.");
                    partition.TotalSectors = devicePlatformInput.MMOSPartitionTotalSectorsOverride;
                }

                if (devicePlatformInput.Rules == null)
                    return;
                Rules = devicePlatformInput.Rules;
            }
            catch (ImageCommonException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::ProcessInputXML: There was a problem parsing the OEM Device Platform input",
                    ex);
            }
        }

        private void InitializeV1DeviceLayout(string DeviceLayoutXMLFile)
        {
            var xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(DeviceLayoutXMLFile);
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: Unable to validate Device Layout XSD.",
                    ex);
            }

            var deviceLayoutInput = (DeviceLayoutInput) null;
            using (var streamReader = new StreamReader(DeviceLayoutXMLFile))
            {
                try
                {
                    deviceLayoutInput =
                        (DeviceLayoutInput) new XmlSerializer(typeof(DeviceLayoutInput)).Deserialize(streamReader);
                }
                catch (Exception ex)
                {
                    throw new ImageCommonException(
                        "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: Unable to parse Device Layout XML.",
                        ex);
                }
            }

            try
            {
                var inputStore = new InputStore("MainOSStore");
                if (deviceLayoutInput.Partitions != null)
                    inputStore.Partitions = deviceLayoutInput.Partitions;
                SectorSize = deviceLayoutInput.SectorSize;
                ChunkSize = deviceLayoutInput.ChunkSize;
                VirtualHardDiskSectorSize = deviceLayoutInput.SectorSize;
                DefaultPartitionByteAlignment = deviceLayoutInput.DefaultPartitionByteAlignment;
                foreach (var partition in inputStore.Partitions)
                {
                    if (partition.MinFreeSectors != 0U)
                    {
                        if (partition.TotalSectors != 0U || partition.UseAllSpace)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: MinFreeSectors cannot be set for partition '" +
                                partition.Name + "' when either TotalSectors or UseAllSpace is set.");
                        if (partition.MinFreeSectors < 8192U)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: MinFreeSectors cannot be set for partition '" +
                                partition.Name + "' less than " + 8192U + " sectors.");
                    }

                    if (partition.GeneratedFileOverheadSectors != 0U && partition.MinFreeSectors == 0U)
                        throw new ImageCommonException(
                            "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: GeneratedFileOverheadSectors cannot be set for partition '" +
                            partition.Name + "' without MinFreeSectors being set.");
                }

                Stores.Add(inputStore);
            }
            catch (ImageCommonException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::InitializeV1DeviceLayout: There was a problem parsing the Device Layout input",
                    ex);
            }
        }

        private void InitializeV2DeviceLayout(string DeviceLayoutXMLFile)
        {
            var xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(DeviceLayoutXMLFile);
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Unable to validate Device Layout XSD.",
                    ex);
            }

            var deviceLayoutInputv2 = (DeviceLayoutInputv2) null;
            using (var streamReader = new StreamReader(DeviceLayoutXMLFile))
            {
                try
                {
                    deviceLayoutInputv2 =
                        (DeviceLayoutInputv2) new XmlSerializer(typeof(DeviceLayoutInputv2)).Deserialize(streamReader);
                }
                catch (Exception ex)
                {
                    throw new ImageCommonException(
                        "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Unable to parse Device Layout XML.",
                        ex);
                }
            }

            try
            {
                if (deviceLayoutInputv2.Stores != null)
                    Stores = new List<InputStore>(deviceLayoutInputv2.Stores);
                SectorSize = deviceLayoutInputv2.SectorSize;
                ChunkSize = deviceLayoutInputv2.ChunkSize;
                VirtualHardDiskSectorSize = deviceLayoutInputv2.SectorSize;
                DefaultPartitionByteAlignment = deviceLayoutInputv2.DefaultPartitionByteAlignment;
                foreach (var store in Stores)
                {
                    if (store.IsMainOSStore())
                    {
                        if (store.SizeInSectors != 0U)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: SizeInSector cannot be set for MainOS store.'");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(store.Id))
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Id needs to be set for individual stores.'");
                        if (store.SizeInSectors == 0U)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: SizeInSector needs to be set for non-MainOS store '" +
                                store.Id + "'.");
                        if (store.SizeInSectors * (ulong) SectorSize < 3145728UL)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Minimum size of a store '" +
                                store.Id + "' must be 3MB or larger.");
                    }

                    if (string.IsNullOrEmpty(store.StoreType))
                        throw new ImageCommonException(
                            "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: StoreType needs to be set for store '" +
                            store.Id + "'.");
                    if (string.IsNullOrEmpty(store.DevicePath))
                        throw new ImageCommonException(
                            "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: DevicePath needs to be set for store '" +
                            store.Id + "'.");
                    if (store.OnlyAllocateDefinedGptEntries && store.Partitions.Count() > 32)
                        throw new ImageCommonException(
                            "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: Cannot use shortened GPT as it has more than 32 partitions for store '" +
                            store.Id + "'.");
                    foreach (var partition in store.Partitions)
                    {
                        if (partition.MinFreeSectors != 0U)
                        {
                            if (partition.TotalSectors != 0U || partition.UseAllSpace)
                                throw new ImageCommonException(
                                    "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: MinFreeSectors cannot be set for partition '" +
                                    partition.Name + "' when either TotalSectors or UseAllSpace is set.");
                            if (partition.MinFreeSectors < 8192U)
                                throw new ImageCommonException(
                                    "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: MinFreeSectors cannot be set for partition '" +
                                    partition.Name + "' less than " + 8192U + " sectors.");
                        }

                        if (partition.GeneratedFileOverheadSectors != 0U && partition.MinFreeSectors == 0U)
                            throw new ImageCommonException(
                                "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: GeneratedFileOverheadSectors cannot be set for partition '" +
                                partition.Name + "' without MinFreeSectors being set.");
                    }
                }
            }
            catch (ImageCommonException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::InitializeV2DeviceLayout: There was a problem parsing the Device Layout input",
                    ex);
            }

            DeviceLayoutVersion = 2U;
        }

        private void AddSectorsToMainOs(uint additionalFreeSectors, uint runtimeConfigurationDataSectors)
        {
            var partition = FindPartition("MainOS");
            if (partition == null)
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: No MainOS partition found for additional free sectors.");
            if ((additionalFreeSectors != 0U || runtimeConfigurationDataSectors != 0U) &&
                partition.MinFreeSectors == 0U)
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: The OEM Device Platform XML specifies that the MainOS should have additional free sectors but the MainOS partition is not using MinFreeSectors.");
            if (runtimeConfigurationDataSectors > 104857600U / SectorSize)
                throw new ImageCommonException(
                    "ImageCommon!ImageGeneratorParameters::AddSectorsToMainOs: Runtime configuration data reservation is limited to 100MB. Please reduce the number of sectors requested in 'MainOSMVDataReservedSectors' in the OEM device platform input.");
            if (additionalFreeSectors != 0U)
                _logger.LogInfo(
                    "OEM device platform input requested {0} additional free sectors in the MainOS partition.",
                    (object) additionalFreeSectors);
            if (runtimeConfigurationDataSectors != 0U)
                _logger.LogInfo(
                    "OEM device platform input requested {0} additional sectors for runtime configuration data be reserved in the MainOS partition.",
                    (object) runtimeConfigurationDataSectors);
            partition.MinFreeSectors += additionalFreeSectors;
            partition.MinFreeSectors += runtimeConfigurationDataSectors;
        }
    }
}