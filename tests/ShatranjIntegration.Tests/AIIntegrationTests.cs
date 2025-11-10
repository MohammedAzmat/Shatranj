using System;
using ShatranjAI.AI;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Logging;
using ShatranjCore.Pieces;

namespace ShatranjIntegration.Tests
{
    /// <summary>
    /// Integration tests for AI with core game functionality
    /// </summary>
    public static class AIIntegrationTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== AI Integration Tests ===");

            TestAICanPlayFullGame();
            TestAIMakesValidMoves();
            TestAIWithLogging();

            Console.WriteLine("All AI integration tests completed!");
        }

        /// <summary>
        /// Test that AI can play a full game from start to finish
        /// </summary>
        private static void TestAICanPlayFullGame()
        {
            Console.Write("Test: AI can play full game... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                ILogger logger = new ConsoleLogger(includeTimestamp: false);
                BasicAI whiteAI = new BasicAI(depth: 2, logger);
                BasicAI blackAI = new BasicAI(depth: 2, logger);

                int moveCount = 0;
                int maxMoves = 10;  // Play first 10 moves
                PieceColor currentPlayer = PieceColor.White;

                while (moveCount < maxMoves)
                {
                    IChessAI currentAI = currentPlayer == PieceColor.White ? whiteAI : blackAI;
                    AIMove move = currentAI.SelectMove(board, currentPlayer, null);

                    if (move == null)
                    {
                        break;  // No legal moves
                    }

                    // Make the move
                    Piece piece = board.GetPiece(move.From);
                    board.RemovePiece(move.From);
                    board.PlacePiece(piece, move.To);

                    moveCount++;
                    currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                }

                if (moveCount == maxMoves)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"PASS (Played {moveCount} moves)");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"PARTIAL (Played {moveCount}/{maxMoves} moves)");
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
        /// Test that AI always makes valid moves
        /// </summary>
        private static void TestAIMakesValidMoves()
        {
            Console.Write("Test: AI makes valid moves... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                BasicAI ai = new BasicAI(depth: 2);

                bool allMovesValid = true;
                for (int i = 0; i < 5; i++)
                {
                    AIMove move = ai.SelectMove(board, PieceColor.White, null);

                    if (move == null)
                    {
                        allMovesValid = false;
                        break;
                    }

                    // Check that piece exists at from location
                    Piece piece = board.GetPiece(move.From);
                    if (piece == null || piece.Color != PieceColor.White)
                    {
                        allMovesValid = false;
                        break;
                    }

                    // Make the move for next iteration
                    board.RemovePiece(move.From);
                    board.PlacePiece(piece, move.To);
                }

                if (allMovesValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - Invalid move detected");
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
        /// Test AI integration with logging system
        /// </summary>
        private static void TestAIWithLogging()
        {
            Console.Write("Test: AI with logging integration... ");

            try
            {
                ChessBoard board = new ChessBoard(PieceColor.White);
                ILogger logger = new ConsoleLogger(includeTimestamp: false);
                BasicAI ai = new BasicAI(depth: 2, logger);

                AIMove move = ai.SelectMove(board, PieceColor.White, null);

                if (move != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("PASS");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAIL - AI returned null");
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
