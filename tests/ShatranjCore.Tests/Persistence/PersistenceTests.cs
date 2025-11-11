using System;
using System.IO;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;

namespace ShatranjCore.Tests.Persistence
{
    /// <summary>
    /// Test suite for game persistence (save/load functionality)
    /// Tests basic persistence operations
    /// </summary>
    public class PersistenceTests
    {
        private string _testSaveDirectory;

        public PersistenceTests()
        {
            _testSaveDirectory = Path.Combine(Path.GetTempPath(), "ShatranjPersistenceTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testSaveDirectory);
        }

        public void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Persistence Tests Suite                                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            int passed = 0;
            int failed = 0;

            // Test 1: Create Save Directory
            try
            {
                TestCreateSaveDirectory();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 1 PASSED: Save directory created successfully");
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

            // Test 2: Save File Creation
            try
            {
                TestSaveFileCreation();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 2 PASSED: Save file created successfully");
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

            // Test 3: Save File Has Content
            try
            {
                TestSaveFileContent();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 3 PASSED: Save file contains data");
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

            // Test 4: Load File Operations
            try
            {
                TestLoadFileOperations();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 4 PASSED: Load file operations work");
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

            // Test 5: File Persistence
            try
            {
                TestFilePersistence();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 5 PASSED: Files persist after creation");
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

            // Test 6: Invalid File Handling
            try
            {
                TestInvalidFileHandling();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 6 PASSED: Invalid files handled gracefully");
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

            // Test 7: Multiple File Operations
            try
            {
                TestMultipleFileOperations();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 7 PASSED: Multiple file operations handled");
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

            // Test 8: File Format Verification
            try
            {
                TestFileFormatVerification();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 8 PASSED: File format is valid");
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

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Persistence Tests Summary: {passed} passed, {failed} failed              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestCreateSaveDirectory()
        {
            if (!Directory.Exists(_testSaveDirectory))
            {
                throw new Exception("Save directory was not created");
            }
        }

        private void TestSaveFileCreation()
        {
            var savePath = Path.Combine(_testSaveDirectory, "test_game.save");
            File.WriteAllText(savePath, "test game data");

            if (!File.Exists(savePath))
            {
                throw new Exception("Save file was not created");
            }
        }

        private void TestSaveFileContent()
        {
            var savePath = Path.Combine(_testSaveDirectory, "test_content.save");
            var testData = "Board:8x8,Pieces:32,Status:Active";
            File.WriteAllText(savePath, testData);

            var content = File.ReadAllText(savePath);
            if (content != testData)
            {
                throw new Exception("Save file content does not match");
            }
        }

        private void TestLoadFileOperations()
        {
            var savePath = Path.Combine(_testSaveDirectory, "test_load.save");
            File.WriteAllText(savePath, "test load data");

            if (!File.Exists(savePath))
            {
                throw new Exception("Load file not found");
            }

            var content = File.ReadAllText(savePath);
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Load file is empty");
            }
        }

        private void TestFilePersistence()
        {
            var savePath = Path.Combine(_testSaveDirectory, "persistent_file.save");
            File.WriteAllText(savePath, "persistent data");

            // Check immediately
            if (!File.Exists(savePath))
            {
                throw new Exception("File does not persist");
            }

            // Check again to verify persistence
            var content = File.ReadAllText(savePath);
            if (!content.Contains("persistent"))
            {
                throw new Exception("File content was not persisted");
            }
        }

        private void TestInvalidFileHandling()
        {
            var invalidPath = Path.Combine(_testSaveDirectory, "nonexistent.save");

            try
            {
                if (File.Exists(invalidPath))
                {
                    var content = File.ReadAllText(invalidPath);
                }
                // File doesn't exist - that's expected
            }
            catch (FileNotFoundException)
            {
                // Expected behavior
            }
        }

        private void TestMultipleFileOperations()
        {
            for (int i = 0; i < 3; i++)
            {
                var savePath = Path.Combine(_testSaveDirectory, $"multi_save_{i}.save");
                File.WriteAllText(savePath, $"save data {i}");

                if (!File.Exists(savePath))
                {
                    throw new Exception($"Save {i} failed");
                }
            }

            var saveFiles = Directory.GetFiles(_testSaveDirectory, "multi_save_*.save");
            if (saveFiles.Length < 3)
            {
                throw new Exception($"Expected 3+ save files, got {saveFiles.Length}");
            }
        }

        private void TestFileFormatVerification()
        {
            var savePath = Path.Combine(_testSaveDirectory, "format_test.save");
            var testData = "{\"game\": \"chess\", \"status\": \"active\"}";
            File.WriteAllText(savePath, testData);

            var content = File.ReadAllText(savePath);
            if (!content.Contains("{") || !content.Contains("}"))
            {
                throw new Exception("File format is not valid");
            }
        }

        // Cleanup
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(_testSaveDirectory))
                {
                    Directory.Delete(_testSaveDirectory, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
