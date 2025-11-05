using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Tests.PieceTests
{
    /// <summary>
    /// Unit tests for King piece movement.
    /// Tests one-square movement in all directions.
    /// </summary>
    public static class KingTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("│              King Movement Tests                         │");
            Console.WriteLine("└─────────────────────────────────────────────────────────┘");

            Test_King_Center_EmptyBoard_Has8Moves();
            Test_King_Corner_EmptyBoard_Has3Moves();
            Test_King_Edge_EmptyBoard_Has5Moves();
            Test_King_CanCaptureEnemyPiece();
            Test_King_CannotCaptureFriendlyPiece();
            Test_King_OnlyMovesOneSquare();
        }

        /// <summary>
        /// Test that a king in the center of an empty board has 8 possible moves (all adjacent squares).
        /// </summary>
        public static void Test_King_Center_EmptyBoard_Has8Moves()
        {
            Console.Write("  Testing king center position (8 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at d4 (row 4, col 3)
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            if (moves.Count == 8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 8 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a king in a corner has 3 possible moves.
        /// </summary>
        public static void Test_King_Corner_EmptyBoard_Has3Moves()
        {
            Console.Write("  Testing king corner position (3 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at a1 (row 7, col 0)
            King king = new King(7, 0, PieceColor.White);
            board.PlacePiece(king, new Location(7, 0));

            List<Move> moves = king.GetMoves(new Location(7, 0), board);

            if (moves.Count == 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 3 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a king on an edge has 5 possible moves.
        /// </summary>
        public static void Test_King_Edge_EmptyBoard_Has5Moves()
        {
            Console.Write("  Testing king edge position (5 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at e1 (row 7, col 4)
            King king = new King(7, 4, PieceColor.White);
            board.PlacePiece(king, new Location(7, 4));

            List<Move> moves = king.GetMoves(new Location(7, 4), board);

            if (moves.Count == 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 5 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a king can capture an enemy piece.
        /// </summary>
        public static void Test_King_CanCaptureEnemyPiece()
        {
            Console.Write("  Testing king can capture enemy piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at d4 (row 4, col 3)
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Place black pawn at d5 (row 3, col 3)
            Pawn enemyPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(3, 3));

            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Check if king can capture the pawn at d5
            bool canCapture = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3 && m.CapturedPiece != null);

            if (canCapture)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (King should be able to capture enemy pawn)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a king cannot capture a friendly piece.
        /// </summary>
        public static void Test_King_CannotCaptureFriendlyPiece()
        {
            Console.Write("  Testing king cannot capture friendly piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at d4 (row 4, col 3)
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            // Place white pawn at d5 (row 3, col 3) - friendly piece
            Pawn friendlyPawn = new Pawn(3, 3, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(friendlyPawn, new Location(3, 3));

            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Check that king cannot move to d5
            bool canMoveToPawn = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3);

            if (!canMoveToPawn)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (King should not move to friendly piece square)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a king only moves one square in any direction.
        /// </summary>
        public static void Test_King_OnlyMovesOneSquare()
        {
            Console.Write("  Testing king only moves one square... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white king at d4 (row 4, col 3)
            King king = new King(4, 3, PieceColor.White);
            board.PlacePiece(king, new Location(4, 3));

            List<Move> moves = king.GetMoves(new Location(4, 3), board);

            // Check that all moves are exactly one square away
            bool allOneSquare = true;
            foreach (Move move in moves)
            {
                int rowDiff = Math.Abs(move.To.Location.Row - 4);
                int colDiff = Math.Abs(move.To.Location.Column - 3);

                // King moves should be at most 1 square in any direction
                if (rowDiff > 1 || colDiff > 1)
                {
                    allOneSquare = false;
                    break;
                }
            }

            if (allOneSquare)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (King should only move one square)");
                Console.ResetColor();
            }
        }
    }
}
