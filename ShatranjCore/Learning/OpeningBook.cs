using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ShatranjCore.Learning
{
    /// <summary>
    /// Represents a single chess opening with moves and metadata
    /// </summary>
    public class Opening
    {
        public string Eco { get; set; }
        public string Name { get; set; }
        public List<string> Moves { get; set; }
        public string Category { get; set; }

        public Opening()
        {
            Moves = new List<string>();
        }
    }

    /// <summary>
    /// Container for opening book data loaded from JSON
    /// </summary>
    public class OpeningBookData
    {
        public List<Opening> Openings { get; set; }

        public OpeningBookData()
        {
            Openings = new List<Opening>();
        }
    }

    /// <summary>
    /// Loads and manages chess opening theory
    /// Provides methods to match game moves to known openings
    /// </summary>
    public class OpeningBook
    {
        private readonly List<Opening> openings;
        private readonly Dictionary<string, Opening> ecoToOpening;

        public OpeningBook()
        {
            openings = new List<Opening>();
            ecoToOpening = new Dictionary<string, Opening>();
            LoadOpenings();
        }

        /// <summary>
        /// Loads openings from embedded JSON resource or file
        /// </summary>
        private void LoadOpenings()
        {
            try
            {
                // Try to load from file in Learning folder
                string openingBookPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Learning",
                    "OpeningBook.json"
                );

                // Fallback: try current directory
                if (!File.Exists(openingBookPath))
                {
                    openingBookPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "ShatranjCore",
                        "Learning",
                        "OpeningBook.json"
                    );
                }

                // Fallback: try one level up
                if (!File.Exists(openingBookPath))
                {
                    openingBookPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "..",
                        "ShatranjCore",
                        "Learning",
                        "OpeningBook.json"
                    );
                }

                if (File.Exists(openingBookPath))
                {
                    string json = File.ReadAllText(openingBookPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var data = JsonSerializer.Deserialize<OpeningBookData>(json, options);

                    if (data?.Openings != null)
                    {
                        openings.AddRange(data.Openings);

                        foreach (var opening in data.Openings)
                        {
                            if (!string.IsNullOrEmpty(opening.Eco))
                            {
                                ecoToOpening[opening.Eco] = opening;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If loading fails, continue with empty opening book
                // This allows the system to work without openings
            }
        }

        /// <summary>
        /// Matches a sequence of moves to the best opening
        /// </summary>
        /// <param name="moves">List of moves in algebraic notation</param>
        /// <param name="maxMoves">Maximum number of moves to consider (default 12)</param>
        /// <returns>The matched opening or null</returns>
        public Opening MatchOpening(List<string> moves, int maxMoves = 12)
        {
            if (moves == null || moves.Count == 0)
                return null;

            // Take first N moves
            int movesToCheck = Math.Min(moves.Count, maxMoves);
            var gameMoves = moves.Take(movesToCheck).ToList();

            // Find longest matching opening
            Opening bestMatch = null;
            int longestMatch = 0;

            foreach (var opening in openings)
            {
                if (opening.Moves == null || opening.Moves.Count == 0)
                    continue;

                // Check if opening moves match game moves
                int matchLength = 0;
                for (int i = 0; i < Math.Min(gameMoves.Count, opening.Moves.Count); i++)
                {
                    if (NormalizeMoveNotation(gameMoves[i]) == NormalizeMoveNotation(opening.Moves[i]))
                    {
                        matchLength++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Update best match if this is longer
                if (matchLength > longestMatch && matchLength == opening.Moves.Count)
                {
                    longestMatch = matchLength;
                    bestMatch = opening;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Gets opening by ECO code
        /// </summary>
        public Opening GetOpeningByEco(string eco)
        {
            if (string.IsNullOrEmpty(eco))
                return null;

            return ecoToOpening.ContainsKey(eco) ? ecoToOpening[eco] : null;
        }

        /// <summary>
        /// Gets all openings in a category
        /// </summary>
        public List<Opening> GetOpeningsByCategory(string category)
        {
            return openings.Where(o =>
                o.Category != null &&
                o.Category.Equals(category, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        /// <summary>
        /// Searches openings by name (case-insensitive)
        /// </summary>
        public List<Opening> SearchOpenings(string query)
        {
            if (string.IsNullOrEmpty(query))
                return new List<Opening>();

            return openings.Where(o =>
                o.Name != null &&
                o.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        /// <summary>
        /// Gets total number of openings in the book
        /// </summary>
        public int Count => openings.Count;

        /// <summary>
        /// Normalizes move notation for comparison
        /// Removes check symbols, captures, etc.
        /// </summary>
        private string NormalizeMoveNotation(string move)
        {
            if (string.IsNullOrEmpty(move))
                return string.Empty;

            // Remove check/checkmate symbols
            move = move.Replace("+", "").Replace("#", "");

            // For basic comparison, just use the move as-is
            // More sophisticated normalization can be added later
            return move.Trim();
        }

        /// <summary>
        /// Gets all loaded openings
        /// </summary>
        public List<Opening> GetAllOpenings()
        {
            return new List<Opening>(openings);
        }
    }
}
