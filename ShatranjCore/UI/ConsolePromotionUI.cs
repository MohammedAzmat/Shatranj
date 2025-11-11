using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjCore.UI
{
    /// <summary>
    /// Console-based user interface for pawn promotion.
    /// Responsibility: Handle user interaction and input for promotion selection.
    /// </summary>
    public class ConsolePromotionUI : IPromotionUI
    {
        /// <summary>
        /// Prompts user for promotion choice and returns the selected piece type.
        /// </summary>
        /// <param name="color">The color of the pawn being promoted (for informational purposes)</param>
        /// <returns>The Type of piece to promote to, or null if cancelled</returns>
        public Type PromptForPromotion(PieceColor color)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    PAWN PROMOTION!                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Choose a piece to promote to:");
            Console.WriteLine("  1. Queen   (Q)");
            Console.WriteLine("  2. Rook    (R)");
            Console.WriteLine("  3. Bishop  (B)");
            Console.WriteLine("  4. Knight  (N)");
            Console.WriteLine();
            Console.WriteLine("Press ESC to cancel (pawn will not move)");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice: ");

                // Check for ESC key
                var keyInfo = Console.ReadKey(intercept: false);
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Promotion cancelled.");
                    Console.ResetColor();
                    return null;
                }

                string input = keyInfo.KeyChar.ToString().ToLower();

                // Check if user needs to type more
                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    // Read rest of line if they're typing a full word
                    string rest = Console.ReadLine();
                    input += rest.ToLower();
                }

                Type pieceType = ParsePromotionChoice(input.Trim());

                if (pieceType != null)
                {
                    return pieceType;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please enter: Queen/Q, Rook/R, Bishop/B, or Knight/N");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Parses user input to determine promotion piece type.
        /// </summary>
        private Type ParsePromotionChoice(string input)
        {
            switch (input)
            {
                case "queen":
                case "q":
                case "1":
                    return typeof(Queen);

                case "rook":
                case "r":
                case "2":
                    return typeof(Rook);

                case "bishop":
                case "b":
                case "3":
                    return typeof(Bishop);

                case "knight":
                case "n":
                case "4":
                    return typeof(Knight);

                default:
                    return null;
            }
        }
    }
}
