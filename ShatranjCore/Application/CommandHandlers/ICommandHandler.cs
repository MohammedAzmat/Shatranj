using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Base interface for all command handlers.
    /// Each command type gets its own handler implementation.
    /// This follows the Single Responsibility Principle.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Handles the given command.
        /// </summary>
        void Handle(GameCommand command);

        /// <summary>
        /// Returns true if this handler can process the given command.
        /// </summary>
        bool CanHandle(GameCommand command);
    }
}
