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
    /// Unit tests for Knight piece movement.
    /// Knights move in an L-shape: 2 squares in one direction, 1 square perpendicular.
    /// </summary>
    public class KnightMovementTests
    {
        /// <summary>
        /// Test that a Knight in the center of an empty board has 8 valid moves.
        /// </summary>
        [Fact]
        public void KnightCenterEmptyBoard_Has8Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(3, 3, PieceColor.White);
            board.PlacePiece(knight, new Location(3, 3));

            // Act
            List<Move> moves = knight.GetMoves(new Location(3, 3), board);

            // Assert
            Assert.Equal(8, moves.Count);
        }

        /// <summary>
        /// Test that a Knight in the corner has only 2 valid moves.
        /// </summary>
        [Fact]
        public void KnightCornerEmptyBoard_Has2Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(0, 0, PieceColor.White);
            board.PlacePiece(knight, new Location(0, 0));

            // Act
            List<Move> moves = knight.GetMoves(new Location(0, 0), board);

            // Assert
            Assert.Equal(2, moves.Count);
        }

        /// <summary>
        /// Test that a Knight can jump over other pieces.
        /// </summary>
        [Fact]
        public void KnightCanJumpOverPieces()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(3, 3, PieceColor.White);

            // Surround knight with pawns
            for (int row = 2; row <= 4; row++)
            {
                for (int col = 2; col <= 4; col++)
                {
                    if (row != 3 || col != 3)
                    {
                        board.PlacePiece(new Pawn(row, col, PieceColor.White, PawnMoves.Up), new Location(row, col));
                    }
                }
            }

            board.PlacePiece(knight, new Location(3, 3));

            // Act
            List<Move> moves = knight.GetMoves(new Location(3, 3), board);

            // Assert - Knight should still have 8 moves despite being surrounded
            Assert.Equal(8, moves.Count);
        }

        /// <summary>
        /// Test that a Knight can capture enemy pieces.
        /// </summary>
        [Fact]
        public void KnightCanCaptureEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(3, 3, PieceColor.White);
            Pawn enemyPawn = new Pawn(1, 2, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(knight, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(1, 2));

            // Act
            List<Move> moves = knight.GetMoves(new Location(3, 3), board);
            var captureMove = moves.FirstOrDefault(m =>
                m.To.Location.Row == 1 && m.To.Location.Column == 2 && m.CapturedPiece != null);

            // Assert
            Assert.NotNull(captureMove);
        }

        /// <summary>
        /// Test that a Knight cannot move to a square occupied by a friendly piece.
        /// </summary>
        [Fact]
        public void KnightCannotMoveToFriendlyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(3, 3, PieceColor.White);
            Pawn friendlyPawn = new Pawn(1, 2, PieceColor.White, PawnMoves.Up);

            board.PlacePiece(knight, new Location(3, 3));
            board.PlacePiece(friendlyPawn, new Location(1, 2));

            // Act
            List<Move> moves = knight.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m =>
                m.To.Location.Row == 1 && m.To.Location.Column == 2);

            // Assert
            Assert.Null(blockedSquare);
        }

        /// <summary>
        /// Test that Knight moves are exactly L-shaped (2+1 squares).
        /// </summary>
        [Fact]
        public void KnightMovesAreLShaped()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Knight knight = new Knight(3, 3, PieceColor.White);
            board.PlacePiece(knight, new Location(3, 3));

            // Act
            List<Move> moves = knight.GetMoves(new Location(3, 3), board);

            // Assert - all moves should be L-shaped (2+1 or 1+2)
            foreach (var move in moves)
            {
                int rowDiff = System.Math.Abs(move.To.Location.Row - 3);
                int colDiff = System.Math.Abs(move.To.Location.Column - 3);
                Assert.True((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2));
            }
        }
    }
}
