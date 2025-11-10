using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;

namespace ShatranjCore.UI
{
    /// <summary>
    /// Renders the chess board to the console with enhanced visualization.
    /// Follows Single Responsibility Principle - only handles board display.
    /// </summary>
    public class ConsoleBoardRenderer
    {
        private const string HORIZONTAL_LINE = "═";
        private const string VERTICAL_LINE = "║";
        private const string TOP_LEFT = "╔";
        private const string TOP_RIGHT = "╗";
        private const string BOTTOM_LEFT = "╚";
        private const string BOTTOM_RIGHT = "╝";
        private const string CROSS = "╬";
        private const string T_DOWN = "╦";
        private const string T_UP = "╩";
        private const string T_RIGHT = "╠";
        private const string T_LEFT = "╣";

        /// <summary>
        /// Renders the chess board with enhanced visuals.
        /// </summary>
        public void RenderBoard(IChessBoard board, Location? lastMoveFrom = null, Location? lastMoveTo = null)
        {
            Console.Clear();

            // Title
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        SHATRANJ CHESS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            // Column labels (top)
            Console.Write("     ");
            for (char col = 'A'; col <= 'H'; col++)
            {
                Console.Write($"   {col}   ");
            }
            Console.WriteLine();

            // Top border
            Console.Write("   " + TOP_LEFT);
            for (int i = 0; i < 7; i++)
            {
                Console.Write(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + T_DOWN);
            }
            Console.WriteLine(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + TOP_RIGHT);

            // Board rows
            for (int row = 0; row < 8; row++)
            {
                // Row number
                Console.Write($" {8 - row} ");

                for (int col = 0; col < 8; col++)
                {
                    Location loc = new Location(row, col);
                    Piece piece = board.GetPiece(loc);

                    Console.Write(VERTICAL_LINE);

                    // Highlight last move
                    bool isLastMoveSquare = (lastMoveFrom.HasValue && lastMoveFrom.Value.Row == row && lastMoveFrom.Value.Column == col) ||
                                           (lastMoveTo.HasValue && lastMoveTo.Value.Row == row && lastMoveTo.Value.Column == col);

                    if (isLastMoveSquare)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        // Checkerboard pattern
                        if ((row + col) % 2 == 0)
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                        else
                            Console.BackgroundColor = ConsoleColor.Black;
                    }

                    if (piece != null)
                    {
                        // Set piece color
                        Console.ForegroundColor = piece.Color == PieceColor.White ? ConsoleColor.White : ConsoleColor.Red;

                        // Display piece with padding
                        string pieceSymbol = GetPieceSymbol(piece);
                        Console.Write($"  {pieceSymbol}   ");
                    }
                    else
                    {
                        Console.Write("      ");
                    }

                    Console.ResetColor();
                }

                Console.Write(VERTICAL_LINE);
                Console.Write($" {8 - row}");
                Console.WriteLine();

                // Row separator (except last row)
                if (row < 7)
                {
                    Console.Write("   " + T_RIGHT);
                    for (int i = 0; i < 7; i++)
                    {
                        Console.Write(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + CROSS);
                    }
                    Console.WriteLine(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + T_LEFT);
                }
            }

            // Bottom border
            Console.Write("   " + BOTTOM_LEFT);
            for (int i = 0; i < 7; i++)
            {
                Console.Write(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + T_UP);
            }
            Console.WriteLine(HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + HORIZONTAL_LINE + BOTTOM_RIGHT);

            // Column labels (bottom)
            Console.Write("     ");
            for (char col = 'A'; col <= 'H'; col++)
            {
                Console.Write($"   {col}   ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Gets the visual symbol for a piece.
        /// </summary>
        private string GetPieceSymbol(Piece piece)
        {
            string symbol = piece.GetType().Name.Substring(0, 1);
            if (piece is Knight)
                symbol = "N"; // Use N for Knight to avoid confusion with King

            return piece.Color == PieceColor.White ? symbol : symbol.ToLower();
        }

        /// <summary>
        /// Displays game status information.
        /// </summary>
        public void DisplayGameStatus(GameStatus status)
        {
            Console.WriteLine("┌────────────────────────────────────────────────────────────────┐");

            // Current player turn
            Console.ForegroundColor = status.CurrentPlayer == PieceColor.White ? ConsoleColor.White : ConsoleColor.Red;
            Console.WriteLine($"│ Current Turn: {status.CurrentPlayer,-47}│");
            Console.ResetColor();

            // Check status
            if (status.IsCheck)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"│ ⚠ CHECK! {status.CurrentPlayer} King is in check!{new string(' ', 27)}│");
                Console.ResetColor();
            }

            // Last move
            if (status.LastMove != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"│ Last Move: {status.LastMove,-48}│");
                Console.ResetColor();
            }

            // Captured pieces
            if (status.CapturedPieces.Count > 0)
            {
                string captured = string.Join(", ", status.CapturedPieces.Select(p => GetPieceSymbol(p)));
                Console.WriteLine($"│ Captured: {captured,-49}│");
            }

            Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays available commands.
        /// </summary>
        public void DisplayCommands()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Available Commands:");
            Console.ResetColor();
            Console.WriteLine("  move [start] [end]  - Move a piece (e.g., 'move e2 e4')");
            Console.WriteLine("  castle [side]       - Castle (king/k, queen/q, or just 'castle' to be prompted)");
            Console.WriteLine("  help [location]     - Show possible moves for a piece (e.g., 'help e2')");
            Console.WriteLine("  help                - Show this command list");
            Console.WriteLine("  history             - Display move history");
            Console.WriteLine("  game start          - Start a new game");
            Console.WriteLine("  game save           - Save current game");
            Console.WriteLine("  game end            - End current game");
            Console.WriteLine("  game restart        - Restart the game");
            Console.WriteLine("  quit                - Exit the game");
            Console.WriteLine();
            Console.WriteLine("Note: Press ESC to cancel during castling or pawn promotion prompts");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays possible moves for a piece.
        /// </summary>
        public void DisplayPossibleMoves(Location pieceLocation, List<Move> possibleMoves)
        {
            if (possibleMoves.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No valid moves available for this piece.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nPossible moves for piece at {LocationToAlgebraic(pieceLocation)}:");
            Console.ResetColor();

            foreach (var move in possibleMoves)
            {
                string destination = LocationToAlgebraic(move.To.Location);
                if (move.CapturedPiece != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  → {destination} (captures {move.CapturedPiece.GetType().Name})");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  → {destination}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays game over message.
        /// </summary>
        public void DisplayGameOver(GameResult result, PieceColor? winner = null)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");

            switch (result)
            {
                case GameResult.Checkmate:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"║                        CHECKMATE!                              ║");
                    Console.WriteLine($"║              {winner} wins the game!{new string(' ', 30)}║");
                    break;

                case GameResult.Stalemate:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"║                        STALEMATE!                              ║");
                    Console.WriteLine($"║                    Game ends in a draw                         ║");
                    break;

                case GameResult.Draw:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"║                          DRAW!                                 ║");
                    Console.WriteLine($"║                    Game ends in a draw                         ║");
                    break;

                case GameResult.Resignation:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"║                       RESIGNATION                              ║");
                    Console.WriteLine($"║              {winner} wins by resignation{new string(' ', 24)}║");
                    break;
            }

            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Displays an error message.
        /// </summary>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠ Error: {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Displays an informational message.
        /// </summary>
        public void DisplayInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Converts a Location to algebraic notation (e.g., e2).
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }

    /// <summary>
    /// Represents the current status of the game.
    /// </summary>
    public class GameStatus
    {
        public PieceColor CurrentPlayer { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }
        public bool IsStalemate { get; set; }
        public string LastMove { get; set; }
        public List<Piece> CapturedPieces { get; set; } = new List<Piece>();
    }

    /// <summary>
    /// Enum for game results.
    /// </summary>
    public enum GameResult
    {
        Checkmate,
        Stalemate,
        Draw,
        Resignation,
        InProgress
    }
}
