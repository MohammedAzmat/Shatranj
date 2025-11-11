namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for parsing user input commands
    /// </summary>
    public interface ICommandParser
    {
        GameCommand Parse(string input);
    }

    /// <summary>
    /// Represents a parsed game command
    /// </summary>
    public class GameCommand
    {
        public CommandType Type { get; set; }
        public Location? From { get; set; }
        public Location? To { get; set; }
        public CastlingSide? CastlingSide { get; set; }
        public int? GameId { get; set; }
        public string StringValue { get; set; }
    }

    /// <summary>
    /// Types of game commands
    /// </summary>
    public enum CommandType
    {
        Move,
        Castle,
        ShowMoves,
        Help,
        Quit,
        Save,
        Load,
        Settings,
        History,
        Rollback,
        Redo,
        Invalid
    }

    /// <summary>
    /// Castling side
    /// </summary>
    public enum CastlingSide
    {
        Kingside,
        Queenside
    }
}
