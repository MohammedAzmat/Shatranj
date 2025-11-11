using System;
using Xunit;
using Moq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Application;
using ShatranjCore.Interfaces;

namespace ShatranjCore.Tests.ComponentTests
{
    /// <summary>
    /// Unit tests for GameOrchestrator component
    /// Tests top-level game coordination
    /// </summary>
    public class GameOrchestratorTests
    {
        private readonly Mock<IGameLoop> _mockGameLoop;
        private readonly Mock<IChessBoard> _mockBoard;
        private readonly Mock<ILogger> _mockLogger;
        private readonly GameOrchestrator _orchestrator;

        public GameOrchestratorTests()
        {
            _mockGameLoop = new Mock<IGameLoop>();
            _mockBoard = new Mock<IChessBoard>();
            _mockLogger = new Mock<ILogger>();

            _orchestrator = new GameOrchestrator(
                _mockGameLoop.Object,
                _mockBoard.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void Start_CallsGameLoopRun()
        {
            // Act
            _orchestrator.Start();

            // Assert
            _mockGameLoop.Verify(x => x.Run(), Times.Once);
        }

        [Fact]
        public void Start_LogsStartMessage()
        {
            // Act
            _orchestrator.Start();

            // Assert
            _mockLogger.Verify(
                x => x.Info(It.Is<string>(s => s.Contains("starting game"))),
                Times.Once
            );
        }

        [Fact]
        public void Start_LogsEndMessage()
        {
            // Act
            _orchestrator.Start();

            // Assert
            _mockLogger.Verify(
                x => x.Info(It.Is<string>(s => s.Contains("game ended"))),
                Times.Once
            );
        }

        [Fact]
        public void Start_LogsInCorrectOrder()
        {
            // Arrange
            var logSequence = new System.Collections.Generic.List<string>();
            _mockLogger.Setup(x => x.Info(It.IsAny<string>()))
                      .Callback<string>(msg => logSequence.Add(msg));

            // Act
            _orchestrator.Start();

            // Assert
            Assert.Equal(2, logSequence.Count);
            Assert.Contains("starting", logSequence[0].ToLower());
            Assert.Contains("ended", logSequence[1].ToLower());
        }
    }
}
