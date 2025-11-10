using System;
using ShatranjCore.Abstractions;

namespace ShatranjCore.UI
{
    /// <summary>
    /// Handles the game mode selection menu.
    /// </summary>
    public class GameMenuHandler
    {
        /// <summary>
        /// Displays the game mode selection menu and returns user's choice.
        /// </summary>
        public GameMode ShowGameModeMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MODE SELECTION                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Please select a game mode:");
            Console.WriteLine();
            Console.WriteLine("  [1] Human vs Human    - Two players on the same computer");
            Console.WriteLine("  [2] Human vs AI       - Play against BasicAI (Minimax depth-3)");
            Console.WriteLine("  [3] AI vs AI          - Watch two AIs compete");
            Console.WriteLine();
            Console.WriteLine("Press ESC to exit");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice (1-3): ");
                var keyInfo = Console.ReadKey();
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        return GameMode.HumanVsHuman;
                    case '2':
                        return GameMode.HumanVsAI;
                    case '3':
                        return GameMode.AIVsAI;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        /// <summary>
        /// Shows color selection menu for Human vs AI mode.
        /// Returns true for White, false for Black.
        /// </summary>
        public PieceColor ShowColorSelectionMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    COLOR SELECTION                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Which color would you like to play?");
            Console.WriteLine();
            Console.WriteLine("  [1] White  - You move first");
            Console.WriteLine("  [2] Black  - AI moves first");
            Console.WriteLine();
            Console.WriteLine("Press ESC to go back");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice (1-2): ");
                var keyInfo = Console.ReadKey();
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return ShowColorSelectionMenu(); // Re-show menu
                }

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        return PieceColor.White;
                    case '2':
                        return PieceColor.Black;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        /// <summary>
        /// Shows the main menu with Resume, New Game, Settings, Exit
        /// </summary>
        public enum MainMenuChoice
        {
            Resume,
            NewGame,
            Settings,
            Exit
        }

        public MainMenuChoice ShowMainMenu(bool hasAutosave)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        MAIN MENU                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            int optionNumber = 1;
            int resumeOption = -1;
            int newGameOption = -1;
            int settingsOption = -1;
            int exitOption = -1;

            if (hasAutosave)
            {
                Console.WriteLine($"  [{optionNumber}] Resume Game  - Continue your last game");
                resumeOption = optionNumber++;
            }

            Console.WriteLine($"  [{optionNumber}] New Game     - Start a fresh game");
            newGameOption = optionNumber++;

            Console.WriteLine($"  [{optionNumber}] Settings     - Configure game settings");
            settingsOption = optionNumber++;

            Console.WriteLine($"  [{optionNumber}] Exit         - Quit Shatranj");
            exitOption = optionNumber++;

            Console.WriteLine();
            Console.WriteLine("Press ESC to exit");
            Console.WriteLine();

            while (true)
            {
                Console.Write($"Your choice (1-{optionNumber - 1}): ");
                var keyInfo = Console.ReadKey();
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return MainMenuChoice.Exit;
                }

                int choice;
                if (int.TryParse(keyInfo.KeyChar.ToString(), out choice))
                {
                    if (hasAutosave && choice == resumeOption)
                    {
                        return MainMenuChoice.Resume;
                    }
                    else if (choice == newGameOption)
                    {
                        return MainMenuChoice.NewGame;
                    }
                    else if (choice == settingsOption)
                    {
                        return MainMenuChoice.Settings;
                    }
                    else if (choice == exitOption)
                    {
                        return MainMenuChoice.Exit;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid choice. Please enter 1-{optionNumber - 1}.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Displays a "feature not yet implemented" message.
        /// </summary>
        public void ShowFeatureNotImplemented(string featureName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  FEATURE NOT YET IMPLEMENTED                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  {featureName} is coming in Phase 2!");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  Phase 2 will include:");
            Console.WriteLine("  • Basic AI opponent with minimax algorithm");
            Console.WriteLine("  • Material evaluation and positional awareness");
            Console.WriteLine("  • AI vs AI simulation mode");
            Console.WriteLine();
            Console.WriteLine("  For now, please select Human vs Human mode.");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey(true);
        }
    }
}
