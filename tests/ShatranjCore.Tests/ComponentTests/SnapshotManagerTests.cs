using System;
using Xunit;
using Moq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;
using ShatranjCore.Persistence;
using ShatranjCore.State;

namespace ShatranjCore.Tests.ComponentTests
{
    /// <summary>
    /// Unit tests for SnapshotManager component
    /// Tests game state snapshot creation and restoration
    /// </summary>
    public class SnapshotManagerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IPieceFactory> _mockPieceFactory;
        private readonly SnapshotManager _snapshotManager;

        public SnapshotManagerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockPieceFactory = new Mock<IPieceFactory>();

            _snapshotManager = new SnapshotManager(
                _mockLogger.Object,
                _mockPieceFactory.Object
            );
        }

        [Fact]
        public void CreateSnapshot_WithValidContext_ReturnsSnapshot()
        {
            // Arrange
            var mockBoard = new Mock<IBoardState>();
            mockBoard.Setup(x => x.GetPiece(It.IsAny<Location>())).Returns((Pieces.Piece)null);

            var context = new GameContext
            {
                GameId = 1,
                GameMode = GameMode.HumanVsHuman,
                CurrentPlayer = PieceColor.White,
                HumanColor = PieceColor.White,
                GameResult = GameResult.InProgress,
                Difficulty = DifficultyLevel.Medium,
                WhitePlayerName = "Player1",
                BlackPlayerName = "Player2"
            };

            // Act
            var result = _snapshotManager.CreateSnapshot(mockBoard.Object, context);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GameStateSnapshot>(result);
        }

        [Fact]
        public void CreateSnapshot_WithPiecesOnBoard_IncludesPiecesInSnapshot()
        {
            // Arrange
            var mockBoard = new Mock<IBoardState>();
            var mockPiece = new Mock<Pieces.Piece>();
            mockPiece.Setup(x => x.Color).Returns(PieceColor.White);
            mockPiece.Setup(x => x.isMoved).Returns(false);
            mockPiece.Setup(x => x.GetType()).Returns(typeof(Pieces.Pawn));

            // Setup board to have a piece at one location
            mockBoard.Setup(x => x.GetPiece(new Location(0, 0))).Returns(mockPiece.Object);
            mockBoard.Setup(x => x.GetPiece(It.Is<Location>(l => l.Row != 0 || l.Column != 0)))
                    .Returns((Pieces.Piece)null);

            var context = new GameContext
            {
                GameId = 1,
                GameMode = GameMode.HumanVsHuman,
                CurrentPlayer = PieceColor.White,
                HumanColor = PieceColor.White,
                GameResult = GameResult.InProgress,
                Difficulty = DifficultyLevel.Easy,
                WhitePlayerName = "Test",
                BlackPlayerName = "Test2"
            };

            // Act
            var result = (GameStateSnapshot)_snapshotManager.CreateSnapshot(mockBoard.Object, context);

            // Assert
            Assert.NotEmpty(result.Pieces);
        }

        [Fact]
        public void RestoreSnapshot_WithValidSnapshot_LogsInfo()
        {
            // Arrange
            var mockBoard = new Mock<IBoardState>();
            var snapshot = new GameStateSnapshot
            {
                GameId = 1,
                GameMode = "HumanVsHuman",
                CurrentPlayer = "White",
                HumanColor = "White",
                GameResult = "InProgress",
                Difficulty = "Medium",
                WhitePlayerName = "Test",
                BlackPlayerName = "Test2"
            };

            // Act
            _snapshotManager.RestoreSnapshot(snapshot, mockBoard.Object, out GameContext context);

            // Assert
            _mockLogger.Verify(
                x => x.Info(It.Is<string>(s => s.Contains("restore"))),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public void RestoreSnapshot_WithValidSnapshot_ReturnsCorrectContext()
        {
            // Arrange
            var mockBoard = new Mock<IBoardState>();
            var snapshot = new GameStateSnapshot
            {
                GameId = 42,
                GameMode = "HumanVsAI",
                CurrentPlayer = "Black",
                HumanColor = "White",
                GameResult = "InProgress",
                Difficulty = "Hard",
                WhitePlayerName = "Human",
                BlackPlayerName = "AI"
            };

            // Act
            _snapshotManager.RestoreSnapshot(snapshot, mockBoard.Object, out GameContext context);

            // Assert
            Assert.Equal(42, context.GameId);
            Assert.Equal(GameMode.HumanVsAI, context.GameMode);
            Assert.Equal(PieceColor.Black, context.CurrentPlayer);
            Assert.Equal(DifficultyLevel.Hard, context.Difficulty);
            Assert.Equal("Human", context.WhitePlayerName);
            Assert.Equal("AI", context.BlackPlayerName);
        }

        [Fact]
        public void RestoreSnapshot_WhenExceptionThrown_LogsError()
        {
            // Arrange
            var mockBoard = new Mock<IBoardState>();
            var invalidSnapshot = new GameStateSnapshot
            {
                GameMode = "InvalidMode", // Will cause parse exception
                CurrentPlayer = "White",
                HumanColor = "White",
                GameResult = "InProgress",
                Difficulty = "Medium"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _snapshotManager.RestoreSnapshot(invalidSnapshot, mockBoard.Object, out GameContext context)
            );

            _mockLogger.Verify(
                x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()),
                Times.Once
            );
        }
    }
}
