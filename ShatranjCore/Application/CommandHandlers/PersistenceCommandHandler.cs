using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.UI;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Persistence-related commands (Save, Load, Rollback, Redo).
    /// Single Responsibility: Delegate to save/load management.
    /// </summary>
    public class PersistenceCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;

        private Action saveGameDelegate;
        private Action<int> loadGameDelegate;
        private Action rollbackDelegate;
        private Action redoDelegate;
        private Action waitForKeyDelegate;

        public PersistenceCommandHandler(
            ConsoleBoardRenderer renderer,
            ILogger logger)
        {
            this.renderer = renderer;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for game state operations.
        /// </summary>
        public void SetDelegates(
            Action saveGame,
            Action<int> loadGame,
            Action rollback,
            Action redo,
            Action waitForKey)
        {
            this.saveGameDelegate = saveGame;
            this.loadGameDelegate = loadGame;
            this.rollbackDelegate = rollback;
            this.redoDelegate = redo;
            this.waitForKeyDelegate = waitForKey;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.SaveGame
                || command.Type == CommandType.LoadGame
                || command.Type == CommandType.Rollback
                || command.Type == CommandType.Redo;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"PersistenceCommandHandler cannot handle {command.Type}");

            try
            {
                switch (command.Type)
                {
                    case CommandType.SaveGame:
                        logger.Info("Save game command received");
                        saveGameDelegate?.Invoke();
                        break;

                    case CommandType.LoadGame:
                        if (int.TryParse(command.FileName, out int gameId))
                        {
                            logger.Info($"Load game command received for game ID: {gameId}");
                            loadGameDelegate?.Invoke(gameId);
                        }
                        else
                        {
                            renderer.DisplayError("Invalid game ID for load command");
                            waitForKeyDelegate?.Invoke();
                        }
                        break;

                    case CommandType.Rollback:
                        logger.Info("Rollback command received");
                        rollbackDelegate?.Invoke();
                        break;

                    case CommandType.Redo:
                        logger.Info("Redo command received");
                        redoDelegate?.Invoke();
                        break;
                }
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing persistence command: {ex.Message}");
                logger.Error("Persistence command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }
    }
}
