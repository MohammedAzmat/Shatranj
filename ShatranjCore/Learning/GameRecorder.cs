using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Records games for AI learning and analysis
    /// Implements IGameRecorder interface for dependency injection
    /// Can optionally save to IGameDatabase for queryable storage
    /// </summary>
    public class GameRecorder : IGameRecorder
    {
        private readonly string recordDirectory;
        private readonly ILogger logger;
        private readonly IGameDatabase database;
        private GameRecord currentGame;
        private DateTime gameStartTime;

        public GameRecorder(ILogger logger = null, string recordDirectory = null, IGameDatabase database = null)
        {
            this.logger = logger;
            this.database = database;

            if (string.IsNullOrEmpty(recordDirectory))
            {
                this.recordDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "GameRecords"
                );
            }
            else
            {
                this.recordDirectory = recordDirectory;
            }

            Directory.CreateDirectory(this.recordDirectory);
        }

        /// <summary>
        /// Property indicating if recording is in progress (IGameRecorder interface)
        /// </summary>
        public bool IsRecording => currentGame != null;

        /// <summary>
        /// Starts recording a new game (IGameRecorder interface)
        /// </summary>
        /// <param name="whiteName">Name of white player</param>
        /// <param name="blackName">Name of black player</param>
        public void StartGame(string whiteName, string blackName)
        {
            currentGame = new GameRecord
            {
                WhitePlayer = whiteName,
                BlackPlayer = blackName,
                PlayedAt = DateTime.Now,
                GameMode = "Unknown" // Will be set via SetAIMetadata or externally
            };
            gameStartTime = DateTime.Now;

            logger?.Info($"Started recording new game: {currentGame.GameId}");
        }

        /// <summary>
        /// Records a move with its evaluation (IGameRecorder interface)
        /// </summary>
        /// <param name="moveNotation">Move in algebraic notation (e.g., "e4")</param>
        /// <param name="evaluationScore">AI evaluation of the position (optional)</param>
        /// <param name="player">Which player made the move</param>
        public void RecordMove(string moveNotation, double? evaluationScore, PieceColor player)
        {
            if (currentGame == null)
            {
                logger?.Warning("Attempted to record move without an active game");
                return;
            }

            var moveRecord = new MoveRecord
            {
                MoveNumber = currentGame.Moves.Count + 1,
                Player = player.ToString(),
                AlgebraicNotation = moveNotation,
                PositionEvaluation = evaluationScore,
                // From/To/PieceType will be empty for simplified interface
                // Can be enriched by calling the detailed RecordMove method
            };

            currentGame.Moves.Add(moveRecord);
        }

        /// <summary>
        /// Gets the recorded game data (IGameRecorder interface)
        /// </summary>
        /// <returns>The recorded game</returns>
        public GameRecord GetRecordedGame()
        {
            return currentGame;
        }

        /// <summary>
        /// Starts recording a new game
        /// </summary>
        public void StartNewGame(GameMode mode, string whitePlayerType, string blackPlayerType)
        {
            currentGame = new GameRecord
            {
                GameMode = mode.ToString(),
                WhitePlayer = whitePlayerType,
                BlackPlayer = blackPlayerType,
                PlayedAt = DateTime.Now
            };
            gameStartTime = DateTime.Now;

            logger?.Info($"Started recording new game: {currentGame.GameId}");
        }

        /// <summary>
        /// Records a move
        /// </summary>
        public void RecordMove(
            PieceColor player,
            Location from,
            Location to,
            string pieceType,
            string algebraicNotation,
            bool wasCapture,
            bool causedCheck,
            bool causedCheckmate,
            double? evaluation = null,
            int thinkingTimeMs = 0)
        {
            if (currentGame == null)
            {
                logger?.Warning("Attempted to record move without an active game");
                return;
            }

            var moveRecord = new MoveRecord
            {
                MoveNumber = currentGame.Moves.Count + 1,
                Player = player.ToString(),
                From = LocationToAlgebraic(from),
                To = LocationToAlgebraic(to),
                PieceType = pieceType,
                AlgebraicNotation = algebraicNotation,
                WasCapture = wasCapture,
                CausedCheck = causedCheck,
                CausedCheckmate = causedCheckmate,
                PositionEvaluation = evaluation,
                ThinkingTimeMs = thinkingTimeMs
            };

            currentGame.Moves.Add(moveRecord);
        }

        /// <summary>
        /// Ends the current game and saves the record (IGameRecorder interface)
        /// Saves to both JSON file and database (if database is provided)
        /// </summary>
        /// <param name="result">Game result (1-0, 0-1, 1/2-1/2)</param>
        /// <param name="reason">Reason for game end (checkmate, resignation, etc)</param>
        public void EndGame(string result, string reason = null)
        {
            if (currentGame == null)
            {
                logger?.Warning("Attempted to end game without an active game");
                return;
            }

            currentGame.Winner = result;
            currentGame.EndCondition = reason ?? "Unknown";
            currentGame.TotalMoves = currentGame.Moves.Count;
            currentGame.GameDurationMs = (int)(DateTime.Now - gameStartTime).TotalMilliseconds;

            // Save to JSON file
            SaveGameRecord(currentGame);

            // Save to database if available
            if (database != null)
            {
                try
                {
                    int gameId = database.SaveGame(currentGame);
                    logger?.Info($"Game saved to database with ID: {gameId}");
                }
                catch (Exception ex)
                {
                    logger?.Error("Failed to save game to database", ex);
                }
            }

            currentGame = null;
        }

        /// <summary>
        /// Ends the current game and saves the record (returns file path)
        /// This is the extended version that returns the saved file path
        /// Also saves to database if available
        /// </summary>
        public string EndGameWithPath(string winner, string endCondition)
        {
            if (currentGame == null)
            {
                logger?.Warning("Attempted to end game without an active game");
                return null;
            }

            currentGame.Winner = winner;
            currentGame.EndCondition = endCondition;
            currentGame.TotalMoves = currentGame.Moves.Count;
            currentGame.GameDurationMs = (int)(DateTime.Now - gameStartTime).TotalMilliseconds;

            // Save to JSON file
            string filePath = SaveGameRecord(currentGame);

            // Save to database if available
            if (database != null)
            {
                try
                {
                    int gameId = database.SaveGame(currentGame);
                    logger?.Info($"Game saved to database with ID: {gameId}");
                }
                catch (Exception ex)
                {
                    logger?.Error("Failed to save game to database", ex);
                }
            }

            currentGame = null;

            return filePath;
        }

        /// <summary>
        /// Sets AI metadata for the current game
        /// </summary>
        public void SetAIMetadata(string aiVersion, int depth)
        {
            if (currentGame != null)
            {
                currentGame.AIVersion = aiVersion;
                currentGame.AIDepth = depth;
            }
        }

        /// <summary>
        /// Adds a custom statistic to the current game
        /// </summary>
        public void AddStatistic(string key, double value)
        {
            if (currentGame != null)
            {
                currentGame.Statistics[key] = value;
            }
        }

        /// <summary>
        /// Saves a game record to a JSON file
        /// </summary>
        private string SaveGameRecord(GameRecord record)
        {
            try
            {
                string fileName = $"game_{record.PlayedAt:yyyyMMdd_HHmmss}_{record.GameId.Substring(0, 8)}.json";
                string filePath = Path.Combine(recordDirectory, fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(record, options);
                File.WriteAllText(filePath, jsonString, Encoding.UTF8);

                logger?.Info($"Game record saved: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to save game record", ex);
                return null;
            }
        }

        /// <summary>
        /// Loads all game records from the directory
        /// </summary>
        public List<GameRecord> LoadAllRecords()
        {
            var records = new List<GameRecord>();

            try
            {
                string[] files = Directory.GetFiles(recordDirectory, "*.json");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                foreach (string file in files)
                {
                    try
                    {
                        string jsonString = File.ReadAllText(file, Encoding.UTF8);
                        var record = JsonSerializer.Deserialize<GameRecord>(jsonString, options);
                        records.Add(record);
                    }
                    catch (Exception ex)
                    {
                        logger?.Warning($"Failed to load record from {file}: {ex.Message}");
                    }
                }

                logger?.Info($"Loaded {records.Count} game records");
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to load game records", ex);
            }

            return records;
        }

        /// <summary>
        /// Gets statistics from all recorded games
        /// </summary>
        public Dictionary<string, object> GetStatistics()
        {
            var records = LoadAllRecords();
            var stats = new Dictionary<string, object>();

            if (records.Count == 0)
                return stats;

            stats["TotalGames"] = records.Count;
            stats["WhiteWins"] = records.Count(r => r.Winner == "White");
            stats["BlackWins"] = records.Count(r => r.Winner == "Black");
            stats["Draws"] = records.Count(r => r.Winner == "Draw");
            stats["AverageMovesPerGame"] = records.Average(r => r.TotalMoves);
            stats["AIGames"] = records.Count(r => r.GameMode == "AIVsAI");

            return stats;
        }

        /// <summary>
        /// Converts a Location to algebraic notation
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
