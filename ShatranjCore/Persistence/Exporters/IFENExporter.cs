using ShatranjCore.Interfaces;

namespace ShatranjCore.Persistence.Exporters
{
    /// <summary>
    /// Interface for exporting board position in FEN (Forsyth-Edwards Notation) format.
    /// Single Responsibility: Convert board state to FEN string.
    /// </summary>
    public interface IFENExporter
    {
        /// <summary>
        /// Exports a board position to FEN format.
        /// </summary>
        string Export(IChessBoard board);
    }
}
