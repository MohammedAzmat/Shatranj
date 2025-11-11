using ShatranjCore.Abstractions;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Records game information for AI learning and analysis.
    /// Single Responsibility: Record moves, outcomes, and evaluations.
    /// Used for building AI training data and game history.
    /// </summary>
    public interface IGameRecorder
    {
        /// <summary>
        /// Starts recording a new game.
        /// </summary>
        /// <param name="whiteName">Name of white player</param>
        /// <param name="blackName">Name of black player</param>
        void StartGame(string whiteName, string blackName);

        /// <summary>
        /// Records a move with its evaluation.
        /// </summary>
        /// <param name="moveNotation">Move in algebraic notation (e.g., "e4")</param>
        /// <param name="evaluationScore">AI evaluation of the position (optional)</param>
        /// <param name="player">Which player made the move</param>
        void RecordMove(string moveNotation, double? evaluationScore, PieceColor player);

        /// <summary>
        /// Records the end of game with result.
        /// </summary>
        /// <param name="result">Game result (1-0, 0-1, 1/2-1/2)</param>
        /// <param name="reason">Reason for game end (checkmate, resignation, etc)</param>
        void EndGame(string result, string reason = null);

        /// <summary>
        /// Gets the recorded game data.
        /// </summary>
        /// <returns>The recorded game</returns>
        GameRecord GetRecordedGame();

        /// <summary>
        /// Checks if recording is in progress.
        /// </summary>
        bool IsRecording { get; }
    }
}
