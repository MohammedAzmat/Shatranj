using ShatranjCore.Abstractions.Commands;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for parsing user input commands
    /// </summary>
    public interface ICommandParser
    {
        GameCommand Parse(string input);
    }
}
