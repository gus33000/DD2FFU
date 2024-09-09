// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BootConfigurationDatabase
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BootConfigurationDatabase : IDisposable
    {
        public static readonly string SubKeyName = "BootConfigurationKey";
        private bool _alreadyDisposed;
        private OfflineRegistryHandle _bcdKey;
        private string _filePath;
        private IULogger _logger = new();

        public BootConfigurationDatabase(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ImageStorageException(string.Format("{0}: The filePath is empty or null.",
                                MethodBase.GetCurrentMethod().Name));
            }

            if (!File.Exists(filePath))
            {
                throw new ImageStorageException(string.Format("{0}: The file ({1}) does not exist or is inaccessible.",
                                MethodBase.GetCurrentMethod().Name, filePath));
            }

            _filePath = filePath;
            Objects = [];
        }


        public BootConfigurationDatabase(string filePath, IULogger logger)
            : this(filePath)
        {
            _logger = logger;
        }

        public List<BcdObject> Objects
        {
            get;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BootConfigurationDatabase()
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
                _filePath = null;
                _logger = null;
            }

            if (_bcdKey != null)
            {
                _bcdKey.Close();
                _bcdKey = null;
            }

            _alreadyDisposed = true;
        }

        public void Mount()
        {
            _bcdKey = new OfflineRegistryHandle(_filePath);
            try
            {
                OfflineRegistryHandle offlineRegistryHandle2;
                try
                {
                    offlineRegistryHandle2 = _bcdKey.OpenSubKey("Objects");
                }
                catch (Exception ex)
                {
                    throw new ImageStorageException(
                        string.Format("{0}: The BCD hive is invalid.  Unable to open the 'Objects' key.",
                            MethodBase.GetCurrentMethod().Name), ex);
                }

                try
                {
                    string[] subKeyNames = offlineRegistryHandle2.GetSubKeyNames();
                    if (subKeyNames == null || subKeyNames.Length == 0)
                    {
                        throw new ImageStorageException(string.Format(
                                                "{0}: The BCD hive is invalid. There are no keys under 'Objects'.",
                                                MethodBase.GetCurrentMethod().Name));
                    }

                    foreach (string str in subKeyNames)
                    {
                        BcdObject bcdObject = new(str);
                        OfflineRegistryHandle objectKey = offlineRegistryHandle2.OpenSubKey(str);
                        try
                        {
                            bcdObject.ReadFromRegistry(objectKey);
                        }
                        finally
                        {
                            objectKey?.Close();
                        }

                        Objects.Add(bcdObject);
                    }
                }
                finally
                {
                    offlineRegistryHandle2?.Close();
                }
            }
            catch (ImageStorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to mount and parse the BCD key.", MethodBase.GetCurrentMethod().Name),
                    ex);
            }
        }

        public void DismountHive(bool save)
        {
            try
            {
                string tempFileName = Path.GetTempFileName();
                File.Delete(tempFileName);
                _bcdKey.SaveHive(tempFileName);
                _bcdKey.Close();
                _bcdKey = null;
                FileAttributes attributes = File.GetAttributes(_filePath);
                if ((attributes & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(_filePath, attributes & ~FileAttributes.ReadOnly);
                }

                File.Delete(_filePath);
                File.Move(tempFileName, _filePath);
                File.SetAttributes(_filePath, attributes);
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to save the hive.", MethodBase.GetCurrentMethod().Name), ex);
            }
        }

        public BcdObject GetObject(Guid objectId)
        {
            foreach (BcdObject bcdObject in Objects)
            {
                if (bcdObject.Id == objectId)
                {
                    return bcdObject;
                }
            }

            return null;
        }

        public void AddObject(BcdObject bcdObject)
        {
            foreach (BcdObject bcdObject1 in Objects)
            {
                if (bcdObject1.Id == bcdObject.Id)
                {
                    throw new ImageStorageException(string.Format("{0}: The object already exists in the BCD.",
                                        MethodBase.GetCurrentMethod().Name));
                }
            }

            Objects.Add(bcdObject);
        }

        public void LogInfo(int indentLevel)
        {
            _logger.LogInfo(new StringBuilder().Append(' ', indentLevel) + "Boot Configuration Database");
            foreach (BcdObject bcdObject in Objects)
            {
                bcdObject.LogInfo(_logger, checked(indentLevel + 2));
                _logger.LogInfo("");
            }
        }

        public void SaveObject(BcdObject bcdObject)
        {
            OfflineRegistryHandle offlineRegistryHandle1 = null;
            OfflineRegistryHandle offlineRegistryHandle2 = null;
            OfflineRegistryHandle offlineRegistryHandle3 = null;
            OfflineRegistryHandle offlineRegistryHandle4 = null;
            try
            {
                offlineRegistryHandle1 = _bcdKey.OpenSubKey("objects");
                offlineRegistryHandle2 = offlineRegistryHandle1.CreateSubKey(bcdObject.Name);
                offlineRegistryHandle3 = offlineRegistryHandle2.CreateSubKey("Elements");
                offlineRegistryHandle4 = offlineRegistryHandle2.CreateSubKey("Description");
                offlineRegistryHandle4.SetValue("Type", bcdObject.Type);
            }
            finally
            {
                offlineRegistryHandle2?.Close();
                offlineRegistryHandle1?.Close();
                offlineRegistryHandle3?.Close();
                offlineRegistryHandle4?.Close();
            }
        }

        private void SaveBinaryDeviceElement(OfflineRegistryHandle elementKey, byte[] binaryData)
        {
            elementKey.SetValue("Element", binaryData, 3U);
        }

        [Conditional("DEBUG")]
        private void ValidateDeviceElement(BcdObject bcdObject, BcdElement bcdElement, OfflineRegistryHandle elementKey)
        {
            BcdElementDevice bcdElementDevice = bcdElement as BcdElementDevice;
            byte[] buffer = new byte[(int)bcdElementDevice.BinarySize];
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(buffer);
                bcdElementDevice.WriteToStream(memoryStream);
                byte[] binaryData = bcdElement.GetBinaryData();
                if (binaryData.Length != buffer.Length)
                {
                    throw new ImageStorageException("The binary data length is wrong.");
                }

                for (int index = 0; index < binaryData.Length; ++index)
                {
                    if (buffer[index] != binaryData[index])
                    {
                        throw new ImageStorageException("The binary data is wrong.");
                    }
                }
            }
            finally
            {
                memoryStream?.Close();
            }
        }

        private void SaveStringDeviceElement(OfflineRegistryHandle elementKey, string value)
        {
            elementKey.SetValue("Element", value);
        }

        private void SaveMultiStringDeviceElement(OfflineRegistryHandle elementKey, List<string> values)
        {
            elementKey.SetValue("Element", values);
        }

        public void SaveElementValue(BcdObject bcdObject, BcdElement bcdElement)
        {
            OfflineRegistryHandle offlineRegistryHandle1 = null;
            OfflineRegistryHandle offlineRegistryHandle2 = null;
            OfflineRegistryHandle offlineRegistryHandle3 = null;
            OfflineRegistryHandle elementKey = null;
            string str = bcdElement.DataType.ToString();
            try
            {
                offlineRegistryHandle1 = _bcdKey.OpenSubKey("Objects");
                offlineRegistryHandle2 = offlineRegistryHandle1.OpenSubKey(bcdObject.Name);
                offlineRegistryHandle3 = offlineRegistryHandle2.OpenSubKey("Elements");
                elementKey = offlineRegistryHandle3.OpenSubKey(str) ?? offlineRegistryHandle3.CreateSubKey(str);
                switch (bcdElement.DataType.Format)
                {
                    case ElementFormat.Device:
                        SaveBinaryDeviceElement(elementKey, bcdElement.GetBinaryData());
                        break;
                    case ElementFormat.String:
                        SaveStringDeviceElement(elementKey, bcdElement.StringData);
                        break;
                    case ElementFormat.Object:
                        SaveStringDeviceElement(elementKey, bcdElement.StringData);
                        break;
                    case ElementFormat.ObjectList:
                        SaveMultiStringDeviceElement(elementKey, bcdElement.MultiStringData);
                        break;
                    case ElementFormat.Integer:
                        SaveBinaryDeviceElement(elementKey, bcdElement.GetBinaryData());
                        break;
                    case ElementFormat.Boolean:
                        SaveBinaryDeviceElement(elementKey, bcdElement.GetBinaryData());
                        break;
                    case ElementFormat.IntegerList:
                        SaveBinaryDeviceElement(elementKey, bcdElement.GetBinaryData());
                        break;
                    default:
                        throw new ImageStorageException(string.Format("{0}: Unknown element format: {1}.",
                            MethodBase.GetCurrentMethod().Name, bcdElement.DataType.RawValue));
                }
            }
            finally
            {
                elementKey?.Close();
                offlineRegistryHandle3?.Close();
                offlineRegistryHandle2?.Close();
                offlineRegistryHandle1?.Close();
            }
        }
    }
}