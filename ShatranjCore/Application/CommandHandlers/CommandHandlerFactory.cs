using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Factory for creating and managing command handlers.
    /// Instantiates all handlers and routes commands to appropriate handlers.
    /// Single Responsibility: Handler lifecycle and command routing.
    /// </summary>
    public class CommandHandlerFactory
    {
        private readonly Dictionary<CommandType, ICommandHandler> handlers;

        public CommandHandlerFactory(
            ConsoleBoardRenderer renderer,
            IChessBoard board,
            CheckDetector checkDetector,
            CastlingValidator castlingValidator,
            MoveHistory moveHistory,
            ILogger logger)
        {
            handlers = new Dictionary<CommandType, ICommandHandler>();

            // Initialize all handlers
            var moveHandler = new MoveCommandHandler(renderer, board, checkDetector, logger);
            var castleHandler = new CastleCommandHandler(renderer, board, castlingValidator, logger);
            var uiHandler = new UICommandHandler(renderer, moveHistory, logger);
            var persistenceHandler = new PersistenceCommandHandler(renderer, logger);
            var gameControlHandler = new GameControlCommandHandler(renderer, logger);
            var settingsHandler = new SettingsCommandHandler(renderer, logger);
            var invalidHandler = new InvalidCommandHandler(renderer, logger);

            // Register handlers by the command types they handle
            RegisterHandler(moveHandler, CommandType.Move);
            RegisterHandler(castleHandler, CommandType.Castle);
            RegisterHandler(uiHandler, CommandType.ShowHelp);
            RegisterHandler(uiHandler, CommandType.ShowHistory);
            RegisterHandler(uiHandler, CommandType.ShowSettings);
            RegisterHandler(uiHandler, CommandType.ShowMoves);
            RegisterHandler(persistenceHandler, CommandType.SaveGame);
            RegisterHandler(persistenceHandler, CommandType.LoadGame);
            RegisterHandler(persistenceHandler, CommandType.Rollback);
            RegisterHandler(persistenceHandler, CommandType.Redo);
            RegisterHandler(gameControlHandler, CommandType.Quit);
            RegisterHandler(gameControlHandler, CommandType.EndGame);
            RegisterHandler(gameControlHandler, CommandType.RestartGame);
            RegisterHandler(settingsHandler, CommandType.SetDifficulty);
            RegisterHandler(settingsHandler, CommandType.SetProfile);
            RegisterHandler(settingsHandler, CommandType.SetOpponent);
            RegisterHandler(settingsHandler, CommandType.ResetSettings);
            RegisterHandler(invalidHandler, CommandType.Invalid);
        }

        /// <summary>
        /// Register a handler for one or more command types.
        /// </summary>
        private void RegisterHandler(ICommandHandler handler, CommandType commandType)
        {
            handlers[commandType] = handler;
        }

        /// <summary>
        /// Get the handler for a command type.
        /// </summary>
        public ICommandHandler GetHandler(CommandType commandType)
        {
            if (handlers.TryGetValue(commandType, out var handler))
            {
                return handler;
            }

            // Return invalid handler if type not found
            return handlers[CommandType.Invalid];
        }

        /// <summary>
        /// Set delegates for all handlers that need them.
        /// This must be called after factory creation and before processing commands.
        /// </summary>
        public void SetHandlerDelegates(
            Action<Location, Location, Piece> executeMove,
            Action switchTurns,
            Action saveGame,
            Action<int> loadGame,
            Action rollback,
            Action redo,
            Action showSettings,
            Action waitForKey,
            Action endGame,
            Action restartGame,
            Func<GameCommand, CastlingSide?> promptForCastlingSide,
            PieceColor currentPlayer)
        {
            // Move handler
            var moveHandler = handlers[CommandType.Move] as MoveCommandHandler;
            if (moveHandler != null)
            {
                moveHandler.SetDelegates(executeMove, switchTurns, waitForKey, currentPlayer);
            }

            // Castle handler
            var castleHandler = handlers[CommandType.Castle] as CastleCommandHandler;
            if (castleHandler != null)
            {
                castleHandler.SetDelegates(switchTurns, waitForKey, promptForCastlingSide, currentPlayer);
            }

            // UI handler
            var uiHandler = handlers[CommandType.ShowHelp] as UICommandHandler;
            if (uiHandler != null)
            {
                uiHandler.SetDelegates(waitForKey, showSettings);
            }

            // Persistence handler
            var persistenceHandler = handlers[CommandType.SaveGame] as PersistenceCommandHandler;
            if (persistenceHandler != null)
            {
                persistenceHandler.SetDelegates(saveGame, loadGame, rollback, redo, waitForKey);
            }

            // Game control handler
            var gameControlHandler = handlers[CommandType.Quit] as GameControlCommandHandler;
            if (gameControlHandler != null)
            {
                gameControlHandler.SetDelegates(
                    () => { /* quit implementation */ },
                    endGame,
                    restartGame,
                    waitForKey,
                    currentPlayer);
            }

            // Settings handler
            var settingsHandler = handlers[CommandType.SetDifficulty] as SettingsCommandHandler;
            if (settingsHandler != null)
            {
                settingsHandler.SetDelegates(
                    level => { /* set difficulty */ },
                    profile => { /* set profile */ },
                    opponent => { /* set opponent */ },
                    () => { /* reset settings */ },
                    waitForKey);
            }

            // Invalid handler
            var invalidHandler = handlers[CommandType.Invalid] as InvalidCommandHandler;
            if (invalidHandler != null)
            {
                invalidHandler.SetDelegates(waitForKey);
            }
        }
    }
}
