using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.UI;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Game Control commands (Quit, EndGame, RestartGame, etc).
    /// Single Responsibility: Game lifecycle control.
    /// </summary>
    public class GameControlCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;

        private Action quitDelegate;
        private Action endGameDelegate;
        private Action restartGameDelegate;
        private Action waitForKeyDelegate;

        private PieceColor currentPlayer;

        public GameControlCommandHandler(
            ConsoleBoardRenderer renderer,
            ILogger logger)
        {
            this.renderer = renderer;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for game control operations.
        /// </summary>
        public void SetDelegates(
            Action quit,
            Action endGame,
            Action restartGame,
            Action waitForKey,
            PieceColor currentPlayer)
        {
            this.quitDelegate = quit;
            this.endGameDelegate = endGame;
            this.restartGameDelegate = restartGame;
            this.waitForKeyDelegate = waitForKey;
            this.currentPlayer = currentPlayer;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.Quit
                || command.Type == CommandType.EndGame
                || command.Type == CommandType.RestartGame;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"GameControlCommandHandler cannot handle {command.Type}");

            try
            {
                switch (command.Type)
                {
                    case CommandType.Quit:
                        logger.Info($"{currentPlayer} player quit the game");
                        renderer.DisplayInfo("Thanks for playing Shatranj!");
                        quitDelegate?.Invoke();
                        break;

                    case CommandType.EndGame:
                        logger.Info($"{currentPlayer} player ended the game");
                        renderer.DisplayInfo("Game ended.");
                        endGameDelegate?.Invoke();
                        break;

                    case CommandType.RestartGame:
                        logger.Info("Game restart requested");
                        renderer.DisplayInfo("Restarting game...");
                        restartGameDelegate?.Invoke();
                        break;
                }
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing game control command: {ex.Message}");
                logger.Error("Game control command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }
    }
}
