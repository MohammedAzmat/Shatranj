using System.Collections.Generic;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for generating legal moves
    /// </summary>
    public interface IMoveGenerator
    {
        List<Move> GetLegalMoves(IBoardState board, Location from, PieceColor color, Location? enPassantTarget);
    }
}
