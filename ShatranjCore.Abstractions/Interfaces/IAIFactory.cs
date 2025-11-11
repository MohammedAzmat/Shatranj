namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for creating AI instances
    /// </summary>
    public interface IAIFactory
    {
        IChessAI CreateAI(DifficultyLevel difficulty);
    }
}
