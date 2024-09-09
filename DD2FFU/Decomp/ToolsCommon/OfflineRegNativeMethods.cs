﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.OffRegNativeMethods
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    internal static class OffRegNativeMethods
    {
        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORCreateHive(ref nint handle);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int OROpenHive([MarshalAs(UnmanagedType.LPWStr)] string Path, ref nint handle);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORCloseHive(nint handle);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORSaveHive(nint handle, [MarshalAs(UnmanagedType.LPWStr)] string path, int osMajor,
            int osMinor);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int OROpenKey(nint handle, [MarshalAs(UnmanagedType.LPWStr)] string subKeyName,
            ref nint subkeyHandle);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORCreateKey(nint handle, string subKeyName, string className, uint dwOptions,
            byte[] secbuf, ref nint keyHandle, ref uint dwDisposition);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORCloseKey(nint handle);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int OREnumKey(nint handle, uint dwIndex, StringBuilder name, ref uint count,
            StringBuilder classname, ref uint classnamecount, ref nint filetimeptr);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int ORQueryInfoKey(nint handle, StringBuilder classname, ref uint lpcClass,
            out uint lpcSubKeys, out uint lpcMaxSubKeyLen, out uint lpcMaxClassLen, out uint lpcValues,
            out uint lpcMaxValueNameLen, out uint lpcMaxValueLen, out uint lpcbSecurityDescriptor, nint filetimeptr);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORGetValue(nint Handle, string lpSubKey, string lpValue, out uint pdwType,
            byte[] pvData, ref uint pcbData);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORSetValue(nint Handle, string lpValueName, uint dwType, byte[] pvData, uint cbData);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORDeleteValue(nint Handle, string lpValueName);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORDeleteKey(nint Handle, string lpKeyName);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORGetVirtualFlags(nint Handle, ref int pbFlags);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int OREnumValue(nint Handle, uint index, StringBuilder lpValueName,
            ref uint lpcValueName, out uint lpType, nint pvData, nint lpcbData);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORGetKeySecurity(nint handle, SecurityInformationFlags secinfo, byte[] lpSecBuf,
            ref uint size);

        [DllImport("Offreg.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int ORSetKeySecurity(nint handle, SecurityInformationFlags secinfo, byte[] lpSecBuf);
    }
}