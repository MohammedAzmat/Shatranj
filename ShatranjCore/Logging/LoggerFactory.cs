using System.Collections.Generic;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// Factory for creating properly configured logger instances
    /// Provides pre-configured loggers for different environments
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates default logger with rolling file, error trace, and optional console logging
        /// </summary>
        /// <param name="includeConsole">Whether to include console output (useful for development)</param>
        /// <returns>Composite logger with all configured loggers</returns>
        public static ILogger CreateDefaultLogger(bool includeConsole = true)
        {
            var loggers = new List<ILogger>();

            // Rolling file logger for general logs (10MB max, 2 files)
            loggers.Add(new RollingFileLogger(
                maxFileSizeMB: 10,
                maxFiles: 2
            ));

            // Error trace logger for dedicated error tracking
            loggers.Add(new ErrorTraceLogger());

            // Console logger (optional, useful for development)
            if (includeConsole)
            {
                loggers.Add(new ConsoleLogger(includeTimestamp: false));
            }

            return new CompositeLogger(loggers.ToArray());
        }

        /// <summary>
        /// Creates production logger without console output
        /// </summary>
        /// <returns>Logger configured for production use</returns>
        public static ILogger CreateProductionLogger()
        {
            return CreateDefaultLogger(includeConsole: false);
        }

        /// <summary>
        /// Creates development logger with console output
        /// </summary>
        /// <returns>Logger configured for development use</returns>
        public static ILogger CreateDevelopmentLogger()
        {
            return CreateDefaultLogger(includeConsole: true);
        }

        /// <summary>
        /// Creates test logger with only console output
        /// </summary>
        /// <returns>Logger configured for unit tests</returns>
        public static ILogger CreateTestLogger()
        {
            // For unit tests, only console logging
            return new ConsoleLogger(includeTimestamp: true);
        }

        /// <summary>
        /// Creates a custom logger with specific components
        /// </summary>
        /// <param name="includeRollingFile">Include rolling file logger</param>
        /// <param name="includeErrorTrace">Include error trace logger</param>
        /// <param name="includeConsole">Include console logger</param>
        /// <returns>Custom configured logger</returns>
        public static ILogger CreateCustomLogger(
            bool includeRollingFile = true,
            bool includeErrorTrace = true,
            bool includeConsole = true)
        {
            var loggers = new List<ILogger>();

            if (includeRollingFile)
            {
                loggers.Add(new RollingFileLogger(maxFileSizeMB: 10, maxFiles: 2));
            }

            if (includeErrorTrace)
            {
                loggers.Add(new ErrorTraceLogger());
            }

            if (includeConsole)
            {
                loggers.Add(new ConsoleLogger(includeTimestamp: false));
            }

            // If no loggers selected, default to console
            if (loggers.Count == 0)
            {
                loggers.Add(new ConsoleLogger(includeTimestamp: true));
            }

            return new CompositeLogger(loggers.ToArray());
        }
    }
}
