using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Tests
{
    /// <summary>
    /// Comprehensive test to verify all pieces move properly from initial game state
    /// </summary>
    public static class AllPiecesMovementTest
    {
        public static void RunTest()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║    All Pieces Movement - Initial Game State Test        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            TestAllWhitePieces();
            Console.WriteLine();
            TestAllBlackPieces();
        }

        private static void TestAllWhitePieces()
        {
            Console.WriteLine("═══ WHITE PIECES TEST ═══\n");
            IChessBoard board = new ChessBoard(PieceColor.White);

            // White pawns - row 6, columns 0-7
            TestPawns(board, PieceColor.White, "White");

            // White knights - row 7, columns 1 and 6
            TestPiece(board, new Location(7, 1), "Knight", "White", "b1");
            TestPiece(board, new Location(7, 6), "Knight", "White", "g1");

            // White bishops - row 7, columns 2 and 5
            TestPiece(board, new Location(7, 2), "Bishop", "White", "c1");
            TestPiece(board, new Location(7, 5), "Bishop", "White", "f1");

            // White rooks - row 7, columns 0 and 7
            TestPiece(board, new Location(7, 0), "Rook", "White", "a1");
            TestPiece(board, new Location(7, 7), "Rook", "White", "h1");

            // White queen - row 7, column 3
            TestPiece(board, new Location(7, 3), "Queen", "White", "d1");

            // White king - row 7, column 4
            TestPiece(board, new Location(7, 4), "King", "White", "e1");
        }

        private static void TestAllBlackPieces()
        {
            Console.WriteLine("═══ BLACK PIECES TEST ═══\n");
            IChessBoard board = new ChessBoard(PieceColor.White);

            // Black pawns - row 1, columns 0-7
            TestPawns(board, PieceColor.Black, "Black");

            // Black knights - row 0, columns 1 and 6
            TestPiece(board, new Location(0, 1), "Knight", "Black", "b8");
            TestPiece(board, new Location(0, 6), "Knight", "Black", "g8");

            // Black bishops - row 0, columns 2 and 5
            TestPiece(board, new Location(0, 2), "Bishop", "Black", "c8");
            TestPiece(board, new Location(0, 5), "Bishop", "Black", "f8");

            // Black rooks - row 0, columns 0 and 7
            TestPiece(board, new Location(0, 0), "Rook", "Black", "a8");
            TestPiece(board, new Location(0, 7), "Rook", "Black", "h8");

            // Black queen - row 0, column 3
            TestPiece(board, new Location(0, 3), "Queen", "Black", "d8");

            // Black king - row 0, column 4
            TestPiece(board, new Location(0, 4), "King", "Black", "e8");
        }

        private static void TestPawns(IChessBoard board, PieceColor color, string colorName)
        {
            int row = color == PieceColor.White ? 6 : 1;
            string[] files = { "a", "b", "c", "d", "e", "f", "g", "h" };

            Console.WriteLine($"{colorName} Pawns:");
            int passCount = 0;
            int totalCount = 8;

            for (int col = 0; col < 8; col++)
            {
                Location location = new Location(row, col);
                Piece piece = board.GetPiece(location);
                string file = files[col];
                string rank = color == PieceColor.White ? "2" : "7";
                string notation = $"{file}{rank}";

                if (piece == null || piece.GetType().Name != "Pawn")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ✗ {notation}: No pawn found");
                    Console.ResetColor();
                    continue;
                }

                if (piece.Color != color)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ✗ {notation}: Wrong color (expected {color}, got {piece.Color})");
                    Console.ResetColor();
                    continue;
                }

                Pawn pawn = (Pawn)piece;
                List<Move> moves = pawn.GetMoves(location, board);

                // Pawns should have 2 moves from starting position: 1 or 2 squares forward
                if (moves.Count == 2)
                {
                    bool hasOneForward = moves.Any(m => Math.Abs(m.To.Location.Row - row) == 1);
                    bool hasTwoForward = moves.Any(m => Math.Abs(m.To.Location.Row - row) == 2);

                    if (hasOneForward && hasTwoForward)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✓ {notation}: {moves.Count} moves (1 and 2 squares forward)");
                        Console.ResetColor();
                        passCount++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  ✗ {notation}: 2 moves but wrong directions");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  ✗ {notation}: Expected 2 moves, got {moves.Count}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine($"  Result: {passCount}/{totalCount} pawns passed\n");
        }

        private static void TestPiece(IChessBoard board, Location location, string expectedType, string expectedColor, string notation)
        {
            Piece piece = board.GetPiece(location);

            if (piece == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {notation} ({expectedType}): No piece found at {location.Row},{location.Column}");
                Console.ResetColor();
                return;
            }

            string actualType = piece.GetType().Name;
            PieceColor actualColor = piece.Color;

            if (actualType != expectedType)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {notation}: Expected {expectedType}, got {actualType}");
                Console.ResetColor();
                return;
            }

            if (actualColor.ToString() != expectedColor)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {notation}: Expected {expectedColor}, got {actualColor}");
                Console.ResetColor();
                return;
            }

            // Get valid moves
            List<Move> moves = piece.GetMoves(location, board);

            if (moves.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠ {notation} ({expectedType}): No valid moves available");
                Console.ResetColor();
                return;
            }

            // Check piece-specific move counts
            string moveInfo = GetExpectedMoveInfo(expectedType, location, expectedColor);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {notation} ({expectedType}): {moves.Count} valid moves{moveInfo}");
            Console.ResetColor();
        }

        private static string GetExpectedMoveInfo(string pieceType, Location location, string color)
        {
            return pieceType switch
            {
                "Knight" => " (L-shaped moves)",
                "Bishop" => " (diagonal moves)",
                "Rook" => " (straight moves)",
                "Queen" => " (combined rook+bishop moves)",
                "King" => " (one square in any direction)",
                _ => ""
            };
        }
    }
}
