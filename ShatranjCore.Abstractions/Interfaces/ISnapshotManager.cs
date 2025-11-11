namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for creating and restoring game snapshots
    /// </summary>
    public interface ISnapshotManager
    {
        object CreateSnapshot(IBoardState board, GameContext context);
        void RestoreSnapshot(object snapshot, IBoardState board, out GameContext context);
    }

    /// <summary>
    /// Game context containing current game state
    /// </summary>
    public class GameContext
    {
        public int GameId { get; set; }
        public GameMode GameMode { get; set; }
        public PieceColor CurrentPlayer { get; set; }
        public PieceColor HumanColor { get; set; }
        public GameResult GameResult { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string WhitePlayerName { get; set; }
        public string BlackPlayerName { get; set; }
        public object[] Players { get; set; }
    }
}
