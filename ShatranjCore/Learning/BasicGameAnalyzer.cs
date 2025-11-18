using System;
using System.Collections.Generic;
using System.Linq;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Basic implementation of IGameAnalyzer
    /// Analyzes games for mistakes, accuracy, and opening adherence
    /// Uses position evaluations to detect blunders and inaccuracies
    /// </summary>
    public class BasicGameAnalyzer : IGameAnalyzer
    {
        private readonly OpeningBook openingBook;
        private const double MISTAKE_THRESHOLD = 100.0;      // centipawns loss for mistake
        private const double BLUNDER_THRESHOLD = 300.0;      // centipawns loss for blunder
        private const double INACCURACY_THRESHOLD = 50.0;    // centipawns loss for inaccuracy
        private const int OPENING_PHASE_MOVES = 12;
        private const int ENDGAME_PIECE_THRESHOLD = 13;      // Fewer than 13 pieces = endgame

        public BasicGameAnalyzer()
        {
            openingBook = new OpeningBook();
        }

        public BasicGameAnalyzer(OpeningBook customOpeningBook)
        {
            openingBook = customOpeningBook ?? new OpeningBook();
        }

        /// <summary>
        /// Analyzes a complete game and returns detailed analysis
        /// </summary>
        public GameAnalysis Analyze(GameRecord game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var mistakes = FindMistakes(game);
            var criticalMoves = mistakes.Where(m => m.IsMistake).ToList();

            var analysis = new GameAnalysis
            {
                TotalMoves = game.TotalMoves,
                MistakeCount = mistakes.Count(m => m.IsMistake),
                AverageAccuracy = CalculateAverageAccuracy(game),
                OverallAssessment = GenerateAssessment(game, mistakes),
                CriticalMoves = criticalMoves
            };

            return analysis;
        }

        /// <summary>
        /// Finds all mistakes in a game based on evaluation changes
        /// </summary>
        public List<MoveAnalysis> FindMistakes(GameRecord game)
        {
            if (game == null || game.Moves == null)
                return new List<MoveAnalysis>();

            var mistakes = new List<MoveAnalysis>();

            for (int i = 1; i < game.Moves.Count; i++)
            {
                var previousMove = game.Moves[i - 1];
                var currentMove = game.Moves[i];

                // Skip if evaluations are missing
                if (!previousMove.PositionEvaluation.HasValue || !currentMove.PositionEvaluation.HasValue)
                    continue;

                double evalBefore = previousMove.PositionEvaluation.Value;
                double evalAfter = currentMove.PositionEvaluation.Value;

                // Adjust evaluation based on whose move it is
                // If it's Black's move, flip the evaluation
                bool isBlackMove = currentMove.Player == "Black";
                if (isBlackMove)
                {
                    evalBefore = -evalBefore;
                    evalAfter = -evalAfter;
                }

                // Calculate evaluation loss (in centipawns)
                double evalLoss = (evalBefore - evalAfter) * 100;

                // Determine if it's a mistake
                bool isMistake = evalLoss >= MISTAKE_THRESHOLD;
                string assessment = ClassifyMove(evalLoss);

                if (evalLoss >= INACCURACY_THRESHOLD)
                {
                    mistakes.Add(new MoveAnalysis
                    {
                        MoveNumber = currentMove.MoveNumber,
                        Move = currentMove.AlgebraicNotation ?? $"{currentMove.From}-{currentMove.To}",
                        EvaluationBefore = evalBefore,
                        EvaluationAfter = evalAfter,
                        Assessment = assessment,
                        IsMistake = isMistake
                    });
                }
            }

            return mistakes;
        }

        /// <summary>
        /// Analyzes the opening phase of the game
        /// </summary>
        public OpeningAnalysis AnalyzeOpening(GameRecord game)
        {
            if (game == null || game.Moves == null)
            {
                return new OpeningAnalysis
                {
                    OpeningName = "Unknown",
                    ECOCode = "???",
                    MovesInOpening = 0,
                    FollowedTheory = false,
                    Notes = "Insufficient move data"
                };
            }

            // Extract move notation
            var moveNotations = game.Moves
                .Take(OPENING_PHASE_MOVES)
                .Select(m => m.AlgebraicNotation ?? $"{m.From}{m.To}")
                .ToList();

            // Match to opening book
            var matchedOpening = openingBook.MatchOpening(moveNotations);

            if (matchedOpening != null)
            {
                return new OpeningAnalysis
                {
                    OpeningName = matchedOpening.Name,
                    ECOCode = matchedOpening.Eco,
                    MovesInOpening = matchedOpening.Moves.Count,
                    FollowedTheory = true,
                    Notes = $"Followed {matchedOpening.Category} theory for {matchedOpening.Moves.Count} moves"
                };
            }
            else
            {
                return new OpeningAnalysis
                {
                    OpeningName = "Unknown Opening",
                    ECOCode = "???",
                    MovesInOpening = 0,
                    FollowedTheory = false,
                    Notes = "Did not match any known opening in the book"
                };
            }
        }

        /// <summary>
        /// Analyzes the endgame phase
        /// </summary>
        public EndgameAnalysis AnalyzeEndgame(GameRecord game)
        {
            if (game == null || game.Moves == null || game.Moves.Count == 0)
            {
                return new EndgameAnalysis
                {
                    StartMove = 0,
                    EndgameType = "None",
                    Accuracy = 0.0,
                    FoundBestMoves = false,
                    Notes = "No endgame phase detected"
                };
            }

            // Determine when endgame started
            // In a real implementation, we'd count pieces on the board
            // For now, we'll estimate based on game length
            int estimatedEndgameStart = Math.Max(1, game.TotalMoves - 15);

            // Calculate accuracy in endgame phase
            var endgameMoves = game.Moves.Skip(estimatedEndgameStart).ToList();
            double accuracy = CalculatePhaseAccuracy(endgameMoves);

            // Determine endgame type based on remaining pieces (simplified)
            string endgameType = "General Endgame";
            if (game.TotalMoves < 20)
                endgameType = "Middlegame (No Endgame)";

            return new EndgameAnalysis
            {
                StartMove = estimatedEndgameStart,
                EndgameType = endgameType,
                Accuracy = accuracy,
                FoundBestMoves = accuracy >= 0.80,
                Notes = accuracy >= 0.80 ? "Good endgame technique" : "Some inaccuracies in endgame"
            };
        }

        // Helper methods

        private double CalculateAverageAccuracy(GameRecord game)
        {
            if (game.Moves == null || game.Moves.Count < 2)
                return 1.0;

            return CalculatePhaseAccuracy(game.Moves);
        }

        private double CalculatePhaseAccuracy(List<MoveRecord> moves)
        {
            if (moves == null || moves.Count < 2)
                return 1.0;

            int totalMoves = 0;
            double totalAccuracy = 0.0;

            for (int i = 1; i < moves.Count; i++)
            {
                var previousMove = moves[i - 1];
                var currentMove = moves[i];

                if (!previousMove.PositionEvaluation.HasValue || !currentMove.PositionEvaluation.HasValue)
                    continue;

                double evalBefore = previousMove.PositionEvaluation.Value;
                double evalAfter = currentMove.PositionEvaluation.Value;

                // Flip evaluation for Black
                if (currentMove.Player == "Black")
                {
                    evalBefore = -evalBefore;
                    evalAfter = -evalAfter;
                }

                double evalLoss = Math.Max(0, (evalBefore - evalAfter) * 100);

                // Convert eval loss to accuracy score (0.0 to 1.0)
                // No loss = 100% accuracy, large loss = low accuracy
                double moveAccuracy = Math.Max(0.0, 1.0 - (evalLoss / 500.0));

                totalAccuracy += moveAccuracy;
                totalMoves++;
            }

            return totalMoves > 0 ? totalAccuracy / totalMoves : 1.0;
        }

        private string ClassifyMove(double evalLoss)
        {
            if (evalLoss >= BLUNDER_THRESHOLD)
                return "Blunder";
            else if (evalLoss >= MISTAKE_THRESHOLD)
                return "Mistake";
            else if (evalLoss >= INACCURACY_THRESHOLD)
                return "Inaccuracy";
            else if (evalLoss < 0)
                return "Excellent";
            else
                return "Good";
        }

        private string GenerateAssessment(GameRecord game, List<MoveAnalysis> mistakes)
        {
            int blunders = mistakes.Count(m => m.Assessment == "Blunder");
            int mistakes = mistakes.Count(m => m.Assessment == "Mistake");
            int inaccuracies = mistakes.Count(m => m.Assessment == "Inaccuracy");

            double accuracy = CalculateAverageAccuracy(game);

            if (accuracy >= 0.95)
                return "Excellent game with very few mistakes";
            else if (accuracy >= 0.85)
                return $"Good game with {mistakes + blunders} mistake(s)";
            else if (accuracy >= 0.70)
                return $"Average game with {mistakes + blunders} mistake(s) and {inaccuracies} inaccuracy/ies";
            else
                return $"Below average game with {blunders} blunder(s), {mistakes} mistake(s), and {inaccuracies} inaccuracy/ies";
        }
    }
}
