namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for detecting check, checkmate, and stalemate
    /// </summary>
    public interface ICheckDetector
    {
        bool IsKingInCheck(IBoardState board, PieceColor color);
        bool IsCheckmate(IBoardState board, PieceColor color);
        bool IsStalemate(IBoardState board, PieceColor color);
        bool WouldMoveCauseCheck(IBoardState board, Location from, Location to, PieceColor color);
    }
}
