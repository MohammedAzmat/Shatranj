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
    /// Unit tests for Bishop piece movement.
    /// Tests diagonal movement, capture logic, and blocking.
    /// </summary>
    public class BishopMovementTests
    {
        /// <summary>
        /// Test that a bishop in the center of an empty board has 13 possible moves (all diagonals).
        /// </summary>
        [Fact]
        public void BishopCenterEmptyBoard_Has13Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Assert
            Assert.Equal(13, moves.Count);
        }

        /// <summary>
        /// Test that a bishop in the corner of an empty board has 7 possible moves.
        /// </summary>
        [Fact]
        public void BishopCornerEmptyBoard_Has7Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(0, 0, PieceColor.White);
            board.PlacePiece(bishop, new Location(0, 0));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(0, 0), board);

            // Assert
            Assert.Equal(7, moves.Count);
        }

        /// <summary>
        /// Test that a bishop can capture an enemy piece on a diagonal.
        /// </summary>
        [Fact]
        public void BishopCanCaptureEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            Pawn enemy = new Pawn(6, 5, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(bishop, new Location(4, 3));
            board.PlacePiece(enemy, new Location(6, 5));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Assert - should have move to capture enemy pawn
            var captureMove = moves.FirstOrDefault(m =>
                m.To.Location.Row == 6 && m.To.Location.Column == 5);
            Assert.NotNull(captureMove);
        }

        /// <summary>
        /// Test that a bishop cannot move past a friendly piece.
        /// </summary>
        [Fact]
        public void BishopCannotMovePastFriendlyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            Pawn friendly = new Pawn(6, 5, PieceColor.White, PawnMoves.Up);

            board.PlacePiece(bishop, new Location(4, 3));
            board.PlacePiece(friendly, new Location(6, 5));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Assert - should have move to square with friendly piece blocked, but not beyond
            var moveToFriendly = moves.FirstOrDefault(m =>
                m.To.Location.Row == 6 && m.To.Location.Column == 5);
            Assert.Null(moveToFriendly);

            // Should not have move to 7,6 (beyond the friendly piece)
            var moveBeyond = moves.FirstOrDefault(m =>
                m.To.Location.Row == 7 && m.To.Location.Column == 6);
            Assert.Null(moveBeyond);
        }

        /// <summary>
        /// Test that a bishop cannot move past an enemy piece.
        /// </summary>
        [Fact]
        public void BishopCannotMovePastEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            Pawn enemy = new Pawn(6, 5, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(bishop, new Location(4, 3));
            board.PlacePiece(enemy, new Location(6, 5));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Assert - should have move to capture enemy, but not beyond
            var moveToEnemy = moves.FirstOrDefault(m =>
                m.To.Location.Row == 6 && m.To.Location.Column == 5);
            Assert.NotNull(moveToEnemy);

            // Should not have move beyond the enemy piece
            var moveBeyond = moves.FirstOrDefault(m =>
                m.To.Location.Row == 7 && m.To.Location.Column == 6);
            Assert.Null(moveBeyond);
        }

        /// <summary>
        /// Test that a bishop only moves diagonally.
        /// </summary>
        [Fact]
        public void BishopOnlyMovesDiagonally()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            // Act
            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Assert - all moves should be diagonal (abs(row_delta) == abs(col_delta))
            foreach (var move in moves)
            {
                int rowDelta = System.Math.Abs(move.To.Location.Row - 4);
                int colDelta = System.Math.Abs(move.To.Location.Column - 3);
                Assert.Equal(rowDelta, colDelta);
            }
        }
    }
}
