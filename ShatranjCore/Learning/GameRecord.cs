using System;
using System.Collections.Generic;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Represents a single move in a game
    /// </summary>
    [Serializable]
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; }  // "White" or "Black"
        public string From { get; set; }  // e.g., "e2"
        public string To { get; set; }  // e.g., "e4"
        public string PieceType { get; set; }  // e.g., "Pawn"
        public string AlgebraicNotation { get; set; }  // e.g., "e4"
        public bool WasCapture { get; set; }
        public bool CausedCheck { get; set; }
        public bool CausedCheckmate { get; set; }
        public double? PositionEvaluation { get; set; }  // Evaluation after this move
        public int ThinkingTimeMs { get; set; }  // Time taken to make decision
    }

    /// <summary>
    /// Represents a complete game for AI learning
    /// </summary>
    [Serializable]
    public class GameRecord
    {
        public string GameId { get; set; }
        public DateTime PlayedAt { get; set; }
        public string GameMode { get; set; }  // "HumanVsHuman", "HumanVsAI", "AIVsAI"
        public string WhitePlayer { get; set; }  // "Human" or "AI:{AlgorithmName}"
        public string BlackPlayer { get; set; }
        public List<MoveRecord> Moves { get; set; }
        public string Winner { get; set; }  // "White", "Black", "Draw"
        public string EndCondition { get; set; }  // "Checkmate", "Stalemate", "Resignation"
        public int TotalMoves { get; set; }
        public int GameDurationMs { get; set; }

        // AI-specific data
        public string AIVersion { get; set; }  // e.g., "BasicAI_v1.0"
        public int AIDepth { get; set; }  // Search depth used
        public Dictionary<string, double> Statistics { get; set; }  // Custom stats

        public GameRecord()
        {
            GameId = Guid.NewGuid().ToString();
            PlayedAt = DateTime.Now;
            Moves = new List<MoveRecord>();
            Statistics = new Dictionary<string, double>();
        }
    }
}
