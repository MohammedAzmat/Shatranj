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
    /// Unit tests for Rook piece movement.
    /// Tests all scenarios: normal moves, captures, blocked paths, edge cases.
    /// </summary>
    public class RookMovementTests
    {
        /// <summary>
        /// Test that a Rook in the center of an empty board has 14 valid moves (7 vertical + 7 horizontal).
        /// </summary>
        [Fact]
        public void RookCenterEmptyBoard_Has14Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(3, 3, PieceColor.White);
            board.PlacePiece(rook, new Location(3, 3));

            // Act
            List<Move> moves = rook.GetMoves(new Location(3, 3), board);

            // Assert
            Assert.Equal(14, moves.Count);
        }

        /// <summary>
        /// Test that a Rook in the corner has 14 valid moves.
        /// </summary>
        [Fact]
        public void RookCornerEmptyBoard_Has14Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(0, 0, PieceColor.White);
            board.PlacePiece(rook, new Location(0, 0));

            // Act
            List<Move> moves = rook.GetMoves(new Location(0, 0), board);

            // Assert
            Assert.Equal(14, moves.Count);
        }

        /// <summary>
        /// Test that a Rook can capture an enemy piece.
        /// </summary>
        [Fact]
        public void RookCanCaptureEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(3, 3, PieceColor.White);
            Pawn enemyPawn = new Pawn(3, 6, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(3, 6));

            // Act
            List<Move> moves = rook.GetMoves(new Location(3, 3), board);
            var captureMove = moves.FirstOrDefault(m =>
                m.To.Location.Row == 3 && m.To.Location.Column == 6 && m.CapturedPiece != null);

            // Assert
            Assert.NotNull(captureMove);
        }

        /// <summary>
        /// Test that a Rook cannot move past a friendly piece.
        /// </summary>
        [Fact]
        public void RookCannotMovePastFriendlyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(3, 3, PieceColor.White);
            Pawn friendlyPawn = new Pawn(3, 5, PieceColor.White, PawnMoves.Up);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(friendlyPawn, new Location(3, 5));

            // Act
            List<Move> moves = rook.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 6);
            var friendlySquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 5);

            // Assert
            Assert.Null(blockedSquare);
            Assert.Null(friendlySquare);
        }

        /// <summary>
        /// Test that a Rook cannot move past an enemy piece (but can capture it).
        /// </summary>
        [Fact]
        public void RookCannotMovePastEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(3, 3, PieceColor.White);
            Pawn enemyPawn = new Pawn(3, 5, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(3, 5));

            // Act
            List<Move> moves = rook.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 6);
            var captureSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 5);

            // Assert
            Assert.Null(blockedSquare);
            Assert.NotNull(captureSquare);
            Assert.NotNull(captureSquare.CapturedPiece);
        }

        /// <summary>
        /// Test that a Rook moves only horizontally and vertically, not diagonally.
        /// </summary>
        [Fact]
        public void RookOnlyMovesHorizontallyAndVertically()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Rook rook = new Rook(3, 3, PieceColor.White);
            board.PlacePiece(rook, new Location(3, 3));

            // Act
            List<Move> moves = rook.GetMoves(new Location(3, 3), board);
            var diagonalMove = moves.FirstOrDefault(m =>
                m.To.Location.Row != 3 && m.To.Location.Column != 3);

            // Assert
            Assert.Null(diagonalMove);
        }
    }
}
