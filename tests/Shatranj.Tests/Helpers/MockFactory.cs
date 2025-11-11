using System;
using Moq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;

namespace Shatranj.Tests.Helpers
{
    /// <summary>
    /// Factory for creating Moq mocks of common interfaces and dependencies.
    /// </summary>
    public static class MockFactory
    {
        /// <summary>
        /// Creates a mock logger.
        /// </summary>
        public static Mock<ILogger> CreateMockLogger()
        {
            return new Mock<ILogger>();
        }

        /// <summary>
        /// Creates a mock board state.
        /// </summary>
        public static Mock<IBoardState> CreateMockBoardState()
        {
            return new Mock<IBoardState>();
        }

        /// <summary>
        /// Creates a mock renderer.
        /// </summary>
        public static Mock<IRenderer> CreateMockRenderer()
        {
            return new Mock<IRenderer>();
        }

        /// <summary>
        /// Creates a mock move history.
        /// </summary>
        public static Mock<IMoveHistory> CreateMockMoveHistory()
        {
            return new Mock<IMoveHistory>();
        }

        /// <summary>
        /// Creates a mock game state manager.
        /// </summary>
        public static Mock<IGameStateManager> CreateMockGameStateManager()
        {
            return new Mock<IGameStateManager>();
        }

        /// <summary>
        /// Creates a mock check detector.
        /// </summary>
        public static Mock<ICheckDetector> CreateMockCheckDetector()
        {
            return new Mock<ICheckDetector>();
        }

        /// <summary>
        /// Creates a mock en passant tracker.
        /// </summary>
        public static Mock<IEnPassantTracker> CreateMockEnPassantTracker()
        {
            return new Mock<IEnPassantTracker>();
        }

        /// <summary>
        /// Creates a mock AI.
        /// </summary>
        public static Mock<IChessAI> CreateMockAI()
        {
            return new Mock<IChessAI>();
        }

        /// <summary>
        /// Creates a logger that doesn't do anything (useful for testing when you want to suppress logging).
        /// </summary>
        public static ILogger CreateNullLogger()
        {
            return new NullLogger();
        }

        /// <summary>
        /// Creates a real logger that logs to a string builder (for assertion testing).
        /// </summary>
        public static NullLogger CreateStringBuilderLogger()
        {
            return new NullLogger();
        }
    }

    /// <summary>
    /// A no-op logger for testing.
    /// </summary>
    public class NullLogger : ILogger
    {
        private readonly System.Text.StringBuilder _messages = new();

        public string GetMessages() => _messages.ToString();
        public void Clear() => _messages.Clear();

        public void Trace(string message) => _messages.AppendLine($"[TRACE] {message}");
        public void Debug(string message) => _messages.AppendLine($"[DEBUG] {message}");
        public void Info(string message) => _messages.AppendLine($"[INFO] {message}");
        public void Warning(string message) => _messages.AppendLine($"[WARNING] {message}");
        public void Error(string message) => _messages.AppendLine($"[ERROR] {message}");
        public void Error(string message, Exception ex)
        {
            _messages.AppendLine($"[ERROR] {message}");
            if (ex != null)
                _messages.AppendLine($"  Exception: {ex.Message}");
        }
        public void Critical(string message, Exception ex)
        {
            _messages.AppendLine($"[CRITICAL] {message}");
            if (ex != null)
                _messages.AppendLine($"  Exception: {ex.Message}");
        }
        public void Log(LogLevel level, string message) => _messages.AppendLine($"[{level}] {message}");
    }
}
