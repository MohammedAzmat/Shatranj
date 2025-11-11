using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.Movement;
using ShatranjCore.UI;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles UI-related commands (Help, History, Settings).
    /// Single Responsibility: Display information and user interface elements.
    /// </summary>
    public class UICommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly MoveHistory moveHistory;
        private readonly ILogger logger;

        private Action waitForKeyDelegate;
        private Action showSettingsDelegate;

        public UICommandHandler(
            ConsoleBoardRenderer renderer,
            MoveHistory moveHistory,
            ILogger logger)
        {
            this.renderer = renderer;
            this.moveHistory = moveHistory;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for actions that need external implementation.
        /// </summary>
        public void SetDelegates(
            Action waitForKey,
            Action showSettings)
        {
            this.waitForKeyDelegate = waitForKey;
            this.showSettingsDelegate = showSettings;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.ShowHelp
                || command.Type == CommandType.ShowHistory
                || command.Type == CommandType.ShowSettings
                || command.Type == CommandType.ShowMoves;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"UICommandHandler cannot handle {command.Type}");

            try
            {
                switch (command.Type)
                {
                    case CommandType.ShowHelp:
                        logger.Debug("Displaying help menu");
                        renderer.DisplayCommands();
                        waitForKeyDelegate?.Invoke();
                        break;

                    case CommandType.ShowHistory:
                        logger.Debug("Displaying move history");
                        moveHistory.DisplayHistory();
                        waitForKeyDelegate?.Invoke();
                        break;

                    case CommandType.ShowSettings:
                        logger.Debug("Displaying settings");
                        showSettingsDelegate?.Invoke();
                        break;

                    case CommandType.ShowMoves:
                        logger.Debug("Show moves not yet implemented");
                        renderer.DisplayInfo("Show moves not yet implemented in CommandProcessor");
                        waitForKeyDelegate?.Invoke();
                        break;
                }
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing UI command: {ex.Message}");
                logger.Error("UI command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }
    }
}
