using System;
using ShatranjCore.Abstractions;
using System.Linq;
using ShatranjCore.Validators;

namespace ShatranjCore.UI
{
    /// <summary>
    /// Parses user commands for the chess game.
    /// Follows Single Responsibility Principle - only handles command parsing.
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// Parses a command string into a GameCommand.
        /// </summary>
        public GameCommand Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new GameCommand { Type = CommandType.Invalid, ErrorMessage = "Empty command" };
            }

            string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLower();

            switch (command)
            {
                case "move":
                    return ParseMoveCommand(parts);

                case "castle":
                    return ParseCastleCommand(parts);

                case "help":
                    return ParseHelpCommand(parts);

                case "game":
                    return ParseGameCommand(parts);

                case "quit":
                case "exit":
                    return new GameCommand { Type = CommandType.Quit };

                case "history":
                    return new GameCommand { Type = CommandType.ShowHistory };

                default:
                    return new GameCommand
                    {
                        Type = CommandType.Invalid,
                        ErrorMessage = $"Unknown command: {command}. Type 'help' for available commands."
                    };
            }
        }

        /// <summary>
        /// Parses a move command: move [start] [end]
        /// Example: move e2 e4
        /// </summary>
        private GameCommand ParseMoveCommand(string[] parts)
        {
            if (parts.Length != 3)
            {
                return new GameCommand
                {
                    Type = CommandType.Invalid,
                    ErrorMessage = "Invalid move command. Usage: move [start] [end] (e.g., 'move e2 e4')"
                };
            }

            Location? from = ParseLocation(parts[1]);
            Location? to = ParseLocation(parts[2]);

            if (!from.HasValue || !to.HasValue)
            {
                return new GameCommand
                {
                    Type = CommandType.Invalid,
                    ErrorMessage = "Invalid location format. Use algebraic notation (e.g., e2, d4)"
                };
            }

            return new GameCommand
            {
                Type = CommandType.Move,
                From = from.Value,
                To = to.Value
            };
        }

        /// <summary>
        /// Parses a castle command: castle [side]
        /// Valid: castle, castle king, castle queen, castle k, castle q
        /// </summary>
        private GameCommand ParseCastleCommand(string[] parts)
        {
            if (parts.Length == 1)
            {
                // Just "castle" - need to prompt user
                return new GameCommand { Type = CommandType.Castle, CastleSide = null };
            }
            else if (parts.Length == 2)
            {
                // "castle [side]" - parse the side
                string side = parts[1].ToLower();

                switch (side)
                {
                    case "king":
                    case "kingside":
                    case "k":
                        return new GameCommand
                        {
                            Type = CommandType.Castle,
                            CastleSide = CastlingSide.Kingside
                        };

                    case "queen":
                    case "queenside":
                    case "q":
                        return new GameCommand
                        {
                            Type = CommandType.Castle,
                            CastleSide = CastlingSide.Queenside
                        };

                    default:
                        return new GameCommand
                        {
                            Type = CommandType.Invalid,
                            ErrorMessage = "Invalid castle side. Use: castle king/queen/k/q"
                        };
                }
            }

            return new GameCommand
            {
                Type = CommandType.Invalid,
                ErrorMessage = "Invalid castle command. Usage: castle [king|queen|k|q]"
            };
        }

        /// <summary>
        /// Parses a help command.
        /// </summary>
        private GameCommand ParseHelpCommand(string[] parts)
        {
            if (parts.Length == 1)
            {
                // Just "help" - show general help
                return new GameCommand { Type = CommandType.ShowHelp };
            }
            else if (parts.Length == 2)
            {
                // "help [location]" - show moves for specific piece
                Location? location = ParseLocation(parts[1]);
                if (!location.HasValue)
                {
                    return new GameCommand
                    {
                        Type = CommandType.Invalid,
                        ErrorMessage = "Invalid location format. Use algebraic notation (e.g., e2)"
                    };
                }

                return new GameCommand
                {
                    Type = CommandType.ShowMoves,
                    From = location.Value
                };
            }

            return new GameCommand
            {
                Type = CommandType.Invalid,
                ErrorMessage = "Invalid help command. Usage: help or help [location]"
            };
        }

        /// <summary>
        /// Parses a game command: game [action] [filename]
        /// </summary>
        private GameCommand ParseGameCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                return new GameCommand
                {
                    Type = CommandType.Invalid,
                    ErrorMessage = "Invalid game command. Usage: game [start|save|load|end|restart] [filename]"
                };
            }

            string action = parts[1].ToLower();
            string fileName = parts.Length > 2 ? parts[2] : null;

            switch (action)
            {
                case "start":
                    return new GameCommand { Type = CommandType.StartGame };

                case "save":
                    return new GameCommand { Type = CommandType.SaveGame, FileName = fileName };

                case "load":
                    return new GameCommand { Type = CommandType.LoadGame, FileName = fileName };

                case "end":
                    return new GameCommand { Type = CommandType.EndGame };

                case "restart":
                    return new GameCommand { Type = CommandType.RestartGame };

                default:
                    return new GameCommand
                    {
                        Type = CommandType.Invalid,
                        ErrorMessage = $"Unknown game action: {action}. Valid actions: start, save, load, end, restart"
                    };
            }
        }

        /// <summary>
        /// Parses a location in algebraic notation (e.g., "e2" or "a8").
        /// </summary>
        public Location? ParseLocation(string locationStr)
        {
            if (string.IsNullOrWhiteSpace(locationStr) || locationStr.Length != 2)
                return null;

            locationStr = locationStr.ToLower();
            char file = locationStr[0];
            char rank = locationStr[1];

            // File must be a-h
            if (file < 'a' || file > 'h')
                return null;

            // Rank must be 1-8
            if (rank < '1' || rank > '8')
                return null;

            int column = file - 'a';
            int row = 8 - (rank - '0');

            return new Location(row, column);
        }
    }

    /// <summary>
    /// Represents a parsed game command.
    /// </summary>
    public class GameCommand
    {
        public CommandType Type { get; set; }
        public Location From { get; set; }
        public Location To { get; set; }
        public CastlingSide? CastleSide { get; set; }
        public Type PromotionPiece { get; set; }
        public string ErrorMessage { get; set; }
        public string FileName { get; set; }  // For save/load operations
    }

    /// <summary>
    /// Types of game commands.
    /// </summary>
    public enum CommandType
    {
        Invalid,
        Move,
        Castle,
        ShowMoves,
        ShowHelp,
        ShowHistory,
        StartGame,
        SaveGame,
        LoadGame,
        EndGame,
        RestartGame,
        Quit
    }
}
