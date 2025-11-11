using System;

namespace ShatranjCore.Abstractions.Interfaces
{
    /// <summary>
    /// Interface for creating chess pieces
    /// </summary>
    public interface IPieceFactory
    {
        object CreatePiece(string type, PieceColor color, int row, int column);
        object CreatePiece(Type type, PieceColor color, Location location);
    }
}
