using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Persistence;

namespace ShatranjCore.Settings
{
    /// <summary>
    /// Manages game settings including profile names, opponent names, and difficulty levels.
    /// Responsibility: Business logic only (no UI rendering).
    ///
    /// NOTE: UI display logic has been extracted to SettingsMenuUI.
    /// Use ShowSettingsMenu() -> SettingsMenuUI for user-facing operations.
    /// </summary>
    public class SettingsManager
    {
        private readonly GameConfigManager configManager;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new SettingsManager
        /// </summary>
        /// <param name="configManager">Configuration manager for persisting settings</param>
        /// <param name="logger">Logger for recording settings operations</param>
        public SettingsManager(GameConfigManager configManager, ILogger logger)
        {
            this.configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// DEPRECATED: Use SettingsMenuUI.ShowSettingsMenu() instead.
        /// This method is kept for backward compatibility but does nothing.
        /// </summary>
        [Obsolete("Use SettingsMenuUI.ShowSettingsMenu() instead", false)]
        public void ShowSettingsMenu()
        {
            logger.Info("ShowSettingsMenu called on SettingsManager (deprecated)");
            // UI display logic moved to SettingsMenuUI
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
                logger.Info($"Profile name changed to: {name}");
                return name;
            }
            catch (Exception ex)
            {
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
                logger.Info($"Opponent name changed to: {name}");
                return name;
            }
            catch (Exception ex)
            {
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
                        throw new ArgumentException("Difficulty must be between 1-5", nameof(difficultyStr));
                }
            }
            else
            {
                // Try parsing as difficulty name
                if (!Enum.TryParse<DifficultyLevel>(difficultyStr, true, out newDifficulty))
                {
                    throw new ArgumentException("Invalid difficulty level. Use: easy, medium, hard, veryhard, titan, or 1-5", nameof(difficultyStr));
                }
            }

            try
            {
                configManager.SetDifficulty(newDifficulty);
                logger.Info($"Difficulty changed to: {newDifficulty}");
                return newDifficulty;
            }
            catch (Exception ex)
            {
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

                logger.Info("Settings reset to defaults");
                return config;
            }
            catch (Exception ex)
            {
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
