namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for handling game menus
    /// </summary>
    public interface IMenuHandler
    {
        GameMode ShowGameModeMenu();
        PieceColor ShowColorSelectionMenu();
        MainMenuChoice ShowMainMenu(bool hasAutosave);
    }

    /// <summary>
    /// Main menu choices
    /// </summary>
    public enum MainMenuChoice
    {
        Resume,
        NewGame,
        LoadGame,
        Settings,
        Exit
    }
}
