using System;

namespace ShatranjCore.Abstractions
{
    /// <summary>
    /// Log level enumeration
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Logger interface for game logging
    /// </summary>
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(string message, Exception ex);
        void Log(LogLevel level, string message);
    }
}
