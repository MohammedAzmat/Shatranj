using System;
using Xunit;
using Moq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Application;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;

namespace ShatranjCore.Tests.ComponentTests
{
    /// <summary>
    /// Unit tests for AIHandler component
    /// Tests AI move selection and coordination
    /// </summary>
    public class AIHandlerTests
    {
        private readonly Mock<IRenderer> _mockRenderer;
        private readonly Mock<IEnPassantTracker> _mockEnPassantTracker;
        private readonly Mock<ICheckDetector> _mockCheckDetector;
        private readonly Mock<IGameRecorder> _mockRecorder;
        private readonly Mock<ILogger> _mockLogger;
        private readonly AIHandler _aiHandler;

        public AIHandlerTests()
        {
            _mockRenderer = new Mock<IRenderer>();
            _mockEnPassantTracker = new Mock<IEnPassantTracker>();
            _mockCheckDetector = new Mock<ICheckDetector>();
            _mockRecorder = new Mock<IGameRecorder>();
            _mockLogger = new Mock<ILogger>();

            _aiHandler = new AIHandler(
                _mockRenderer.Object,
                _mockEnPassantTracker.Object,
                _mockCheckDetector.Object,
                _mockRecorder.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void SelectAIMove_WithValidAI_DisplaysThinkingMessage()
        {
            // Arrange
            var mockAI = new Mock<IChessAI>();
            var mockBoard = new Mock<IChessBoard>();
            var aiMove = new AIMove
            {
                From = new Location(6, 4),
                To = new Location(4, 4),
                Evaluation = 0.5
            };
            mockAI.Setup(x => x.SelectMove(It.IsAny<IChessBoard>(), It.IsAny<PieceColor>(), It.IsAny<Location?>()))
                  .Returns(aiMove);

            // Act
            _aiHandler.SelectAIMove(mockAI.Object, mockBoard.Object, PieceColor.White);

            // Assert
            _mockRenderer.Verify(
                x => x.DisplayInfo(It.Is<string>(s => s.Contains("thinking"))),
                Times.Once
            );
        }

        [Fact]
        public void SelectAIMove_WhenAIReturnsNull_DisplaysError()
        {
            // Arrange
            var mockAI = new Mock<IChessAI>();
            var mockBoard = new Mock<IChessBoard>();
            mockAI.Setup(x => x.SelectMove(It.IsAny<IChessBoard>(), It.IsAny<PieceColor>(), It.IsAny<Location?>()))
                  .Returns((AIMove)null);

            // Act
            var result = _aiHandler.SelectAIMove(mockAI.Object, mockBoard.Object, PieceColor.White);

            // Assert
            Assert.Null(result);
            _mockRenderer.Verify(
                x => x.DisplayError(It.Is<string>(s => s.Contains("failed"))),
                Times.Once
            );
        }

        [Fact]
        public void SelectAIMove_WhenAIReturnsMove_LogsDetails()
        {
            // Arrange
            var mockAI = new Mock<IChessAI>();
            var mockBoard = new Mock<IChessBoard>();
            var aiMove = new AIMove
            {
                From = new Location(6, 4),
                To = new Location(4, 4),
                Evaluation = 1.25,
                NodesEvaluated = 5000,
                ThinkingTimeMs = 250
            };
            mockAI.Setup(x => x.SelectMove(It.IsAny<IChessBoard>(), It.IsAny<PieceColor>(), It.IsAny<Location?>()))
                  .Returns(aiMove);

            // Act
            _aiHandler.SelectAIMove(mockAI.Object, mockBoard.Object, PieceColor.White);

            // Assert
            _mockLogger.Verify(
                x => x.Info(It.Is<string>(s => s.Contains("AI move") && s.Contains("1.25"))),
                Times.Once
            );
        }

        [Fact]
        public void SelectAIMove_WhenExceptionThrown_LogsErrorAndReturnsNull()
        {
            // Arrange
            var mockAI = new Mock<IChessAI>();
            var mockBoard = new Mock<IChessBoard>();
            mockAI.Setup(x => x.SelectMove(It.IsAny<IChessBoard>(), It.IsAny<PieceColor>(), It.IsAny<Location?>()))
                  .Throws(new Exception("AI Error"));

            // Act
            var result = _aiHandler.SelectAIMove(mockAI.Object, mockBoard.Object, PieceColor.White);

            // Assert
            Assert.Null(result);
            _mockLogger.Verify(
                x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()),
                Times.Once
            );
        }

        [Fact]
        public void SelectAIMove_CallsEnPassantTracker()
        {
            // Arrange
            var mockAI = new Mock<IChessAI>();
            var mockBoard = new Mock<IChessBoard>();
            var aiMove = new AIMove { From = new Location(6, 4), To = new Location(4, 4) };
            mockAI.Setup(x => x.SelectMove(It.IsAny<IChessBoard>(), It.IsAny<PieceColor>(), It.IsAny<Location?>()))
                  .Returns(aiMove);

            // Act
            _aiHandler.SelectAIMove(mockAI.Object, mockBoard.Object, PieceColor.White);

            // Assert
            _mockEnPassantTracker.Verify(x => x.GetEnPassantTarget(), Times.Once);
        }
    }
}
