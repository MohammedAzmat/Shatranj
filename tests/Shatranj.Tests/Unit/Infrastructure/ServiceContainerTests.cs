using System;
using Xunit;
using Moq;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Logging;

namespace Shatranj.Tests.Unit.Infrastructure
{
    /// <summary>
    /// Tests for ServiceContainer - verifies DI container functionality
    /// </summary>
    public class ServiceContainerTests
    {
        [Fact]
        public void Register_Singleton_ReturnsInstance()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger = new ConsoleLogger();

            // Act
            container.Register(typeof(ILogger), logger);
            var retrieved = container.GetService(typeof(ILogger));

            // Assert
            Assert.NotNull(retrieved);
            Assert.Same(logger, retrieved);
        }

        [Fact]
        public void Register_SameSingleton_ReturnsSameInstance()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger = new ConsoleLogger();
            container.Register(typeof(ILogger), logger);

            // Act
            var first = container.GetService(typeof(ILogger));
            var second = container.GetService(typeof(ILogger));

            // Assert
            Assert.Same(first, second);
        }

        [Fact]
        public void RegisterNamed_NamedService_RetrievableByName()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger = new ConsoleLogger();
            var serviceName = "primaryLogger";

            // Act
            container.Register(serviceName, logger);
            var retrieved = container.GetNamedService(serviceName);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Same(logger, retrieved);
        }

        [Fact]
        public void GetService_NotRegistered_ReturnsNull()
        {
            // Arrange
            var container = new ServiceContainer();

            // Act
            var service = container.GetService(typeof(IChessBoard));

            // Assert
            Assert.Null(service);
        }

        [Fact]
        public void GetNamedService_NotRegistered_ReturnsNull()
        {
            // Arrange
            var container = new ServiceContainer();

            // Act
            var service = container.GetNamedService("nonexistent");

            // Assert
            Assert.Null(service);
        }

        [Fact]
        public void Register_MultipleServices_AllRetrievable()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger = new ConsoleLogger();
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            container.Register(typeof(ILogger), logger);
            container.Register(typeof(IChessBoard), board);
            var retrievedLogger = container.GetService(typeof(ILogger));
            var retrievedBoard = container.GetService(typeof(IChessBoard));

            // Assert
            Assert.Same(logger, retrievedLogger);
            Assert.Same(board, retrievedBoard);
        }

        [Fact]
        public void Register_DifferentServices_ReturnDifferentInstances()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger1 = new ConsoleLogger();
            var logger2 = new ConsoleLogger();

            // Act
            container.Register("logger1", logger1);
            container.Register("logger2", logger2);
            var retrieved1 = container.GetNamedService("logger1");
            var retrieved2 = container.GetNamedService("logger2");

            // Assert
            Assert.NotSame(retrieved1, retrieved2);
            Assert.Same(logger1, retrieved1);
            Assert.Same(logger2, retrieved2);
        }

        [Fact]
        public void Register_NullService_DoesNotCrash()
        {
            // Arrange
            var container = new ServiceContainer();

            // Act & Assert - should handle gracefully
            container.Register(typeof(ILogger), null);
            var service = container.GetService(typeof(ILogger));
            Assert.Null(service);
        }

        [Fact]
        public void GetService_WithValidType_ReturnsCorrectType()
        {
            // Arrange
            var container = new ServiceContainer();
            var logger = new ConsoleLogger();
            container.Register(typeof(ILogger), logger);

            // Act
            var service = container.GetService(typeof(ILogger));

            // Assert
            Assert.NotNull(service);
            Assert.IsAssignableFrom<ILogger>(service);
        }

        [Fact]
        public void Container_IsDisposable_ImplementsInterface()
        {
            // Arrange & Act
            var container = new ServiceContainer();

            // Assert
            Assert.IsAssignableFrom<IServiceProvider>(container);
        }

        [Fact]
        public void RegisterCoreServices_ReturnsContainer()
        {
            // Arrange & Act
            var provider = ServiceRegistration.RegisterCoreServices();

            // Assert
            Assert.NotNull(provider);
            Assert.IsAssignableFrom<IServiceProvider>(provider);
        }

        [Fact]
        public void RegisterCoreServices_RegistersLogger()
        {
            // Arrange & Act
            var provider = ServiceRegistration.RegisterCoreServices();
            var logger = provider.GetService(typeof(ILogger));

            // Assert
            Assert.NotNull(logger);
            Assert.IsAssignableFrom<ILogger>(logger);
        }

        [Fact]
        public void RegisterCoreServices_RegistersBoard()
        {
            // Arrange & Act
            var provider = ServiceRegistration.RegisterCoreServices();
            var board = provider.GetService(typeof(IChessBoard));

            // Assert
            Assert.NotNull(board);
            Assert.IsAssignableFrom<IChessBoard>(board);
        }

        [Fact]
        public void RegisterCoreServices_RegistersMultipleServices()
        {
            // Arrange & Act
            var provider = ServiceRegistration.RegisterCoreServices();

            // Assert - Verify key services are registered
            var logger = provider.GetService(typeof(ILogger));
            var board = provider.GetService(typeof(IChessBoard));

            Assert.NotNull(logger);
            Assert.NotNull(board);
        }

        [Fact]
        public void RegisterWithAI_AddsAIServices()
        {
            // Arrange
            var provider = ServiceRegistration.RegisterCoreServices();

            // Act
            var aiProvider = ServiceRegistration.RegisterWithAI(provider);

            // Assert
            Assert.NotNull(aiProvider);
        }
    }
}
