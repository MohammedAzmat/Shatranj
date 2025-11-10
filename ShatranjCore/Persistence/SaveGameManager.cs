using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace ShatranjCore.Persistence
{
    /// <summary>
    /// Manages save game slots, autosave, and cleanup
    /// </summary>
    public class SaveGameManager
    {
        private const int MAX_SAVE_SLOTS = 10;
        private const string AUTOSAVE_FILENAME = "autosave.json";

        private readonly string saveDirectory;
        private readonly GameSerializer serializer;
        private readonly ILogger logger;

        public SaveGameManager(ILogger logger = null, string saveDirectory = null)
        {
            this.logger = logger;

            if (string.IsNullOrEmpty(saveDirectory))
            {
                this.saveDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj",
                    "SavedGames"
                );
            }
            else
            {
                this.saveDirectory = saveDirectory;
            }

            Directory.CreateDirectory(this.saveDirectory);
            this.serializer = new GameSerializer(logger, this.saveDirectory);
        }

        /// <summary>
        /// Saves a game with a specific game ID
        /// </summary>
        public string SaveGame(GameStateSnapshot snapshot, int gameId)
        {
            string fileName = $"game_{gameId}.json";
            string filePath = Path.Combine(saveDirectory, fileName);

            // Ensure we don't exceed max saves (excluding autosave)
            EnforceMaxSaveSlots();

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(filePath, jsonString);

                logger?.Info($"Game {gameId} saved successfully");
                return filePath;
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to save game {gameId}", ex);
                throw new InvalidOperationException($"Failed to save game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves the game as autosave
        /// </summary>
        public string SaveAutosave(GameStateSnapshot snapshot)
        {
            string filePath = Path.Combine(saveDirectory, AUTOSAVE_FILENAME);

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(filePath, jsonString);

                logger?.Debug("Autosave completed");
                return filePath;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to autosave game", ex);
                throw new InvalidOperationException($"Failed to autosave: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a game by game ID
        /// </summary>
        public GameStateSnapshot LoadGame(int gameId)
        {
            string fileName = $"game_{gameId}.json";
            string filePath = Path.Combine(saveDirectory, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Save file for game {gameId} not found");
            }

            return LoadGameFromPath(filePath);
        }

        /// <summary>
        /// Loads the autosave file
        /// </summary>
        public GameStateSnapshot LoadAutosave()
        {
            string filePath = Path.Combine(saveDirectory, AUTOSAVE_FILENAME);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("No autosave file found");
            }

            return LoadGameFromPath(filePath);
        }

        /// <summary>
        /// Checks if an autosave exists
        /// </summary>
        public bool AutosaveExists()
        {
            string filePath = Path.Combine(saveDirectory, AUTOSAVE_FILENAME);
            return File.Exists(filePath);
        }

        /// <summary>
        /// Loads a game from a file path
        /// </summary>
        private GameStateSnapshot LoadGameFromPath(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var snapshot = JsonSerializer.Deserialize<GameStateSnapshot>(jsonString, options);
                logger?.Info($"Game loaded from {Path.GetFileName(filePath)}");
                return snapshot;
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to load game from {filePath}", ex);
                throw new InvalidOperationException($"Failed to load game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lists all saved games with metadata
        /// </summary>
        public List<GameMetadata> ListSavedGames()
        {
            var savedGames = new List<GameMetadata>();

            try
            {
                var files = Directory.GetFiles(saveDirectory, "game_*.json")
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList();

                foreach (var file in files)
                {
                    try
                    {
                        var snapshot = LoadGameFromPath(file);
                        savedGames.Add(snapshot.ToMetadata());
                    }
                    catch (Exception ex)
                    {
                        logger?.Warning($"Failed to load metadata from {Path.GetFileName(file)}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to list saved games", ex);
            }

            return savedGames;
        }

        /// <summary>
        /// Gets metadata for autosave if it exists
        /// </summary>
        public GameMetadata GetAutosaveMetadata()
        {
            try
            {
                if (AutosaveExists())
                {
                    var snapshot = LoadAutosave();
                    return snapshot.ToMetadata();
                }
            }
            catch (Exception ex)
            {
                logger?.Warning($"Failed to load autosave metadata: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Deletes a saved game by game ID
        /// </summary>
        public bool DeleteGame(int gameId)
        {
            string fileName = $"game_{gameId}.json";
            string filePath = Path.Combine(saveDirectory, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    logger?.Info($"Deleted save game {gameId}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger?.Error($"Failed to delete save game {gameId}", ex);
                return false;
            }
        }

        /// <summary>
        /// Deletes the autosave file
        /// </summary>
        public bool DeleteAutosave()
        {
            string filePath = Path.Combine(saveDirectory, AUTOSAVE_FILENAME);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    logger?.Info("Deleted autosave");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to delete autosave", ex);
                return false;
            }
        }

        /// <summary>
        /// Enforces the maximum number of save slots
        /// Deletes oldest saves if we exceed MAX_SAVE_SLOTS
        /// </summary>
        private void EnforceMaxSaveSlots()
        {
            try
            {
                var files = Directory.GetFiles(saveDirectory, "game_*.json")
                    .OrderBy(f => File.GetLastWriteTime(f))
                    .ToList();

                // If we're at or over the limit, delete the oldest
                while (files.Count >= MAX_SAVE_SLOTS)
                {
                    string oldestFile = files[0];
                    File.Delete(oldestFile);
                    logger?.Info($"Deleted oldest save to maintain {MAX_SAVE_SLOTS} slot limit: {Path.GetFileName(oldestFile)}");
                    files.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                logger?.Warning($"Failed to enforce save slot limit: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the save directory path
        /// </summary>
        public string GetSaveDirectory() => saveDirectory;
    }
}
