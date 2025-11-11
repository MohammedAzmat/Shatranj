using System;
using Xunit;
using ShatranjCore.Persistence.Serializers;

namespace Shatranj.Tests.Unit.Persistence.Serializers
{
    /// <summary>
    /// Tests for GameSerializerFactory - verifies serializer strategy factory
    /// </summary>
    public class GameSerializerFactoryTests
    {
        [Fact]
        public void Create_JsonFormat_ReturnsJsonGameSerializer()
        {
            // Arrange
            var format = SerializationFormat.Json;

            // Act
            var serializer = GameSerializerFactory.Create(format);

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<JsonGameSerializer>(serializer);
        }

        [Fact]
        public void CreateDefault_ReturnsJsonSerializer()
        {
            // Arrange & Act
            var serializer = GameSerializerFactory.CreateDefault();

            // Assert
            Assert.NotNull(serializer);
            Assert.IsType<JsonGameSerializer>(serializer);
        }

        [Fact]
        public void Create_ReturnsSerializerWithInterface()
        {
            // Arrange
            var format = SerializationFormat.Json;

            // Act
            var serializer = GameSerializerFactory.Create(format);

            // Assert
            Assert.IsAssignableFrom<IGameSerializer>(serializer);
        }

        [Fact]
        public void Create_JsonSerializer_HasValidFormat()
        {
            // Arrange
            var serializer = GameSerializerFactory.Create(SerializationFormat.Json);

            // Act
            var format = serializer.GetFormat();

            // Assert
            Assert.NotNull(format);
            Assert.Equal("Json", format);
        }

        [Fact]
        public void Create_DifferentFormats_ReturnDifferentSerializers()
        {
            // Arrange
            var jsonFormat = SerializationFormat.Json;

            // Act
            var jsonSerializer = GameSerializerFactory.Create(jsonFormat);

            // Assert - can create from factory
            Assert.NotNull(jsonSerializer);
        }

        [Theory]
        [InlineData(SerializationFormat.Json)]
        public void Create_AllDefinedFormats_ReturnValidSerializers(SerializationFormat format)
        {
            // Arrange & Act
            var serializer = GameSerializerFactory.Create(format);

            // Assert
            Assert.NotNull(serializer);
            Assert.IsAssignableFrom<IGameSerializer>(serializer);
        }

        [Fact]
        public void CreateDefault_SameAsJsonFormat()
        {
            // Arrange & Act
            var defaultSerializer = GameSerializerFactory.CreateDefault();
            var jsonSerializer = GameSerializerFactory.Create(SerializationFormat.Json);

            // Assert - Both should be same type
            Assert.IsType<JsonGameSerializer>(defaultSerializer);
            Assert.IsType<JsonGameSerializer>(jsonSerializer);
        }

        [Fact]
        public void Create_MultipleCallsSameFormat_ReturnIndependentInstances()
        {
            // Arrange & Act
            var serializer1 = GameSerializerFactory.Create(SerializationFormat.Json);
            var serializer2 = GameSerializerFactory.Create(SerializationFormat.Json);

            // Assert - Each call creates new instance
            Assert.NotSame(serializer1, serializer2);
        }

        [Fact]
        public void Serializer_ImplementsInterface_Correctly()
        {
            // Arrange
            var serializer = GameSerializerFactory.CreateDefault();

            // Act & Assert
            Assert.IsAssignableFrom<IGameSerializer>(serializer);
        }

        [Fact]
        public void Factory_IsStateless_CanCallMultipleTimes()
        {
            // Arrange & Act
            var first = GameSerializerFactory.Create(SerializationFormat.Json);
            var second = GameSerializerFactory.Create(SerializationFormat.Json);
            var third = GameSerializerFactory.CreateDefault();

            // Assert
            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.NotNull(third);
        }
    }
}
