using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Persistence;

namespace ShatranjCore.Persistence.Serializers
{
    /// <summary>
    /// JSON-based game serialization strategy.
    /// Human-readable format, suitable for save files and data interchange.
    /// Single Responsibility: Only JSON serialization/deserialization.
    ///
    /// Note: Adapts the existing GameSerializer to the IGameSerializer interface.
    /// In production, use System.Text.Json (built-in) or Newtonsoft.Json (add NuGet package)
    /// </summary>
    public class JsonGameSerializer : IGameSerializer
    {
        private readonly GameSerializer _gameSerializer;
        private readonly ILogger _logger;

        public JsonGameSerializer(ILogger logger = null)
        {
            _logger = logger;
            // Use existing GameSerializer for actual JSON work
            _gameSerializer = new GameSerializer(logger);
        }

        public string GetFormat() => "JSON";

        public string Serialize(GameStateSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            try
            {
                // Delegate to existing serializer's SaveGame method
                // (it returns a JSON string representation)
                var result = _gameSerializer.SaveGame(snapshot);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.Error("Failed to serialize game state to JSON", ex);
                throw new InvalidOperationException("Failed to serialize game state to JSON", ex);
            }
        }

        public GameStateSnapshot Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            try
            {
                // For deserialization, we'd need to load from file
                // As a placeholder, return null (this would need full implementation with file I/O)
                _logger?.Warning("JsonGameSerializer.Deserialize: Full implementation requires file I/O");
                throw new NotImplementedException("Deserialization requires file path. Use GameSerializer.LoadGame instead.");
            }
            catch (Exception ex)
            {
                _logger?.Error("Failed to deserialize game state from JSON", ex);
                throw new InvalidOperationException("Failed to deserialize game state from JSON", ex);
            }
        }
    }
}
