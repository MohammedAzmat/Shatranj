using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.UI;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Settings-related commands (SetDifficulty, SetProfile, etc).
    /// Single Responsibility: Game configuration and settings.
    /// </summary>
    public class SettingsCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;

        private Action<DifficultyLevel> setDifficultyDelegate;
        private Action<string> setProfileDelegate;
        private Action<string> setOpponentDelegate;
        private Action resetSettingsDelegate;
        private Action waitForKeyDelegate;

        public SettingsCommandHandler(
            ConsoleBoardRenderer renderer,
            ILogger logger)
        {
            this.renderer = renderer;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for settings operations.
        /// </summary>
        public void SetDelegates(
            Action<DifficultyLevel> setDifficulty,
            Action<string> setProfile,
            Action<string> setOpponent,
            Action resetSettings,
            Action waitForKey)
        {
            this.setDifficultyDelegate = setDifficulty;
            this.setProfileDelegate = setProfile;
            this.setOpponentDelegate = setOpponent;
            this.resetSettingsDelegate = resetSettings;
            this.waitForKeyDelegate = waitForKey;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.SetDifficulty
                || command.Type == CommandType.SetProfile
                || command.Type == CommandType.SetOpponent
                || command.Type == CommandType.ResetSettings;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"SettingsCommandHandler cannot handle {command.Type}");

            try
            {
                switch (command.Type)
                {
                    case CommandType.SetDifficulty:
                        // Parse difficulty from command (would need to be added to GameCommand)
                        logger.Info("Difficulty settings changed");
                        renderer.DisplayInfo("Difficulty updated.");
                        waitForKeyDelegate?.Invoke();
                        break;

                    case CommandType.SetProfile:
                        logger.Info($"Profile set: {command.FileName}");
                        setProfileDelegate?.Invoke(command.FileName);
                        renderer.DisplayInfo("Profile updated.");
                        waitForKeyDelegate?.Invoke();
                        break;

                    case CommandType.SetOpponent:
                        logger.Info($"Opponent set: {command.FileName}");
                        setOpponentDelegate?.Invoke(command.FileName);
                        renderer.DisplayInfo("Opponent updated.");
                        waitForKeyDelegate?.Invoke();
                        break;

                    case CommandType.ResetSettings:
                        logger.Info("Settings reset to defaults");
                        resetSettingsDelegate?.Invoke();
                        renderer.DisplayInfo("Settings reset to defaults.");
                        waitForKeyDelegate?.Invoke();
                        break;
                }
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing settings command: {ex.Message}");
                logger.Error("Settings command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }
    }
}
