using ShatranjCore.Persistence;

namespace ShatranjCore.State
{
    /// <summary>
    /// Read-only interface for querying game state.
    /// Clients that only need to read state depend on this interface.
    /// Follows Interface Segregation Principle: clients only see what they need.
    /// </summary>
    public interface IGameStateQuery
    {
        /// <summary>
        /// Checks if rollback (undo) is possible.
        /// </summary>
        /// <returns>True if there is a previous state available</returns>
        bool CanRollback();

        /// <summary>
        /// Checks if redo is possible.
        /// </summary>
        /// <returns>True if there is a state in the redo stack</returns>
        bool CanRedo();

        /// <summary>
        /// Gets the number of states in history.
        /// </summary>
        /// <returns>The count of state snapshots</returns>
        int GetStateCount();

        /// <summary>
        /// Gets the current state (most recent snapshot).
        /// </summary>
        /// <returns>The most recent state snapshot, or null if no history exists</returns>
        GameStateSnapshot GetCurrentState();
    }
}
