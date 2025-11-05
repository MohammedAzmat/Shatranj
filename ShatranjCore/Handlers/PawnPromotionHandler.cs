using System;
using System.Collections.Generic;
using ShatranjCore.Pieces;

namespace ShatranjCore.Handlers
{
    /// <summary>
    /// Handles pawn promotion logic and user interaction.
    /// Follows Single Responsibility Principle - only handles pawn promotion.
    /// </summary>
    public class PawnPromotionHandler
    {
        /// <summary>
        /// Checks if a pawn at the given location needs promotion.
        /// </summary>
        public bool NeedsPromotion(Piece piece, Location location)
        {
            if (!(piece is Pawn pawn))
                return false;

            // White pawns promote on row 0, black pawns on row 7
            if (pawn.Color == PieceColor.White && location.Row == 0)
                return true;

            if (pawn.Color == PieceColor.Black && location.Row == 7)
                return true;

            return false;
        }

        /// <summary>
        /// Prompts user for promotion choice and returns the selected piece type.
        /// </summary>
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

        /// <summary>
        /// Creates a new piece for promotion.
        /// </summary>
        public Piece CreatePromotionPiece(Type pieceType, Location location, PieceColor color)
        {
            if (pieceType == typeof(Queen))
                return new Queen(location.Row, location.Column, color);

            if (pieceType == typeof(Rook))
                return new Rook(location.Row, location.Column, color);

            if (pieceType == typeof(Bishop))
                return new Bishop(location.Row, location.Column, color);

            if (pieceType == typeof(Knight))
                return new Knight(location.Row, location.Column, color);

            throw new ArgumentException($"Invalid promotion piece type: {pieceType.Name}");
        }
    }
}
