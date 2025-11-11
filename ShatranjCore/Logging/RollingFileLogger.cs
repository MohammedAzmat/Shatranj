using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// File logger with automatic rotation when size exceeds limit
    /// Max file size: 10MB, Max files: 2
    /// When both files are full, oldest is deleted to make way for new one
    /// </summary>
    public class RollingFileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly string _logFilePrefix;
        private readonly long _maxFileSizeBytes;
        private readonly int _maxFiles;
        private readonly object _lockObject = new object();

        private string _currentLogFile;
        private long _currentFileSize;

        public RollingFileLogger(
            string logDirectory = null,
            string logFilePrefix = "shatranj",
            long maxFileSizeMB = 10,
            int maxFiles = 2)
        {
            if (string.IsNullOrEmpty(logDirectory))
            {
                _logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "Logs"
                );
            }
            else
            {
                _logDirectory = logDirectory;
            }

            _logFilePrefix = logFilePrefix;
            _maxFileSizeBytes = maxFileSizeMB * 1024 * 1024; // Convert MB to bytes
            _maxFiles = maxFiles;

            Directory.CreateDirectory(_logDirectory);
            InitializeCurrentLogFile();
        }

        private void InitializeCurrentLogFile()
        {
            // Find the most recent log file or create new one
            var existingLogs = GetLogFiles();

            if (existingLogs.Any())
            {
                var latestLog = existingLogs.OrderByDescending(f => f.CreatedTime).First();

                if (new FileInfo(latestLog.FilePath).Length < _maxFileSizeBytes)
                {
                    _currentLogFile = latestLog.FilePath;
                    _currentFileSize = new FileInfo(_currentLogFile).Length;
                    return;
                }
            }

            // Create new log file
            CreateNewLogFile();
        }

        private void CreateNewLogFile()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _currentLogFile = Path.Combine(_logDirectory, $"{_logFilePrefix}_{timestamp}.log");
            _currentFileSize = 0;

            // Write header
            string header =
                $"=== Shatranj Log File ===\n" +
                $"Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                $"Max Size: {_maxFileSizeBytes / 1024 / 1024} MB\n" +
                $"========================================\n\n";

            File.WriteAllText(_currentLogFile, header);
            _currentFileSize = new FileInfo(_currentLogFile).Length;
        }

        public void Trace(string message) => Log(LogLevel.Trace, message);
        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);

        public void Error(string message, Exception ex)
        {
            string fullMessage = FormatExceptionMessage(message, ex);
            Log(LogLevel.Error, fullMessage);
        }

        public void Critical(string message, Exception ex)
        {
            string fullMessage = FormatExceptionMessage(message, ex);
            Log(LogLevel.Critical, fullMessage);
        }

        public void Log(LogLevel level, string message)
        {
            lock (_lockObject)
            {
                try
                {
                    // Check if rotation needed
                    if (_currentFileSize >= _maxFileSizeBytes)
                    {
                        RotateLogs();
                    }

                    // Format log entry
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string logEntry = $"[{timestamp}] [{level.ToString().ToUpper().PadRight(8)}] {message}";
                    string fullEntry = logEntry + Environment.NewLine;

                    // Write to file
                    File.AppendAllText(_currentLogFile, fullEntry);
                    _currentFileSize += System.Text.Encoding.UTF8.GetByteCount(fullEntry);
                }
                catch (Exception ex)
                {
                    // Fallback to console
                    Console.Error.WriteLine($"Failed to write to log: {ex.Message}");
                    Console.Error.WriteLine($"Original message: [{level}] {message}");
                }
            }
        }

        private void RotateLogs()
        {
            try
            {
                // Get all log files
                var logFiles = GetLogFiles();

                // If we have max files and current is full, delete oldest
                if (logFiles.Count >= _maxFiles)
                {
                    var filesToDelete = logFiles
                        .OrderBy(f => f.CreatedTime)
                        .Take(logFiles.Count - _maxFiles + 1)
                        .ToList();

                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            File.Delete(file.FilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Failed to delete old log: {ex.Message}");
                        }
                    }
                }

                // Create new log file
                CreateNewLogFile();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Log rotation failed: {ex.Message}");
            }
        }

        public void CleanupOldLogs()
        {
            var logFiles = GetLogFiles();

            // Keep only the most recent maxFiles
            var filesToDelete = logFiles
                .OrderByDescending(f => f.CreatedTime)
                .Skip(_maxFiles)
                .ToList();

            foreach (var file in filesToDelete)
            {
                try
                {
                    File.Delete(file.FilePath);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to delete log file: {ex.Message}");
                }
            }
        }

        public long GetCurrentLogSize() => _currentFileSize;

        public List<string> GetActiveLogFiles()
        {
            return GetLogFiles().Select(f => f.FilePath).ToList();
        }

        private List<LogFileInfo> GetLogFiles()
        {
            if (!Directory.Exists(_logDirectory))
                return new List<LogFileInfo>();

            var files = Directory.GetFiles(_logDirectory, $"{_logFilePrefix}_*.log");

            return files.Select(f => new LogFileInfo
            {
                FilePath = f,
                CreatedTime = File.GetCreationTime(f),
                Size = new FileInfo(f).Length
            }).ToList();
        }

        private string FormatExceptionMessage(string message, Exception ex)
        {
            return $"{message}\n" +
                   $"Exception: {ex.GetType().FullName}\n" +
                   $"Message: {ex.Message}\n" +
                   $"Stack Trace:\n{ex.StackTrace}\n" +
                   (ex.InnerException != null
                       ? $"Inner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}\n"
                       : "");
        }

        private class LogFileInfo
        {
            public string FilePath { get; set; }
            public DateTime CreatedTime { get; set; }
            public long Size { get; set; }
        }
    }
}
