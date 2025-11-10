using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Persistence;
using ShatranjCore.UI;

namespace ShatranjCore.Settings
{
    /// <summary>
    /// Manages game settings including profile names, opponent names, and difficulty levels.
    /// Provides UI for displaying and updating settings.
    /// </summary>
    public class SettingsManager
    {
        private readonly GameConfigManager configManager;
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new SettingsManager
        /// </summary>
        /// <param name="configManager">Configuration manager for persisting settings</param>
        /// <param name="renderer">Renderer for displaying settings UI</param>
        /// <param name="logger">Logger for recording settings operations</param>
        public SettingsManager(GameConfigManager configManager, ConsoleBoardRenderer renderer, ILogger logger)
        {
            this.configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the settings menu with current configuration
        /// </summary>
        public void ShowSettingsMenu()
        {
            try
            {
                var config = configManager.GetConfig();

                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo("           GAME SETTINGS");
                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo($"  Profile Name: {config.ProfileName}");
                renderer.DisplayInfo($"  Opponent Name: {config.OpponentProfileName}");
                renderer.DisplayInfo($"  Difficulty: {config.Difficulty} (Depth {(int)config.Difficulty})");
                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo("");
                renderer.DisplayInfo("Commands:");
                renderer.DisplayInfo("  settings profile [name]     - Set your name");
                renderer.DisplayInfo("  settings opponent [name]    - Set opponent name");
                renderer.DisplayInfo("  settings difficulty [level] - Set AI difficulty");
                renderer.DisplayInfo("    Difficulty options: easy, medium, hard, veryhard, titan");
                renderer.DisplayInfo("    Or use numbers: 1 (Easy) to 5 (Titan)");
                renderer.DisplayInfo("  settings reset              - Reset to defaults");
                renderer.DisplayInfo("");

                logger.Info("Settings menu displayed");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to display settings: {ex.Message}");
                logger.Error("Settings display failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the player profile name
        /// </summary>
        /// <param name="name">The new profile name</param>
        /// <returns>The updated profile name</returns>
        public string SetProfileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be empty", nameof(name));

            try
            {
                configManager.SetProfileName(name);
                renderer.DisplayInfo($"Profile name set to: {name}");
                logger.Info($"Profile name changed to: {name}");
                return name;
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set profile name: {ex.Message}");
                logger.Error("Set profile failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the opponent profile name
        /// </summary>
        /// <param name="name">The new opponent name</param>
        /// <returns>The updated opponent name</returns>
        public string SetOpponentName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Opponent name cannot be empty", nameof(name));

            try
            {
                configManager.SetOpponentProfileName(name);
                renderer.DisplayInfo($"Opponent name set to: {name}");
                logger.Info($"Opponent name changed to: {name}");
                return name;
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set opponent name: {ex.Message}");
                logger.Error("Set opponent failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the AI difficulty level
        /// </summary>
        /// <param name="difficultyStr">The difficulty level as string (name or number)</param>
        /// <returns>The updated difficulty level</returns>
        public DifficultyLevel SetDifficulty(string difficultyStr)
        {
            if (string.IsNullOrWhiteSpace(difficultyStr))
                throw new ArgumentException("Difficulty cannot be empty", nameof(difficultyStr));

            DifficultyLevel newDifficulty;

            // Try parsing as number first (1-5)
            if (int.TryParse(difficultyStr, out int difficultyNum))
            {
                // Map 1-5 to difficulty levels
                switch (difficultyNum)
                {
                    case 1:
                        newDifficulty = DifficultyLevel.Easy;
                        break;
                    case 2:
                        newDifficulty = DifficultyLevel.Medium;
                        break;
                    case 3:
                        newDifficulty = DifficultyLevel.Hard;
                        break;
                    case 4:
                        newDifficulty = DifficultyLevel.VeryHard;
                        break;
                    case 5:
                        newDifficulty = DifficultyLevel.Titan;
                        break;
                    default:
                        renderer.DisplayError("Difficulty must be between 1-5");
                        throw new ArgumentException("Difficulty must be between 1-5", nameof(difficultyStr));
                }
            }
            else
            {
                // Try parsing as difficulty name
                if (!Enum.TryParse<DifficultyLevel>(difficultyStr, true, out newDifficulty))
                {
                    renderer.DisplayError("Invalid difficulty. Use: easy, medium, hard, veryhard, titan, or 1-5");
                    throw new ArgumentException("Invalid difficulty level", nameof(difficultyStr));
                }
            }

            try
            {
                configManager.SetDifficulty(newDifficulty);
                renderer.DisplayInfo($"Difficulty set to: {newDifficulty} (Depth {(int)newDifficulty})");
                logger.Info($"Difficulty changed to: {newDifficulty}");
                return newDifficulty;
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set difficulty: {ex.Message}");
                logger.Error("Set difficulty failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Resets all settings to their default values
        /// </summary>
        /// <returns>The default configuration</returns>
        public GameConfig ResetToDefaults()
        {
            try
            {
                configManager.ResetToDefaults();
                var config = configManager.GetConfig();

                renderer.DisplayInfo("Settings reset to defaults:");
                renderer.DisplayInfo($"  Profile: {config.ProfileName}");
                renderer.DisplayInfo($"  Opponent: {config.OpponentProfileName}");
                renderer.DisplayInfo($"  Difficulty: {config.Difficulty}");

                logger.Info("Settings reset to defaults");
                return config;
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to reset settings: {ex.Message}");
                logger.Error("Settings reset failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the current configuration
        /// </summary>
        /// <returns>The current game configuration</returns>
        public GameConfig GetCurrentConfig()
        {
            return configManager.GetConfig();
        }
    }
}
