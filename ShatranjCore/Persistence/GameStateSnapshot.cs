using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using ShatranjCore.Movement;

namespace ShatranjCore.Persistence
{
    /// <summary>
    /// Represents a piece on the board for serialization
    /// </summary>
    [Serializable]
    public class PieceData
    {
        public string Type { get; set; }  // e.g., "Pawn", "Rook"
        public string Color { get; set; }  // "White" or "Black"
        public int Row { get; set; }
        public int Column { get; set; }
        public bool HasMoved { get; set; }
    }

    /// <summary>
    /// Metadata for a saved game (used for listing save files)
    /// </summary>
    [Serializable]
    public class GameMetadata
    {
        public int GameId { get; set; }
        public DateTime SavedAt { get; set; }
        public string Difficulty { get; set; }
        public int TurnCount { get; set; }
        public string CurrentPlayer { get; set; }  // "White" or "Black"
        public string GameMode { get; set; }
        public string GameResult { get; set; }
        public string WhitePlayerName { get; set; }
        public string BlackPlayerName { get; set; }

        public GameMetadata()
        {
            SavedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Represents a complete game state for save/load
    /// </summary>
    [Serializable]
    public class GameStateSnapshot
    {
        // Game identification
        public int GameId { get; set; }
        public DateTime SavedAt { get; set; }
        public string GameMode { get; set; }  // "HumanVsHuman", "HumanVsAI", "AIVsAI"

        // Player information
        public string CurrentPlayer { get; set; }  // "White" or "Black"
        public string HumanColor { get; set; }  // For AI games
        public string WhitePlayerType { get; set; }  // "Human" or "AI"
        public string BlackPlayerType { get; set; }
        public string WhitePlayerName { get; set; }
        public string BlackPlayerName { get; set; }

        // Game state
        public string GameResult { get; set; }  // "InProgress", "WhiteWins", "BlackWins", "Stalemate", "Draw"
        public int MoveCount { get; set; }
        public string Difficulty { get; set; }  // "Easy", "Medium", "Hard", "VeryHard", "Titan"

        // Board state
        public List<PieceData> Pieces { get; set; }
        public List<string> MoveHistory { get; set; }  // Algebraic notation
        public List<PieceData> CapturedPieces { get; set; }

        // En passant tracking
        public int? EnPassantPawnRow { get; set; }
        public int? EnPassantPawnColumn { get; set; }
        public int? EnPassantTargetRow { get; set; }
        public int? EnPassantTargetColumn { get; set; }

        // Castling tracking
        public bool WhiteKingMoved { get; set; }
        public bool BlackKingMoved { get; set; }
        public bool WhiteKingsideRookMoved { get; set; }
        public bool WhiteQueensideRookMoved { get; set; }
        public bool BlackKingsideRookMoved { get; set; }
        public bool BlackQueensideRookMoved { get; set; }

        public GameStateSnapshot()
        {
            SavedAt = DateTime.Now;
            Pieces = new List<PieceData>();
            MoveHistory = new List<string>();
            CapturedPieces = new List<PieceData>();
        }

        /// <summary>
        /// Creates metadata from the snapshot
        /// </summary>
        public GameMetadata ToMetadata()
        {
            return new GameMetadata
            {
                GameId = this.GameId,
                SavedAt = this.SavedAt,
                Difficulty = this.Difficulty,
                TurnCount = this.MoveCount,
                CurrentPlayer = this.CurrentPlayer,
                GameMode = this.GameMode,
                GameResult = this.GameResult,
                WhitePlayerName = this.WhitePlayerName,
                BlackPlayerName = this.BlackPlayerName
            };
        }
    }
}
