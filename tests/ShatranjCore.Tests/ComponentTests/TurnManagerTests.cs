using System;
using Xunit;
using Moq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Domain;
using ShatranjCore.Game;

namespace ShatranjCore.Tests.ComponentTests
{
    /// <summary>
    /// Unit tests for TurnManager component
    /// Tests turn switching, player management, and state transitions
    /// </summary>
    public class TurnManagerTests
    {
        private readonly Mock<IEnPassantTracker> _mockEnPassantTracker;
        private readonly Mock<IGameStateManager> _mockStateManager;
        private readonly Mock<ILogger> _mockLogger;
        private readonly TurnManager _turnManager;

        public TurnManagerTests()
        {
            _mockEnPassantTracker = new Mock<IEnPassantTracker>();
            _mockStateManager = new Mock<IGameStateManager>();
            _mockLogger = new Mock<ILogger>();

            _turnManager = new TurnManager(
                _mockEnPassantTracker.Object,
                _mockStateManager.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void Constructor_InitializesWithWhiteAsCurrentPlayer()
        {
            // Assert
            Assert.Equal(PieceColor.White, _turnManager.CurrentPlayer);
        }

        [Fact]
        public void SetCurrentPlayer_UpdatesCurrentPlayer()
        {
            // Act
            _turnManager.SetCurrentPlayer(PieceColor.Black);

            // Assert
            Assert.Equal(PieceColor.Black, _turnManager.CurrentPlayer);
        }

        [Fact]
        public void SwitchTurns_TogglesFromWhiteToBlack()
        {
            // Arrange
            _turnManager.SetCurrentPlayer(PieceColor.White);

            // Act
            _turnManager.SwitchTurns();

            // Assert
            Assert.Equal(PieceColor.Black, _turnManager.CurrentPlayer);
        }

        [Fact]
        public void SwitchTurns_TogglesFromBlackToWhite()
        {
            // Arrange
            _turnManager.SetCurrentPlayer(PieceColor.Black);

            // Act
            _turnManager.SwitchTurns();

            // Assert
            Assert.Equal(PieceColor.White, _turnManager.CurrentPlayer);
        }

        [Fact]
        public void SwitchTurns_CallsEnPassantTrackerNextTurn()
        {
            // Act
            _turnManager.SwitchTurns();

            // Assert
            _mockEnPassantTracker.Verify(x => x.NextTurn(), Times.Once);
        }

        [Fact]
        public void SwitchTurns_ClearsRedoStack()
        {
            // Act
            _turnManager.SwitchTurns();

            // Assert
            _mockStateManager.Verify(x => x.ClearRedoStack(), Times.Once);
        }

        [Fact]
        public void SwitchTurns_LogsDebugMessage()
        {
            // Arrange
            _turnManager.SetCurrentPlayer(PieceColor.White);

            // Act
            _turnManager.SwitchTurns();

            // Assert
            _mockLogger.Verify(
                x => x.Debug(It.Is<string>(s => s.Contains("Turn switched to Black"))),
                Times.Once
            );
        }

        [Fact]
        public void SwitchTurns_WithPlayers_TogglesPlayerHasTurnFlag()
        {
            // Arrange
            var players = new Player[]
            {
                new Player(PieceColor.White, PlayerType.Human) { HasTurn = true },
                new Player(PieceColor.Black, PlayerType.Human) { HasTurn = false }
            };
            _turnManager.SetPlayers(players);
            _turnManager.SetCurrentPlayer(PieceColor.White);

            // Act
            _turnManager.SwitchTurns();

            // Assert
            Assert.False(players[0].HasTurn);
            Assert.True(players[1].HasTurn);
        }

        [Fact]
        public void SwitchTurns_MultipleTimes_AlternatesCorrectly()
        {
            // Act & Assert
            Assert.Equal(PieceColor.White, _turnManager.CurrentPlayer);

            _turnManager.SwitchTurns();
            Assert.Equal(PieceColor.Black, _turnManager.CurrentPlayer);

            _turnManager.SwitchTurns();
            Assert.Equal(PieceColor.White, _turnManager.CurrentPlayer);

            _turnManager.SwitchTurns();
            Assert.Equal(PieceColor.Black, _turnManager.CurrentPlayer);
        }
    }
}
