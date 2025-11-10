using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore;
using ShatranjCore.Game;
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

            // Show game mode menu
            GameMenuHandler menuHandler = new GameMenuHandler();
            GameMode selectedMode = menuHandler.ShowGameModeMenu();

            // Get color preference for Human vs AI mode
            PieceColor humanColor = PieceColor.White;
            if (selectedMode == GameMode.HumanVsAI)
            {
                humanColor = menuHandler.ShowColorSelectionMenu();
            }

            // Start the chess game with selected mode
            ChessGame game = new ChessGame(selectedMode, humanColor);
            game.Start();

            Console.WriteLine();
            Console.WriteLine("Thank you for playing Shatranj!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
