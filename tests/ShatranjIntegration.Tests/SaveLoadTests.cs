using System;
using System.IO;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore.Persistence;
using ShatranjCore.Logging;

namespace ShatranjIntegration.Tests
{
    /// <summary>
    /// Integration tests for Save/Load functionality including SaveType feature.
    /// Tests scenarios: Manual saves, Auto saves, Save slot limits, SaveType metadata.
    /// </summary>
    public class SaveLoadTests
    {
        private static string testSaveDirectory;
        private static SaveGameManager saveManager;
        private static ConsoleLogger logger;

        /// <summary>
        /// Runs all save/load tests
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Save/Load & SaveType Tests                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // Setup test environment
            SetupTestEnvironment();

            // Run tests
            Test_SaveGame_SetsManualSaveType();
            Test_SaveAutosave_SetsAutoSaveType();
            Test_Autosave_OverwritesSingleFile();
            Test_EnforceMaxSaveSlots_Maintains10Limit();
            Test_Autosave_NotCountedInSlotLimit();
            Test_ListSavedGames_IncludesSaveType();
            Test_LoadGame_PreservesSaveType();
            Test_GetAutosaveMetadata_ReturnsAutoSaveType();

            // Cleanup
            CleanupTestEnvironment();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("Save/Load Tests Complete");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
        }

        /// <summary>
        /// Test that SaveGame sets SaveType to "Manual"
        /// </summary>
        public static void Test_SaveGame_SetsManualSaveType()
        {
            try
            {
                // Arrange
                var snapshot = CreateTestSnapshot(1);

                // Act
                saveManager.SaveGame(snapshot, 1);

                // Assert
                var loaded = saveManager.LoadGame(1);
                if (loaded.SaveType == SaveType.Manual.ToString())
                    Console.WriteLine("✓ PASS: SaveGame sets SaveType to Manual");
                else
                    Console.WriteLine($"✗ FAIL: SaveGame SaveType incorrect (Expected 'Manual', got '{loaded.SaveType}')");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: SaveGame_SetsManualSaveType threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that SaveAutosave sets SaveType to "Auto"
        /// </summary>
        public static void Test_SaveAutosave_SetsAutoSaveType()
        {
            try
            {
                // Arrange
                var snapshot = CreateTestSnapshot(999);

                // Act
                saveManager.SaveAutosave(snapshot);

                // Assert
                var loaded = saveManager.LoadAutosave();
                if (loaded.SaveType == SaveType.Auto.ToString())
                    Console.WriteLine("✓ PASS: SaveAutosave sets SaveType to Auto");
                else
                    Console.WriteLine($"✗ FAIL: SaveAutosave SaveType incorrect (Expected 'Auto', got '{loaded.SaveType}')");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: SaveAutosave_SetsAutoSaveType threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that autosave overwrites the same file (single autosave.json)
        /// </summary>
        public static void Test_Autosave_OverwritesSingleFile()
        {
            try
            {
                // Arrange
                var snapshot1 = CreateTestSnapshot(998);
                snapshot1.MoveCount = 10;
                var snapshot2 = CreateTestSnapshot(998);
                snapshot2.MoveCount = 20;

                // Act
                saveManager.SaveAutosave(snapshot1);
                var autosaveFiles1 = Directory.GetFiles(testSaveDirectory, "autosave.json");

                saveManager.SaveAutosave(snapshot2);
                var autosaveFiles2 = Directory.GetFiles(testSaveDirectory, "autosave.json");

                // Assert
                bool singleFile = autosaveFiles1.Length == 1 && autosaveFiles2.Length == 1;
                var loaded = saveManager.LoadAutosave();
                bool overwritten = loaded.MoveCount == 20;

                if (singleFile && overwritten)
                    Console.WriteLine("✓ PASS: Autosave overwrites single file");
                else
                    Console.WriteLine($"✗ FAIL: Autosave file management incorrect (Files: {autosaveFiles2.Length}, MoveCount: {loaded.MoveCount})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: Autosave_OverwritesSingleFile threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that SaveGameManager enforces max 10 save slots
        /// </summary>
        public static void Test_EnforceMaxSaveSlots_Maintains10Limit()
        {
            try
            {
                // Arrange - Create 12 save files
                for (int i = 1; i <= 12; i++)
                {
                    var snapshot = CreateTestSnapshot(i);
                    saveManager.SaveGame(snapshot, i);
                    System.Threading.Thread.Sleep(10); // Ensure different timestamps
                }

                // Act
                var savedGames = saveManager.ListSavedGames();
                var gameFiles = Directory.GetFiles(testSaveDirectory, "game_*.json");

                // Assert
                if (savedGames.Count == 10 && gameFiles.Length == 10)
                    Console.WriteLine("✓ PASS: EnforceMaxSaveSlots maintains 10 slot limit");
                else
                    Console.WriteLine($"✗ FAIL: Save slot limit not enforced (Metadata: {savedGames.Count}, Files: {gameFiles.Length})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: EnforceMaxSaveSlots_Maintains10Limit threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that autosave is not counted toward the 10-slot limit
        /// </summary>
        public static void Test_Autosave_NotCountedInSlotLimit()
        {
            try
            {
                // Arrange
                CleanupTestEnvironment();
                SetupTestEnvironment();

                // Create autosave
                var autosaveSnapshot = CreateTestSnapshot(999);
                saveManager.SaveAutosave(autosaveSnapshot);

                // Create 10 manual saves
                for (int i = 1; i <= 10; i++)
                {
                    var snapshot = CreateTestSnapshot(i);
                    saveManager.SaveGame(snapshot, i);
                }

                // Act
                var gameFiles = Directory.GetFiles(testSaveDirectory, "game_*.json");
                var autosaveFiles = Directory.GetFiles(testSaveDirectory, "autosave.json");
                bool autosaveExists = saveManager.AutosaveExists();

                // Assert
                if (gameFiles.Length == 10 && autosaveFiles.Length == 1 && autosaveExists)
                    Console.WriteLine("✓ PASS: Autosave not counted in slot limit");
                else
                    Console.WriteLine($"✗ FAIL: Autosave slot counting incorrect (Games: {gameFiles.Length}, Autosave exists: {autosaveExists})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: Autosave_NotCountedInSlotLimit threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that ListSavedGames includes SaveType in metadata
        /// </summary>
        public static void Test_ListSavedGames_IncludesSaveType()
        {
            try
            {
                // Arrange
                CleanupTestEnvironment();
                SetupTestEnvironment();

                var snapshot1 = CreateTestSnapshot(1);
                var snapshot2 = CreateTestSnapshot(2);
                saveManager.SaveGame(snapshot1, 1);
                saveManager.SaveGame(snapshot2, 2);

                // Act
                var savedGames = saveManager.ListSavedGames();

                // Assert
                bool allHaveSaveType = savedGames.All(g => !string.IsNullOrEmpty(g.SaveType));
                bool allAreManual = savedGames.All(g => g.SaveType == SaveType.Manual.ToString());

                if (allHaveSaveType && allAreManual)
                    Console.WriteLine("✓ PASS: ListSavedGames includes SaveType metadata");
                else
                    Console.WriteLine($"✗ FAIL: SaveType metadata incorrect in list (All have: {allHaveSaveType}, All manual: {allAreManual})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: ListSavedGames_IncludesSaveType threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that LoadGame preserves SaveType
        /// </summary>
        public static void Test_LoadGame_PreservesSaveType()
        {
            try
            {
                // Arrange
                var snapshot = CreateTestSnapshot(5);
                saveManager.SaveGame(snapshot, 5);

                // Act
                var loaded = saveManager.LoadGame(5);

                // Assert
                if (loaded.SaveType == SaveType.Manual.ToString())
                    Console.WriteLine("✓ PASS: LoadGame preserves SaveType");
                else
                    Console.WriteLine($"✗ FAIL: LoadGame SaveType not preserved (Expected 'Manual', got '{loaded.SaveType}')");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: LoadGame_PreservesSaveType threw exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Test that GetAutosaveMetadata returns Auto SaveType
        /// </summary>
        public static void Test_GetAutosaveMetadata_ReturnsAutoSaveType()
        {
            try
            {
                // Arrange
                var snapshot = CreateTestSnapshot(999);
                saveManager.SaveAutosave(snapshot);

                // Act
                var metadata = saveManager.GetAutosaveMetadata();

                // Assert
                if (metadata != null && metadata.SaveType == SaveType.Auto.ToString())
                    Console.WriteLine("✓ PASS: GetAutosaveMetadata returns Auto SaveType");
                else
                    Console.WriteLine($"✗ FAIL: Autosave metadata incorrect (Null: {metadata == null}, SaveType: {metadata?.SaveType})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: GetAutosaveMetadata_ReturnsAutoSaveType threw exception: {ex.Message}");
            }
        }

        #region Helper Methods

        private static void SetupTestEnvironment()
        {
            testSaveDirectory = Path.Combine(Path.GetTempPath(), "Shatranj_Tests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(testSaveDirectory);
            logger = new ConsoleLogger();
            saveManager = new SaveGameManager(logger, testSaveDirectory);
        }

        private static void CleanupTestEnvironment()
        {
            try
            {
                if (Directory.Exists(testSaveDirectory))
                {
                    Directory.Delete(testSaveDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        private static GameStateSnapshot CreateTestSnapshot(int gameId)
        {
            return new GameStateSnapshot
            {
                GameId = gameId,
                SavedAt = DateTime.Now,
                GameMode = "HumanVsAI",
                CurrentPlayer = "White",
                HumanColor = "White",
                WhitePlayerType = "Human",
                BlackPlayerType = "AI",
                WhitePlayerName = "TestPlayer",
                BlackPlayerName = "AI",
                GameResult = "InProgress",
                MoveCount = 1,
                Difficulty = "Medium"
            };
        }

        #endregion
    }
}
