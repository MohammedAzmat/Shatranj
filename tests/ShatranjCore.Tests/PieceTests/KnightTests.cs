using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore;

namespace ShatranjCore.Tests.PieceTests
{
    /// <summary>
    /// Unit tests for Knight movement logic.
    /// Knights move in an L-shape: 2 squares in one direction, 1 square perpendicular.
    /// </summary>
    public class KnightTests
    {
        /// <summary>
        /// Test that a Knight in the center of an empty board has 8 valid moves.
        /// </summary>
        public static void Test_Knight_Center_EmptyBoard_Has8Moves()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(3, 3, PieceColor.White);
            board.PlacePiece(knight, new Location(3, 3));

            // Act
            var moves = knight.GetMoves(new Location(3, 3), board);

            // Assert
            if (moves.Count == 8)
                Console.WriteLine("✓ PASS: Knight_Center_EmptyBoard_Has8Moves");
            else
                Console.WriteLine($"✗ FAIL: Knight_Center_EmptyBoard_Has8Moves (Expected 8, got {moves.Count})");
        }

        /// <summary>
        /// Test that a Knight in the corner has only 2 valid moves.
        /// </summary>
        public static void Test_Knight_Corner_EmptyBoard_Has2Moves()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(0, 0, PieceColor.White);
            board.PlacePiece(knight, new Location(0, 0));

            // Act
            var moves = knight.GetMoves(new Location(0, 0), board);

            // Assert
            if (moves.Count == 2)
                Console.WriteLine("✓ PASS: Knight_Corner_EmptyBoard_Has2Moves");
            else
                Console.WriteLine($"✗ FAIL: Knight_Corner_EmptyBoard_Has2Moves (Expected 2, got {moves.Count})");
        }

        /// <summary>
        /// Test that a Knight can jump over other pieces.
        /// </summary>
        public static void Test_Knight_CanJumpOverPieces()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(3, 3, PieceColor.White);

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
            var moves = knight.GetMoves(new Location(3, 3), board);

            // Assert
            // Knight should still have 8 moves despite being surrounded
            if (moves.Count == 8)
                Console.WriteLine("✓ PASS: Knight_CanJumpOverPieces");
            else
                Console.WriteLine($"✗ FAIL: Knight_CanJumpOverPieces (Expected 8, got {moves.Count})");
        }

        /// <summary>
        /// Test that a Knight can capture enemy pieces.
        /// </summary>
        public static void Test_Knight_CanCaptureEnemyPiece()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(3, 3, PieceColor.White);
            var enemyPawn = new Pawn(1, 2, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(knight, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(1, 2));

            // Act
            var moves = knight.GetMoves(new Location(3, 3), board);
            var captureMove = moves.FirstOrDefault(m =>
                m.To.Location.Row == 1 && m.To.Location.Column == 2 && m.CapturedPiece != null);

            // Assert
            if (captureMove != null)
                Console.WriteLine("✓ PASS: Knight_CanCaptureEnemyPiece");
            else
                Console.WriteLine("✗ FAIL: Knight_CanCaptureEnemyPiece");
        }

        /// <summary>
        /// Test that a Knight cannot move to a square occupied by a friendly piece.
        /// </summary>
        public static void Test_Knight_CannotMoveToFriendlyPiece()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(3, 3, PieceColor.White);
            var friendlyPawn = new Pawn(1, 2, PieceColor.White, PawnMoves.Up);

            board.PlacePiece(knight, new Location(3, 3));
            board.PlacePiece(friendlyPawn, new Location(1, 2));

            // Act
            var moves = knight.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m =>
                m.To.Location.Row == 1 && m.To.Location.Column == 2);

            // Assert
            if (blockedSquare == null)
                Console.WriteLine("✓ PASS: Knight_CannotMoveToFriendlyPiece");
            else
                Console.WriteLine("✗ FAIL: Knight_CannotMoveToFriendlyPiece");
        }

        /// <summary>
        /// Test that Knight moves are exactly L-shaped (2+1 squares).
        /// </summary>
        public static void Test_Knight_MovesAreLShaped()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var knight = new Knight(3, 3, PieceColor.White);
            board.PlacePiece(knight, new Location(3, 3));

            // Act
            var moves = knight.GetMoves(new Location(3, 3), board);

            // Assert
            bool allLShaped = moves.All(m =>
            {
                int rowDiff = Math.Abs(m.To.Location.Row - 3);
                int colDiff = Math.Abs(m.To.Location.Column - 3);
                return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
            });

            if (allLShaped)
                Console.WriteLine("✓ PASS: Knight_MovesAreLShaped");
            else
                Console.WriteLine("✗ FAIL: Knight_MovesAreLShaped");
        }

        /// <summary>
        /// Runs all Knight tests.
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("\n═══ Running Knight Tests ═══");
            Test_Knight_Center_EmptyBoard_Has8Moves();
            Test_Knight_Corner_EmptyBoard_Has2Moves();
            Test_Knight_CanJumpOverPieces();
            Test_Knight_CanCaptureEnemyPiece();
            Test_Knight_CannotMoveToFriendlyPiece();
            Test_Knight_MovesAreLShaped();
            Console.WriteLine();
        }

        /// <summary>
        /// Helper method to create an empty chess board.
        /// </summary>
        private static IChessBoard CreateEmptyBoard()
        {
            return new ChessBoard(PieceColor.White);
        }
    }
}
