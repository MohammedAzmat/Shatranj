using System;
using ShatranjAI.AI;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Pieces;

namespace ShatranjAI.Tests
{
    /// <summary>
    /// Tests for MoveEvaluator class
    /// </summary>
    public static class MoveEvaluatorTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== MoveEvaluator Tests ===");

            TestInitialPositionEvaluation();
            TestMaterialAdvantage();
            TestPieceValues();

            Console.WriteLine("All MoveEvaluator tests completed!");
        }

        /// <summary>
        /// Test that initial position evaluates to approximately zero
        /// </summary>
        private static void TestInitialPositionEvaluation()
        {
            Console.Write("Test: Initial position evaluation... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                MoveEvaluator evaluator = new MoveEvaluator();

                double eval = evaluator.Evaluate(board, PieceColor.White);

                // Initial position should be close to 0 (balanced)
                if (Math.Abs(eval) < 200)  // Allow small positional differences
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"PASS (Eval: {eval})");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL - Expected near 0, got {eval}");
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
        /// Test that material advantage is correctly evaluated
        /// </summary>
        private static void TestMaterialAdvantage()
        {
            Console.Write("Test: Material advantage detection... ");

            try
            {
                // Create a board with White having an extra queen
                ChessBoard board = new ChessBoard(PieceColor.White);
                MoveEvaluator evaluator = new MoveEvaluator();

                // Remove black queen
                board.RemovePiece(new Location(0, 3));

                double eval = evaluator.Evaluate(board, PieceColor.White);

                // White should have ~900 centipawn advantage
                if (eval > 700 && eval < 1100)  // Queen value ~900 +/- positional
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"PASS (Eval: {eval})");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL - Expected 700-1100, got {eval}");
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
        /// Test that piece values are correctly retrieved
        /// </summary>
        private static void TestPieceValues()
        {
            Console.Write("Test: Piece value constants... ");

            try
            {
                bool allCorrect = true;

                if (MoveEvaluator.GetPieceValueByType("Pawn") != 100) allCorrect = false;
                if (MoveEvaluator.GetPieceValueByType("Knight") != 320) allCorrect = false;
                if (MoveEvaluator.GetPieceValueByType("Bishop") != 330) allCorrect = false;
                if (MoveEvaluator.GetPieceValueByType("Rook") != 500) allCorrect = false;
                if (MoveEvaluator.GetPieceValueByType("Queen") != 900) allCorrect = false;
                if (MoveEvaluator.GetPieceValueByType("King") != 20000) allCorrect = false;

                if (allCorrect)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - Incorrect piece values");
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
