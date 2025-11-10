using System;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// Console-based logger implementation
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly bool includeTimestamp;

        public ConsoleLogger(bool includeTimestamp = true)
        {
            this.includeTimestamp = includeTimestamp;
        }

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);

        public void Error(string message, Exception ex)
        {
            Log(LogLevel.Error, $"{message}\nException: {ex.GetType().Name}: {ex.Message}");
        }

        public void Log(LogLevel level, string message)
        {
            string timestamp = includeTimestamp ? $"[{DateTime.Now:HH:mm:ss}] " : "";
            string prefix = $"{timestamp}[{level.ToString().ToUpper()}]";

            ConsoleColor originalColor = Console.ForegroundColor;

            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine($"{prefix} {message}");
            Console.ForegroundColor = originalColor;
        }
    }
}
