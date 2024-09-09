// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageGenerator
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Linq;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ImageGenerator
    {
        private const string c_RulesVersion = "1.0";
        private IULogger _logger;
        private ImageGeneratorParameters _parameters;

        public void Initialize(ImageGeneratorParameters parameters, IULogger logger)
        {
            _logger = logger;
            if (logger == null)
            {
                _logger = new IULogger();
            }

            _parameters = parameters;
        }

        public FullFlashUpdateImage CreateFFU()
        {
            FullFlashUpdateImage image = new();
            if (_parameters == null)
            {
                throw new ImageCommonException(
                                "ImageCommon!ImageGenerator::CreateFFU: ImageGenerator has not been initialized.");
            }

            try
            {
                _ = _parameters.VerifyInputParameters();
                image.Initialize();
                image.Description = _parameters.Description;
                image.DevicePlatformIDs = _parameters.DevicePlatformIDs.ToList();
                image.ChunkSize = _parameters.ChunkSize;
                image.HashAlgorithmID = _parameters.Algid;
                image.DefaultPartitionAlignmentInBytes = _parameters.DefaultPartitionByteAlignment;
                image.UEFI = _parameters.UEFI;
            }
            catch (Exception ex)
            {
                throw new ImageCommonException("ImageCommon!ImageGenerator::CreateFFU: Failed to Initialize FFU: ", ex);
            }

            if (_parameters.Rules != null)
            {
                StringBuilder stringBuilder1 = new();
                foreach (InputIntegerRule integerRule in _parameters.Rules.IntegerRules)
                {
                    ulong? nullable1 = integerRule.Min;
                    if (!nullable1.HasValue)
                    {
                        nullable1 = integerRule.Max;
                        if (!nullable1.HasValue)
                        {
                            goto label_10;
                        }
                    }

                    if (integerRule.Values != null && integerRule.Values.Length != 0)
                    {
                        throw new ImageCommonException(
                                                "ImageCommon!ImageGenerator::CreateFFU: Cannot specify both min/max value and list at the same time");
                    }

                label_10:
                    if (!integerRule.Property.All(char.IsLetterOrDigit))
                    {
                        throw new ImageCommonException(
                                                "ImageCommon!ImageGenerator::CreateFFU: Only alphanumerics are allowed for the rule property");
                    }

                    nullable1 = integerRule.Min;
                    if (!nullable1.HasValue)
                    {
                        nullable1 = integerRule.Max;
                        if (!nullable1.HasValue)
                        {
                            _ = stringBuilder1.AppendFormat("{0}={1}[{2}", integerRule.Property, integerRule.ModeCharacter,
                                integerRule.Values[0]);
                            foreach (ulong num in integerRule.Values.Skip(1))
                            {
                                _ = stringBuilder1.AppendFormat(",{0}", num);
                            }

                            _ = stringBuilder1.Append("];");
                            continue;
                        }
                    }

                    nullable1 = integerRule.Min;
                    ulong? nullable2;
                    if (nullable1.HasValue)
                    {
                        nullable1 = integerRule.Max;
                        if (nullable1.HasValue)
                        {
                            nullable1 = integerRule.Min;
                            nullable2 = integerRule.Max;
                            if ((nullable1.GetValueOrDefault() > nullable2.GetValueOrDefault()
                                    ? nullable1.HasValue & nullable2.HasValue ? 1 : 0
                                    : 0) != 0)
                            {
                                throw new ImageCommonException(
                                                                "ImageCommon!ImageGenerator::CreateFFU: Invalid min/max integer rule");
                            }
                        }
                    }

                    StringBuilder stringBuilder2 = stringBuilder1;
                    string format = "{0}={1}<{2},{3}>;";
                    object[] objArray = new object[4]
                    {
                        integerRule.Property,
                        integerRule.ModeCharacter,
                        null,
                        null
                    };
                    int index1 = 2;
                    nullable2 = integerRule.Min;
                    string empty1;
                    if (nullable2.HasValue)
                    {
                        nullable2 = integerRule.Min;
                        empty1 = nullable2.ToString();
                    }
                    else
                    {
                        empty1 = string.Empty;
                    }

                    objArray[index1] = empty1;
                    int index2 = 3;
                    nullable2 = integerRule.Max;
                    string empty2;
                    if (nullable2.HasValue)
                    {
                        nullable2 = integerRule.Max;
                        empty2 = nullable2.ToString();
                    }
                    else
                    {
                        empty2 = string.Empty;
                    }

                    objArray[index2] = empty2;
                    _ = stringBuilder2.AppendFormat(format, objArray);
                }

                UnicodeEncoding unicodeEncoding = new();
                foreach (InputStringRule stringRule in _parameters.Rules.StringRules)
                {
                    if (!stringRule.Property.All(char.IsLetterOrDigit))
                    {
                        throw new ImageCommonException(
                                                "ImageCommon!ImageGenerator::CreateFFU: Only alphanumerics are allowed for the rule property");
                    }

                    _ = stringBuilder1.AppendFormat("{0}={1}", stringRule.Property, stringRule.ModeCharacter);
                    _ = stringBuilder1.Append("{");
                    _ = stringBuilder1.Append(Convert.ToBase64String(unicodeEncoding.GetBytes(stringRule.Values[0])));
                    foreach (string s in stringRule.Values.Skip(1))
                    {
                        _ = stringBuilder1.AppendFormat(",{0}", Convert.ToBase64String(unicodeEncoding.GetBytes(s)));
                    }

                    _ = stringBuilder1.Append("};");
                }

                image.RulesVersion = "1.0";
                image.RulesData = stringBuilder1.ToString();
            }

            try
            {
                foreach (InputStore store1 in _parameters.Stores)
                {
                    FullFlashUpdateImage.FullFlashUpdateStore store2 = new();
                    uint minSectorCount = _parameters.MinSectorCount;
                    if (!store1.IsMainOSStore())
                    {
                        minSectorCount = store1.SizeInSectors;
                    }

                    store2.Initialize(image, store1.Id, store1.IsMainOSStore(), store1.DevicePath,
                                            store1.OnlyAllocateDefinedGptEntries, minSectorCount, _parameters.SectorSize);
                    foreach (InputPartition partition1 in store1.Partitions)
                    {
                        FullFlashUpdateImage.FullFlashUpdatePartition partition2 = new();
                        partition2.Initialize(0U, partition1.TotalSectors, partition1.Type, partition1.Id,
                            partition1.Name, store2, partition1.UseAllSpace);
                        partition2.FileSystem = partition1.FileSystem;
                        partition2.Bootable = partition1.Bootable;
                        partition2.ReadOnly = partition1.ReadOnly;
                        partition2.Hidden = partition1.Hidden;
                        partition2.AttachDriveLetter = partition1.AttachDriveLetter;
                        partition2.PrimaryPartition = partition1.PrimaryPartition;
                        partition2.RequiredToFlash = partition1.RequiredToFlash;
                        partition2.ByteAlignment = partition1.ByteAlignment;
                        partition2.ClusterSize = partition1.ClusterSize;
                        partition2.OffsetInSectors = partition1.OffsetInSectors;
                        store2.AddPartition(partition2);
                        if (!store1.IsMainOSStore() && partition1.ByteAlignment == 0U)
                        {
                            partition2.ByteAlignment = image.ChunkSize * 1024U;
                        }
                    }

                    image.AddStore(store2);
                }
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageGenerator::CreateFFU: Failed to add partitions to FFU: ", ex);
            }

            return image;
        }
    }
}