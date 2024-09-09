// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElement
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

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

        public BcdElementDataType DataType
        {
            get; set;
        }

        public string StringData
        {
            get => _stringData;
            set
            {
                if (DataType.RegistryValueType != RegistryValueKind.String)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: Cannot set string data for an element format: {1}", MethodBase.GetCurrentMethod().Name,
                                        DataType.Format));
                }

                _stringData = value;
            }
        }

        public List<string> MultiStringData => _multiStringData;

        public override string ToString()
        {
            return DataType != null ? DataType.ToString() : "Unnamed BcdElement";
        }

        public static BcdElement CreateElement(OfflineRegistryHandle elementKey)
        {
            BcdElementDataType dataType = new();
            byte[] binaryData = null;
            string stringData = null;
            string[] multiStringData = null;
            uint num = uint.Parse(elementKey.Name[(elementKey.Name.LastIndexOf('\\') + 1)..],
                NumberStyles.HexNumber);
            dataType.RawValue = num;
            switch (dataType.RegistryValueType)
            {
                case RegistryValueKind.String:
                    stringData = (string)elementKey.GetValue("Element", string.Empty);
                    break;
                case RegistryValueKind.Binary:
                    binaryData = (byte[])elementKey.GetValue("Element", null);
                    break;
                case RegistryValueKind.MultiString:
                    multiStringData = (string[])elementKey.GetValue("Element", null);
                    break;
                default:
                    return null;
            }

            return dataType.Format switch
            {
                ElementFormat.Device => new BcdElementDevice(binaryData, dataType),
                ElementFormat.String => new BcdElementString(stringData, dataType),
                ElementFormat.Object => new BcdElementObject(stringData, dataType),
                ElementFormat.ObjectList => new BcdElementObjectList(multiStringData, dataType),
                ElementFormat.Integer => new BcdElementInteger(binaryData, dataType),
                ElementFormat.Boolean => new BcdElementBoolean(binaryData, dataType),
                ElementFormat.IntegerList => new BcdElementIntegerList(binaryData, dataType),
                _ => throw new ImageStorageException(string.Format("{0}: Unknown element format: {1}.",
                                        MethodBase.GetCurrentMethod().Name, dataType.RawValue)),
            };
        }

        public byte[] GetBinaryData()
        {
            return _binaryData;
        }

        public void SetBinaryData(byte[] binaryData)
        {
            if (DataType.RegistryValueType != RegistryValueKind.Binary)
            {
                throw new ImageStorageException(string.Format("{0}: Cannot set binary data for an element format: {1}",
                                MethodBase.GetCurrentMethod().Name, DataType.Format));
            }

            _binaryData = binaryData;
        }


        public virtual void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "BCD Element:        {0:x}", DataType.RawValue);
            DataType.LogInfo(logger, indentLevel);
        }
    }
}