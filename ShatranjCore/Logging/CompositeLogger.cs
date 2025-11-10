using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Logging
{
    /// <summary>
    /// Composite logger that writes to multiple loggers
    /// </summary>
    public class CompositeLogger : ILogger
    {
        private readonly List<ILogger> loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            this.loggers = new List<ILogger>(loggers);
        }

        public void AddLogger(ILogger logger)
        {
            loggers.Add(logger);
        }

        public void Debug(string message)
        {
            foreach (var logger in loggers)
                logger.Debug(message);
        }

        public void Info(string message)
        {
            foreach (var logger in loggers)
                logger.Info(message);
        }

        public void Warning(string message)
        {
            foreach (var logger in loggers)
                logger.Warning(message);
        }

        public void Error(string message)
        {
            foreach (var logger in loggers)
                logger.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            foreach (var logger in loggers)
                logger.Error(message, ex);
        }

        public void Log(LogLevel level, string message)
        {
            foreach (var logger in loggers)
                logger.Log(level, message);
        }
    }
}
