namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for managing player turns
    /// </summary>
    public interface ITurnManager
    {
        void SwitchTurns();
        PieceColor CurrentPlayer { get; }
    }
}
