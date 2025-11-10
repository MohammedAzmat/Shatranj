using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore.Persistence;

namespace ShatranjCore.State
{
    /// <summary>
    /// Manages game state snapshots, autosave, rollback, and redo functionality.
    /// Maintains a history of the last 10 turns and provides undo/redo capabilities.
    /// </summary>
    public class GameStateManager
    {
        private readonly SaveGameManager saveManager;
        private readonly ILogger logger;
        private readonly List<GameStateSnapshot> stateHistory;
        private readonly Stack<GameStateSnapshot> redoStack;
        private const int MaxHistorySize = 10;

        /// <summary>
        /// Initializes a new GameStateManager
        /// </summary>
        /// <param name="saveManager">Manager for saving game states</param>
        /// <param name="logger">Logger for recording state operations</param>
        public GameStateManager(SaveGameManager saveManager, ILogger logger)
        {
            this.saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.stateHistory = new List<GameStateSnapshot>();
            this.redoStack = new Stack<GameStateSnapshot>();
        }

        /// <summary>
        /// Records a new game state snapshot to history
        /// </summary>
        /// <param name="snapshot">The state snapshot to record</param>
        public void RecordState(GameStateSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            stateHistory.Add(snapshot);

            // Maintain maximum history size (keep last 10)
            if (stateHistory.Count > MaxHistorySize)
            {
                stateHistory.RemoveAt(0);
            }

            logger.Debug($"State recorded. Turn {snapshot.MoveCount}, History size: {stateHistory.Count}");
        }

        /// <summary>
        /// Performs autosave of the current game state
        /// </summary>
        /// <param name="snapshot">The state snapshot to autosave</param>
        public void Autosave(GameStateSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            try
            {
                saveManager.SaveAutosave(snapshot);
                logger.Debug($"Autosave completed. Turn {snapshot.MoveCount}");
            }
            catch (Exception ex)
            {
                logger.Warning($"Autosave failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Rolls back to the previous game state
        /// </summary>
        /// <returns>The previous state snapshot, or null if rollback is not possible</returns>
        public GameStateSnapshot Rollback()
        {
            if (stateHistory.Count < 2)
            {
                logger.Info("Rollback failed - insufficient state history");
                return null;
            }

            // Get the current and previous states
            GameStateSnapshot currentState = stateHistory[stateHistory.Count - 1];
            GameStateSnapshot previousState = stateHistory[stateHistory.Count - 2];

            // Push current state to redo stack before rolling back
            redoStack.Push(currentState);

            // Remove current state from history
            stateHistory.RemoveAt(stateHistory.Count - 1);

            logger.Info($"Game rolled back to turn {previousState.MoveCount}");
            return previousState;
        }

        /// <summary>
        /// Redoes the last undone turn
        /// </summary>
        /// <returns>The redo state snapshot, or null if redo is not possible</returns>
        public GameStateSnapshot Redo()
        {
            if (redoStack.Count == 0)
            {
                logger.Info("Redo failed - redo stack is empty");
                return null;
            }

            // Pop the state from redo stack
            GameStateSnapshot redoState = redoStack.Pop();

            // Add it back to state history
            stateHistory.Add(redoState);

            logger.Info($"Game redone to turn {redoState.MoveCount}");
            return redoState;
        }

        /// <summary>
        /// Clears the redo stack (typically called after a new move is made)
        /// </summary>
        public void ClearRedoStack()
        {
            if (redoStack.Count > 0)
            {
                redoStack.Clear();
                logger.Debug("Redo stack cleared");
            }
        }

        /// <summary>
        /// Cleans up autosave file (typically called when game ends)
        /// </summary>
        public void CleanupAutosave()
        {
            try
            {
                saveManager.DeleteAutosave();
                logger.Info("Autosave file cleaned up");
            }
            catch (Exception ex)
            {
                logger.Warning($"Failed to cleanup autosave: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if rollback is possible
        /// </summary>
        /// <returns>True if there is a previous state available</returns>
        public bool CanRollback()
        {
            return stateHistory.Count >= 2;
        }

        /// <summary>
        /// Checks if redo is possible
        /// </summary>
        /// <returns>True if there is a state in the redo stack</returns>
        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        /// <summary>
        /// Gets the number of states in history
        /// </summary>
        /// <returns>The count of state snapshots</returns>
        public int GetStateCount()
        {
            return stateHistory.Count;
        }

        /// <summary>
        /// Gets the current state (most recent snapshot)
        /// </summary>
        /// <returns>The most recent state snapshot, or null if no history exists</returns>
        public GameStateSnapshot GetCurrentState()
        {
            return stateHistory.Count > 0 ? stateHistory[stateHistory.Count - 1] : null;
        }

        /// <summary>
        /// Clears all state history and redo stack
        /// </summary>
        public void ClearAll()
        {
            stateHistory.Clear();
            redoStack.Clear();
            logger.Info("All state history cleared");
        }
    }
}
