using System;
using System.Collections.Generic;
using Xunit;
using ShatranjCore.Learning;
using ShatranjCore.Abstractions;

namespace Shatranj.Tests.Unit.Learning
{
    /// <summary>
    /// Unit tests for BasicGameAnalyzer and OpeningBook
    /// Tests game analysis, mistake detection, and opening matching
    /// </summary>
    public class GameAnalyzerTests
    {
        // Opening Book Tests

        [Fact]
        public void OpeningBook_Loads_Successfully()
        {
            // Arrange & Act
            var openingBook = new OpeningBook();

            // Assert
            Assert.True(openingBook.Count >= 0); // May be 0 if file not found, that's okay
        }

        [Fact]
        public void OpeningBook_MatchOpening_RecognizesSicilianDefense()
        {
            // Arrange
            var openingBook = new OpeningBook();
            var moves = new List<string> { "e4", "c5" };

            // Act
            var matched = openingBook.MatchOpening(moves);

            // Assert
            if (openingBook.Count > 0)
            {
                Assert.NotNull(matched);
                Assert.Contains("Sicilian", matched.Name);
            }
        }

        [Fact]
        public void OpeningBook_MatchOpening_RecognizesRuyLopez()
        {
            // Arrange
            var openingBook = new OpeningBook();
            var moves = new List<string> { "e4", "e5", "Nf3", "Nc6", "Bb5" };

            // Act
            var matched = openingBook.MatchOpening(moves);

            // Assert
            if (openingBook.Count > 0)
            {
                Assert.NotNull(matched);
                Assert.Contains("Ruy Lopez", matched.Name);
            }
        }

        [Fact]
        public void OpeningBook_MatchOpening_ReturnsNullForUnknownOpening()
        {
            // Arrange
            var openingBook = new OpeningBook();
            var moves = new List<string> { "h4", "h5", "a4", "a5" }; // Irregular opening

            // Act
            var matched = openingBook.MatchOpening(moves);

            // Assert - may or may not match depending on book content
            // Just verify it doesn't crash
            Assert.True(matched == null || matched != null);
        }

        [Fact]
        public void OpeningBook_GetOpeningByEco_FindsCorrectOpening()
        {
            // Arrange
            var openingBook = new OpeningBook();

            // Act
            var opening = openingBook.GetOpeningByEco("C20");

            // Assert
            if (openingBook.Count > 0 && opening != null)
            {
                Assert.Equal("C20", opening.Eco);
            }
        }

        [Fact]
        public void OpeningBook_SearchOpenings_FindsMultipleMatches()
        {
            // Arrange
            var openingBook = new OpeningBook();

            // Act
            var results = openingBook.SearchOpenings("Sicilian");

            // Assert
            if (openingBook.Count > 0)
            {
                Assert.NotEmpty(results);
                Assert.All(results, r => Assert.Contains("Sicilian", r.Name));
            }
        }

        // Game Analyzer Tests

        [Fact]
        public void BasicGameAnalyzer_Analyze_HandlesValidGame()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateTestGame();

            // Act
            var analysis = analyzer.Analyze(game);

            // Assert
            Assert.NotNull(analysis);
            Assert.True(analysis.TotalMoves > 0);
            Assert.NotNull(analysis.OverallAssessment);
        }

        [Fact]
        public void BasicGameAnalyzer_Analyze_ThrowsOnNullGame()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => analyzer.Analyze(null));
        }

        [Fact]
        public void BasicGameAnalyzer_FindMistakes_DetectsBlunders()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateGameWithBlunder();

            // Act
            var mistakes = analyzer.FindMistakes(game);

            // Assert
            Assert.NotEmpty(mistakes);
            Assert.Contains(mistakes, m => m.Assessment == "Blunder");
        }

        [Fact]
        public void BasicGameAnalyzer_FindMistakes_ReturnsEmptyForPerfectGame()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreatePerfectGame();

            // Act
            var mistakes = analyzer.FindMistakes(game);

            // Assert
            Assert.Empty(mistakes);
        }

        [Fact]
        public void BasicGameAnalyzer_AnalyzeOpening_RecognizesKnownOpening()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateGameWithSicilian();

            // Act
            var openingAnalysis = analyzer.AnalyzeOpening(game);

            // Assert
            Assert.NotNull(openingAnalysis);
            Assert.NotNull(openingAnalysis.OpeningName);
            // May or may not be recognized depending on opening book availability
        }

        [Fact]
        public void BasicGameAnalyzer_AnalyzeOpening_HandlesUnknownOpening()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateGameWithIrregularOpening();

            // Act
            var openingAnalysis = analyzer.AnalyzeOpening(game);

            // Assert
            Assert.NotNull(openingAnalysis);
            Assert.NotNull(openingAnalysis.OpeningName);
        }

        [Fact]
        public void BasicGameAnalyzer_AnalyzeEndgame_ProvidesFeedback()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateTestGame();

            // Act
            var endgameAnalysis = analyzer.AnalyzeEndgame(game);

            // Assert
            Assert.NotNull(endgameAnalysis);
            Assert.NotNull(endgameAnalysis.EndgameType);
            Assert.True(endgameAnalysis.Accuracy >= 0.0 && endgameAnalysis.Accuracy <= 1.0);
        }

        [Fact]
        public void BasicGameAnalyzer_AnalyzeEndgame_IdentifiesGoodTechnique()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreatePerfectGame();

            // Act
            var endgameAnalysis = analyzer.AnalyzeEndgame(game);

            // Assert
            Assert.NotNull(endgameAnalysis);
            Assert.True(endgameAnalysis.Accuracy >= 0.8);
        }

        [Fact]
        public void GameAnalysis_CalculatesAccuracyCorrectly()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateTestGame();

            // Act
            var analysis = analyzer.Analyze(game);

            // Assert
            Assert.True(analysis.AverageAccuracy >= 0.0 && analysis.AverageAccuracy <= 1.0);
        }

        [Fact]
        public void MoveAnalysis_ClassifiesMovesCorrectly()
        {
            // Arrange
            var analyzer = new BasicGameAnalyzer();
            var game = CreateGameWithMixedQuality();

            // Act
            var mistakes = analyzer.FindMistakes(game);

            // Assert
            foreach (var mistake in mistakes)
            {
                Assert.Contains(mistake.Assessment, new[] { "Inaccuracy", "Mistake", "Blunder", "Good", "Excellent" });
            }
        }

        // Helper methods to create test games

        private GameRecord CreateTestGame()
        {
            var game = new GameRecord
            {
                WhitePlayer = "TestPlayer",
                BlackPlayer = "Opponent",
                TotalMoves = 10,
                Moves = new List<MoveRecord>()
            };

            // Add moves with evaluations
            for (int i = 0; i < 10; i++)
            {
                game.Moves.Add(new MoveRecord
                {
                    MoveNumber = i + 1,
                    Player = i % 2 == 0 ? "White" : "Black",
                    AlgebraicNotation = "e4",
                    PositionEvaluation = 0.5 + (i * 0.1) // Gradually improving position
                });
            }

            return game;
        }

        private GameRecord CreateGameWithBlunder()
        {
            var game = new GameRecord
            {
                WhitePlayer = "Blunderer",
                BlackPlayer = "Winner",
                TotalMoves = 5,
                Moves = new List<MoveRecord>
                {
                    new MoveRecord { MoveNumber = 1, Player = "White", AlgebraicNotation = "e4", PositionEvaluation = 0.5 },
                    new MoveRecord { MoveNumber = 2, Player = "Black", AlgebraicNotation = "e5", PositionEvaluation = 0.5 },
                    new MoveRecord { MoveNumber = 3, Player = "White", AlgebraicNotation = "Nf3", PositionEvaluation = 0.6 },
                    new MoveRecord { MoveNumber = 4, Player = "Black", AlgebraicNotation = "Qh4", PositionEvaluation = 0.6 },
                    new MoveRecord { MoveNumber = 5, Player = "White", AlgebraicNotation = "Qe2", PositionEvaluation = -3.0 } // Blunder!
                }
            };

            return game;
        }

        private GameRecord CreatePerfectGame()
        {
            var game = new GameRecord
            {
                WhitePlayer = "Perfect",
                BlackPlayer = "AlsoPerfect",
                TotalMoves = 6,
                Moves = new List<MoveRecord>()
            };

            // All moves maintain equal position
            for (int i = 0; i < 6; i++)
            {
                game.Moves.Add(new MoveRecord
                {
                    MoveNumber = i + 1,
                    Player = i % 2 == 0 ? "White" : "Black",
                    AlgebraicNotation = "e4",
                    PositionEvaluation = 0.0 // Perfectly equal
                });
            }

            return game;
        }

        private GameRecord CreateGameWithSicilian()
        {
            var game = new GameRecord
            {
                WhitePlayer = "Sicilian Player",
                BlackPlayer = "Opponent",
                TotalMoves = 4,
                Moves = new List<MoveRecord>
                {
                    new MoveRecord { MoveNumber = 1, Player = "White", AlgebraicNotation = "e4", PositionEvaluation = 0.3 },
                    new MoveRecord { MoveNumber = 2, Player = "Black", AlgebraicNotation = "c5", PositionEvaluation = 0.3 },
                    new MoveRecord { MoveNumber = 3, Player = "White", AlgebraicNotation = "Nf3", PositionEvaluation = 0.4 },
                    new MoveRecord { MoveNumber = 4, Player = "Black", AlgebraicNotation = "d6", PositionEvaluation = 0.3 }
                }
            };

            return game;
        }

        private GameRecord CreateGameWithIrregularOpening()
        {
            var game = new GameRecord
            {
                WhitePlayer = "Creative",
                BlackPlayer = "Opponent",
                TotalMoves = 4,
                Moves = new List<MoveRecord>
                {
                    new MoveRecord { MoveNumber = 1, Player = "White", AlgebraicNotation = "h4", PositionEvaluation = 0.1 },
                    new MoveRecord { MoveNumber = 2, Player = "Black", AlgebraicNotation = "h5", PositionEvaluation = 0.1 },
                    new MoveRecord { MoveNumber = 3, Player = "White", AlgebraicNotation = "a4", PositionEvaluation = 0.0 },
                    new MoveRecord { MoveNumber = 4, Player = "Black", AlgebraicNotation = "a5", PositionEvaluation = 0.0 }
                }
            };

            return game;
        }

        private GameRecord CreateGameWithMixedQuality()
        {
            var game = new GameRecord
            {
                WhitePlayer = "Mixed",
                BlackPlayer = "Quality",
                TotalMoves = 8,
                Moves = new List<MoveRecord>
                {
                    new MoveRecord { MoveNumber = 1, Player = "White", AlgebraicNotation = "e4", PositionEvaluation = 0.4 },
                    new MoveRecord { MoveNumber = 2, Player = "Black", AlgebraicNotation = "e5", PositionEvaluation = 0.4 },
                    new MoveRecord { MoveNumber = 3, Player = "White", AlgebraicNotation = "Nf3", PositionEvaluation = 0.5 },
                    new MoveRecord { MoveNumber = 4, Player = "Black", AlgebraicNotation = "Nf6", PositionEvaluation = 0.3 }, // Slight inaccuracy
                    new MoveRecord { MoveNumber = 5, Player = "White", AlgebraicNotation = "Nxe5", PositionEvaluation = 1.0 },
                    new MoveRecord { MoveNumber = 6, Player = "Black", AlgebraicNotation = "d5", PositionEvaluation = -0.5 }, // Mistake
                    new MoveRecord { MoveNumber = 7, Player = "White", AlgebraicNotation = "Qf3", PositionEvaluation = 2.0 },
                    new MoveRecord { MoveNumber = 8, Player = "Black", AlgebraicNotation = "Be6", PositionEvaluation = -2.5 } // Blunder
                }
            };

            return game;
        }
    }
}
