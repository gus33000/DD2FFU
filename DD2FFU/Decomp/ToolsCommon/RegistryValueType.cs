// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.RegValueType
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public enum RegistryValueType : uint
    {
        None,
        String,
        ExpandString,
        Binary,
        DWord,
        DWordBigEndian,
        Link,
        MultiString,
        RegResourceList,
        RegFullResourceDescriptor,
        RegResourceRequirementsList,
        QWord
    }
}