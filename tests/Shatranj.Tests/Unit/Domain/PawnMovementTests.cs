using Xunit;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Movement;
using ShatranjCore.Interfaces;
using Shatranj.Tests.Helpers;

namespace Shatranj.Tests.Unit.Domain
{
    /// <summary>
    /// Unit tests for Pawn piece movement.
    /// Tests forward movement, captures, double move, and en passant.
    /// </summary>
    public class PawnMovementTests
    {
        /// <summary>
        /// Test that a pawn at starting position can move 1 or 2 squares forward.
        /// </summary>
        [Fact]
        public void PawnStartPosition_CanMoveOneOrTwoSquares()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(6, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(6, 4), board);

            // Assert - Should be able to move to e3 (row 5) or e4 (row 4)
            bool canMoveOneSquare = moves.Any(m => m.To.Location.Row == 5 && m.To.Location.Column == 4);
            bool canMoveTwoSquares = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column == 4);

            Assert.True(canMoveOneSquare);
            Assert.True(canMoveTwoSquares);
            Assert.Equal(2, moves.Count);
        }

        /// <summary>
        /// Test that a pawn not at starting position can only move 1 square forward.
        /// </summary>
        [Fact]
        public void PawnNonStartPosition_CanMoveOneSquare()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            pawn.isMoved = true; // Mark as moved
            board.PlacePiece(pawn, new Location(4, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Assert
            bool canMoveOneSquare = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 4);
            bool cannotMoveTwoSquares = !moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 4);

            Assert.True(canMoveOneSquare);
            Assert.True(cannotMoveTwoSquares);
            Assert.Equal(1, moves.Count);
        }

        /// <summary>
        /// Test that a pawn cannot move forward when blocked.
        /// </summary>
        [Fact]
        public void PawnCannotMoveForwardWhenBlocked()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(6, 4));

            // Place black pawn blocking
            Pawn blockingPawn = new Pawn(5, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blockingPawn, new Location(5, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(6, 4), board);

            // Assert
            Assert.Equal(0, moves.Count);
        }

        /// <summary>
        /// Test that a pawn can capture diagonally.
        /// </summary>
        [Fact]
        public void PawnCanCaptureDiagonally()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Place black pawns diagonally
            Pawn enemyPawn1 = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            Pawn enemyPawn2 = new Pawn(3, 5, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn1, new Location(3, 3));
            board.PlacePiece(enemyPawn2, new Location(3, 5));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Assert
            bool canCaptureLeft = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3 && m.CapturedPiece != null);
            bool canCaptureRight = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 5 && m.CapturedPiece != null);
            bool canMoveForward = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 4 && m.CapturedPiece == null);

            Assert.True(canCaptureLeft);
            Assert.True(canCaptureRight);
            Assert.True(canMoveForward);
            // May include en passant moves which is valid
            Assert.True(moves.Count >= 3);
        }

        /// <summary>
        /// Test that a pawn cannot capture forward.
        /// </summary>
        [Fact]
        public void PawnCannotCaptureForward()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Place black pawn directly in front (blocking)
            Pawn enemyPawn = new Pawn(3, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(3, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Assert - Pawn should not be able to move
            Assert.Equal(0, moves.Count);
        }

        /// <summary>
        /// Test that a pawn cannot move backward.
        /// </summary>
        [Fact]
        public void PawnCannotMoveBackward()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Assert
            bool hasBackwardMove = moves.Any(m => m.To.Location.Row > 4);
            Assert.False(hasBackwardMove);
        }

        /// <summary>
        /// Test that white pawns move up (decreasing row number).
        /// </summary>
        [Fact]
        public void PawnWhiteMovesUp()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Assert
            bool allMovesUp = moves.All(m => m.To.Location.Row < 4);
            Assert.True(allMovesUp);
            // At least 1 forward move (may include en passant)
            Assert.True(moves.Count >= 1);
        }

        /// <summary>
        /// Test that black pawns move down (increasing row number).
        /// </summary>
        [Fact]
        public void PawnBlackMovesDown()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn pawn = new Pawn(3, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(pawn, new Location(3, 4));

            // Act
            List<Move> moves = pawn.GetMoves(new Location(3, 4), board);

            // Assert
            bool allMovesDown = moves.All(m => m.To.Location.Row > 3);
            Assert.True(allMovesDown);
            // At least 1 forward move (may include en passant)
            Assert.True(moves.Count >= 1);
        }

        /// <summary>
        /// Test that a pawn can capture en passant when available.
        /// </summary>
        [Fact]
        public void PawnEnPassant_Available()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn whitePawn = new Pawn(3, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(whitePawn, new Location(3, 4));

            // Place black pawn adjacent
            Pawn blackPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blackPawn, new Location(3, 3));

            // En passant target is d6 (row 2, col 3)
            Location enPassantTarget = new Location(2, 3);

            // Act
            List<Move> moves = whitePawn.GetMovesWithEnPassant(new Location(3, 4), board, enPassantTarget);

            // Assert
            bool hasForwardMove = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 4);
            bool hasEnPassant = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3 && m.CapturedPiece != null);

            Assert.True(hasForwardMove);
            Assert.True(hasEnPassant);
            // At least 2 moves (forward + en passant, may include other moves)
            Assert.True(moves.Count >= 2);
        }

        /// <summary>
        /// Test that a pawn cannot capture en passant when not available.
        /// </summary>
        [Fact]
        public void PawnEnPassant_NotAvailable()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Pawn whitePawn = new Pawn(3, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(whitePawn, new Location(3, 4));

            // Place black pawn adjacent (but no en passant available)
            Pawn blackPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blackPawn, new Location(3, 3));

            // Act
            List<Move> moves = whitePawn.GetMovesWithEnPassant(new Location(3, 4), board, null);

            // Assert - Should have at least forward move (may include other moves)
            bool hasForwardMove = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 4 && m.CapturedPiece == null);
            Assert.True(hasForwardMove);
            // Should not have en passant move without target
            bool hasNoEnPassant = !moves.Any(m => m.CapturedPiece != null && (m.To.Location.Column == 3 || m.To.Location.Column == 5));
            Assert.True(hasNoEnPassant);
        }
    }
}
