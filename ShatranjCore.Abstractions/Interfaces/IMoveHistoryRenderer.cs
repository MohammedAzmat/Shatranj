namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for rendering move history (separate from storage)
    /// </summary>
    public interface IMoveHistoryRenderer
    {
        void DisplayHistory(IMoveHistory history);
    }
}
