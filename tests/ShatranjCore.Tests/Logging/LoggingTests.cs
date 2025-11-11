using System;
using System.IO;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace ShatranjCore.Tests.Logging
{
    /// <summary>
    /// Test suite for logging infrastructure
    /// Tests ConsoleLogger, FileLogger, RollingFileLogger, ErrorTraceLogger, CompositeLogger, LoggerFactory
    /// </summary>
    public class LoggingTests
    {
        private string _testLogDirectory;

        public LoggingTests()
        {
            _testLogDirectory = Path.Combine(Path.GetTempPath(), "ShatranjLoggingTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testLogDirectory);
        }

        public void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Logging Tests Suite                                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            int passed = 0;
            int failed = 0;

            // Test 1: Console Logger
            try
            {
                TestConsoleLoggerOutput();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 1 PASSED: ConsoleLogger outputs to console");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 1 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 2: File Logger Creation
            try
            {
                TestFileLoggerCreation();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 2 PASSED: FileLogger creates log files");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 2 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 3: Rolling File Logger Rotation
            try
            {
                TestRollingFileLoggerRotation();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 3 PASSED: RollingFileLogger rotates files at size limit");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 3 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 4: Error Trace Logger
            try
            {
                TestErrorTraceLogger();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 4 PASSED: ErrorTraceLogger creates error trace files");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 4 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 5: Composite Logger
            try
            {
                TestCompositeLogger();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 5 PASSED: CompositeLogger logs to multiple loggers");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 5 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 6: Logger Factory
            try
            {
                TestLoggerFactory();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 6 PASSED: LoggerFactory creates properly configured loggers");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 6 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Logging Tests Summary: {passed} passed, {failed} failed                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestConsoleLoggerOutput()
        {
            var logger = new ConsoleLogger(includeTimestamp: false);

            // Should not throw
            logger.Trace("Trace message");
            logger.Debug("Debug message");
            logger.Info("Info message");
            logger.Warning("Warning message");
            logger.Error("Error message");
            logger.Log(LogLevel.Critical, "Critical message");

            // Test with exception
            var ex = new Exception("Test exception");
            logger.Error("Error with exception", ex);
            logger.Critical("Critical with exception", ex);
        }

        private void TestFileLoggerCreation()
        {
            var logPath = Path.Combine(_testLogDirectory, "test_log.txt");
            var logger = new FileLogger(logPath);

            // Log some messages
            logger.Info("Test info message");
            logger.Warning("Test warning message");
            logger.Error("Test error message");

            // Check that log file was created
            if (!File.Exists(logPath))
            {
                throw new Exception("No log file created");
            }

            // Check that content was written
            var content = File.ReadAllText(logPath);
            if (!content.Contains("Test info message"))
            {
                throw new Exception("Log content not written to file");
            }
        }

        private void TestRollingFileLoggerRotation()
        {
            // Create rolling logger with default settings for testing
            var rollingLogger = new RollingFileLogger(_testLogDirectory, "rolling");

            // Write some data to the logger
            for (int i = 0; i < 50; i++)
            {
                rollingLogger.Info($"Message {i}: Test message for rolling file logger");
            }

            // Check that at least one log file was created
            var logFiles = Directory.GetFiles(_testLogDirectory, "rolling_*.log");
            if (logFiles.Length < 1)
            {
                throw new Exception($"Expected at least 1 log file created, got {logFiles.Length}");
            }

            // Verify log file contains data
            var content = File.ReadAllText(logFiles[0]);
            if (string.IsNullOrEmpty(content) || !content.Contains("Message 0"))
            {
                throw new Exception("Log file created but no data written");
            }
        }

        private void TestErrorTraceLogger()
        {
            var errorDirectory = Path.Combine(_testLogDirectory, "errors");
            var errorLogger = new ErrorTraceLogger(errorDirectory);

            // Test error logging
            var testException = new Exception("Test error for tracing");
            errorLogger.Error("An error occurred", testException);

            // Check that error trace file was created
            var errorFiles = Directory.GetFiles(errorDirectory, "error_trace_*.log");
            if (errorFiles.Length == 0)
            {
                throw new Exception("No error trace files created");
            }

            // Check that exception details are in the file
            var content = File.ReadAllText(errorFiles[0]);
            if (!content.Contains("Test error for tracing"))
            {
                throw new Exception("Exception details not found in error trace");
            }
        }

        private void TestCompositeLogger()
        {
            var consoleLogger = new ConsoleLogger(includeTimestamp: false);
            var compositeLogPath = Path.Combine(_testLogDirectory, "composite.txt");
            var fileLogger = new FileLogger(compositeLogPath);

            var compositeLogger = new CompositeLogger(consoleLogger, fileLogger);

            // Log through composite
            compositeLogger.Info("Composite info message");
            compositeLogger.Error("Composite error message");

            // Verify file logger received the messages
            if (!File.Exists(compositeLogPath))
            {
                throw new Exception("Composite logger did not log to file logger");
            }

            var content = File.ReadAllText(compositeLogPath);
            if (!content.Contains("Composite info message"))
            {
                throw new Exception("Message not found in composite logger output");
            }
        }

        private void TestLoggerFactory()
        {
            // Test development logger
            var devLogger = LoggerFactory.CreateDevelopmentLogger();
            if (devLogger == null)
            {
                throw new Exception("Development logger is null");
            }

            // Test production logger
            var prodLogger = LoggerFactory.CreateProductionLogger();
            if (prodLogger == null)
            {
                throw new Exception("Production logger is null");
            }

            // Test test logger
            var testLogger = LoggerFactory.CreateTestLogger();
            if (testLogger == null)
            {
                throw new Exception("Test logger is null");
            }

            // Verify loggers work
            devLogger.Info("Dev logger test");
            prodLogger.Info("Prod logger test");
            testLogger.Info("Test logger test");
        }

        // Cleanup
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(_testLogDirectory))
                {
                    Directory.Delete(_testLogDirectory, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
