using System.Collections.Generic;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Analyzes recorded games to identify patterns, mistakes, and improvements.
    /// Single Responsibility: Analyze game quality and player performance.
    /// Used for AI learning and human player improvement suggestions.
    /// </summary>
    public interface IGameAnalyzer
    {
        /// <summary>
        /// Analyzes a game and returns detailed analysis.
        /// </summary>
        /// <param name="game">The game to analyze</param>
        /// <returns>Analysis results</returns>
        GameAnalysis Analyze(GameRecord game);

        /// <summary>
        /// Finds critical mistakes in a game.
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>List of moves that were mistakes</returns>
        List<MoveAnalysis> FindMistakes(GameRecord game);

        /// <summary>
        /// Identifies opening theory followed in the game.
        /// </summary>
        /// <param name="game">The game to analyze</param>
        /// <returns>Opening name and theory adherence</returns>
        OpeningAnalysis AnalyzeOpening(GameRecord game);

        /// <summary>
        /// Evaluates endgame play quality.
        /// </summary>
        /// <param name="game">The game to analyze</param>
        /// <returns>Endgame analysis</returns>
        EndgameAnalysis AnalyzeEndgame(GameRecord game);
    }

    /// <summary>
    /// Results of game analysis.
    /// </summary>
    public class GameAnalysis
    {
        public int TotalMoves { get; set; }
        public int MistakeCount { get; set; }
        public double AverageAccuracy { get; set; }
        public string OverallAssessment { get; set; }
        public List<MoveAnalysis> CriticalMoves { get; set; }
    }

    /// <summary>
    /// Analysis of a single move.
    /// </summary>
    public class MoveAnalysis
    {
        public int MoveNumber { get; set; }
        public string Move { get; set; }
        public double EvaluationBefore { get; set; }
        public double EvaluationAfter { get; set; }
        public string Assessment { get; set; }
        public bool IsMistake { get; set; }
    }

    /// <summary>
    /// Analysis of opening play.
    /// </summary>
    public class OpeningAnalysis
    {
        public string OpeningName { get; set; }
        public string ECOCode { get; set; }
        public int MovesInOpening { get; set; }
        public bool FollowedTheory { get; set; }
        public string Notes { get; set; }
    }

    /// <summary>
    /// Analysis of endgame play.
    /// </summary>
    public class EndgameAnalysis
    {
        public int StartMove { get; set; }
        public string EndgameType { get; set; }
        public double Accuracy { get; set; }
        public bool FoundBestMoves { get; set; }
        public string Notes { get; set; }
    }
}
