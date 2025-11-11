using System;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Abstractions.Commands
{
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
        Rollback,
        Redo,
        ShowSettings,
        ResetSettings,
        SetProfile,
        SetOpponent,
        SetDifficulty,
        Quit
    }

    /// <summary>
    /// Enum for castling side.
    /// </summary>
    public enum CastlingSide
    {
        Kingside,
        Queenside
    }
}
