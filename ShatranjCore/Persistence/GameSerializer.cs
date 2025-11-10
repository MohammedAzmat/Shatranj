using System;
using System.IO;
using System.Text;
using System.Text.Json;
using ShatranjCore.Logging;

namespace ShatranjCore.Persistence
{
    /// <summary>
    /// Handles game state serialization and deserialization
    /// </summary>
    public class GameSerializer
    {
        private readonly string saveDirectory;
        private readonly ILogger logger;

        public GameSerializer(ILogger logger = null, string saveDirectory = null)
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
        }

        /// <summary>
        /// Saves a game state to a JSON file
        /// </summary>
        public string SaveGame(GameStateSnapshot snapshot, string fileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"shatranj_save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                }

                if (!fileName.EndsWith(".json"))
                {
                    fileName += ".json";
                }

                string filePath = Path.Combine(saveDirectory, fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(filePath, jsonString, Encoding.UTF8);

                logger?.Info($"Game saved successfully to: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to save game", ex);
                throw new InvalidOperationException($"Failed to save game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a game state from a JSON file
        /// </summary>
        public GameStateSnapshot LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Save file not found: {filePath}");
                }

                string jsonString = File.ReadAllText(filePath, Encoding.UTF8);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var snapshot = JsonSerializer.Deserialize<GameStateSnapshot>(jsonString, options);

                logger?.Info($"Game loaded successfully from: {filePath}");
                return snapshot;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to load game", ex);
                throw new InvalidOperationException($"Failed to load game: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lists all saved games in the save directory
        /// </summary>
        public string[] ListSavedGames()
        {
            try
            {
                return Directory.GetFiles(saveDirectory, "*.json");
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to list saved games", ex);
                return new string[0];
            }
        }

        /// <summary>
        /// Deletes a saved game file
        /// </summary>
        public bool DeleteGame(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    logger?.Info($"Deleted saved game: {filePath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to delete saved game", ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the save directory path
        /// </summary>
        public string GetSaveDirectory() => saveDirectory;
    }
}
