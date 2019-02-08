// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.IDeploymentLogger
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using System;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public interface IDeploymentLogger
    {
        void Log(LoggingLevel level, string format, params object[] list);

        void LogException(Exception exp);

        void LogException(Exception exp, LoggingLevel level);

        void LogDebug(string format, params object[] list);

        void LogInfo(string format, params object[] list);

        void LogWarning(string format, params object[] list);

        void LogError(string format, params object[] list);
    }
}