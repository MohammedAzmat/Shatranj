using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Game;

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
            Console.WriteLine("Press any key to start a new game...");
            Console.ReadKey();

            // Start the enhanced chess game
            EnhancedChessGame game = new EnhancedChessGame();
            game.Start();

            Console.WriteLine();
            Console.WriteLine("Thank you for playing Shatranj!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
