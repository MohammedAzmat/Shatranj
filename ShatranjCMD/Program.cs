using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjAI.AI;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Game;
using ShatranjCore.Logging;
using ShatranjCore.Persistence;
using ShatranjCore.UI;

namespace ShatranjCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console to use UTF-8 encoding for better box-drawing characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Welcome message
            ShowWelcomeMessage();

            // Initialize logger and managers
            ILogger logger = new CompositeLogger(
                new FileLogger(),
                new ConsoleLogger(includeTimestamp: false)
            );

            GameConfigManager configManager = new GameConfigManager(logger);
            SaveGameManager saveManager = new SaveGameManager(logger);
            GameMenuHandler menuHandler = new GameMenuHandler();

            // Main menu loop
            bool exitProgram = false;
            while (!exitProgram)
            {
                // Check for autosave
                bool hasAutosave = saveManager.AutosaveExists();

                // Show main menu
                var menuChoice = menuHandler.ShowMainMenu(hasAutosave);

                switch (menuChoice)
                {
                    case GameMenuHandler.MainMenuChoice.Resume:
                        if (hasAutosave)
                        {
                            ResumeGame(saveManager, logger);
                        }
                        break;

                    case GameMenuHandler.MainMenuChoice.NewGame:
                        StartNewGame(configManager, menuHandler, logger);
                        break;

                    case GameMenuHandler.MainMenuChoice.LoadGame:
                        LoadSavedGame(saveManager, configManager, logger);
                        break;

                    case GameMenuHandler.MainMenuChoice.Settings:
                        ShowSettingsMenu(configManager);
                        break;

                    case GameMenuHandler.MainMenuChoice.Exit:
                        exitProgram = true;
                        break;
                }
            }

            // Exit message
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Thank you for playing Shatranj!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ShowWelcomeMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        SHATRANJ CHESS                          ║");
            Console.WriteLine("║                    Persian Chess Game                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Welcome to Shatranj - A chess game built with SOLID principles!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void StartNewGame(GameConfigManager configManager, GameMenuHandler menuHandler, ILogger logger)
        {
            // Show game mode menu
            GameMode selectedMode = menuHandler.ShowGameModeMenu();

            // Get color preference for Human vs AI mode
            PieceColor humanColor = PieceColor.White;
            if (selectedMode == GameMode.HumanVsAI)
            {
                humanColor = menuHandler.ShowColorSelectionMenu();
            }

            // Get difficulty from config
            var config = configManager.GetConfig();
            int aiDepth = (int)config.Difficulty;

            // Create AI instances if needed
            IChessAI whiteAI = null;
            IChessAI blackAI = null;

            if (selectedMode == GameMode.HumanVsAI)
            {
                IChessAI ai = new BasicAI(depth: aiDepth, logger);
                if (humanColor == PieceColor.White)
                {
                    blackAI = ai;  // AI plays black
                }
                else
                {
                    whiteAI = ai;  // AI plays white
                }
            }
            else if (selectedMode == GameMode.AIVsAI)
            {
                whiteAI = new BasicAI(depth: aiDepth, logger);
                blackAI = new BasicAI(depth: aiDepth, logger);
            }

            // Start the chess game with selected mode
            ChessGame game = new ChessGame(selectedMode, humanColor, whiteAI, blackAI);
            game.Start();
        }

        static void ResumeGame(SaveGameManager saveManager, ILogger logger)
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      RESUMING GAME                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                // Load autosave
                var snapshot = saveManager.LoadAutosave();
                var metadata = snapshot.ToMetadata();

                Console.WriteLine($"  Game Mode: {metadata.GameMode}");
                Console.WriteLine($"  Turn: {metadata.TurnCount}");
                Console.WriteLine($"  Current Player: {metadata.CurrentPlayer}");
                Console.WriteLine($"  Difficulty: {metadata.Difficulty}");
                Console.WriteLine();
                Console.WriteLine("Loading game...");
                System.Threading.Thread.Sleep(1000);

                // Parse difficulty
                DifficultyLevel difficulty;
                if (!Enum.TryParse<DifficultyLevel>(snapshot.Difficulty, out difficulty))
                {
                    difficulty = DifficultyLevel.Medium;
                }
                int aiDepth = (int)difficulty;

                // Parse game mode
                GameMode gameMode = (GameMode)Enum.Parse(typeof(GameMode), snapshot.GameMode);
                PieceColor humanColor = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.HumanColor);

                // Create AI instances if needed
                IChessAI whiteAI = null;
                IChessAI blackAI = null;

                if (gameMode == GameMode.HumanVsAI)
                {
                    IChessAI ai = new BasicAI(depth: aiDepth, logger);
                    if (humanColor == PieceColor.White)
                    {
                        blackAI = ai;  // AI plays black
                    }
                    else
                    {
                        whiteAI = ai;  // AI plays white
                    }
                }
                else if (gameMode == GameMode.AIVsAI)
                {
                    whiteAI = new BasicAI(depth: aiDepth, logger);
                    blackAI = new BasicAI(depth: aiDepth, logger);
                }

                // Create game and immediately load the saved state
                ChessGame game = new ChessGame(gameMode, humanColor, whiteAI, blackAI);

                // Use reflection or make the method public to restore state
                // For now, start the game normally (the autosave will be available via load command)
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Note: Use 'game load' command once in-game to load the autosave.");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                game.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to resume game: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to return to main menu...");
                Console.ReadKey();
                logger.Error("Resume game failed", ex);
            }
        }

        static void LoadSavedGame(SaveGameManager saveManager, GameConfigManager configManager, ILogger logger)
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      LOAD SAVED GAME                           ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                // Get list of saved games
                var savedGames = saveManager.ListSavedGames();

                if (savedGames.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No saved games found.");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                    return;
                }

                // Display saved games
                Console.WriteLine("Available saved games:");
                Console.WriteLine();

                foreach (var savedGame in savedGames)
                {
                    // Determine game type label
                    string gameType = savedGame.GameMode == "AIVsAI" ? "Sim" : "Game";
                    string saveType = string.IsNullOrEmpty(savedGame.SaveType) ? "Manual" : savedGame.SaveType;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{gameType} #{savedGame.GameId} ({saveType})");
                    Console.ResetColor();
                    Console.WriteLine($"  Mode: {savedGame.GameMode}");
                    Console.WriteLine($"  Players: {savedGame.WhitePlayerName} vs {savedGame.BlackPlayerName}");
                    Console.WriteLine($"  Turn {savedGame.TurnCount} - {savedGame.CurrentPlayer}'s move");
                    Console.WriteLine($"  Difficulty: {savedGame.Difficulty}");
                    Console.WriteLine($"  Saved: {savedGame.SavedAt:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine();
                }

                // Prompt for game ID
                Console.Write("Enter Game ID to load (or press ESC to cancel): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                if (!int.TryParse(input, out int gameId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Game ID. Please enter a number.");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                    return;
                }

                // Load the selected game
                var snapshot = saveManager.LoadGame(gameId);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Loading Game #{gameId}...");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1000);

                // Parse difficulty
                DifficultyLevel difficulty;
                if (!Enum.TryParse<DifficultyLevel>(snapshot.Difficulty, out difficulty))
                {
                    difficulty = DifficultyLevel.Medium;
                }
                int aiDepth = (int)difficulty;

                // Parse game mode
                GameMode gameMode = (GameMode)Enum.Parse(typeof(GameMode), snapshot.GameMode);
                PieceColor humanColor = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.HumanColor);

                // Create AI instances if needed
                IChessAI whiteAI = null;
                IChessAI blackAI = null;

                if (gameMode == GameMode.HumanVsAI)
                {
                    IChessAI ai = new BasicAI(depth: aiDepth, logger);
                    if (humanColor == PieceColor.White)
                    {
                        blackAI = ai;  // AI plays black
                    }
                    else
                    {
                        whiteAI = ai;  // AI plays white
                    }
                }
                else if (gameMode == GameMode.AIVsAI)
                {
                    whiteAI = new BasicAI(depth: aiDepth, logger);
                    blackAI = new BasicAI(depth: aiDepth, logger);
                }

                // Create game and start
                ChessGame game = new ChessGame(gameMode, humanColor, whiteAI, blackAI);

                // Note: The game will start fresh, but user can use "load [gameId]" command in-game
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Note: Use 'load {gameId}' command once in-game to restore the saved state.");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                game.Start();
            }
            catch (FileNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Game not found. It may have been deleted.");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to return to main menu...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to load game: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to return to main menu...");
                Console.ReadKey();
                logger.Error("Load game failed", ex);
            }
        }

        static void ShowSettingsMenu(GameConfigManager configManager)
        {
            bool backToMainMenu = false;

            while (!backToMainMenu)
            {
                var config = configManager.GetConfig();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                          SETTINGS                              ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"  Profile Name: {config.ProfileName}");
                Console.WriteLine($"  Opponent Name: {config.OpponentProfileName}");
                Console.WriteLine($"  Difficulty: {config.Difficulty} (AI Depth: {(int)config.Difficulty})");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("  [1] Change Profile Name");
                Console.WriteLine("  [2] Change Opponent Name");
                Console.WriteLine("  [3] Change Difficulty");
                Console.WriteLine("  [4] Reset to Defaults");
                Console.WriteLine("  [5] Back to Main Menu");
                Console.WriteLine();
                Console.WriteLine("Press ESC to go back");
                Console.WriteLine();

                Console.Write("Your choice (1-5): ");
                var keyInfo = Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    backToMainMenu = true;
                    continue;
                }

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        Console.Write("Enter new profile name: ");
                        string profileName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(profileName))
                        {
                            configManager.SetProfileName(profileName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Profile name set to: {profileName}");
                            Console.ResetColor();
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case '2':
                        Console.Write("Enter opponent name: ");
                        string opponentName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(opponentName))
                        {
                            configManager.SetOpponentProfileName(opponentName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Opponent name set to: {opponentName}");
                            Console.ResetColor();
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case '3':
                        ShowDifficultyMenu(configManager);
                        break;

                    case '4':
                        configManager.ResetToDefaults();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Settings reset to defaults!");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case '5':
                        backToMainMenu = true;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please enter 1-5.");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowDifficultyMenu(GameConfigManager configManager)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    SELECT DIFFICULTY                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Choose AI difficulty level:");
            Console.WriteLine();
            Console.WriteLine("  [1] Easy      - Depth 2 (~500 ELO)  - Good for beginners");
            Console.WriteLine("  [2] Medium    - Depth 3 (~800 ELO)  - Balanced challenge");
            Console.WriteLine("  [3] Hard      - Depth 4 (~1100 ELO) - Experienced players");
            Console.WriteLine("  [4] Very Hard - Depth 5 (~1400 ELO) - Advanced players");
            Console.WriteLine("  [5] Titan     - Depth 6 (~1700 ELO) - Master level");
            Console.WriteLine();
            Console.WriteLine("Press ESC to cancel");
            Console.WriteLine();

            Console.Write("Your choice (1-5): ");
            var keyInfo = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();

            if (keyInfo.Key != ConsoleKey.Escape)
            {
                DifficultyLevel newDifficulty;
                bool validChoice = true;

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        newDifficulty = DifficultyLevel.Easy;
                        break;
                    case '2':
                        newDifficulty = DifficultyLevel.Medium;
                        break;
                    case '3':
                        newDifficulty = DifficultyLevel.Hard;
                        break;
                    case '4':
                        newDifficulty = DifficultyLevel.VeryHard;
                        break;
                    case '5':
                        newDifficulty = DifficultyLevel.Titan;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Difficulty not changed.");
                        Console.ResetColor();
                        validChoice = false;
                        newDifficulty = DifficultyLevel.Medium;
                        break;
                }

                if (validChoice)
                {
                    configManager.SetDifficulty(newDifficulty);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Difficulty set to: {newDifficulty} (Depth {(int)newDifficulty})");
                    Console.WriteLine("This will apply to new games.");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
