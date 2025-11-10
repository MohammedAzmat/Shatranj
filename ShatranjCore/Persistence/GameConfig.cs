using System;
using System.IO;
using System.Text.Json;
using ShatranjCore.Abstractions;
using ShatranjCore.Logging;

namespace ShatranjCore.Persistence
{
    /// <summary>
    /// Manages global game configuration and settings
    /// </summary>
    [Serializable]
    public class GameConfig
    {
        public int NextGameId { get; set; } = 1;
        public string ProfileName { get; set; } = "Player";
        public string OpponentProfileName { get; set; } = "Player2";
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public GameConfig()
        {
        }
    }

    /// <summary>
    /// Manages loading and saving of game configuration
    /// </summary>
    public class GameConfigManager
    {
        private readonly string configDirectory;
        private readonly string configFilePath;
        private readonly ILogger logger;
        private GameConfig config;

        public GameConfigManager(ILogger logger = null, string configDirectory = null)
        {
            this.logger = logger;

            if (string.IsNullOrEmpty(configDirectory))
            {
                this.configDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Shatranj"
                );
            }
            else
            {
                this.configDirectory = configDirectory;
            }

            this.configFilePath = Path.Combine(this.configDirectory, "config.json");
            Directory.CreateDirectory(this.configDirectory);

            LoadConfig();
        }

        /// <summary>
        /// Loads configuration from file or creates default
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string jsonString = File.ReadAllText(configFilePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    config = JsonSerializer.Deserialize<GameConfig>(jsonString, options);
                    logger?.Info($"Config loaded successfully. Next Game ID: {config.NextGameId}");
                }
                else
                {
                    config = new GameConfig();
                    SaveConfig();
                    logger?.Info("Created new config file with default settings");
                }
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to load config, using defaults", ex);
                config = new GameConfig();
            }
        }

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                config.LastUpdated = DateTime.Now;
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                string jsonString = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configFilePath, jsonString);
                logger?.Debug("Config saved successfully");
            }
            catch (Exception ex)
            {
                logger?.Error("Failed to save config", ex);
            }
        }

        /// <summary>
        /// Gets the current configuration
        /// </summary>
        public GameConfig GetConfig() => config;

        /// <summary>
        /// Gets and increments the game ID
        /// </summary>
        public int GetNextGameId()
        {
            int gameId = config.NextGameId;
            config.NextGameId++;
            SaveConfig();
            return gameId;
        }

        /// <summary>
        /// Updates profile name
        /// </summary>
        public void SetProfileName(string name)
        {
            config.ProfileName = name;
            SaveConfig();
        }

        /// <summary>
        /// Updates opponent profile name
        /// </summary>
        public void SetOpponentProfileName(string name)
        {
            config.OpponentProfileName = name;
            SaveConfig();
        }

        /// <summary>
        /// Updates difficulty level
        /// </summary>
        public void SetDifficulty(DifficultyLevel difficulty)
        {
            config.Difficulty = difficulty;
            SaveConfig();
        }

        /// <summary>
        /// Resets all settings to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            config.NextGameId = 1;
            config.ProfileName = "Player";
            config.OpponentProfileName = "Player2";
            config.Difficulty = DifficultyLevel.Medium;
            SaveConfig();
            logger?.Info("Config reset to defaults");
        }

        /// <summary>
        /// Gets the config directory path
        /// </summary>
        public string GetConfigDirectory() => configDirectory;
    }
}
