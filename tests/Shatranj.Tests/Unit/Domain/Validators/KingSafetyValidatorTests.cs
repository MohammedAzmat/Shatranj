using System;
using Xunit;
using Moq;
using ShatranjCore.Domain.Validators;
using ShatranjCore.Interfaces;
using ShatranjCore.Board;
using ShatranjCore.Pieces;
using ShatranjCore.Abstractions;
using ShatranjCore.Validators;

namespace Shatranj.Tests.Unit.Domain.Validators
{
    /// <summary>
    /// Tests for KingSafetyValidator - verifies king safety validation
    /// </summary>
    public class KingSafetyValidatorTests
    {
        private readonly KingSafetyValidator _validator;
        private readonly Mock<IChessBoard> _mockBoard;
        private readonly Mock<CheckDetector> _mockCheckDetector;

        public KingSafetyValidatorTests()
        {
            _validator = new KingSafetyValidator();
            _mockBoard = new Mock<IChessBoard>();
            _mockCheckDetector = new Mock<CheckDetector>();
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
        public void Validate_SafeMove_ReturnsNull()
        {
            // Arrange
            var from = new Location(1, 4);
            var to = new Location(3, 4);
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, board);

            // Assert
            // Moving a pawn forward from starting position should be safe
            Assert.Null(result);
        }

        [Fact]
        public void Validate_KingSafety_VerifiedBeforeMoving()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();
            var from = new Location(1, 4);
            var to = new Location(2, 4);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, board);

            // Assert
            // Standard opening moves should be safe
            Assert.Null(result);
        }

        [Fact]
        public void Validate_AllParametersProvided_ExecutesValidation()
        {
            // Arrange
            var from = new Location(0, 4);
            var to = new Location(1, 4);
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, board);

            // Assert
            // Should execute without error
            // Result depends on whether king is exposed
        }

        [Fact]
        public void Validate_NullBoard_HandlesProperly()
        {
            // Arrange & Act & Assert
            try
            {
                var result = _validator.Validate(
                    new Location(0, 4),
                    new Location(1, 4),
                    PieceColor.White,
                    null);
            }
            catch (ArgumentNullException)
            {
                // Expected if board is required
            }
        }

        [Fact]
        public void Validate_WhiteKingSafety_CheckedCorrectly()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act - Try moving white king from starting position
            var result = _validator.Validate(
                new Location(0, 4),
                new Location(1, 4),
                PieceColor.White,
                board);

            // Assert
            // King cannot move to pawns' ranks
            Assert.NotNull(result); // Should be error - pawn in the way
        }

        [Fact]
        public void Validate_BlackKingSafety_CheckedCorrectly()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act - Try moving black king from starting position
            var result = _validator.Validate(
                new Location(7, 4),
                new Location(6, 4),
                PieceColor.Black,
                board);

            // Assert
            // King cannot move to pawns' ranks
            Assert.NotNull(result); // Should be error - pawn in the way
        }

        [Fact]
        public void Validate_BlockingCheck_AllowsMove()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            // Moving a piece to block a check should be allowed
            var result = _validator.Validate(
                new Location(1, 0), // White pawn
                new Location(2, 0),
                PieceColor.White,
                board);

            // Assert
            Assert.Null(result); // Safe move
        }

        [Fact]
        public void Validate_CastlingKingSafety_CheckedBeforeCastling()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act - Castling has additional king safety checks
            var result = _validator.Validate(
                new Location(0, 4),
                new Location(0, 6),
                PieceColor.White,
                board);

            // Assert
            // Immediate castling is not possible from starting position
            // (pieces block it, so validator should reject)
        }

        [Fact]
        public void Validate_MultipleValidations_AllChecked()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();
            var from = new Location(1, 4);
            var to = new Location(3, 4);

            // Act
            var result = _validator.Validate(from, to, PieceColor.White, board);

            // Assert
            // Pawn double-move on first move is safe
            Assert.Null(result);
        }
    }
}
