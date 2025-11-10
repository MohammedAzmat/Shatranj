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
    /// Represents a complete game state for save/load
    /// </summary>
    [Serializable]
    public class GameStateSnapshot
    {
        public string GameId { get; set; }
        public DateTime SavedAt { get; set; }
        public string GameMode { get; set; }  // "HumanVsHuman", "HumanVsAI", "AIVsAI"
        public string CurrentPlayer { get; set; }  // "White" or "Black"
        public string HumanColor { get; set; }  // For AI games
        public string GameResult { get; set; }  // "InProgress", "Checkmate", "Stalemate"
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

        // Metadata
        public string WhitePlayerType { get; set; }  // "Human" or "AI"
        public string BlackPlayerType { get; set; }
        public int MoveCount { get; set; }

        public GameStateSnapshot()
        {
            GameId = Guid.NewGuid().ToString();
            SavedAt = DateTime.Now;
            Pieces = new List<PieceData>();
            MoveHistory = new List<string>();
            CapturedPieces = new List<PieceData>();
        }
    }
}
