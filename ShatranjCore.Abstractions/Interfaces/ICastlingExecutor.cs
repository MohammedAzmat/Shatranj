namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for executing castling moves
    /// </summary>
    public interface ICastlingExecutor
    {
        void ExecuteCastle(IBoardState board, PieceColor color, CastlingSide side);
    }
}
