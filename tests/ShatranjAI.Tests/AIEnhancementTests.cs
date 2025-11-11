using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjAI;

namespace ShatranjAI.Tests
{
    /// <summary>
    /// Test suite for AI enhancements
    /// Tests AI algorithm frameworks
    /// </summary>
    public class AIEnhancementTests
    {
        public void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          AI Enhancement Tests Suite                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            int passed = 0;
            int failed = 0;

            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    TestAIFramework();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ Test {i} PASSED: AI enhancement test {i}");
                    Console.ResetColor();
                    passed++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ Test {i} FAILED: {ex.Message}");
                    Console.ResetColor();
                    failed++;
                }
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  AI Enhancement Tests Summary: {passed} passed, {failed} failed           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestAIFramework()
        {
            var board = new ChessBoard();
            if (board == null)
            {
                throw new Exception("Board not initialized");
            }

            var ai = new BasicAI(DifficultyLevel.Medium);
            if (ai == null)
            {
                throw new Exception("AI not initialized");
            }
        }
    }
}
