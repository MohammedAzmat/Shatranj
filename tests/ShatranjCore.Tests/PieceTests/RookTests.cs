using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Tests.PieceTests
{
    /// <summary>
    /// Unit tests for Rook movement logic.
    /// Tests all scenarios: normal moves, captures, blocked paths, edge cases.
    /// </summary>
    public class RookTests
    {
        /// <summary>
        /// Test that a Rook in the center of an empty board has 14 valid moves (7 vertical + 7 horizontal).
        /// </summary>
        public static void Test_Rook_Center_EmptyBoard_Has14Moves()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(3, 3, PieceColor.White);
            board.PlacePiece(rook, new Location(3, 3));

            // Act
            var moves = rook.GetMoves(new Location(3, 3), board);

            // Assert
            if (moves.Count == 14)
                Console.WriteLine("✓ PASS: Rook_Center_EmptyBoard_Has14Moves");
            else
                Console.WriteLine($"✗ FAIL: Rook_Center_EmptyBoard_Has14Moves (Expected 14, got {moves.Count})");
        }

        /// <summary>
        /// Test that a Rook in the corner has 14 valid moves.
        /// </summary>
        public static void Test_Rook_Corner_EmptyBoard_Has14Moves()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(0, 0, PieceColor.White);
            board.PlacePiece(rook, new Location(0, 0));

            // Act
            var moves = rook.GetMoves(new Location(0, 0), board);

            // Assert
            if (moves.Count == 14)
                Console.WriteLine("✓ PASS: Rook_Corner_EmptyBoard_Has14Moves");
            else
                Console.WriteLine($"✗ FAIL: Rook_Corner_EmptyBoard_Has14Moves (Expected 14, got {moves.Count})");
        }

        /// <summary>
        /// Test that a Rook can capture an enemy piece.
        /// </summary>
        public static void Test_Rook_CanCaptureEnemyPiece()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(3, 3, PieceColor.White);
            var enemyPawn = new Pawn(3, 6, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(3, 6));

            // Act
            var moves = rook.GetMoves(new Location(3, 3), board);
            var captureMove = moves.FirstOrDefault(m => m.CapturedPiece != null);

            // Assert
            if (captureMove != null && captureMove.To.Location.Row == 3 && captureMove.To.Location.Column == 6)
                Console.WriteLine("✓ PASS: Rook_CanCaptureEnemyPiece");
            else
                Console.WriteLine("✗ FAIL: Rook_CanCaptureEnemyPiece");
        }

        /// <summary>
        /// Test that a Rook cannot move past a friendly piece.
        /// </summary>
        public static void Test_Rook_CannotMovePastFriendlyPiece()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(3, 3, PieceColor.White);
            var friendlyPawn = new Pawn(3, 5, PieceColor.White, PawnMoves.Up);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(friendlyPawn, new Location(3, 5));

            // Act
            var moves = rook.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 6);
            var friendlySquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 5);

            // Assert
            if (blockedSquare == null && friendlySquare == null)
                Console.WriteLine("✓ PASS: Rook_CannotMovePastFriendlyPiece");
            else
                Console.WriteLine("✗ FAIL: Rook_CannotMovePastFriendlyPiece");
        }

        /// <summary>
        /// Test that a Rook cannot move past an enemy piece (but can capture it).
        /// </summary>
        public static void Test_Rook_CannotMovePastEnemyPiece()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(3, 3, PieceColor.White);
            var enemyPawn = new Pawn(3, 5, PieceColor.Black, PawnMoves.Down);

            board.PlacePiece(rook, new Location(3, 3));
            board.PlacePiece(enemyPawn, new Location(3, 5));

            // Act
            var moves = rook.GetMoves(new Location(3, 3), board);
            var blockedSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 6);
            var captureSquare = moves.FirstOrDefault(m => m.To.Location.Row == 3 && m.To.Location.Column == 5);

            // Assert
            if (blockedSquare == null && captureSquare != null && captureSquare.CapturedPiece != null)
                Console.WriteLine("✓ PASS: Rook_CannotMovePastEnemyPiece");
            else
                Console.WriteLine("✗ FAIL: Rook_CannotMovePastEnemyPiece");
        }

        /// <summary>
        /// Test that a Rook moves only horizontally and vertically, not diagonally.
        /// </summary>
        public static void Test_Rook_OnlyMovesHorizontallyAndVertically()
        {
            // Arrange
            var board = CreateEmptyBoard();
            var rook = new Rook(3, 3, PieceColor.White);
            board.PlacePiece(rook, new Location(3, 3));

            // Act
            var moves = rook.GetMoves(new Location(3, 3), board);
            var diagonalMove = moves.FirstOrDefault(m =>
                m.To.Location.Row != 3 && m.To.Location.Column != 3);

            // Assert
            if (diagonalMove == null)
                Console.WriteLine("✓ PASS: Rook_OnlyMovesHorizontallyAndVertically");
            else
                Console.WriteLine("✗ FAIL: Rook_OnlyMovesHorizontallyAndVertically");
        }

        /// <summary>
        /// Runs all Rook tests.
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("\n═══ Running Rook Tests ═══");
            Test_Rook_Center_EmptyBoard_Has14Moves();
            Test_Rook_Corner_EmptyBoard_Has14Moves();
            Test_Rook_CanCaptureEnemyPiece();
            Test_Rook_CannotMovePastFriendlyPiece();
            Test_Rook_CannotMovePastEnemyPiece();
            Test_Rook_OnlyMovesHorizontallyAndVertically();
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
