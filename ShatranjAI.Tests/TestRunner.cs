using System;

namespace ShatranjAI.Tests
{
    /// <summary>
    /// Main test runner for Shatranj AI tests.
    /// Run this to execute all AI unit tests.
    /// </summary>
    class TestRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Shatranj AI - Test Suite                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            int totalTests = 0;
            int passedTests = 0;

            // Run Move Evaluator tests
            Console.WriteLine("Running MoveEvaluator tests...");
            MoveEvaluatorTests.RunAllTests();
            Console.WriteLine();

            // Run BasicAI tests
            Console.WriteLine("Running BasicAI tests...");
            BasicAITests.RunAllTests();
            Console.WriteLine();

            // Run AI Enhancement tests
            Console.WriteLine("Running AI Enhancement tests...");
            var enhancementTests = new AIEnhancementTests();
            enhancementTests.RunAllTests();
            Console.WriteLine();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"AI Test Suite Complete");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
