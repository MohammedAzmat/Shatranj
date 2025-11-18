using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using ShatranjCore.Learning;
using ShatranjCore.Abstractions;

namespace Shatranj.Tests.Unit.Learning
{
    /// <summary>
    /// Tests for Learning Infrastructure Interfaces - verifies learning layer contracts
    /// Uses mocks since implementations are not yet complete
    /// </summary>
    public class LearningInfrastructureTests
    {
        [Fact]
        public void IGameRecorder_ImplementsInterface_Correctly()
        {
            // Arrange
            var mockRecorder = new Mock<IGameRecorder>();

            // Act
            var recorder = mockRecorder.Object;

            // Assert
            Assert.NotNull(recorder);
            Assert.IsAssignableFrom<IGameRecorder>(recorder);
        }

        [Fact]
        public void IGameAnalyzer_ImplementsInterface_Correctly()
        {
            // Arrange
            var mockAnalyzer = new Mock<IGameAnalyzer>();

            // Act
            var analyzer = mockAnalyzer.Object;

            // Assert
            Assert.NotNull(analyzer);
            Assert.IsAssignableFrom<IGameAnalyzer>(analyzer);
        }

        [Fact]
        public void IGameDatabase_ImplementsInterface_Correctly()
        {
            // Arrange
            var mockDatabase = new Mock<IGameDatabase>();

            // Act
            var database = mockDatabase.Object;

            // Assert
            Assert.NotNull(database);
            Assert.IsAssignableFrom<IGameDatabase>(database);
        }

        [Fact]
        public void IAILearningEngine_ImplementsInterface_Correctly()
        {
            // Arrange
            var mockEngine = new Mock<IAILearningEngine>();

            // Act
            var engine = mockEngine.Object;

            // Assert
            Assert.NotNull(engine);
            Assert.IsAssignableFrom<IAILearningEngine>(engine);
        }

        [Fact]
        public void GameRecorder_StartGame_CanBeCalled()
        {
            // Arrange
            var mockRecorder = new Mock<IGameRecorder>();
            mockRecorder.Setup(r => r.StartGame("White", "Black"));

            // Act
            mockRecorder.Object.StartGame("White", "Black");

            // Assert
            mockRecorder.Verify(r => r.StartGame("White", "Black"), Times.Once);
        }

        [Fact]
        public void GameRecorder_RecordMove_CanBeCalled()
        {
            // Arrange
            var mockRecorder = new Mock<IGameRecorder>();
            mockRecorder.Setup(r => r.RecordMove("e4", 50.0, PieceColor.White));

            // Act
            mockRecorder.Object.RecordMove("e4", 50.0, PieceColor.White);

            // Assert
            mockRecorder.Verify(r => r.RecordMove("e4", 50.0, PieceColor.White), Times.Once);
        }

        [Fact]
        public void GameRecorder_EndGame_CanBeCalled()
        {
            // Arrange
            var mockRecorder = new Mock<IGameRecorder>();
            mockRecorder.Setup(r => r.EndGame("1-0", "Checkmate"));

            // Act
            mockRecorder.Object.EndGame("1-0", "Checkmate");

            // Assert
            mockRecorder.Verify(r => r.EndGame("1-0", "Checkmate"), Times.Once);
        }

        [Fact]
        public void GameRecorder_IsRecording_PropertyReadable()
        {
            // Arrange
            var mockRecorder = new Mock<IGameRecorder>();
            mockRecorder.Setup(r => r.IsRecording).Returns(true);

            // Act
            var isRecording = mockRecorder.Object.IsRecording;

            // Assert
            Assert.True(isRecording);
        }

        [Fact]
        public void GameAnalyzer_Analyze_ReturnsGameAnalysis()
        {
            // Arrange
            var mockAnalyzer = new Mock<IGameAnalyzer>();
            var game = new GameRecord();
            var expectedAnalysis = new GameAnalysis { TotalMoves = 10, MistakeCount = 2 };
            mockAnalyzer.Setup(a => a.Analyze(game)).Returns(expectedAnalysis);

            // Act
            var analysis = mockAnalyzer.Object.Analyze(game);

            // Assert
            Assert.NotNull(analysis);
            Assert.Equal(10, analysis.TotalMoves);
            Assert.Equal(2, analysis.MistakeCount);
        }

        [Fact]
        public void GameAnalyzer_FindMistakes_ReturnsMoveAnalysisList()
        {
            // Arrange
            var mockAnalyzer = new Mock<IGameAnalyzer>();
            var game = new GameRecord();
            var expectedMistakes = new List<MoveAnalysis>
            {
                new MoveAnalysis { MoveNumber = 5, IsMistake = true }
            };
            mockAnalyzer.Setup(a => a.FindMistakes(game)).Returns(expectedMistakes);

            // Act
            var mistakes = mockAnalyzer.Object.FindMistakes(game);

            // Assert
            Assert.NotNull(mistakes);
            Assert.Single(mistakes);
            Assert.True(mistakes[0].IsMistake);
        }

        [Fact]
        public void GameDatabase_SaveGame_ReturnsGameId()
        {
            // Arrange
            var mockDatabase = new Mock<IGameDatabase>();
            var game = new GameRecord();
            mockDatabase.Setup(d => d.SaveGame(game)).Returns(42);

            // Act
            var gameId = mockDatabase.Object.SaveGame(game);

            // Assert
            Assert.Equal(42, gameId);
        }

        [Fact]
        public void GameDatabase_GetGameCount_ReturnsCount()
        {
            // Arrange
            var mockDatabase = new Mock<IGameDatabase>();
            mockDatabase.Setup(d => d.GetGameCount()).Returns(15);

            // Act
            var count = mockDatabase.Object.GetGameCount();

            // Assert
            Assert.Equal(15, count);
        }

        [Fact]
        public void GameDatabase_GetStatistics_ReturnsDatabaseStatistics()
        {
            // Arrange
            var mockDatabase = new Mock<IGameDatabase>();
            var expectedStats = new DatabaseStatistics
            {
                TotalGames = 100,
                TotalMoves = 5000
            };
            mockDatabase.Setup(d => d.GetStatistics()).Returns(expectedStats);

            // Act
            var stats = mockDatabase.Object.GetStatistics();

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(100, stats.TotalGames);
            Assert.Equal(5000, stats.TotalMoves);
        }

        [Fact]
        public void AILearningEngine_TrainFromGames_CanBeCalled()
        {
            // Arrange
            var mockEngine = new Mock<IAILearningEngine>();
            var games = new List<GameRecord> { new GameRecord() };
            mockEngine.Setup(e => e.TrainFromGames(games, 10));

            // Act
            mockEngine.Object.TrainFromGames(games, 10);

            // Assert
            mockEngine.Verify(e => e.TrainFromGames(games, 10), Times.Once);
        }

        [Fact]
        public void AILearningEngine_RunSelfPlayTraining_ReturnsGameList()
        {
            // Arrange
            var mockEngine = new Mock<IAILearningEngine>();
            var expectedGames = new List<GameRecord>
            {
                new GameRecord(),
                new GameRecord()
            };
            mockEngine.Setup(e => e.RunSelfPlayTraining(2)).Returns(expectedGames);

            // Act
            var games = mockEngine.Object.RunSelfPlayTraining(2);

            // Assert
            Assert.NotNull(games);
            Assert.Equal(2, games.Count);
        }

        [Fact]
        public void AILearningEngine_EvaluatePerformance_ReturnsMetrics()
        {
            // Arrange
            var mockEngine = new Mock<IAILearningEngine>();
            var testGames = new List<GameRecord> { new GameRecord() };
            var expectedMetrics = new PerformanceMetrics
            {
                WinRate = 0.75,
                Accuracy = 0.85
            };
            mockEngine.Setup(e => e.EvaluatePerformance(testGames)).Returns(expectedMetrics);

            // Act
            var metrics = mockEngine.Object.EvaluatePerformance(testGames);

            // Assert
            Assert.NotNull(metrics);
            Assert.Equal(0.75, metrics.WinRate);
            Assert.Equal(0.85, metrics.Accuracy);
        }

        [Fact]
        public void AILearningEngine_GetTrainingStatus_ReturnsStatus()
        {
            // Arrange
            var mockEngine = new Mock<IAILearningEngine>();
            var expectedStatus = new TrainingStatus
            {
                IsTraining = true,
                ProgressPercentage = 50.0
            };
            mockEngine.Setup(e => e.GetTrainingStatus()).Returns(expectedStatus);

            // Act
            var status = mockEngine.Object.GetTrainingStatus();

            // Assert
            Assert.NotNull(status);
            Assert.True(status.IsTraining);
            Assert.Equal(50.0, status.ProgressPercentage);
        }

        [Fact]
        public void GameRecord_CanBeInstantiated()
        {
            // Arrange & Act
            var game = new GameRecord();

            // Assert
            Assert.NotNull(game);
        }

        [Fact]
        public void GameAnalysis_ContainsRequiredProperties()
        {
            // Arrange & Act
            var analysis = new GameAnalysis
            {
                TotalMoves = 40,
                MistakeCount = 3,
                AverageAccuracy = 0.92
            };

            // Assert
            Assert.Equal(40, analysis.TotalMoves);
            Assert.Equal(3, analysis.MistakeCount);
            Assert.Equal(0.92, analysis.AverageAccuracy);
        }

        [Fact]
        public void DatabaseStatistics_ContainsRequiredProperties()
        {
            // Arrange & Act
            var stats = new DatabaseStatistics
            {
                TotalGames = 500,
                TotalMoves = 25000,
                AverageGameLength = 50.0
            };

            // Assert
            Assert.Equal(500, stats.TotalGames);
            Assert.Equal(25000, stats.TotalMoves);
            Assert.Equal(50.0, stats.AverageGameLength);
        }

        [Fact]
        public void PerformanceMetrics_ContainsRequiredProperties()
        {
            // Arrange & Act
            var metrics = new PerformanceMetrics
            {
                Accuracy = 0.88,
                WinRate = 0.70,
                WinCount = 14,
                LossCount = 4,
                DrawCount = 2
            };

            // Assert
            Assert.Equal(0.88, metrics.Accuracy);
            Assert.Equal(0.70, metrics.WinRate);
            Assert.Equal(14, metrics.WinCount);
            Assert.Equal(4, metrics.LossCount);
            Assert.Equal(2, metrics.DrawCount);
        }

        [Fact]
        public void TrainingStatus_ContainsRequiredProperties()
        {
            // Arrange & Act
            var status = new TrainingStatus
            {
                IsTraining = true,
                GamesProcessed = 100,
                TotalGames = 200,
                ProgressPercentage = 50.0,
                CurrentPhase = "Evaluation"
            };

            // Assert
            Assert.True(status.IsTraining);
            Assert.Equal(100, status.GamesProcessed);
            Assert.Equal(200, status.TotalGames);
            Assert.Equal(50.0, status.ProgressPercentage);
            Assert.Equal("Evaluation", status.CurrentPhase);
        }

        // ==========================================
        // REAL IMPLEMENTATION TESTS
        // ==========================================

        [Fact]
        public void GameRecorder_RealImplementation_ImplementsInterface()
        {
            // Arrange & Act
            var recorder = new GameRecorder();

            // Assert
            Assert.IsAssignableFrom<IGameRecorder>(recorder);
            Assert.False(recorder.IsRecording);
        }

        [Fact]
        public void GameRecorder_RealImplementation_StartsAndEndsGame()
        {
            // Arrange
            var recorder = new GameRecorder();

            // Act
            recorder.StartGame("Alice", "Bob");
            bool isRecording = recorder.IsRecording;
            recorder.EndGame("1-0", "Checkmate");
            bool isRecordingAfter = recorder.IsRecording;

            // Assert
            Assert.True(isRecording);
            Assert.False(isRecordingAfter);
        }

        [Fact]
        public void GameRecorder_RealImplementation_RecordsMove()
        {
            // Arrange
            var recorder = new GameRecorder();
            recorder.StartGame("Alice", "Bob");

            // Act
            recorder.RecordMove("e4", 0.5, PieceColor.White);
            recorder.RecordMove("e5", -0.5, PieceColor.Black);
            var game = recorder.GetRecordedGame();

            // Assert
            Assert.NotNull(game);
            Assert.Equal(2, game.Moves.Count);
            Assert.Equal("e4", game.Moves[0].AlgebraicNotation);
            Assert.Equal("e5", game.Moves[1].AlgebraicNotation);
        }

        [Fact]
        public void FileGameDatabase_RealImplementation_ImplementsInterface()
        {
            // Arrange & Act
            var database = new FileGameDatabase();

            // Assert
            Assert.IsAssignableFrom<IGameDatabase>(database);
        }

        [Fact]
        public void GameRecorder_WithDatabase_SavesGameToDatabase()
        {
            // Arrange
            var testDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ShatranjIntegrationTest", System.Guid.NewGuid().ToString());
            var database = new FileGameDatabase(null, testDir);
            var recorder = new GameRecorder(null, testDir, database);

            try
            {
                // Act
                recorder.StartGame("Alice", "Bob");
                recorder.RecordMove("e4", 0.5, PieceColor.White);
                recorder.RecordMove("e5", -0.5, PieceColor.Black);
                recorder.EndGame("1-0", "Checkmate");

                // Assert
                var games = database.GetAllGames();
                Assert.Single(games);
                Assert.Equal("Alice", games[0].WhitePlayer);
                Assert.Equal("Bob", games[0].BlackPlayer);
                Assert.Equal("1-0", games[0].Winner);
                Assert.Equal(2, games[0].Moves.Count);
            }
            finally
            {
                // Cleanup
                if (System.IO.Directory.Exists(testDir))
                {
                    System.IO.Directory.Delete(testDir, true);
                }
            }
        }
    }
}
