namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for validating castling moves
    /// </summary>
    public interface ICastlingValidator
    {
        bool CanCastleKingside(IBoardState board, PieceColor color);
        bool CanCastleQueenside(IBoardState board, PieceColor color);
    }
}
