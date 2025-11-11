namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for creating game instances
    /// </summary>
    public interface IGameFactory
    {
        IGameOrchestrator CreateGame(GameMode mode, PieceColor humanColor);
    }
}
