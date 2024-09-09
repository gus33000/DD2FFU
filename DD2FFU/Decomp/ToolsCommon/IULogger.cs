// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.IULogger
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\toolscommon.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class IULogger : IDeploymentLogger
    {
        private readonly Dictionary<LoggingLevel, ConsoleColor> LoggingColors =
            [];

        private readonly Dictionary<LoggingLevel, LogString> LoggingFunctions =
            [];

        private readonly Dictionary<LoggingLevel, string> LoggingMessage = [];
        private LoggingLevel MinLogLevel;

        public IULogger()
        {
            MinLogLevel = LoggingLevel.Debug;
            LoggingMessage.Add(LoggingLevel.Debug, "DEBUG");
            LoggingMessage.Add(LoggingLevel.Info, "INFO");
            LoggingMessage.Add(LoggingLevel.Warning, "WARNING");
            LoggingMessage.Add(LoggingLevel.Error, "ERROR");
            LoggingFunctions.Add(LoggingLevel.Debug, LogToConsole);
            LoggingFunctions.Add(LoggingLevel.Info, LogToConsole);
            LoggingFunctions.Add(LoggingLevel.Warning, LogToError);
            LoggingFunctions.Add(LoggingLevel.Error, LogToError);
            LoggingColors.Add(LoggingLevel.Debug, ConsoleColor.DarkGray);
            LoggingColors.Add(LoggingLevel.Info, ConsoleColor.Gray);
            LoggingColors.Add(LoggingLevel.Warning, ConsoleColor.Yellow);
            LoggingColors.Add(LoggingLevel.Error, ConsoleColor.Red);
        }

        public ConsoleColor OverrideColor
        {
            get; set;
        }

        public bool UseOverrideColor => (uint)OverrideColor > 0U;

        public LogString ErrorLogger
        {
            get => LoggingFunctions[LoggingLevel.Error];
            set => SetLogFunction(LoggingLevel.Error, value);
        }

        public LogString WarningLogger
        {
            get => LoggingFunctions[LoggingLevel.Warning];
            set => SetLogFunction(LoggingLevel.Warning, value);
        }

        public LogString InformationLogger
        {
            get => LoggingFunctions[LoggingLevel.Info];
            set => SetLogFunction(LoggingLevel.Info, value);
        }

        public LogString DebugLogger
        {
            get => LoggingFunctions[LoggingLevel.Debug];
            set => SetLogFunction(LoggingLevel.Debug, value);
        }

        public void Log(LoggingLevel level, string format, params object[] list)
        {
            if (level < MinLogLevel)
            {
                return;
            }

            int foregroundColor = (int)Console.ForegroundColor;
            Console.ForegroundColor = UseOverrideColor ? OverrideColor : LoggingColors[level];
            LoggingFunctions[level](format, list);
            Console.ForegroundColor = (ConsoleColor)foregroundColor;
        }

        public void LogException(Exception exp)
        {
            LogException(exp, LoggingLevel.Error);
        }

        public void LogException(Exception exp, LoggingLevel level)
        {
            StringBuilder stringBuilder = new();
            StackTrace stackTrace1 = new(exp, true);
            if (stackTrace1.FrameCount > 0)
            {
                StackTrace stackTrace2 = stackTrace1;
                StackFrame frame = stackTrace2.GetFrame(stackTrace2.FrameCount - 1);
                if (frame != null)
                {
                    string str = string.Format("{0}({1},{2}):", frame.GetFileName(), frame.GetFileLineNumber(),
                        frame.GetFileColumnNumber());
                    _ = stringBuilder.Append(string.Format("{0}{1}", str, Environment.NewLine));
                }
            }

            _ = stringBuilder.Append(string.Format("{0}: {1}{2}", LoggingMessage[level],
                "0x" + Marshal.GetHRForException(exp).ToString("X"), Environment.NewLine));
            _ = stringBuilder.Append(string.Format("{0}:{1}",
                Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName), Environment.NewLine));
            _ = stringBuilder.Append(string.Format("EXCEPTION: {0}{1}", exp, Environment.NewLine));
            Log(level, stringBuilder.ToString());
        }

        public void LogError(string format, params object[] list)
        {
            Log(LoggingLevel.Error, format, list);
        }

        public void LogWarning(string format, params object[] list)
        {
            Log(LoggingLevel.Warning, format, list);
        }

        public void LogInfo(string format, params object[] list)
        {
            Log(LoggingLevel.Info, format, list);
        }

        public void LogDebug(string format, params object[] list)
        {
            Log(LoggingLevel.Debug, format, list);
        }

        public static void LogToConsole(string format, params object[] list)
        {
            if (list.Length != 0)
            {
                Console.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
            }
            else
            {
                Console.WriteLine(format);
            }
        }

        public static void LogToError(string format, params object[] list)
        {
            if (list.Length != 0)
            {
                Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
            }
            else
            {
                Console.Error.WriteLine(format);
            }
        }

        public static void LogToNull(string format, params object[] list)
        {
        }

        public void SetLoggingLevel(LoggingLevel level)
        {
            MinLogLevel = level;
        }

        public void SetLogFunction(LoggingLevel level, LogString logFunc)
        {
            LoggingFunctions[level] = logFunc ?? LogToNull;
        }

        public void ResetOverrideColor()
        {
            OverrideColor = ConsoleColor.Black;
        }
    }
}