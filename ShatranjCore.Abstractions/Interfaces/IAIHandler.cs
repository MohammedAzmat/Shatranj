namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for handling AI turns
    /// </summary>
    public interface IAIHandler
    {
        void HandleAITurn(IChessAI ai, PieceColor color);
    }
}
