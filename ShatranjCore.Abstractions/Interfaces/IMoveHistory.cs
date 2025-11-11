using System.Collections.Generic;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for storing move history
    /// </summary>
    public interface IMoveHistory
    {
        void AddMove(Move move, PieceColor player, bool wasCapture, bool wasCheck, bool wasCheckmate);
        MoveRecord GetLastMove();
        List<MoveRecord> GetAllMoves();
        void Clear();
    }

    /// <summary>
    /// Record of a move
    /// </summary>
    public class MoveRecord
    {
        public Move Move { get; set; }
        public PieceColor Player { get; set; }
        public bool WasCapture { get; set; }
        public bool WasCheck { get; set; }
        public bool WasCheckmate { get; set; }
        public string Notation { get; set; }
    }
}
