using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.UI;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Invalid commands.
    /// Single Responsibility: Display error for invalid commands.
    /// </summary>
    public class InvalidCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;

        private Action waitForKeyDelegate;

        public InvalidCommandHandler(
            ConsoleBoardRenderer renderer,
            ILogger logger)
        {
            this.renderer = renderer;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for user interaction.
        /// </summary>
        public void SetDelegates(Action waitForKey)
        {
            this.waitForKeyDelegate = waitForKey;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.Invalid;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"InvalidCommandHandler cannot handle {command.Type}");

            try
            {
                logger.Debug("Invalid command received");
                renderer.DisplayError(command.ErrorMessage ?? "Invalid command. Type 'help' for available commands.");
                waitForKeyDelegate?.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error("Error handling invalid command", ex);
            }
        }
    }
}
