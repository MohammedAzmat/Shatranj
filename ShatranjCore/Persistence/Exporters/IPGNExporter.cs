using System.Collections.Generic;
using ShatranjCore.Movement;

namespace ShatranjCore.Persistence.Exporters
{
    /// <summary>
    /// Interface for exporting games in PGN (Portable Game Notation) format.
    /// Single Responsibility: Convert move history to PGN string.
    /// </summary>
    public interface IPGNExporter
    {
        /// <summary>
        /// Exports a list of moves to PGN format.
        /// </summary>
        string Export(List<MoveRecord> moves, PGNMetadata metadata = null);
    }

    /// <summary>
    /// Metadata for a PGN game (Event, Site, Date, White, Black, Result, etc).
    /// </summary>
    public class PGNMetadata
    {
        public string Event { get; set; }
        public string Site { get; set; }
        public string Date { get; set; }
        public string White { get; set; }
        public string Black { get; set; }
        public string Result { get; set; }  // "1-0", "0-1", "1/2-1/2", "*"
        public string WhiteElo { get; set; }
        public string BlackElo { get; set; }
        public string TimeControl { get; set; }
        public string ECO { get; set; }  // Opening code
        public string Opening { get; set; }
    }
}
