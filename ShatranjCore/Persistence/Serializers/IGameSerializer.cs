using ShatranjCore.Persistence;

namespace ShatranjCore.Persistence.Serializers
{
    /// <summary>
    /// Strategy interface for different game serialization formats.
    /// Allows swapping between JSON, Binary, XML, etc. without modifying callers.
    /// Single Responsibility: Define serialization contract.
    /// </summary>
    public interface IGameSerializer
    {
        /// <summary>
        /// Serialize game state to string format.
        /// </summary>
        string Serialize(GameStateSnapshot snapshot);

        /// <summary>
        /// Deserialize game state from string format.
        /// </summary>
        GameStateSnapshot Deserialize(string data);

        /// <summary>
        /// Get the format name (JSON, Binary, XML, etc).
        /// </summary>
        string GetFormat();
    }
}
