namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for game command operations
    /// </summary>
    public interface IGameCommands
    {
        void SaveGame();
        void LoadGame(int gameId);
        void ShowSettings();
        void Rollback();
        void Redo();
        void ShowHistory();
    }
}
