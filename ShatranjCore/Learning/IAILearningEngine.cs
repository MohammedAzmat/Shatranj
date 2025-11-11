using System.Collections.Generic;
using ShatranjCore.Abstractions.Interfaces;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// AI learning engine that trains and improves AI performance.
    /// Single Responsibility: AI improvement and self-play training.
    /// Coordinator for game analysis, pattern learning, and strategy improvement.
    /// </summary>
    public interface IAILearningEngine
    {
        /// <summary>
        /// Trains the AI from a collection of games.
        /// </summary>
        /// <param name="games">Games to learn from</param>
        /// <param name="iterations">Number of training iterations</param>
        void TrainFromGames(List<GameRecord> games, int iterations);

        /// <summary>
        /// Runs self-play training where AI plays against itself.
        /// </summary>
        /// <param name="gameCount">Number of games to play</param>
        /// <returns>List of self-play games</returns>
        List<GameRecord> RunSelfPlayTraining(int gameCount);

        /// <summary>
        /// Evaluates AI performance on test games.
        /// </summary>
        /// <param name="testGames">Games to test on</param>
        /// <returns>Performance metrics</returns>
        PerformanceMetrics EvaluatePerformance(List<GameRecord> testGames);

        /// <summary>
        /// Updates AI opening book from recorded games.
        /// </summary>
        /// <param name="games">Games with opening data</param>
        void UpdateOpeningBook(List<GameRecord> games);

        /// <summary>
        /// Analyzes AI decision patterns to improve strategy.
        /// </summary>
        /// <param name="games">Games to analyze</param>
        /// <returns>Decision analysis results</returns>
        DecisionAnalysis AnalyzeDecisions(List<GameRecord> games);

        /// <summary>
        /// Gets the current training status.
        /// </summary>
        TrainingStatus GetTrainingStatus();
    }

    /// <summary>
    /// AI performance metrics.
    /// </summary>
    public class PerformanceMetrics
    {
        public double Accuracy { get; set; }
        public double AverageEvaluation { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public int DrawCount { get; set; }
        public double WinRate { get; set; }
    }

    /// <summary>
    /// Analysis of AI decision patterns.
    /// </summary>
    public class DecisionAnalysis
    {
        public int TotalDecisions { get; set; }
        public double AverageConfidence { get; set; }
        public Dictionary<string, int> MostCommonMoves { get; set; }
        public Dictionary<string, double> MoveSuccessRate { get; set; }
    }

    /// <summary>
    /// Current training status.
    /// </summary>
    public class TrainingStatus
    {
        public bool IsTraining { get; set; }
        public int GamesProcessed { get; set; }
        public int TotalGames { get; set; }
        public double ProgressPercentage { get; set; }
        public string CurrentPhase { get; set; }
    }
}
