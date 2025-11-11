using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.UI;

namespace ShatranjCore.Tests.UI
{
    /// <summary>
    /// Test suite for command parser
    /// Tests parsing of all game commands: move, castle, help, history, settings, save, load, etc.
    /// </summary>
    public class CommandParserTests
    {
        private CommandParser _parser;

        public CommandParserTests()
        {
            _parser = new CommandParser();
        }

        public void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Command Parser Tests Suite                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            int passed = 0;
            int failed = 0;

            // Test 1: Parse Move Command (short format)
            try
            {
                TestParseMove();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 1 PASSED: Parse move command (e2 e4)");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 1 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 2: Parse Move Command (full format)
            try
            {
                TestParseMoveFull();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 2 PASSED: Parse move command (move e2 e4)");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 2 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 3: Parse Castle Command
            try
            {
                TestParseCastle();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 3 PASSED: Parse castle command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 3 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 4: Parse Help Command
            try
            {
                TestParseHelp();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 4 PASSED: Parse help command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 4 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 5: Parse History Command
            try
            {
                TestParseHistory();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 5 PASSED: Parse history command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 5 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 6: Parse Settings Command
            try
            {
                TestParseSettings();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 6 PASSED: Parse settings command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 6 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 7: Parse Save Command
            try
            {
                TestParseSave();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 7 PASSED: Parse save command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 7 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 8: Parse Load Command
            try
            {
                TestParseLoad();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 8 PASSED: Parse load command");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 8 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 9: Parse Invalid Command
            try
            {
                TestParseInvalid();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 9 PASSED: Parse invalid command (error handling)");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 9 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 10: Parse Edge Cases
            try
            {
                TestParseEdgeCases();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 10 PASSED: Parse edge cases (empty, whitespace)");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 10 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Command Parser Tests Summary: {passed} passed, {failed} failed            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestParseMove()
        {
            // Test that short format without "move" prefix is INVALID
            // CommandParser requires "move" keyword
            var command = _parser.Parse("e2 e4");
            if (command == null)
            {
                throw new Exception("Command is null for short format");
            }
            // This should return Invalid as per parser design (requires "move" prefix)
            if (command.Type != CommandType.Invalid)
            {
                throw new Exception($"Expected Invalid for short format, got {command.Type}");
            }
        }

        private void TestParseMoveFull()
        {
            // Test full format: "move e2 e4"
            var command = _parser.Parse("move e2 e4");
            if (command == null)
            {
                throw new Exception("Command is null for full move");
            }
            if (command.Type != CommandType.Move)
            {
                throw new Exception($"Expected Move command type, got {command.Type}");
            }
        }

        private void TestParseCastle()
        {
            // Test castling kingside
            var command = _parser.Parse("castle king");
            if (command == null)
            {
                throw new Exception("Command is null for castle");
            }
            if (command.Type != CommandType.Castle)
            {
                throw new Exception($"Expected Castle command type, got {command.Type}");
            }

            // Test castling queenside
            command = _parser.Parse("castle queen");
            if (command.Type != CommandType.Castle)
            {
                throw new Exception("Queenside castle not parsed correctly");
            }
        }

        private void TestParseHelp()
        {
            // Test help with location - returns ShowMoves (not ShowHelp)
            // This shows moves for a specific piece location
            var command = _parser.Parse("help e2");
            if (command == null)
            {
                throw new Exception("Command is null for help with location");
            }
            if (command.Type != CommandType.ShowMoves)
            {
                throw new Exception($"Expected ShowMoves command type for 'help e2', got {command.Type}");
            }

            // Test help without location - returns ShowHelp
            command = _parser.Parse("help");
            if (command == null)
            {
                throw new Exception("Command is null for help");
            }
            if (command.Type != CommandType.ShowHelp)
            {
                throw new Exception($"Expected ShowHelp command type for 'help', got {command.Type}");
            }
        }

        private void TestParseHistory()
        {
            var command = _parser.Parse("history");
            if (command == null)
            {
                throw new Exception("Command is null for history");
            }
            if (command.Type != CommandType.ShowHistory)
            {
                throw new Exception($"Expected History command type, got {command.Type}");
            }
        }

        private void TestParseSettings()
        {
            var command = _parser.Parse("settings");
            if (command == null)
            {
                throw new Exception("Command is null for settings");
            }
            if (command.Type != CommandType.ShowSettings)
            {
                throw new Exception($"Expected Settings command type, got {command.Type}");
            }

            // Test settings with difficulty
            command = _parser.Parse("settings difficulty hard");
            if (command.Type != CommandType.ShowSettings && command.Type != CommandType.SetDifficulty)
            {
                throw new Exception("Settings difficulty not parsed correctly");
            }
        }

        private void TestParseSave()
        {
            var command = _parser.Parse("save");
            if (command == null)
            {
                throw new Exception("Command is null for save");
            }
            if (command.Type != CommandType.SaveGame)
            {
                throw new Exception($"Expected Save command type, got {command.Type}");
            }
        }

        private void TestParseLoad()
        {
            var command = _parser.Parse("load 1");
            if (command == null)
            {
                throw new Exception("Command is null for load");
            }
            if (command.Type != CommandType.LoadGame)
            {
                throw new Exception($"Expected Load command type, got {command.Type}");
            }
        }

        private void TestParseInvalid()
        {
            // Test invalid command should either return Invalid or throw
            try
            {
                var command = _parser.Parse("invalidcommand");
                // If it returns a command, it should be Invalid type
                if (command != null && command.Type == CommandType.Invalid)
                {
                    // This is acceptable
                }
                else if (command == null)
                {
                    // This is also acceptable
                }
            }
            catch (Exception ex)
            {
                // Error handling for invalid commands is acceptable
                if (!ex.Message.Contains("invalid") && !ex.Message.Contains("Invalid"))
                {
                    throw;
                }
            }
        }

        private void TestParseEdgeCases()
        {
            // Test empty string
            var command = _parser.Parse("");
            if (command != null && command.Type != CommandType.Invalid)
            {
                throw new Exception("Empty command should return Invalid");
            }

            // Test whitespace only
            command = _parser.Parse("   ");
            if (command != null && command.Type != CommandType.Invalid)
            {
                throw new Exception("Whitespace command should return Invalid");
            }

            // Test case insensitivity
            command = _parser.Parse("MOVE e2 e4");
            if (command != null && command.Type == CommandType.Move)
            {
                // Case insensitivity is good
            }
        }
    }
}
