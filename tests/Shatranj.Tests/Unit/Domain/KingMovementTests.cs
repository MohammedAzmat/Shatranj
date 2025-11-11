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
    /// Unit tests for King piece movement.
    /// Tests one-square movement in all directions.
    /// </summary>
    public class KingMovementTests
    {
        /// <summary>
        /// Test that a king in the center of an empty board has 8 possible moves (all adjacent squares).
        /// </summary>
        [Fact]
        public void KingCenterEmptyBoard_Has8Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Act
            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Assert
            Assert.Equal(8, moves.Count);
        }

        /// <summary>
        /// Test that a king in a corner has 3 possible moves.
        /// </summary>
        [Fact]
        public void KingCornerEmptyBoard_Has3Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(7, 0, PieceColor.White);
            board.PlacePiece(king, new Location(7, 0));

            // Act
            List<Move> moves = king.GetMoves(new Location(7, 0), board);

            // Assert
            Assert.Equal(3, moves.Count);
        }

        /// <summary>
        /// Test that a king on an edge has 5 possible moves.
        /// </summary>
        [Fact]
        public void KingEdgeEmptyBoard_Has5Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(7, 4, PieceColor.White);
            board.PlacePiece(king, new Location(7, 4));

            // Act
            List<Move> moves = king.GetMoves(new Location(7, 4), board);

            // Assert
            Assert.Equal(5, moves.Count);
        }

        /// <summary>
        /// Test that a king can capture an enemy piece.
        /// </summary>
        [Fact]
        public void KingCanCaptureEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Place black pawn adjacent
            Pawn enemyPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(3, 3));

            // Act
            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Assert
            bool canCapture = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3 && m.CapturedPiece != null);
            Assert.True(canCapture);
        }

        /// <summary>
        /// Test that a king cannot capture a friendly piece.
        /// </summary>
        [Fact]
        public void KingCannotCaptureFriendlyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Place white pawn adjacent (friendly piece)
            Pawn friendlyPawn = new Pawn(3, 3, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(friendlyPawn, new Location(3, 3));

            // Act
            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Assert
            bool canMoveToPawn = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3);
            Assert.False(canMoveToPawn);
        }

        /// <summary>
        /// Test that a king only moves one square in any direction.
        /// </summary>
        [Fact]
        public void KingOnlyMovesOneSquare()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Act
            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Assert
            foreach (Move move in moves)
            {
                int rowDiff = System.Math.Abs(move.To.Location.Row - 4);
                int colDiff = System.Math.Abs(move.To.Location.Column - 3);

                // King moves should be at most 1 square in any direction
                Assert.True(rowDiff <= 1 && colDiff <= 1);
            }
        }
    }
}
