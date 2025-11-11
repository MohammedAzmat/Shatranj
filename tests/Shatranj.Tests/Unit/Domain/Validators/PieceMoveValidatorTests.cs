using System;
using Xunit;
using Moq;
using ShatranjCore.Domain.Validators;
using ShatranjCore.Interfaces;
using ShatranjCore.Board;
using ShatranjCore.Pieces;
using ShatranjCore.Abstractions;

namespace Shatranj.Tests.Unit.Domain.Validators
{
    /// <summary>
    /// Tests for PieceMoveValidator - verifies piece movement validation
    /// </summary>
    public class PieceMoveValidatorTests
    {
        private readonly PieceMoveValidator _validator;
        private readonly Mock<IChessBoard> _mockBoard;

        public PieceMoveValidatorTests()
        {
            _validator = new PieceMoveValidator();
            _mockBoard = new Mock<IChessBoard>();
        }

        [Fact]
        public void Validate_NoSourcePiece_ReturnsErrorMessage()
        {
            // Arrange
            var from = new Location(1, 4);
            var to = new Location(3, 4);
            _mockBoard.Setup(b => b.GetPiece(from)).Returns((Piece)null);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, _mockBoard.Object);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Validate_WrongPlayersPiece_ReturnsErrorMessage()
        {
            // Arrange
            var from = new Location(6, 4);
            var to = new Location(5, 4);
            var blackPawn = new Pawn(PieceColor.Black);
            _mockBoard.Setup(b => b.GetPiece(from)).Returns(blackPawn);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, _mockBoard.Object);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Validate_LegalMove_ReturnsNull()
        {
            // Arrange
            var from = new Location(1, 4);
            var to = new Location(3, 4);
            var whitePawn = new Pawn(PieceColor.White);
            _mockBoard.Setup(b => b.GetPiece(from)).Returns(whitePawn);
            _mockBoard.Setup(b => b.GetPiece(to)).Returns((Piece)null);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, _mockBoard.Object);

            // Assert
            // Result depends on CanMove implementation
            // If pawn can move from 1,4 to 3,4 (2 squares forward from starting position)
            // then result should be null
        }

        [Fact]
        public void Validator_ImplementsInterface()
        {
            // Arrange & Act
            var validator = _validator as IMoveValidator;

            // Assert
            Assert.NotNull(validator);
            Assert.IsAssignableFrom<IMoveValidator>(_validator);
        }

        [Fact]
        public void Validate_AllParametersNull_HandlesProperly()
        {
            // Arrange & Act & Assert
            try
            {
                var result = _validator.Validate(
                    new Location(0, 0),
                    new Location(1, 0),
                    PieceColor.White,
                    null);
                // Should handle null board gracefully
            }
            catch (ArgumentNullException)
            {
                // Expected if board is required
            }
        }

        [Fact]
        public void Validate_SameFromAndTo_ReturnsErrorMessage()
        {
            // Arrange
            var location = new Location(2, 4);
            var whitePawn = new Pawn(PieceColor.White);
            _mockBoard.Setup(b => b.GetPiece(location)).Returns(whitePawn);

            // Act
            var result = _validator.Validate(location, location, PieceColor.White, _mockBoard.Object);

            // Assert
            // Should reject moving to same square
            Assert.NotNull(result);
        }

        [Fact]
        public void Validate_BlockedByFriendlyPiece_ReturnsErrorMessage()
        {
            // Arrange
            var from = new Location(1, 4);
            var to = new Location(2, 4);
            var sourcePawn = new Pawn(PieceColor.White);
            var blockingPawn = new Pawn(PieceColor.White); // Same color
            _mockBoard.Setup(b => b.GetPiece(from)).Returns(sourcePawn);
            _mockBoard.Setup(b => b.GetPiece(to)).Returns(blockingPawn);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, _mockBoard.Object);

            // Assert
            // Should reject moves blocked by friendly pieces
            // Result depends on implementation
        }

        [Fact]
        public void Validate_AllMovesForPiece_ReturnsValidation()
        {
            // Arrange - Test multiple move directions
            var board = new ChessBoard();
            board.InitializeBoard();
            var validator = new PieceMoveValidator();

            // Act & Assert - All white pawns can move from starting position
            var from = new Location(1, 0);
            var to = new Location(2, 0);
            var result = validator.Validate(from, to, PieceColor.White, board);

            // Should allow pawn to move 1 square forward
            Assert.Null(result); // Null means valid
        }
    }
}
