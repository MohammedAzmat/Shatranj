using System;
using System.IO;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// File-based logger implementation
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string logFilePath;
        private readonly object lockObject = new object();

        public FileLogger(string logFilePath = null)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                string logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "Logs"
                );

                Directory.CreateDirectory(logDirectory);
                this.logFilePath = Path.Combine(logDirectory, $"shatranj_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            }
            else
            {
                this.logFilePath = logFilePath;
                string directory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        public void Trace(string message) => Log(LogLevel.Trace, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);

        public void Error(string message, Exception ex)
        {
            Log(LogLevel.Error, $"{message}\nException: {ex.GetType().Name}: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }

        public void Critical(string message, Exception ex)
        {
            Log(LogLevel.Critical, $"{message}\nException: {ex.GetType().Name}: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }

        public void Log(LogLevel level, string message)
        {
            lock (lockObject)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string logEntry = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";

                    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // Fallback to console if file logging fails
                    Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
                    Console.Error.WriteLine($"Original log message: [{level}] {message}");
                }
            }
        }
    }
}
