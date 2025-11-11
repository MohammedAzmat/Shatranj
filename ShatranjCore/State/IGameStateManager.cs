namespace ShatranjCore.State
{
    /// <summary>
    /// Combined interface for full game state access.
    /// Used when a component needs both read and write capabilities.
    /// Implements both IGameStateQuery and IGameStateModifier.
    /// </summary>
    public interface IGameStateManager : IGameStateQuery, IGameStateModifier
    {
    }
}
