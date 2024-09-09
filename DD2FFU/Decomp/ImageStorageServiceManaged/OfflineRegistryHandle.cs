﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.OfflineRegistryHandle
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public sealed class OfflineRegistryHandle : SafeHandle
    {
        private bool _disposed;
        private readonly bool _hive;
        private readonly string _path;

        public OfflineRegistryHandle(string hivePath)
            : base(nint.Zero, true)
        {
            UnsafeHandle = Win32Exports.OfflineRegistryOpenHive(hivePath);
            _hive = true;
            Name = hivePath;
            _path = "";
        }

        private OfflineRegistryHandle(nint subKeyHandle, string name, string path)
            : base(nint.Zero, true)
        {
            UnsafeHandle = subKeyHandle;
            _hive = false;
            Name = name;
            _path = path;
        }

        public nint UnsafeHandle
        {
            get;
        }

        public override bool IsInvalid => _disposed;

        public string Name
        {
            get;
        }

        public string Path => _hive ? "[" + Name + "]" : _path + "\\" + Name;

        public void SaveHive(string path)
        {
            if (!_hive)
            {
                throw new ImageStorageException(string.Format("{0}: This function can only be called on a hive handle.",
                                MethodBase.GetCurrentMethod().Name));
            }

            Win32Exports.OfflineRegistrySaveHive(UnsafeHandle, path);
        }

        public static implicit operator nint(OfflineRegistryHandle offlineRegistryHandle)
        {
            return offlineRegistryHandle.UnsafeHandle;
        }

        protected override bool ReleaseHandle()
        {
            if (!_disposed)
            {
                _disposed = true;
                GC.SuppressFinalize(this);
                if (UnsafeHandle != nint.Zero)
                {
                    if (_hive)
                    {
                        Win32Exports.OfflineRegistryCloseHive(UnsafeHandle);
                    }
                    else
                    {
                        Win32Exports.OfflineRegistryCloseSubKey(UnsafeHandle);
                    }
                }
            }

            return true;
        }

        public string[] GetSubKeyNames()
        {
            List<string> stringList = [];
            uint num = 0;
            string str;
            do
            {
                str = Win32Exports.OfflineRegistryEnumKey(UnsafeHandle, num++);
                if (str != null)
                {
                    stringList.Add(str);
                }
            } while (str != null);

            return stringList.ToArray();
        }

        public string[] GetValueNames()
        {
            List<string> stringList = [];
            uint num = 0;
            string str;
            do
            {
                str = Win32Exports.OfflineRegistryEnumValue(UnsafeHandle, num++);
                if (str != null)
                {
                    stringList.Add(str);
                }
            } while (str != null);

            return stringList.ToArray();
        }

        public OfflineRegistryHandle OpenSubKey(string keyName)
        {
            nint subKeyHandle = Win32Exports.OfflineRegistryOpenSubKey(UnsafeHandle, keyName);
            return subKeyHandle == nint.Zero ? null : new OfflineRegistryHandle(subKeyHandle, keyName, Path);
        }

        public RegistryValueKind GetValueKind(string valueName)
        {
            return GetValueKind(Win32Exports.OfflineRegistryGetValueKind(UnsafeHandle, valueName));
        }


        public uint GetValueSize(string valueName)
        {
            return Win32Exports.OfflineRegistryGetValueSize(UnsafeHandle, valueName);
        }

        public object GetValue(string valueName)
        {
            return Win32Exports.OfflineRegistryGetValue(UnsafeHandle, valueName);
        }

        public object GetValue(string valueName, object defaultValue)
        {
            object obj = defaultValue;

            try
            {
                obj = Win32Exports.OfflineRegistryGetValue(UnsafeHandle, valueName);
            }
            catch (ImageStorageException)
            {
            }

            return obj;
        }

        public void SetValue(string valueName, byte[] binaryData)
        {
            Win32Exports.OfflineRegistrySetValue(UnsafeHandle, valueName,
                Win32Exports.OfflineRegistryGetValueKind(UnsafeHandle, valueName), binaryData);
        }


        public void SetValue(string valueName, byte[] binaryData, uint valueType)
        {
            Win32Exports.OfflineRegistrySetValue(UnsafeHandle, valueName, valueType, binaryData);
        }

        public void SetValue(string valueName, string stringData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(stringData);
            SetValue(valueName, bytes, 1U);
        }

        public void SetValue(string valueName, List<string> values)
        {
            StringBuilder stringBuilder = new();
            foreach (string str in values)
            {
                _ = stringBuilder.Append(str);
                _ = stringBuilder.Append(char.MinValue);
            }

            byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
            SetValue(valueName, bytes, 7U);
        }


        public void SetValue(string valueName, uint value)
        {
            byte[] binaryData = new byte[4]
            {
                0,
                0,
                0,
                (byte) ((value >> 24) & byte.MaxValue)
            };
            binaryData[2] = (byte)((value >> 16) & byte.MaxValue);
            binaryData[1] = (byte)((value >> 8) & byte.MaxValue);
            binaryData[0] = (byte)(value & byte.MaxValue);
            SetValue(valueName, binaryData, 4U);
        }

        public OfflineRegistryHandle CreateSubKey(string subKey)
        {
            return new OfflineRegistryHandle(Win32Exports.OfflineRegistryCreateKey((nint)this, subKey), subKey,
                Path);
        }

        public override string ToString()
        {
            return Path;
        }

        private static RegistryValueKind GetValueKind(uint valueType)
        {
            RegistryValueKind registryValueKind = RegistryValueKind.Unknown;
            switch (valueType)
            {
                case 0:
                    registryValueKind = RegistryValueKind.None;
                    break;
                case 1:
                    registryValueKind = RegistryValueKind.String;
                    break;
                case 2:
                    registryValueKind = RegistryValueKind.ExpandString;
                    break;
                case 3:
                    registryValueKind = RegistryValueKind.Binary;
                    break;
                case 4:
                    registryValueKind = RegistryValueKind.DWord;
                    break;
                case 5:
                    registryValueKind = RegistryValueKind.DWord;
                    break;
                case 6:
                    registryValueKind = RegistryValueKind.String;
                    break;
                case 7:
                    registryValueKind = RegistryValueKind.MultiString;
                    break;
                case 8:
                    registryValueKind = RegistryValueKind.MultiString;
                    break;
                case 9:
                    registryValueKind = RegistryValueKind.String;
                    break;
                case 10:
                    registryValueKind = RegistryValueKind.Binary;
                    break;
                case 11:
                    registryValueKind = RegistryValueKind.QWord;
                    break;
            }

            return registryValueKind;
        }
    }
}