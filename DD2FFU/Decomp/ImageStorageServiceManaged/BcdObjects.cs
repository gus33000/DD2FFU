// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdObjects
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class BcdObjects
    {
        public static readonly Guid BootManager = new Guid("{9dea862c-5cdd-4e70-acc1-f32b344d4795}");
        public static readonly Guid WindowsLoader = new Guid("{7619dcc9-fafe-11d9-b411-000476eba25f}");
        public static readonly Guid FullFlashUpdateApp = new Guid("{0ff5f24a-3785-4aeb-b8fe-4226215b88c4}");
        public static readonly Guid MobileStartupApp = new Guid("{01de5a27-8705-40db-bad6-96fa5187d4a6}");
        public static readonly Guid UpdateOSWim = new Guid("{311b88b5-9b30-491d-bad9-167ca3e2d417}");
        public static readonly Guid BootManagerSettingsGroup = new Guid("{6efb52bf-1766-41db-a6b3-0ee5eff72bd7}");
        public static readonly Guid FirmwareBootMgr = new Guid("{a5a30fa2-3d06-4e9f-b5f4-a01df9d1fcba}");
        public static readonly Guid WindowsMemoryTester = new Guid("{b2721d73-1db4-4c62-bf78-c548a880142d}");
        public static readonly Guid WindowsResumeApp = new Guid("{147aa509-0358-4473-b83b-d950dda00615}");
        public static readonly Guid WindowsResumeTargetTemplateEfi = new Guid("{0c334284-9a41-4de1-99b3-a7e87e8ff07e}");

        public static readonly Guid WindowsResumeTargetTemplatePcat =
            new Guid("{98b02a23-0674-4ce7-bdad-e0a15a8ff97b}");

        public static readonly Guid WindowsLegacyNtLdr = new Guid("{466f5a88-0af2-4f76-9038-095b170dc21c}");
        public static readonly Guid WindowsSetupPcat = new Guid("{cbd971bf-b7b8-4885-951a-fa03044f5d71}");
        public static readonly Guid WindowsSetupEfi = new Guid("{7254a080-1510-4e85-ac0f-e7fb3d444736}");
        public static readonly Guid WindowsOsTargetTemplatePcat = new Guid("{a1943bbc-ea85-487c-97c7-c9ede908a38a}");
        public static readonly Guid WindowsOsTargetTemplateEfi = new Guid("{b012b84d-c47c-4ed5-b722-c0c42163e569}");
        public static readonly Guid WindowsSetupRamdiskOptions = new Guid("{ae5534e0-a924-466c-b836-758539a3ee3a}");
        public static readonly Guid CurrentBootEntry = new Guid("{fa926493-6f1c-4193-a414-58f0b2456d1e}");
        public static readonly Guid DefaultBootEntry = new Guid("{1cae1eb7-a0df-4d4d-9851-4860e34ef535}");
        public static readonly Guid BadMemoryGroup = new Guid("{5189b25c-5558-4bf2-bca4-289b11bd29e2}");
        public static readonly Guid DebuggerSettingsGroup = new Guid("{4636856e-540f-4170-a130-a84776f4c654}");
        public static readonly Guid EmsSettingsGroup = new Guid("{0ce4991b-e6b3-4b16-b23c-5e0d9250e5d9}");
        public static readonly Guid GlobalSettingsGroup = new Guid("{7ea2e1ac-2e61-4728-aaa3-896d9d0a9f0e}");
        public static readonly Guid ResumeLoaderSettingsGroup = new Guid("{1afa9c49-16ab-4a5c-901b-212802da9460}");
        public static readonly Guid HypervisorSettingsGroup = new Guid("{7ff607e0-4395-11db-b0de-0800200c9a66}");
        public static readonly Guid ResetPhoneApp = new Guid("{BD8951C4-EABD-4c6f-AAFB-4DDB4EB0469B}");
        public static readonly Guid PrebootCrashDumpApp = new Guid("{012cdeb8-68fe-4fb9-be98-dbf20d98c261}");
        public static readonly Guid MMOSLoader = new Guid("{874EF8BB-D20F-4364-B545-A36E88EC40B0}");
        public static readonly Guid MMOSWim = new Guid("{A5935FF2-32BA-4617-BF36-5AC314B3F9BF}");
        public static readonly Guid DeveloperMenuApp = new Guid("{0D1B5E40-42F1-41e7-A690-8DD3CE23CC11}");
        public static readonly Guid RelockApp = new Guid("{F8E167D7-BEDF-41FF-A32E-27043A83CC89}");
        public static readonly Guid HoloLensDisplayApp = new Guid("{17B80A47-C57B-460F-AF39-9BD2D4080A26}");
        public static readonly Guid PhoneOsBoot = new Guid("{00000000-0000-0000-0000-000000000001}");

        public static readonly Dictionary<Guid, BootObjectInfo> BootObjectList = new Dictionary<Guid, BootObjectInfo>
        {
            {
                BootManager,
                new BootObjectInfo("Windows Boot Manager", 269484034U)
            },
            {
                WindowsLoader,
                new BootObjectInfo("Windows Loader", 270532611U)
            },
            {
                FirmwareBootMgr,
                new BootObjectInfo("Firmware BootMgr", 270532611U)
            },
            {
                UpdateOSWim,
                new BootObjectInfo("Windows Phone Update OS", 270532611U)
            },
            {
                MMOSWim,
                new BootObjectInfo("MMOS", 270532611U)
            },
            {
                MobileStartupApp,
                new BootObjectInfo("Mobile Startup App", 270532618U)
            },
            {
                FullFlashUpdateApp,
                new BootObjectInfo("Full Flash Update", 270532618U)
            },
            {
                ResetPhoneApp,
                new BootObjectInfo("Reset My Phone App", 270532618U)
            },
            {
                PrebootCrashDumpApp,
                new BootObjectInfo("Preboot Crash Dump Application", 270532618U)
            },
            {
                MMOSLoader,
                new BootObjectInfo("MMOS Launcher App", 270532618U)
            },
            {
                DeveloperMenuApp,
                new BootObjectInfo("Developer Menu App", 270532618U)
            },
            {
                RelockApp,
                new BootObjectInfo("Relock App", 270532618U)
            },
            {
                HoloLensDisplayApp,
                new BootObjectInfo("HoloLens Display Initialization App", 270532618U)
            },
            {
                WindowsMemoryTester,
                new BootObjectInfo("Windows Memory Tester", 270532613U)
            },
            {
                WindowsResumeApp,
                new BootObjectInfo("Windows Resume App", 270532612U)
            },
            {
                DebuggerSettingsGroup,
                new BootObjectInfo("Debugger Settings Group", 537919488U)
            },
            {
                EmsSettingsGroup,
                new BootObjectInfo("EMS Settings Group", 537919488U)
            },
            {
                GlobalSettingsGroup,
                new BootObjectInfo("Global Settings Group", 537919488U)
            },
            {
                HypervisorSettingsGroup,
                new BootObjectInfo("Hypervisor Settings Group", 538968067U)
            },
            {
                BootManagerSettingsGroup,
                new BootObjectInfo("Boot Loader Settings Group", 538968067U)
            },
            {
                WindowsSetupRamdiskOptions,
                new BootObjectInfo("Ramdisk Options", 805306372U)
            }
        };

        public static Guid IdFromName(string objectName)
        {
            var guid = Guid.Empty;
            foreach (var key in BootObjectList.Keys)
                if (string.Compare(objectName, BootObjectList[key].Name, true, CultureInfo.InvariantCulture) == 0)
                {
                    guid = key;
                    break;
                }

            if (guid == Guid.Empty)
                throw new ImageStorageException(string.Format("{0}: '{1}' doesn't have an associated Id.",
                    MethodBase.GetCurrentMethod().Name, objectName));
            return guid;
        }

        
        public static uint ObjectTypeFromName(string objectName)
        {
            uint num = 0;
            foreach (var key in BootObjectList.Keys)
                if (string.Compare(objectName, BootObjectList[key].Name, true, CultureInfo.InvariantCulture) == 0)
                {
                    num = BootObjectList[key].Type;
                    break;
                }

            if (num == 0U)
                throw new ImageStorageException(string.Format("{0}: '{1}' doesn't have a known type.",
                    MethodBase.GetCurrentMethod().Name, objectName));
            return num;
        }

        public struct BootObjectInfo
        {
            public string Name;
             public uint Type;

            
            public BootObjectInfo(string name, uint type)
            {
                Name = name;
                Type = type;
            }
        }
    }
}