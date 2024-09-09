// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ApplicationTypeValue
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

namespace Decomp.Microsoft.WindowsPhone.Imaging
{

    public enum ApplicationTypeValue : uint
    {
        FirmwareBootManager = 1,
        WindowsBootManager = 2,
        WindowsBootLoader = 3,
        WindowsResumeApplication = 4,
        MemoryTester = 5,
        LegacyNtLdr = 6,
        LegacySetupLdr = 7,
        BootSector = 8,
        StartupModule = 9,
        GenericApplication = 10, // 0x0000000A
        Reserved = 1048575 // 0x000FFFFF
    }
}