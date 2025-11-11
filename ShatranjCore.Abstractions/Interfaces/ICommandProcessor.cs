namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for processing game commands
    /// </summary>
    public interface ICommandProcessor
    {
        void ProcessCommand(string input);
    }
}
