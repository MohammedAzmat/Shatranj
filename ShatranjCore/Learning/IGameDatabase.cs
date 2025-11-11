using System.Collections.Generic;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Persistent storage and retrieval of recorded games.
    /// Single Responsibility: Game persistence (save/load/query).
    /// Supports building game history for AI training and human analysis.
    /// </summary>
    public interface IGameDatabase
    {
        /// <summary>
        /// Saves a game record to the database.
        /// </summary>
        /// <param name="game">The game to save</param>
        /// <returns>The ID of the saved game</returns>
        int SaveGame(GameRecord game);

        /// <summary>
        /// Loads a game record by ID.
        /// </summary>
        /// <param name="gameId">The ID of the game to load</param>
        /// <returns>The game record, or null if not found</returns>
        GameRecord LoadGame(int gameId);

        /// <summary>
        /// Gets all games by a specific player name.
        /// </summary>
        /// <param name="playerName">Name of the player</param>
        /// <returns>List of games played by the player</returns>
        List<GameRecord> GetGamesByPlayer(string playerName);

        /// <summary>
        /// Gets all games with a specific outcome.
        /// </summary>
        /// <param name="outcome">Win, Loss, or Draw</param>
        /// <returns>Games with that outcome</returns>
        List<GameRecord> GetGamesByOutcome(string outcome);

        /// <summary>
        /// Gets games between two players.
        /// </summary>
        /// <param name="player1">First player name</param>
        /// <param name="player2">Second player name</param>
        /// <returns>Games between these players</returns>
        List<GameRecord> GetGamesBetweenPlayers(string player1, string player2);

        /// <summary>
        /// Gets all games in the database.
        /// </summary>
        /// <returns>All recorded games</returns>
        List<GameRecord> GetAllGames();

        /// <summary>
        /// Deletes a game from the database.
        /// </summary>
        /// <param name="gameId">The ID of the game to delete</param>
        /// <returns>True if game was deleted, false if not found</returns>
        bool DeleteGame(int gameId);

        /// <summary>
        /// Gets the total number of games in the database.
        /// </summary>
        /// <returns>Game count</returns>
        int GetGameCount();

        /// <summary>
        /// Searches games by opening name.
        /// </summary>
        /// <param name="openingName">The opening to search for</param>
        /// <returns>Games with this opening</returns>
        List<GameRecord> SearchByOpening(string openingName);

        /// <summary>
        /// Gets statistics about games in the database.
        /// </summary>
        /// <returns>Database statistics</returns>
        DatabaseStatistics GetStatistics();
    }

    /// <summary>
    /// Statistics about the game database.
    /// </summary>
    public class DatabaseStatistics
    {
        public int TotalGames { get; set; }
        public int TotalMoves { get; set; }
        public double AverageGameLength { get; set; }
        public string MostCommonOpening { get; set; }
        public Dictionary<string, int> WinCounts { get; set; }
    }
}
