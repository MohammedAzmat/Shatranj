using System;

namespace ShatranjIntegration.Tests
{
    /// <summary>
    /// Main test runner for Shatranj integration tests.
    /// Run this to execute all integration tests.
    /// </summary>
    class TestRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Shatranj Chess - Integration Test Suite              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Run AI integration tests
            Console.WriteLine("Running AI Integration tests...");
            AIIntegrationTests.RunAllTests();
            Console.WriteLine();

            // Run game flow tests
            Console.WriteLine("Running Game Flow tests...");
            GameFlowTests.RunAllTests();
            Console.WriteLine();

            // Run save/load tests
            Console.WriteLine("Running Save/Load tests...");
            SaveLoadTests.RunAllTests();
            Console.WriteLine();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Integration Test Suite Complete");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
