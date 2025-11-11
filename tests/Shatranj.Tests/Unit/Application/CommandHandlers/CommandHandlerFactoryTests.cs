using System;
using Xunit;
using ShatranjCore.Application.CommandHandlers;
using ShatranjCore.Abstractions.Commands;
using Moq;

namespace Shatranj.Tests.Unit.Application.CommandHandlers
{
    /// <summary>
    /// Tests for CommandHandlerFactory - verifies handler routing and registration
    /// </summary>
    public class CommandHandlerFactoryTests
    {
        private readonly CommandHandlerFactory _factory;

        public CommandHandlerFactoryTests()
        {
            _factory = new CommandHandlerFactory();
        }

        [Fact]
        public void Constructor_InitializesFactory_WithoutError()
        {
            // Arrange & Act
            var factory = new CommandHandlerFactory();

            // Assert
            Assert.NotNull(factory);
        }

        [Fact]
        public void GetHandler_MoveCommand_ReturnsMoveCommandHandler()
        {
            // Arrange
            var command = new GameCommand { Type = CommandType.Move, Parameters = "e2 e4" };

            // Act
            var handler = _factory.GetHandler(CommandType.Move);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<MoveCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_CastleCommand_ReturnsCastleCommandHandler()
        {
            // Arrange
            var commandType = CommandType.Castle;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<CastleCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_SaveCommand_ReturnsPersistenceCommandHandler()
        {
            // Arrange
            var commandType = CommandType.Save;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<PersistenceCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_LoadCommand_ReturnsPersistenceCommandHandler()
        {
            // Arrange
            var commandType = CommandType.Load;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<PersistenceCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_ShowHelpCommand_ReturnsUICommandHandler()
        {
            // Arrange
            var commandType = CommandType.ShowHelp;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<UICommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_QuitCommand_ReturnsGameControlCommandHandler()
        {
            // Arrange
            var commandType = CommandType.Quit;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<GameControlCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_SetDifficultyCommand_ReturnsSettingsCommandHandler()
        {
            // Arrange
            var commandType = CommandType.SetDifficulty;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<SettingsCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_InvalidCommandType_ReturnsInvalidCommandHandler()
        {
            // Arrange
            var commandType = CommandType.Invalid;

            // Act
            var handler = _factory.GetHandler(commandType);

            // Assert
            Assert.NotNull(handler);
            Assert.IsType<InvalidCommandHandler>(handler);
        }

        [Fact]
        public void GetHandler_AllCommandTypes_ReturnValidHandlers()
        {
            // Arrange
            var commandTypes = Enum.GetValues(typeof(CommandType)) as CommandType[];

            // Act & Assert
            foreach (var commandType in commandTypes)
            {
                var handler = _factory.GetHandler(commandType);
                Assert.NotNull(handler);
                Assert.Implements<ICommandHandler>(handler);
            }
        }

        [Fact]
        public void GetHandler_SameCommandType_ReturnsSameHandlerInstance()
        {
            // Arrange
            var commandType = CommandType.Move;

            // Act
            var handler1 = _factory.GetHandler(commandType);
            var handler2 = _factory.GetHandler(commandType);

            // Assert - handlers are singletons per type
            Assert.Same(handler1, handler2);
        }

        [Fact]
        public void GetHandler_DifferentCommandTypes_ReturnDifferentHandlers()
        {
            // Arrange
            var moveType = CommandType.Move;
            var castleType = CommandType.Castle;

            // Act
            var moveHandler = _factory.GetHandler(moveType);
            var castleHandler = _factory.GetHandler(castleType);

            // Assert
            Assert.NotSame(moveHandler, castleHandler);
        }
    }
}
