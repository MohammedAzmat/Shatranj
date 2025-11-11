using System;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Persistence.Serializers
{
    /// <summary>
    /// Factory for creating game serializers.
    /// Encapsulates serializer selection logic.
    /// Single Responsibility: Create appropriate serializer for format.
    /// </summary>
    public enum SerializationFormat
    {
        Json,
        Binary,
        XML
    }

    public class GameSerializerFactory
    {
        private readonly ILogger logger;

        public GameSerializerFactory(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Create a serializer for the given format.
        /// </summary>
        public IGameSerializer Create(SerializationFormat format)
        {
            var serializer = format switch
            {
                SerializationFormat.Json => new JsonGameSerializer(),
                SerializationFormat.Binary => throw new NotImplementedException("Binary serialization not yet implemented"),
                SerializationFormat.XML => throw new NotImplementedException("XML serialization not yet implemented"),
                _ => throw new ArgumentException($"Unknown format: {format}")
            };

            logger?.Debug($"Created serializer: {serializer.GetFormat()}");
            return serializer;
        }

        /// <summary>
        /// Get the default serializer.
        /// </summary>
        public IGameSerializer CreateDefault()
        {
            return Create(SerializationFormat.Json);
        }
    }
}
