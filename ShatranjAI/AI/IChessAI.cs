using System;
using ShatranjCore;
using ShatranjCore.Interfaces;

namespace ShatranjAI.AI
{
    /// <summary>
    /// Represents the result of AI move selection
    /// </summary>
    public class AIMove
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public double Evaluation { get; set; }
        public int ThinkingTimeMs { get; set; }
        public int NodesEvaluated { get; set; }
        public int Depth { get; set; }
        public string Reasoning { get; set; }  // For debugging

        public AIMove(Location from, Location to, double evaluation = 0)
        {
            From = from;
            To = to;
            Evaluation = evaluation;
        }
    }

    /// <summary>
    /// Interface for chess AI implementations
    /// </summary>
    public interface IChessAI
    {
        /// <summary>
        /// Gets the name of the AI
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of the AI
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the search depth
        /// </summary>
        int Depth { get; set; }

        /// <summary>
        /// Selects the best move for the current position
        /// </summary>
        /// <param name="board">The current board state</param>
        /// <param name="color">The color to move</param>
        /// <param name="enPassantTarget">Current en passant target square, if any</param>
        /// <returns>The selected move with evaluation</returns>
        AIMove SelectMove(IChessBoard board, PieceColor color, Location? enPassantTarget);

        /// <summary>
        /// Evaluates the current board position from the perspective of the given color
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <param name="color">The color to evaluate for (positive = good for this color)</param>
        /// <returns>Evaluation score</returns>
        double EvaluatePosition(IChessBoard board, PieceColor color);
    }
}
