using System;
using ShatranjCore.Tests.PieceTests;

namespace ShatranjCore.Tests
{
    /// <summary>
    /// Main test runner for Shatranj chess game tests.
    /// Run this to execute all unit tests.
    /// </summary>
    class TestRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Shatranj Chess - Test Suite                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            int totalTests = 0;
            int passedTests = 0;

            // Run initial game state test first
            InitialGameMoveTest.RunTest();
            Console.WriteLine();

            // Run all piece tests
            RookTests.RunAllTests();
            KnightTests.RunAllTests();
            BishopTests.RunAllTests();
            QueenTests.RunAllTests();
            KingTests.RunAllTests();
            PawnTests.RunAllTests();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Test Suite Complete - All Piece Movement Tests Run");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
