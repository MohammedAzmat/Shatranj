using System;

namespace ShatranjCore.Abstractions
{
    /// <summary>
    /// Log level enumeration
    /// </summary>
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }

    /// <summary>
    /// Logger interface for game logging
    /// </summary>
    public interface ILogger
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(string message, Exception ex);
        void Critical(string message, Exception ex);
        void Log(LogLevel level, string message);
    }
}
