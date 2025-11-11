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
    /// Unit tests for Queen piece movement.
    /// Tests horizontal, vertical, and diagonal movement (combination of Rook and Bishop).
    /// </summary>
    public class QueenMovementTests
    {
        /// <summary>
        /// Test that a queen in the center of an empty board has 27 possible moves.
        /// </summary>
        [Fact]
        public void QueenCenterEmptyBoard_Has27Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Act
            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Assert
            Assert.Equal(27, moves.Count);
        }

        /// <summary>
        /// Test that a queen in a corner has 21 possible moves.
        /// </summary>
        [Fact]
        public void QueenCornerEmptyBoard_Has21Moves()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(7, 0, PieceColor.White);
            board.PlacePiece(queen, new Location(7, 0));

            // Act
            List<Move> moves = queen.GetMoves(new Location(7, 0), board);

            // Assert
            Assert.Equal(21, moves.Count);
        }

        /// <summary>
        /// Test that a queen can capture enemy pieces in all directions.
        /// </summary>
        [Fact]
        public void QueenCanCaptureEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place black pawns in different directions
            Pawn enemyPawn1 = new Pawn(2, 3, PieceColor.Black, PawnMoves.Down); // Vertical
            Pawn enemyPawn2 = new Pawn(4, 6, PieceColor.Black, PawnMoves.Down); // Horizontal
            Pawn enemyPawn3 = new Pawn(2, 5, PieceColor.Black, PawnMoves.Down); // Diagonal

            board.PlacePiece(enemyPawn1, new Location(2, 3));
            board.PlacePiece(enemyPawn2, new Location(4, 6));
            board.PlacePiece(enemyPawn3, new Location(2, 5));

            // Act
            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Assert - Check if queen can capture all three pawns
            bool canCaptureVertical = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3 && m.CapturedPiece != null);
            bool canCaptureHorizontal = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column == 6 && m.CapturedPiece != null);
            bool canCaptureDiagonal = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 5 && m.CapturedPiece != null);

            Assert.True(canCaptureVertical);
            Assert.True(canCaptureHorizontal);
            Assert.True(canCaptureDiagonal);
        }

        /// <summary>
        /// Test that a queen cannot move past friendly pieces.
        /// </summary>
        [Fact]
        public void QueenCannotMovePastFriendlyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place white pawn blocking vertical path
            Pawn friendlyPawn = new Pawn(2, 3, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(friendlyPawn, new Location(2, 3));

            // Act
            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Assert - Queen should not be able to reach the friendly piece or beyond
            bool canReachD6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3);
            bool canReachD7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 3);
            bool canReachD8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 3);

            Assert.False(canReachD6);
            Assert.False(canReachD7);
            Assert.False(canReachD8);
        }

        /// <summary>
        /// Test that a queen cannot move past an enemy piece.
        /// </summary>
        [Fact]
        public void QueenCannotMovePastEnemyPiece()
        {
            // Arrange
            ChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place black pawn blocking vertical path
            Pawn enemyPawn = new Pawn(2, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(2, 3));

            // Act
            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Assert - Queen should be able to capture but not move past
            bool canReachD6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3);
            bool canReachD7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 3);
            bool canReachD8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 3);

            Assert.True(canReachD6);
            Assert.False(canReachD7);
            Assert.False(canReachD8);
        }

        /// <summary>
        /// Test that a queen moves in all 8 directions (horizontal, vertical, and diagonal).
        /// </summary>
        [Fact]
        public void QueenMovesInAllDirections()
        {
            // Arrange
            IChessBoard board = TestBoardFactory.CreateEmptyBoard();
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Act
            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Assert - Check that queen can move in all 8 directions
            bool hasUp = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column == 3);
            bool hasDown = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column == 3);
            bool hasLeft = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column < 3);
            bool hasRight = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column > 3);
            bool hasUpLeft = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column < 3);
            bool hasUpRight = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column > 3);
            bool hasDownLeft = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column < 3);
            bool hasDownRight = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column > 3);

            Assert.True(hasUp && hasDown && hasLeft && hasRight && hasUpLeft && hasUpRight && hasDownLeft && hasDownRight);
        }
    }
}
