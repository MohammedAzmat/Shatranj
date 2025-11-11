using ShatranjCore.Persistence;

namespace ShatranjCore.State
{
    /// <summary>
    /// Write-only interface for modifying game state.
    /// Clients that only need to change state depend on this interface.
    /// Follows Interface Segregation Principle: clients only see what they need.
    /// </summary>
    public interface IGameStateModifier
    {
        /// <summary>
        /// Records a new game state snapshot to history.
        /// </summary>
        /// <param name="snapshot">The state snapshot to record</param>
        void RecordState(GameStateSnapshot snapshot);

        /// <summary>
        /// Performs autosave of the current game state.
        /// </summary>
        /// <param name="snapshot">The state snapshot to autosave</param>
        void Autosave(GameStateSnapshot snapshot);

        /// <summary>
        /// Rolls back to the previous game state.
        /// </summary>
        /// <returns>The previous state snapshot, or null if rollback is not possible</returns>
        GameStateSnapshot Rollback();

        /// <summary>
        /// Redoes the last undone turn.
        /// </summary>
        /// <returns>The redo state snapshot, or null if redo is not possible</returns>
        GameStateSnapshot Redo();

        /// <summary>
        /// Clears the redo stack (typically called after a new move is made).
        /// </summary>
        void ClearRedoStack();

        /// <summary>
        /// Cleans up autosave file (typically called when game ends).
        /// </summary>
        void CleanupAutosave();

        /// <summary>
        /// Clears all state history and redo stack.
        /// </summary>
        void ClearAll();
    }
}
