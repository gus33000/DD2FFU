// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElement
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.Win32;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElement
    {
        private byte[] _binaryData;
        protected List<string> _multiStringData;
        private string _stringData;

        protected BcdElement(BcdElementDataType dataType)
        {
            DataType = dataType;
        }

        public BcdElementDataType DataType { get; set; }

        public string StringData
        {
            get => _stringData;
            set
            {
                if (DataType.RegistryValueType != RegistryValueKind.String)
                    throw new ImageStorageException(string.Format(
                        "{0}: Cannot set string data for an element format: {1}", MethodBase.GetCurrentMethod().Name,
                        DataType.Format));
                _stringData = value;
            }
        }

        public List<string> MultiStringData => _multiStringData;

        public override string ToString()
        {
            if (DataType != null)
                return DataType.ToString();
            return "Unnamed BcdElement";
        }

        public static BcdElement CreateElement(OfflineRegistryHandle elementKey)
        {
            var dataType = new BcdElementDataType();
            var binaryData = (byte[]) null;
            var stringData = (string) null;
            var multiStringData = (string[]) null;
            var num = uint.Parse(elementKey.Name.Substring(elementKey.Name.LastIndexOf('\\') + 1),
                NumberStyles.HexNumber);
            dataType.RawValue = num;
            switch (dataType.RegistryValueType)
            {
                case RegistryValueKind.String:
                    stringData = (string) elementKey.GetValue("Element", string.Empty);
                    break;
                case RegistryValueKind.Binary:
                    binaryData = (byte[]) elementKey.GetValue("Element", null);
                    break;
                case RegistryValueKind.MultiString:
                    multiStringData = (string[]) elementKey.GetValue("Element", null);
                    break;
                default:
                    return null;
            }

            switch (dataType.Format)
            {
                case ElementFormat.Device:
                    return new BcdElementDevice(binaryData, dataType);
                case ElementFormat.String:
                    return new BcdElementString(stringData, dataType);
                case ElementFormat.Object:
                    return new BcdElementObject(stringData, dataType);
                case ElementFormat.ObjectList:
                    return new BcdElementObjectList(multiStringData, dataType);
                case ElementFormat.Integer:
                    return new BcdElementInteger(binaryData, dataType);
                case ElementFormat.Boolean:
                    return new BcdElementBoolean(binaryData, dataType);
                case ElementFormat.IntegerList:
                    return new BcdElementIntegerList(binaryData, dataType);
                default:
                    throw new ImageStorageException(string.Format("{0}: Unknown element format: {1}.",
                        MethodBase.GetCurrentMethod().Name, dataType.RawValue));
            }
        }

        public byte[] GetBinaryData()
        {
            return _binaryData;
        }

        public void SetBinaryData(byte[] binaryData)
        {
            if (DataType.RegistryValueType != RegistryValueKind.Binary)
                throw new ImageStorageException(string.Format("{0}: Cannot set binary data for an element format: {1}",
                    MethodBase.GetCurrentMethod().Name, DataType.Format));
            _binaryData = binaryData;
        }

        
        public virtual void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "BCD Element:        {0:x}", (object) DataType.RawValue);
            DataType.LogInfo(logger, indentLevel);
        }
    }
}