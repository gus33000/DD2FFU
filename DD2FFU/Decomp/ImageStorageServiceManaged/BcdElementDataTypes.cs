// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDataTypes
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Collections.Generic;
using System.Globalization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public static class BcdElementDataTypes
    {
        public static readonly BcdElementDataType DefaultObject =
            new(ElementClass.Application, ElementFormat.Object, 3U);

        public static readonly BcdElementDataType ResumeObject =
            new(ElementClass.Application, ElementFormat.Object, 6U);

        public static readonly BcdElementDataType DisplayOrder =
            new(ElementClass.Application, ElementFormat.ObjectList, 1U);

        public static readonly BcdElementDataType BootSequence =
            new(ElementClass.Application, ElementFormat.ObjectList, 2U);

        public static readonly BcdElementDataType ToolsDisplayOrder =
            new(ElementClass.Application, ElementFormat.ObjectList, 16U);

        public static readonly BcdElementDataType TimeoutValue =
            new(ElementClass.Application, ElementFormat.Integer, 4U);

        public static readonly BcdElementDataType NXPolicy =
            new(ElementClass.Application, ElementFormat.Integer, 32U);

        public static readonly BcdElementDataType PAEPolicy =
            new(ElementClass.Application, ElementFormat.Integer, 33U);

        public static readonly BcdElementDataType DebuggingEnabled =
            new(ElementClass.Application, ElementFormat.Boolean, 6U);

        public static readonly BcdElementDataType DisplayBootMenu =
            new(ElementClass.Application, ElementFormat.Boolean, 32U);

        public static readonly BcdElementDataType WinPEImage =
            new(ElementClass.Application, ElementFormat.Boolean, 34U);

        public static readonly BcdElementDataType RemoveMemory =
            new(ElementClass.Application, ElementFormat.Integer, 49U);

        public static readonly BcdElementDataType KernelDebuggerEnabled =
            new(ElementClass.Application, ElementFormat.Boolean, 160U);

        public static readonly BcdElementDataType KernelEmsEnabled =
            new(ElementClass.Application, ElementFormat.Boolean, 176U);

        public static readonly BcdElementDataType SystemRoot =
            new(ElementClass.Application, ElementFormat.String, 2U);

        public static readonly BcdElementDataType FilePath =
            new(ElementClass.Application, ElementFormat.String, 35U);

        public static readonly BcdElementDataType OsLoaderType =
            new(ElementClass.Application, ElementFormat.Device, 1U);

        public static readonly BcdElementDataType BcdDevice =
            new(ElementClass.Application, ElementFormat.Device, 34U);

        public static readonly BcdElementDataType BootMenuPolicy =
            new(ElementClass.Application, ElementFormat.Integer, 194U);

        public static readonly BcdElementDataType DetectKernelAndHal =
            new(ElementClass.Application, ElementFormat.Boolean, 16U);

        public static readonly BcdElementDataType PersistBootSequence =
            new(ElementClass.Application, ElementFormat.Boolean, 49U);

        public static readonly BcdElementDataType FfuUpdateMode =
            new(ElementClass.Application, ElementFormat.Boolean, 513U);

        public static readonly BcdElementDataType ForceFfu =
            new(ElementClass.Application, ElementFormat.Boolean, 515U);

        public static readonly BcdElementDataType CheckPlatformId =
            new(ElementClass.Application, ElementFormat.Boolean, 516U);

        public static readonly BcdElementDataType DisableCheckPlatformId =
            new(ElementClass.Application, ElementFormat.Boolean, 517U);

        public static readonly BcdElementDataType EnableUFPMode =
            new(ElementClass.Application, ElementFormat.Boolean, 519U);

        public static readonly BcdElementDataType UFPMincryplHashingSupport =
            new(ElementClass.Application, ElementFormat.Boolean, 520U);

        public static readonly BcdElementDataType UFPLogLocation =
            new(ElementClass.Application, ElementFormat.Integer, 521U);

        public static readonly BcdElementDataType CustomActionsList =
            new(ElementClass.Application, ElementFormat.IntegerList, 48U);

        public static readonly BcdElementDataType DebugTransportPath =
            new(ElementClass.Application, ElementFormat.String, 19U);

        public static readonly BcdElementDataType HypervisorDebuggerType =
            new(ElementClass.Application, ElementFormat.Integer, 243U);

        public static readonly BcdElementDataType HypervisorDebuggerPortNumber =
            new(ElementClass.Application, ElementFormat.Integer, 244U);

        public static readonly BcdElementDataType HypervisorDebuggerBaudRate =
            new(ElementClass.Application, ElementFormat.Integer, 245U);

        public static readonly BcdElementDataType MemoryCaptureModeAddress =
            new(ElementClass.Application, ElementFormat.Integer, 1280U);

        public static readonly BcdElementDataType WpDmpSettingsFile =
            new(ElementClass.Application, ElementFormat.String, 1281U);

        public static readonly BcdElementDataType WpDmpLogFile =
            new(ElementClass.Application, ElementFormat.String, 1282U);

        public static readonly BcdElementDataType OemChargingBootThreshold =
            new(ElementClass.Application, ElementFormat.Integer, 1296U);

        public static readonly BcdElementDataType OemChargingModeThreshold =
            new(ElementClass.Application, ElementFormat.Integer, 1297U);

        public static readonly BcdElementDataType OemChargingModeEnabled =
            new(ElementClass.Application, ElementFormat.Boolean, 1298U);

        public static readonly BcdElementDataType BootFlowVariableGlobal =
            new(ElementClass.Application, ElementFormat.Integer, 2730U);

        public static readonly BcdElementDataType ProcessCustomActionsFirst =
            new(ElementClass.Application, ElementFormat.Boolean, 40U);

        public static readonly BcdElementDataType BootStatusPolicy =
            new(ElementClass.Application, ElementFormat.Integer, 224U);

        public static readonly BcdElementDataType AllowUserToResetPhone =
            new(ElementClass.Application, ElementFormat.Boolean, 518U);

        public static readonly BcdElementDataType ManufacturingMode =
            new(ElementClass.Application, ElementFormat.String, 320U);

        public static readonly BcdElementDataType MSEBootDebugPolicy =
            new(ElementClass.Application, ElementFormat.Boolean, 325U);

        public static readonly BcdElementDataType MSEBootOrderClean =
            new(ElementClass.Application, ElementFormat.Boolean, 326U);

        public static readonly BcdElementDataType MSEDeviceId =
            new(ElementClass.Application, ElementFormat.Boolean, 327U);

        public static readonly BcdElementDataType MSEFfuLoader =
            new(ElementClass.Application, ElementFormat.Boolean, 328U);

        public static readonly BcdElementDataType MSEIuLoader =
            new(ElementClass.Application, ElementFormat.Boolean, 329U);

        public static readonly BcdElementDataType MSEMassStorage =
            new(ElementClass.Application, ElementFormat.Boolean, 330U);

        public static readonly BcdElementDataType MSERpmbProvisioning =
            new(ElementClass.Application, ElementFormat.Boolean, 331U);

        public static readonly BcdElementDataType MSESecureBootPolicy =
            new(ElementClass.Application, ElementFormat.Boolean, 332U);

        public static readonly BcdElementDataType MSEStartCharge =
            new(ElementClass.Application, ElementFormat.Boolean, 333U);

        public static readonly BcdElementDataType MSEResetTPM =
            new(ElementClass.Application, ElementFormat.Boolean, 334U);

        public static readonly BcdElementDataType QuietBootEnable =
            new(ElementClass.Application, ElementFormat.Boolean, 65U);

        public static readonly Dictionary<BcdElementDataType, string> ApplicationTypes =
            new()
            {
                {
                    DefaultObject,
                    "Default Object"
                },
                {
                    ResumeObject,
                    "Resume Object"
                },
                {
                    DisplayOrder,
                    "Display Order"
                },
                {
                    BootSequence,
                    "Boot Sequence"
                },
                {
                    ToolsDisplayOrder,
                    "Tools Display Order"
                },
                {
                    TimeoutValue,
                    "Timeout Value"
                },
                {
                    NXPolicy,
                    "NX Policy"
                },
                {
                    PAEPolicy,
                    "PAE Policy"
                },
                {
                    DebuggingEnabled,
                    "Debugging Enabled"
                },
                {
                    DisplayBootMenu,
                    "Display Boot Menu"
                },
                {
                    WinPEImage,
                    "WinPE Image"
                },
                {
                    RemoveMemory,
                    "Remove Memory"
                },
                {
                    KernelDebuggerEnabled,
                    "Kernel Debugger Enabled"
                },
                {
                    KernelEmsEnabled,
                    "Kernel EMS Enabled"
                },
                {
                    SystemRoot,
                    "System Root"
                },
                {
                    FilePath,
                    "File Path"
                },
                {
                    OsLoaderType,
                    "OS Loader Type"
                },
                {
                    BcdDevice,
                    "BCD Device"
                },
                {
                    BootMenuPolicy,
                    "Boot Menu Policy"
                },
                {
                    DetectKernelAndHal,
                    "Detect Kernel and HAL"
                },
                {
                    PersistBootSequence,
                    "Persist Boot Sequence"
                },
                {
                    FfuUpdateMode,
                    "FFU Update Mode"
                },
                {
                    ForceFfu,
                    "Force FFU"
                },
                {
                    CheckPlatformId,
                    "Enable Platform ID Check"
                },
                {
                    DisableCheckPlatformId,
                    "Disable Platform ID Check"
                },
                {
                    EnableUFPMode,
                    "Enable UFP Mode"
                },
                {
                    UFPMincryplHashingSupport,
                    "Use Mincrypl for Hashing"
                },
                {
                    UFPLogLocation,
                    "UFP Log Location"
                },
                {
                    CustomActionsList,
                    "Custom Action List"
                },
                {
                    DebugTransportPath,
                    "Debug Transport Path"
                },
                {
                    HypervisorDebuggerType,
                    "Hypervisor Debugger Type"
                },
                {
                    HypervisorDebuggerPortNumber,
                    "Hypervisor Debugger Port Number"
                },
                {
                    HypervisorDebuggerBaudRate,
                    "Hypervisor Debugger Baud Rate"
                },
                {
                    MemoryCaptureModeAddress,
                    "Memory Capture Mode Address"
                },
                {
                    WpDmpSettingsFile,
                    "WpDmp Settings File"
                },
                {
                    WpDmpLogFile,
                    "WpDmp Log File"
                },
                {
                    OemChargingBootThreshold,
                    "OEM Charging Boot Threshold"
                },
                {
                    OemChargingModeThreshold,
                    "OEM Charging Mode Threshold"
                },
                {
                    OemChargingModeEnabled,
                    "OEM Charging Mode Enabled"
                },
                {
                    BootFlowVariableGlobal,
                    "BootFlowAPI Global"
                },
                {
                    ProcessCustomActionsFirst,
                    "Process Custom Actions First"
                },
                {
                    BootStatusPolicy,
                    "Boot Status Policy"
                },
                {
                    AllowUserToResetPhone,
                    "Allow User To Reset Phone"
                },
                {
                    ManufacturingMode,
                    "Manufacturing Mode"
                },
                {
                    MSEBootDebugPolicy,
                    "Enable Boot Debug Policy"
                },
                {
                    MSEBootOrderClean,
                    "Enable Boot Order Clean"
                },
                {
                    MSEDeviceId,
                    "Enable Device Id"
                },
                {
                    MSEFfuLoader,
                    "Enable FFU Loader"
                },
                {
                    MSEIuLoader,
                    "Enable IU Loader"
                },
                {
                    MSEMassStorage,
                    "Enable Mass Storage"
                },
                {
                    MSERpmbProvisioning,
                    "Enable RPMB Provisioning"
                },
                {
                    MSESecureBootPolicy,
                    "Enable Secure Boot Policy"
                },
                {
                    MSEStartCharge,
                    "Enable Start Charge"
                },
                {
                    MSEResetTPM,
                    "Enable Reset TPM"
                },
                {
                    QuietBootEnable,
                    "Quiet Boot Enable"
                }
            };

        public static readonly BcdElementDataType OsLoaderDevice =
            new(ElementClass.Library, ElementFormat.Device, 1U);

        public static readonly BcdElementDataType ApplicationPath =
            new(ElementClass.Library, ElementFormat.String, 2U);

        public static readonly BcdElementDataType Description =
            new(ElementClass.Library, ElementFormat.String, 4U);

        public static readonly BcdElementDataType PreferredLocale =
            new(ElementClass.Library, ElementFormat.String, 5U);

        public static readonly BcdElementDataType AutoRecoveryEnabled =
            new(ElementClass.Library, ElementFormat.Boolean, 9U);

        public static readonly BcdElementDataType DebuggerEnabled =
            new(ElementClass.Library, ElementFormat.Boolean, 16U);

        public static readonly BcdElementDataType EmsEnabled =
            new(ElementClass.Library, ElementFormat.Boolean, 32U);

        public static readonly BcdElementDataType DisplayAdvanceOptions =
            new(ElementClass.Library, ElementFormat.Boolean, 64U);

        public static readonly BcdElementDataType DisplayOptionsEdit =
            new(ElementClass.Library, ElementFormat.Boolean, 65U);

        public static readonly BcdElementDataType DisableIntegrityChecks =
            new(ElementClass.Library, ElementFormat.Boolean, 72U);

        public static readonly BcdElementDataType AllowPreReleaseSignatures =
            new(ElementClass.Library, ElementFormat.Boolean, 73U);

        public static readonly BcdElementDataType AllowFlightSignatures =
            new(ElementClass.Library, ElementFormat.Boolean, 126U);

        public static readonly BcdElementDataType ConsoleExtendedInput =
            new(ElementClass.Library, ElementFormat.Boolean, 80U);

        public static readonly BcdElementDataType Inherit =
            new(ElementClass.Library, ElementFormat.ObjectList, 6U);

        public static readonly BcdElementDataType RecoverySequence =
            new(ElementClass.Library, ElementFormat.ObjectList, 8U);

        public static readonly BcdElementDataType DebuggerType =
            new(ElementClass.Library, ElementFormat.Integer, 17U);

        public static readonly BcdElementDataType DebuggerPortAddress =
            new(ElementClass.Library, ElementFormat.Integer, 18U);

        public static readonly BcdElementDataType PortNumber =
            new(ElementClass.Library, ElementFormat.Integer, 19U);

        public static readonly BcdElementDataType DebuggerUsbTargetName =
            new(ElementClass.Library, ElementFormat.String, 22U);

        public static readonly BcdElementDataType DebuggerBusParameters =
            new(ElementClass.Library, ElementFormat.String, 25U);

        public static readonly BcdElementDataType DebuggerIgnoreUsermodeExceptions =
            new(ElementClass.Library, ElementFormat.Boolean, 23U);

        public static readonly BcdElementDataType DebuggerStartPolicy =
            new(ElementClass.Library, ElementFormat.Integer, 24U);

        public static readonly BcdElementDataType DebuggerNetworkKey =
            new(ElementClass.Library, ElementFormat.String, 29U);

        public static readonly BcdElementDataType DebuggerNetworkHostIp =
            new(ElementClass.Library, ElementFormat.Integer, 26U);

        public static readonly BcdElementDataType DebuggerNetworkPort =
            new(ElementClass.Library, ElementFormat.Integer, 27U);

        public static readonly BcdElementDataType DebuggerNetworkDhcp =
            new(ElementClass.Library, ElementFormat.Boolean, 28U);

        public static readonly BcdElementDataType DebuggerBaudRate =
            new(ElementClass.Library, ElementFormat.Integer, 20U);

        public static readonly BcdElementDataType ForceNoKeyboard =
            new(ElementClass.Library, ElementFormat.Boolean, 114U);

        public static readonly BcdElementDataType BootUxFadeDisable =
            new(ElementClass.Library, ElementFormat.Boolean, 106U);

        public static readonly BcdElementDataType BootUxLogoTransitionEnable =
            new(ElementClass.Library, ElementFormat.Boolean, 122U);

        public static readonly BcdElementDataType BootUxLogoTransitionTime =
            new(ElementClass.Library, ElementFormat.Integer, 121U);

        public static readonly BcdElementDataType BootUxProgressAnimationDisable =
            new(ElementClass.Library, ElementFormat.Boolean, 105U);

        public static readonly BcdElementDataType BootUxTextDisable =
            new(ElementClass.Library, ElementFormat.Boolean, 104U);

        public static readonly BcdElementDataType BootUxErrorScreen =
            new(ElementClass.Library, ElementFormat.Integer, 125U);

        public static readonly BcdElementDataType BsdFilepath =
            new(ElementClass.Library, ElementFormat.String, 68U);

        public static readonly BcdElementDataType BsdPreservePreviousEntries =
            new(ElementClass.Library, ElementFormat.Boolean, 69U);

        public static readonly BcdElementDataType LoadOptions =
            new(ElementClass.Library, ElementFormat.String, 48U);

        public static readonly BcdElementDataType GraphicsModeDisable =
            new(ElementClass.Library, ElementFormat.Boolean, 70U);

        public static readonly Dictionary<BcdElementDataType, string> LibraryTypes =
            new()
            {
                {
                    OsLoaderDevice,
                    "Boot Device"
                },
                {
                    ApplicationPath,
                    "Application Path"
                },
                {
                    Description,
                    nameof(Description)
                },
                {
                    PreferredLocale,
                    "Preferred Locale"
                },
                {
                    AutoRecoveryEnabled,
                    "Auto-recovery Enabled"
                },
                {
                    DebuggerEnabled,
                    "Debugger Enabled"
                },
                {
                    EmsEnabled,
                    "EMS Enabled"
                },
                {
                    DisplayAdvanceOptions,
                    "Display Advanced Options"
                },
                {
                    DisplayOptionsEdit,
                    "Display Option Edit"
                },
                {
                    DisableIntegrityChecks,
                    "Disable Integrity Checks"
                },
                {
                    AllowPreReleaseSignatures,
                    "Allow Pre-release Signatures"
                },
                {
                    AllowFlightSignatures,
                    "Allow Flight Signatures"
                },
                {
                    ConsoleExtendedInput,
                    "Console Extended Input"
                },
                {
                    Inherit,
                    nameof(Inherit)
                },
                {
                    RecoverySequence,
                    "Recovery Sequence"
                },
                {
                    DebuggerType,
                    "Debugger Type"
                },
                {
                    DebuggerPortAddress,
                    "Debugger Port Address"
                },
                {
                    PortNumber,
                    "Port Number"
                },
                {
                    DebuggerUsbTargetName,
                    "Debugger USB Target Name"
                },
                {
                    DebuggerBusParameters,
                    "Debugger Bus Parameters"
                },
                {
                    DebuggerIgnoreUsermodeExceptions,
                    "Debugger Ignore Usermode Exceptions"
                },
                {
                    DebuggerStartPolicy,
                    "Debugger Start Policy"
                },
                {
                    DebuggerNetworkKey,
                    "Debugger Network Key"
                },
                {
                    DebuggerNetworkHostIp,
                    "Debugger Network Host IP"
                },
                {
                    DebuggerNetworkPort,
                    "Debugger Network Port"
                },
                {
                    DebuggerNetworkDhcp,
                    "Debugger Network DHCP"
                },
                {
                    DebuggerBaudRate,
                    "Debugger Baud Rate"
                },
                {
                    ForceNoKeyboard,
                    "Force No Keyboard"
                },
                {
                    BootUxFadeDisable,
                    "Boot UX Fade Disable"
                },
                {
                    BootUxLogoTransitionEnable,
                    "Boot UX Logo Transition Enable"
                },
                {
                    BootUxLogoTransitionTime,
                    "Boot UX Logo Transition Time"
                },
                {
                    BootUxProgressAnimationDisable,
                    "Boot UX Progress Animation Disable"
                },
                {
                    BootUxTextDisable,
                    "Boot UX Text Disable"
                },
                {
                    BootUxErrorScreen,
                    "Boot UX Error Screen"
                },
                {
                    BsdFilepath,
                    "BSD Filepath"
                },
                {
                    BsdPreservePreviousEntries,
                    "BSD Preserve Previous Entries"
                },
                {
                    LoadOptions,
                    "Load Options"
                },
                {
                    GraphicsModeDisable,
                    "Graphics Mode Disable"
                }
            };

        public static readonly BcdElementDataType RamDiskSdiPath =
            new(ElementClass.Device, ElementFormat.String, 4U);

        public static readonly BcdElementDataType RamDiskSdiDevice =
            new(ElementClass.Device, ElementFormat.Device, 3U);

        public static readonly Dictionary<BcdElementDataType, string> DeviceTypes =
            new()
            {
                {
                    RamDiskSdiPath,
                    "Ramdisk SDI Path"
                },
                {
                    RamDiskSdiDevice,
                    "Ramdisk SDI Device"
                }
            };

        public static readonly BcdElementDataType FlashingAction =
            new(ElementClass.Oem, ElementFormat.ObjectList, 1U);

        public static readonly BcdElementDataType ResetMyPhoneAction =
            new(ElementClass.Oem, ElementFormat.ObjectList, 2U);

        public static readonly BcdElementDataType DeveloperMenuAction =
            new(ElementClass.Oem, ElementFormat.ObjectList, 3U);

        public static readonly Dictionary<BcdElementDataType, string> OEMTypes =
            new()
            {
                {
                    FlashingAction,
                    "Flashing Action"
                },
                {
                    ResetMyPhoneAction,
                    "Reset My Phone Action"
                },
                {
                    DeveloperMenuAction,
                    "Developer Menu Action"
                }
            };

        public static BcdElementDataType GetWellKnownDataType(string dataTypeName)
        {
            BcdElementDataType bcdElementDataType = null;
            foreach (BcdElementDataType key in LibraryTypes.Keys)
            {
                if (string.Compare(LibraryTypes[key], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
                {
                    bcdElementDataType = key;
                    break;
                }
            }

            if (bcdElementDataType == null)
            {
                foreach (BcdElementDataType key in ApplicationTypes.Keys)
                {
                    if (string.Compare(ApplicationTypes[key], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        bcdElementDataType = key;
                        break;
                    }
                }
            }

            if (bcdElementDataType == null)
            {
                foreach (BcdElementDataType key in DeviceTypes.Keys)
                {
                    if (string.Compare(DeviceTypes[key], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        bcdElementDataType = key;
                        break;
                    }
                }
            }

            if (bcdElementDataType == null)
            {
                foreach (BcdElementDataType key in OEMTypes.Keys)
                {
                    if (string.Compare(OEMTypes[key], dataTypeName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        bcdElementDataType = key;
                        break;
                    }
                }
            }

            return bcdElementDataType;
        }
    }
}