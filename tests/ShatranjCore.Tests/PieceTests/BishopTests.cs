using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;

namespace ShatranjCore.Tests.PieceTests
{
    /// <summary>
    /// Unit tests for Bishop piece movement.
    /// Tests diagonal movement, capture logic, and blocking.
    /// </summary>
    public static class BishopTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("│              Bishop Movement Tests                      │");
            Console.WriteLine("└─────────────────────────────────────────────────────────┘");

            Test_Bishop_Center_EmptyBoard_Has13Moves();
            Test_Bishop_Corner_EmptyBoard_Has7Moves();
            Test_Bishop_CanCaptureEnemyPiece();
            Test_Bishop_CannotMovePastFriendlyPiece();
            Test_Bishop_CannotMovePastEnemyPiece();
            Test_Bishop_OnlyMovesDiagonally();
        }

        /// <summary>
        /// Test that a bishop in the center of an empty board has 13 possible moves (all diagonals).
        /// </summary>
        public static void Test_Bishop_Center_EmptyBoard_Has13Moves()
        {
            Console.Write("  Testing bishop center position (13 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at d4 (row 4, col 3)
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            if (moves.Count == 13)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 13 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a bishop in a corner has 7 possible moves.
        /// </summary>
        public static void Test_Bishop_Corner_EmptyBoard_Has7Moves()
        {
            Console.Write("  Testing bishop corner position (7 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at a1 (row 7, col 0)
            Bishop bishop = new Bishop(7, 0, PieceColor.White);
            board.PlacePiece(bishop, new Location(7, 0));

            List<Move> moves = bishop.GetMoves(new Location(7, 0), board);

            if (moves.Count == 7)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 7 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a bishop can capture an enemy piece.
        /// </summary>
        public static void Test_Bishop_CanCaptureEnemyPiece()
        {
            Console.Write("  Testing bishop can capture enemy piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at d4 (row 4, col 3)
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            // Place black pawn at f6 (row 2, col 5)
            Pawn enemyPawn = new Pawn(2, 5, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(2, 5));

            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Check if bishop can capture the pawn at f6
            bool canCapture = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 5 && m.CapturedPiece != null);

            if (canCapture)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Bishop should be able to capture enemy pawn)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a bishop cannot move past a friendly piece.
        /// </summary>
        public static void Test_Bishop_CannotMovePastFriendlyPiece()
        {
            Console.Write("  Testing bishop cannot move past friendly piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at d4 (row 4, col 3)
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            // Place white pawn at f6 (row 2, col 5) - friendly piece
            Pawn friendlyPawn = new Pawn(2, 5, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(friendlyPawn, new Location(2, 5));

            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Bishop should not be able to reach f6 or beyond (g7, h8)
            bool canReachF6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 5);
            bool canReachG7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 6);
            bool canReachH8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 7);

            if (!canReachF6 && !canReachG7 && !canReachH8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Bishop should not move past friendly piece)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a bishop cannot move past an enemy piece.
        /// </summary>
        public static void Test_Bishop_CannotMovePastEnemyPiece()
        {
            Console.Write("  Testing bishop cannot move past enemy piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at d4 (row 4, col 3)
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            // Place black pawn at f6 (row 2, col 5) - enemy piece
            Pawn enemyPawn = new Pawn(2, 5, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(2, 5));

            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Bishop should be able to capture f6 but not reach beyond (g7, h8)
            bool canReachF6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 5);
            bool canReachG7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 6);
            bool canReachH8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 7);

            if (canReachF6 && !canReachG7 && !canReachH8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Bishop should capture but not move past enemy)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a bishop only moves diagonally (not horizontally or vertically).
        /// </summary>
        public static void Test_Bishop_OnlyMovesDiagonally()
        {
            Console.Write("  Testing bishop only moves diagonally... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white bishop at d4 (row 4, col 3)
            Bishop bishop = new Bishop(4, 3, PieceColor.White);
            board.PlacePiece(bishop, new Location(4, 3));

            List<Move> moves = bishop.GetMoves(new Location(4, 3), board);

            // Check that all moves are diagonal
            bool allDiagonal = true;
            foreach (Move move in moves)
            {
                int rowDiff = Math.Abs(move.To.Location.Row - 4);
                int colDiff = Math.Abs(move.To.Location.Column - 3);

                // For diagonal moves, row difference should equal column difference
                if (rowDiff != colDiff)
                {
                    allDiagonal = false;
                    break;
                }
            }

            if (allDiagonal)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Bishop should only move diagonally)");
                Console.ResetColor();
            }
        }
    }
}
