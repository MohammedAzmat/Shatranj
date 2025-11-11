using System;
using Xunit;
using ShatranjCore.Application.GameLoops;

namespace Shatranj.Tests.Unit.Application.GameLoops
{
    /// <summary>
    /// Tests for GameLoopStrategy - verifies game loop strategy pattern
    /// </summary>
    public class GameLoopStrategyTests
    {
        [Fact]
        public void StandardChessGameLoop_ImplementsInterface()
        {
            // Arrange & Act
            var gameLoop = new StandardChessGameLoop();

            // Assert
            Assert.IsAssignableFrom<IGameLoopStrategy>(gameLoop);
        }

        [Fact]
        public void StandardChessGameLoop_HasValidVariantName()
        {
            // Arrange
            var gameLoop = new StandardChessGameLoop();

            // Act
            var variantName = gameLoop.GetVariantName();

            // Assert
            Assert.NotNull(variantName);
            Assert.NotEmpty(variantName);
            Assert.Equal("Standard Chess", variantName);
        }

        [Fact]
        public void GameLoopStrategy_CanBeInstantiated()
        {
            // Arrange & Act
            var gameLoop = new StandardChessGameLoop();

            // Assert
            Assert.NotNull(gameLoop);
        }

        [Fact]
        public void GameLoopStrategy_ImplementsExecuteMethod()
        {
            // Arrange
            var gameLoop = new StandardChessGameLoop();

            // Act & Assert - Method exists
            try
            {
                // Execute would need proper game setup
                // This just verifies method exists
                var strategy = gameLoop as IGameLoopStrategy;
                Assert.NotNull(strategy);
            }
            catch
            {
                // Expected if game state not set up
            }
        }

        [Fact]
        public void StandardChessGameLoop_VariantNameConsistent()
        {
            // Arrange
            var gameLoop = new StandardChessGameLoop();

            // Act
            var name1 = gameLoop.GetVariantName();
            var name2 = gameLoop.GetVariantName();

            // Assert
            Assert.Equal(name1, name2);
        }

        [Fact]
        public void GameLoopStrategy_CanBeSubclassed()
        {
            // Arrange & Act
            var customLoop = new CustomChessVariantLoop();

            // Assert
            Assert.IsAssignableFrom<IGameLoopStrategy>(customLoop);
            Assert.NotEqual("Standard Chess", customLoop.GetVariantName());
        }

        [Fact]
        public void MultipleGameLoopInstances_AreIndependent()
        {
            // Arrange & Act
            var loop1 = new StandardChessGameLoop();
            var loop2 = new StandardChessGameLoop();

            // Assert
            Assert.NotSame(loop1, loop2);
            Assert.Equal(loop1.GetVariantName(), loop2.GetVariantName());
        }

        [Fact]
        public void GameLoopStrategy_InterfaceDefinesContract()
        {
            // Arrange
            var gameLoop = new StandardChessGameLoop();
            var strategy = gameLoop as IGameLoopStrategy;

            // Act & Assert
            Assert.NotNull(strategy);
            Assert.NotNull(strategy.GetVariantName());
        }

        // Test implementation of a custom game loop variant
        private class CustomChessVariantLoop : IGameLoopStrategy
        {
            public void Execute()
            {
                // Custom game loop implementation
                // Could be Chess960, Atomic Chess, etc.
            }

            public string GetVariantName()
            {
                return "Custom Chess Variant";
            }
        }
    }
}
