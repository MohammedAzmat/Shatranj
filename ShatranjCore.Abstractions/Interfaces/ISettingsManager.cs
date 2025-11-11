namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for managing game settings
    /// </summary>
    public interface ISettingsManager
    {
        string SetProfileName(string name);
        string SetOpponentName(string name);
        DifficultyLevel SetDifficulty(string difficultyStr);
        object ResetToDefaults();
        object GetCurrentConfig();
    }
}
