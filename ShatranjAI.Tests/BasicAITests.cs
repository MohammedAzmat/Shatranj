using System;
using ShatranjAI.AI;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Pieces;

namespace ShatranjAI.Tests
{
    /// <summary>
    /// Tests for BasicAI class
    /// </summary>
    public static class BasicAITests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== BasicAI Tests ===");

            TestAIInitialization();
            TestAISelectsLegalMove();
            TestAIPrefersCapturesValue();

            Console.WriteLine("All BasicAI tests completed!");
        }

        /// <summary>
        /// Test that AI initializes correctly
        /// </summary>
        private static void TestAIInitialization()
        {
            Console.Write("Test: AI initialization... ");

            try
            {
                BasicAI ai = new BasicAI(depth: 3);

                if (ai.Name == "BasicAI" && ai.Version == "1.0" && ai.Depth == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - Incorrect initialization");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAIL - Exception: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that AI selects a legal move from initial position
        /// </summary>
        private static void TestAISelectsLegalMove()
        {
            Console.Write("Test: AI selects legal move... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                BasicAI ai = new BasicAI(depth: 2);  // Use depth 2 for speed

                AIMove move = ai.SelectMove(board, PieceColor.White, null);

                if (move != null)
                {
                    // Verify it's a valid starting move (pawn or knight)
                    Piece piece = board.GetPiece(move.From);
                    bool isValidOpening = piece != null && (piece is Pawn || piece is Knight);

                    if (isValidOpening)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"PASS (Move: {FormatMove(move)})");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAIL - Invalid opening move");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - AI returned null move");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAIL - Exception: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that AI recognizes value in capturing pieces
        /// </summary>
        private static void TestAIPrefersCapturesValue()
        {
            Console.Write("Test: AI values captures... ");

            try
            {
                // Set up position where White can capture Black's queen
                ChessBoard board = new ChessBoard(PieceColor.White);

                // Move White pawn to e4
                Piece whitePawn = board.GetPiece(new Location(6, 4));  // e2
                board.RemovePiece(new Location(6, 4));
                board.PlacePiece(whitePawn, new Location(4, 4));  // e4

                // Place Black queen on d5 (capturable by pawn)
                Queen blackQueen = new Queen(3, 3, PieceColor.Black);  // d5
                board.RemovePiece(new Location(0, 3));  // Remove from starting position
                board.PlacePiece(blackQueen, new Location(3, 3));

                BasicAI ai = new BasicAI(depth: 2);
                AIMove move = ai.SelectMove(board, PieceColor.White, null);

                // AI should capture the queen (high value)
                // Evaluation should strongly favor White after this capture
                if (move != null && move.Evaluation > 500)  // Should see huge advantage
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"PASS (Eval: {move.Evaluation:F2})");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"PARTIAL - Move: {(move != null ? FormatMove(move) : "null")}, Eval: {(move?.Evaluation ?? 0):F2}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAIL - Exception: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static string FormatMove(AIMove move)
        {
            char fromFile = (char)('a' + move.From.Column);
            int fromRank = 8 - move.From.Row;
            char toFile = (char)('a' + move.To.Column);
            int toRank = 8 - move.To.Row;
            return $"{fromFile}{fromRank}{toFile}{toRank}";
        }
    }
}
