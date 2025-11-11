using System;
using System.IO;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// Dedicated logger for error traces - creates separate files for each error
    /// Provides detailed diagnostic information with system context
    /// </summary>
    public class ErrorTraceLogger : ILogger
    {
        private readonly string _errorDirectory;
        private readonly object _lockObject = new object();

        public ErrorTraceLogger(string errorDirectory = null)
        {
            if (string.IsNullOrEmpty(errorDirectory))
            {
                _errorDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "ErrorTraces"
                );
            }
            else
            {
                _errorDirectory = errorDirectory;
            }

            Directory.CreateDirectory(_errorDirectory);
        }

        // Only Error and Critical levels are logged to error trace
        public void Trace(string message) { }
        public void Debug(string message) { }
        public void Info(string message) { }
        public void Warning(string message) { }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Error(string message, Exception ex)
        {
            string fullMessage = FormatErrorTrace(message, ex);
            WriteErrorTrace(fullMessage);
        }

        public void Critical(string message, Exception ex)
        {
            string fullMessage = FormatErrorTrace(message, ex, isCritical: true);
            WriteErrorTrace(fullMessage);
        }

        public void Log(LogLevel level, string message)
        {
            if (level >= LogLevel.Error)
            {
                WriteErrorTrace($"[{level.ToString().ToUpper()}] {message}");
            }
        }

        private void WriteErrorTrace(string content)
        {
            lock (_lockObject)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    string fileName = $"error_trace_{timestamp}.log";
                    string filePath = Path.Combine(_errorDirectory, fileName);

                    string header =
                        $"=== ERROR TRACE ===\n" +
                        $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n" +
                        $"Machine: {Environment.MachineName}\n" +
                        $"User: {Environment.UserName}\n" +
                        $"OS: {Environment.OSVersion}\n" +
                        $".NET Version: {Environment.Version}\n" +
                        $"Working Directory: {Environment.CurrentDirectory}\n" +
                        $"==================\n\n";

                    File.WriteAllText(filePath, header + content);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to write error trace: {ex.Message}");
                }
            }
        }

        private string FormatErrorTrace(string message, Exception ex, bool isCritical = false)
        {
            string severity = isCritical ? "CRITICAL ERROR" : "ERROR";

            return $"{severity}: {message}\n\n" +
                   $"Exception Type: {ex.GetType().FullName}\n" +
                   $"Exception Message: {ex.Message}\n\n" +
                   $"Stack Trace:\n{ex.StackTrace}\n\n" +
                   FormatInnerExceptions(ex.InnerException, 1);
        }

        private string FormatInnerExceptions(Exception ex, int level)
        {
            if (ex == null) return "";

            string indent = new string(' ', level * 2);
            string result = $"{indent}Inner Exception (Level {level}):\n" +
                          $"{indent}Type: {ex.GetType().FullName}\n" +
                          $"{indent}Message: {ex.Message}\n" +
                          $"{indent}Stack Trace:\n{ex.StackTrace}\n\n";

            if (ex.InnerException != null)
            {
                result += FormatInnerExceptions(ex.InnerException, level + 1);
            }

            return result;
        }
    }
}
