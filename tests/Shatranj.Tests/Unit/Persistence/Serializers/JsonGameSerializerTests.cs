using System;
using Xunit;
using Moq;
using ShatranjCore.Persistence.Serializers;
using ShatranjCore.Persistence;

namespace Shatranj.Tests.Unit.Persistence.Serializers
{
    /// <summary>
    /// Tests for JsonGameSerializer - verifies JSON serialization strategy
    /// </summary>
    public class JsonGameSerializerTests
    {
        private readonly JsonGameSerializer _serializer;

        public JsonGameSerializerTests()
        {
            _serializer = new JsonGameSerializer();
        }

        [Fact]
        public void Constructor_InitializesSerializer()
        {
            // Arrange & Act
            var serializer = new JsonGameSerializer();

            // Assert
            Assert.NotNull(serializer);
        }

        [Fact]
        public void GetFormat_ReturnsJson()
        {
            // Arrange & Act
            var format = _serializer.GetFormat();

            // Assert
            Assert.NotNull(format);
            Assert.Equal("Json", format);
        }

        [Fact]
        public void Serialize_WithValidSnapshot_ProducesString()
        {
            // Arrange
            var snapshot = new GameStateSnapshot { MoveCount = 5 };

            // Act
            var result = _serializer.Serialize(snapshot);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Serialize_EmptySnapshot_ProducesValidOutput()
        {
            // Arrange
            var snapshot = new GameStateSnapshot();

            // Act
            var result = _serializer.Serialize(snapshot);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Serialize_SnapshotWithData_PreservesData()
        {
            // Arrange
            var snapshot = new GameStateSnapshot { MoveCount = 10 };

            // Act
            var serialized = _serializer.Serialize(snapshot);

            // Assert
            Assert.NotNull(serialized);
            Assert.Contains("10", serialized);
        }

        [Fact]
        public void Serialize_MultipleSnapshots_ProduceDifferentResults()
        {
            // Arrange
            var snapshot1 = new GameStateSnapshot { MoveCount = 5 };
            var snapshot2 = new GameStateSnapshot { MoveCount = 10 };

            // Act
            var serialized1 = _serializer.Serialize(snapshot1);
            var serialized2 = _serializer.Serialize(snapshot2);

            // Assert
            Assert.NotEqual(serialized1, serialized2);
        }

        [Fact]
        public void Deserialize_ThrowsNotImplemented()
        {
            // Arrange
            var json = "{\"moveCount\": 5}";

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _serializer.Deserialize(json));
        }

        [Fact]
        public void Serializer_ImplementsInterface()
        {
            // Arrange & Act
            var serializer = _serializer as IGameSerializer;

            // Assert
            Assert.NotNull(serializer);
            Assert.IsAssignableFrom<IGameSerializer>(_serializer);
        }

        [Fact]
        public void Serialize_NullSnapshot_HandlesGracefully()
        {
            // Arrange
            GameStateSnapshot snapshot = null;

            // Act & Assert
            try
            {
                var result = _serializer.Serialize(snapshot);
                Assert.NotNull(result); // If implemented to handle null
            }
            catch (ArgumentNullException)
            {
                // Expected if null not allowed
            }
        }

        [Fact]
        public void Format_ConsistentAcrossCalls()
        {
            // Arrange & Act
            var format1 = _serializer.GetFormat();
            var format2 = _serializer.GetFormat();

            // Assert
            Assert.Equal(format1, format2);
        }

        [Fact]
        public void Serializer_CanBeCreatedFromFactory()
        {
            // Arrange & Act
            var serializer = GameSerializerFactory.Create(SerializationFormat.Json);

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<JsonGameSerializer>(serializer);
            Assert.Equal("Json", serializer.GetFormat());
        }

        [Fact]
        public void Serialize_ConsistentOutput_SameInputYieldsSameOutput()
        {
            // Arrange
            var snapshot = new GameStateSnapshot { MoveCount = 15 };

            // Act
            var result1 = _serializer.Serialize(snapshot);
            var result2 = _serializer.Serialize(snapshot);

            // Assert
            Assert.Equal(result1, result2);
        }
    }
}
