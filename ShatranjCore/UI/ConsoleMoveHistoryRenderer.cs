using System;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.UI
{
    /// <summary>
    /// Console-based renderer for move history display.
    /// Responsibility: Display move history to console (separated from data storage).
    /// </summary>
    public class ConsoleMoveHistoryRenderer : IMoveHistoryRenderer
    {
        /// <summary>
        /// Displays the move history to console in standard chess notation.
        /// Note: This implementation works with MoveHistory (the concrete implementation).
        /// </summary>
        /// <param name="history">The move history to display (must be cast to MoveHistory)</param>
        public void DisplayHistory(ShatranjCore.Abstractions.Interfaces.IMoveHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            // Cast to concrete type if it's MoveHistory
            // In a fully decoupled system, this would receive IMoveHistory with matching MoveRecord
            if (history is MoveHistory moveHistory)
            {
                var moves = moveHistory.GetAllMoves();

                Console.WriteLine("Move History:");
                Console.WriteLine("─────────────────────────────────────────");

                int moveNum = 1;
                for (int i = 0; i < moves.Count; i += 2)
                {
                    string whiteMove = moves[i].AlgebraicNotation;
                    string blackMove = i + 1 < moves.Count ? moves[i + 1].AlgebraicNotation : "";

                    Console.WriteLine($"{moveNum,3}. {whiteMove,-15} {blackMove}");
                    moveNum++;
                }

                Console.WriteLine("─────────────────────────────────────────");
            }
            else
            {
                throw new ArgumentException(
                    "ConsoleMoveHistoryRenderer currently supports only MoveHistory. " +
                    "IMoveHistory interface needs to be aligned with concrete implementation.");
            }
        }
    }
}
