using System;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Pieces;
using ShatranjCore.Validators;

namespace ShatranjIntegration.Tests
{
    /// <summary>
    /// Integration tests for complete game flow
    /// </summary>
    public static class GameFlowTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Game Flow Tests ===");

            TestCheckDetection();
            TestCastlingIntegration();
            TestEnPassantIntegration();

            Console.WriteLine("All game flow tests completed!");
        }

        /// <summary>
        /// Test check detection in a real game scenario
        /// </summary>
        private static void TestCheckDetection()
        {
            Console.Write("Test: Check detection integration... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                CheckDetector checkDetector = new CheckDetector();

                // Initial position should not be in check
                bool initialCheck = checkDetector.IsKingInCheck(board, PieceColor.White);

                if (!initialCheck)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - Initial position in check");
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
        /// Test castling rights tracking through game
        /// </summary>
        private static void TestCastlingIntegration()
        {
            Console.Write("Test: Castling integration... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                CastlingValidator castlingValidator = new CastlingValidator();

                // Initially, pieces haven't moved
                bool whiteKingMoved = castlingValidator.HasKingMoved(PieceColor.White);
                bool whiteKingsideRookMoved = castlingValidator.HasKingsideRookMoved(PieceColor.White);

                if (!whiteKingMoved && !whiteKingsideRookMoved)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - Incorrect initial castling state");
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
        /// Test en passant integration
        /// </summary>
        private static void TestEnPassantIntegration()
        {
            Console.Write("Test: En passant integration... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                EnPassantTracker tracker = new EnPassantTracker();

                // Move white pawn two squares
                Piece whitePawn = board.GetPiece(new Location(6, 4));  // e2
                board.RemovePiece(new Location(6, 4));
                board.PlacePiece(whitePawn, new Location(4, 4));  // e4

                // Set en passant target
                tracker.SetEnPassantTarget(new Location(5, 4));  // e3

                Location? target = tracker.GetEnPassantTarget();

                if (target.HasValue && target.Value.Row == 5 && target.Value.Column == 4)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - En passant target incorrect");
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
    }
}
