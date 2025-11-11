namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for managing game state (undo/redo/autosave)
    /// </summary>
    public interface IGameStateManager
    {
        void RecordState(object snapshot);
        object Rollback();
        object Redo();
        void ClearRedoStack();
        void Autosave(object snapshot);
        void CleanupAutosave();
        bool CanRollback();
        bool CanRedo();
    }
}
