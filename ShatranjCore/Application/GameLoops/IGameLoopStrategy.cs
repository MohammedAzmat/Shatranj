using ShatranjCore.Board;

namespace ShatranjCore.Application.GameLoops
{
    /// <summary>
    /// Strategy interface for different game loop implementations.
    /// Allows different chess variants (standard, Chess960, etc) to implement their own rules.
    /// Single Responsibility: Define contract for game loop behavior.
    /// </summary>
    public interface IGameLoopStrategy
    {
        /// <summary>
        /// Execute the main game loop with this strategy.
        /// </summary>
        void Execute();

        /// <summary>
        /// Get the name of this game variant.
        /// </summary>
        string GetVariantName();
    }
}
