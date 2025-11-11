using System.Collections.Generic;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for saving and loading games
    /// </summary>
    public interface ISaveGameManager
    {
        string SaveGame(object snapshot, int gameId);
        object LoadGame(int gameId);
        object LoadAutosave();
        bool AutosaveExists();
        List<GameMetadata> ListSavedGames();
        void DeleteGame(int gameId);
    }

    /// <summary>
    /// Metadata about a saved game
    /// </summary>
    public class GameMetadata
    {
        public int GameId { get; set; }
        public string GameMode { get; set; }
        public string SavedAt { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public int MoveCount { get; set; }
    }
}
