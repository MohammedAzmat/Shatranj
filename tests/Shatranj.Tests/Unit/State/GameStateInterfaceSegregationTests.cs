using System;
using Xunit;
using ShatranjCore.State;
using ShatranjCore.Persistence;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace Shatranj.Tests.Unit.State
{
    /// <summary>
    /// Tests for GameState Interface Segregation - verifies ISP implementation
    /// Ensures IGameStateQuery and IGameStateModifier are properly segregated
    /// </summary>
    public class GameStateInterfaceSegregationTests
    {
        private GameStateManager CreateStateManager()
        {
            var saveManager = new SaveGameManager();
            var logger = new ConsoleLogger();
            return new GameStateManager(saveManager, logger);
        }

        [Fact]
        public void GameStateManager_ImplementsQuery_Interface()
        {
            // Arrange
            var manager = CreateStateManager();

            // Act
            var query = manager as IGameStateQuery;

            // Assert
            Assert.NotNull(query);
            Assert.IsAssignableFrom<IGameStateQuery>(manager);
        }

        [Fact]
        public void GameStateManager_ImplementsModifier_Interface()
        {
            // Arrange
            var manager = CreateStateManager();

            // Act
            var modifier = manager as IGameStateModifier;

            // Assert
            Assert.NotNull(modifier);
            Assert.IsAssignableFrom<IGameStateModifier>(manager);
        }

        [Fact]
        public void GameStateManager_ImplementsCombined_Interface()
        {
            // Arrange
            var manager = CreateStateManager();

            // Act
            var combined = manager as IGameStateManager;

            // Assert
            Assert.NotNull(combined);
            Assert.IsAssignableFrom<IGameStateManager>(manager);
        }

        [Fact]
        public void IGameStateQuery_HasReadMethods()
        {
            // Arrange
            var manager = CreateStateManager();
            var query = manager as IGameStateQuery;

            // Act & Assert - Verify read methods exist
            Assert.NotNull(query);

            // These methods should exist on IGameStateQuery
            // CanRollback, CanRedo, GetStateCount, GetCurrentState
            var canRollback = query.CanRollback();
            var canRedo = query.CanRedo();
            var stateCount = query.GetStateCount();
            var currentState = query.GetCurrentState();

            // Assert - All methods callable without error
            Assert.False(canRollback); // Initially false
            Assert.False(canRedo);      // Initially false
            Assert.Equal(0, stateCount);
            Assert.Null(currentState);  // Initially null
        }

        [Fact]
        public void IGameStateModifier_HasWriteMethods()
        {
            // Arrange
            var manager = CreateStateManager();
            var modifier = manager as IGameStateModifier;

            // Act & Assert - Verify write methods exist
            Assert.NotNull(modifier);

            // These methods should exist on IGameStateModifier
            // RecordState, Autosave, Rollback, Redo, ClearRedoStack, CleanupAutosave, ClearAll
            var snapshot = new GameStateSnapshot();

            // Should not throw
            modifier.RecordState(snapshot);
            modifier.ClearRedoStack();
            modifier.ClearAll();

            Assert.True(true); // If we get here, methods exist
        }

        [Fact]
        public void ComponentsCanDependOnQueryOnly()
        {
            // Arrange
            var manager = CreateStateManager();
            var query = manager as IGameStateQuery;

            // Act - Component depends only on IGameStateQuery
            ReadOnlyStateComponent component = new ReadOnlyStateComponent(query);

            // Assert
            Assert.NotNull(component);
            Assert.NotNull(component.GetStateCount());
        }

        [Fact]
        public void ComponentsCanDependOnModifierOnly()
        {
            // Arrange
            var manager = CreateStateManager();
            var modifier = manager as IGameStateModifier;

            // Act - Component depends only on IGameStateModifier
            WriteOnlyStateComponent component = new WriteOnlyStateComponent(modifier);

            // Assert
            Assert.NotNull(component);
            component.RecordGameState(new GameStateSnapshot());
            Assert.True(true); // If we get here, component works
        }

        [Fact]
        public void RecordState_StoresSnapshot()
        {
            // Arrange
            var manager = CreateStateManager();
            var snapshot = new GameStateSnapshot { MoveCount = 5 };

            // Act
            manager.RecordState(snapshot);
            var currentState = manager.GetCurrentState();

            // Assert
            Assert.NotNull(currentState);
            Assert.Equal(5, currentState.MoveCount);
        }

        [Fact]
        public void CanRollback_WithMultipleStates_ReturnsTrue()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });

            // Act
            var canRollback = manager.CanRollback();

            // Assert
            Assert.True(canRollback);
        }

        [Fact]
        public void Rollback_UndoesState()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });

            // Act
            var rolledBack = manager.Rollback();

            // Assert
            Assert.NotNull(rolledBack);
            Assert.Equal(1, rolledBack.MoveCount);
        }

        [Fact]
        public void CanRedo_AfterRollback_ReturnsTrue()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });
            manager.Rollback();

            // Act
            var canRedo = manager.CanRedo();

            // Assert
            Assert.True(canRedo);
        }

        [Fact]
        public void Redo_RestoresState()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });
            manager.Rollback();

            // Act
            var redone = manager.Redo();

            // Assert
            Assert.NotNull(redone);
            Assert.Equal(2, redone.MoveCount);
        }

        [Fact]
        public void ClearRedoStack_RemovesRedoStates()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });
            manager.Rollback();

            // Act
            manager.ClearRedoStack();
            var canRedo = manager.CanRedo();

            // Assert
            Assert.False(canRedo);
        }

        [Fact]
        public void ClearAll_RemovesAllStates()
        {
            // Arrange
            var manager = CreateStateManager();
            manager.RecordState(new GameStateSnapshot { MoveCount = 1 });
            manager.RecordState(new GameStateSnapshot { MoveCount = 2 });

            // Act
            manager.ClearAll();
            var stateCount = manager.GetStateCount();

            // Assert
            Assert.Equal(0, stateCount);
        }

        [Fact]
        public void GetStateCount_ReturnsCorrectCount()
        {
            // Arrange
            var manager = CreateStateManager();

            // Act & Assert
            Assert.Equal(0, manager.GetStateCount());

            manager.RecordState(new GameStateSnapshot());
            Assert.Equal(1, manager.GetStateCount());

            manager.RecordState(new GameStateSnapshot());
            Assert.Equal(2, manager.GetStateCount());
        }

        // Helper component classes for testing interface segregation

        private class ReadOnlyStateComponent
        {
            private readonly IGameStateQuery _stateQuery;

            public ReadOnlyStateComponent(IGameStateQuery stateQuery)
            {
                _stateQuery = stateQuery;
            }

            public int GetStateCount()
            {
                return _stateQuery.GetStateCount();
            }

            public bool CanRollback()
            {
                return _stateQuery.CanRollback();
            }

            public bool CanRedo()
            {
                return _stateQuery.CanRedo();
            }
        }

        private class WriteOnlyStateComponent
        {
            private readonly IGameStateModifier _stateModifier;

            public WriteOnlyStateComponent(IGameStateModifier stateModifier)
            {
                _stateModifier = stateModifier;
            }

            public void RecordGameState(GameStateSnapshot snapshot)
            {
                _stateModifier.RecordState(snapshot);
            }

            public void UndoMove()
            {
                _stateModifier.Rollback();
            }

            public void RedoMove()
            {
                _stateModifier.Redo();
            }
        }
    }
}
