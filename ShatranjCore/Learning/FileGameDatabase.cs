using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// File-based implementation of IGameDatabase.
    /// Stores games as JSON files and maintains an in-memory index for queries.
    /// Designed for up to 10,000 games with acceptable performance.
    /// </summary>
    public class FileGameDatabase : IGameDatabase
    {
        private readonly string databaseDirectory;
        private readonly ILogger logger;
        private Dictionary<int, string> gameIdToFileMap; // gameId -> filePath
        private Dictionary<string, int> fileToGameIdMap; // fileName -> gameId
        private int nextGameId;

        public FileGameDatabase(ILogger logger = null, string databaseDirectory = null)
        {
            this.logger = logger;

            if (string.IsNullOrEmpty(databaseDirectory))
            {
                this.databaseDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "GameRecords"
                );
            }
            else
            {
                this.databaseDirectory = databaseDirectory;
            }

            Directory.CreateDirectory(this.databaseDirectory);

            gameIdToFileMap = new Dictionary<int, string>();
            fileToGameIdMap = new Dictionary<string, int>();
            nextGameId = 1;

            RebuildIndex();
        }

        /// <summary>
        /// Rebuilds the in-memory index by scanning all JSON files in the directory.
        /// </summary>
        private void RebuildIndex()
        {
            try
            {
                var files = Directory.GetFiles(databaseDirectory, "*.json")
                    .OrderBy(f => File.GetCreationTime(f))
                    .ToArray();

                gameIdToFileMap.Clear();
                fileToGameIdMap.Clear();

                for (int i = 0; i < files.Length; i++)
                {
                    int gameId = i + 1;
                    string filePath = files[i];
                    string fileName = Path.GetFileName(filePath);

                    gameIdToFileMap[gameId] = filePath;
                    fileToGameIdMap[fileName] = gameId;
                }

                nextGameId = files.Length + 1;

                logger?.Info($"Database index rebuilt: {files.Length} games found");
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to rebuild database index", ex);
            }
        }

        /// <summary>
        /// Saves a game record to the database.
        /// </summary>
        /// <param name="game">The game to save</param>
        /// <returns>The ID of the saved game</returns>
        public int SaveGame(GameRecord game)
        {
            try
            {
                // Generate filename based on game metadata
                string fileName = $"game_{game.PlayedAt:yyyyMMdd_HHmmss}_{game.GameId.Substring(0, 8)}.json";
                string filePath = Path.Combine(databaseDirectory, fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(game, options);
                File.WriteAllText(filePath, jsonString, Encoding.UTF8);

                // Assign game ID
                int gameId = nextGameId++;
                gameIdToFileMap[gameId] = filePath;
                fileToGameIdMap[fileName] = gameId;

                logger?.Info($"Game saved to database with ID {gameId}: {filePath}");
                return gameId;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to save game to database", ex);
                return -1;
            }
        }

        /// <summary>
        /// Loads a game record by ID.
        /// </summary>
        /// <param name="gameId">The ID of the game to load</param>
        /// <returns>The game record, or null if not found</returns>
        public GameRecord LoadGame(int gameId)
        {
            try
            {
                if (!gameIdToFileMap.ContainsKey(gameId))
                {
                    logger?.Warning($"Game ID {gameId} not found in database");
                    return null;
                }

                string filePath = gameIdToFileMap[gameId];
                return LoadGameFromFile(filePath);
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to load game {gameId}", ex);
                return null;
            }
        }

        /// <summary>
        /// Gets all games by a specific player name.
        /// </summary>
        /// <param name="playerName">Name of the player</param>
        /// <returns>List of games played by the player</returns>
        public List<GameRecord> GetGamesByPlayer(string playerName)
        {
            var games = GetAllGames();
            return games.Where(g =>
                g.WhitePlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase) ||
                g.BlackPlayer.Contains(playerName, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        /// <summary>
        /// Gets all games with a specific outcome.
        /// </summary>
        /// <param name="outcome">Win, Loss, or Draw</param>
        /// <returns>Games with that outcome</returns>
        public List<GameRecord> GetGamesByOutcome(string outcome)
        {
            var games = GetAllGames();
            return games.Where(g =>
                g.Winner != null && g.Winner.Equals(outcome, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        /// <summary>
        /// Gets games between two players.
        /// </summary>
        /// <param name="player1">First player name</param>
        /// <param name="player2">Second player name</param>
        /// <returns>Games between these players</returns>
        public List<GameRecord> GetGamesBetweenPlayers(string player1, string player2)
        {
            var games = GetAllGames();
            return games.Where(g =>
                (g.WhitePlayer.Contains(player1, StringComparison.OrdinalIgnoreCase) &&
                 g.BlackPlayer.Contains(player2, StringComparison.OrdinalIgnoreCase)) ||
                (g.WhitePlayer.Contains(player2, StringComparison.OrdinalIgnoreCase) &&
                 g.BlackPlayer.Contains(player1, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        /// <summary>
        /// Gets all games in the database.
        /// </summary>
        /// <returns>All recorded games</returns>
        public List<GameRecord> GetAllGames()
        {
            var games = new List<GameRecord>();

            try
            {
                foreach (var filePath in gameIdToFileMap.Values)
                {
                    try
                    {
                        var game = LoadGameFromFile(filePath);
                        if (game != null)
                        {
                            games.Add(game);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.Warning($"Failed to load game from {filePath}: {ex.Message}");
                    }
                }

                logger?.Info($"Loaded {games.Count} games from database");
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to load all games", ex);
            }

            return games;
        }

        /// <summary>
        /// Deletes a game from the database.
        /// </summary>
        /// <param name="gameId">The ID of the game to delete</param>
        /// <returns>True if game was deleted, false if not found</returns>
        public bool DeleteGame(int gameId)
        {
            try
            {
                if (!gameIdToFileMap.ContainsKey(gameId))
                {
                    logger?.Warning($"Cannot delete: Game ID {gameId} not found");
                    return false;
                }

                string filePath = gameIdToFileMap[gameId];
                string fileName = Path.GetFileName(filePath);

                File.Delete(filePath);
                gameIdToFileMap.Remove(gameId);
                fileToGameIdMap.Remove(fileName);

                logger?.Info($"Deleted game {gameId}: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to delete game {gameId}", ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the total number of games in the database.
        /// </summary>
        /// <returns>Game count</returns>
        public int GetGameCount()
        {
            return gameIdToFileMap.Count;
        }

        /// <summary>
        /// Searches games by opening name.
        /// Note: Requires opening analysis to be performed first.
        /// </summary>
        /// <param name="openingName">The opening to search for</param>
        /// <returns>Games with this opening</returns>
        public List<GameRecord> SearchByOpening(string openingName)
        {
            var games = GetAllGames();

            // Search in Statistics dictionary where opening info might be stored
            return games.Where(g =>
                g.Statistics != null &&
                g.Statistics.Any(kvp =>
                    kvp.Key.Contains("Opening", StringComparison.OrdinalIgnoreCase) &&
                    kvp.Value.ToString().Contains(openingName, StringComparison.OrdinalIgnoreCase)
                )
            ).ToList();
        }

        /// <summary>
        /// Gets statistics about games in the database.
        /// </summary>
        /// <returns>Database statistics</returns>
        public DatabaseStatistics GetStatistics()
        {
            var games = GetAllGames();

            if (games.Count == 0)
            {
                return new DatabaseStatistics
                {
                    TotalGames = 0,
                    TotalMoves = 0,
                    AverageGameLength = 0,
                    MostCommonOpening = "None",
                    WinCounts = new Dictionary<string, int>()
                };
            }

            var stats = new DatabaseStatistics
            {
                TotalGames = games.Count,
                TotalMoves = games.Sum(g => g.TotalMoves),
                AverageGameLength = games.Average(g => g.TotalMoves),
                WinCounts = new Dictionary<string, int>
                {
                    ["White"] = games.Count(g => g.Winner == "White"),
                    ["Black"] = games.Count(g => g.Winner == "Black"),
                    ["Draw"] = games.Count(g => g.Winner == "Draw")
                }
            };

            // Find most common opening (if stored in statistics)
            var openings = games
                .Where(g => g.Statistics != null && g.Statistics.ContainsKey("Opening"))
                .GroupBy(g => g.Statistics["Opening"].ToString())
                .OrderByDescending(grp => grp.Count())
                .FirstOrDefault();

            stats.MostCommonOpening = openings?.Key ?? "Unknown";

            return stats;
        }

        /// <summary>
        /// Loads a game record from a JSON file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        /// <returns>The game record</returns>
        private GameRecord LoadGameFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath, Encoding.UTF8);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Deserialize<GameRecord>(jsonString, options);
            }
            catch (Exception ex)
            {
                logger?.Warning($"Failed to load game from file {filePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Refreshes the database index (useful if files were added externally).
        /// </summary>
        public void RefreshIndex()
        {
            RebuildIndex();
        }
    }
}
