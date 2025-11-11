using System.Collections.Generic;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for rendering game state to the user
    /// </summary>
    public interface IRenderer
    {
        void RenderBoard(IBoardState board, Location? lastFrom, Location? lastTo);
        void DisplayGameStatus(GameStatus status);
        void DisplayError(string message);
        void DisplayInfo(string message);
        void DisplayPossibleMoves(Location from, List<Move> legalMoves);
        void DisplayGameOver(GameResult result, PieceColor? winner = null);
        void DisplayCommands();
    }

    /// <summary>
    /// Game status information
    /// </summary>
    public class GameStatus
    {
        public PieceColor CurrentPlayer { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }
        public bool IsStalemate { get; set; }
        public string CurrentPlayerName { get; set; }
    }

    /// <summary>
    /// Move representation
    /// </summary>
    public class Move
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public string PieceName { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }
    }
}
