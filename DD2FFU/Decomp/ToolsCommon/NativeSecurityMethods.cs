// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.NativeSecurityMethods
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class NativeSecurityMethods
    {
        
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(
            [In] byte[] pBinarySecurityDescriptor, int RequestedStringSDRevision,
            SecurityInformationFlags SecurityInformation, out IntPtr StringSecurityDescriptor,
            out int StringSecurityDescriptorLen);

        
        [DllImport("AdvAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetFileSecurity(string lpFileName, SecurityInformationFlags RequestedInformation,
            IntPtr pSecurityDescriptor, int nLength, ref int lpnLengthNeeded);

        [DllImport("UpdateDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IU_AdjustProcessPrivilege(string strPrivilegeName, bool fEnabled);
    }
}