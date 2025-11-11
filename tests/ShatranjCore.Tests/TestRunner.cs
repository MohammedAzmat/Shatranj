using System;
using ShatranjCore.Tests.PieceTests;
using ShatranjCore.Tests.Logging;
using ShatranjCore.Tests.UI;
using ShatranjCore.Tests.Movement;

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

            // === PHASE 0: CRITICAL INFRASTRUCTURE TESTS ===
            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("PHASE 0: CRITICAL INFRASTRUCTURE TESTS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            // Run logging tests
            var loggingTests = new LoggingTests();
            loggingTests.RunAllTests();
            loggingTests.Cleanup();

            // Run command parser tests
            var parserTests = new CommandParserTests();
            parserTests.RunAllTests();

            // Run move history tests
            var historyTests = new MoveHistoryTests();
            historyTests.RunAllTests();

            // === PHASE 1: PIECE MOVEMENT TESTS ===
            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("PHASE 1: PIECE MOVEMENT TESTS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            // Run comprehensive all pieces movement test first
            AllPiecesMovementTest.RunTest();
            Console.WriteLine();

            // Run initial game state test
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
            Console.WriteLine($"Test Suite Complete - All Tests Run");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
