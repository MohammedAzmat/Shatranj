using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Pieces;

namespace ShatranjCore.Persistence
{
    /// <summary>
    /// Factory for creating chess pieces from type names
    /// </summary>
    public static class PieceFactory
    {
        /// <summary>
        /// Creates a piece instance from a type name
        /// </summary>
        public static Piece CreatePiece(string typeName, PieceColor color, int row, int col)
        {
            switch (typeName)
            {
                case "Pawn":
                    // Determine pawn direction based on color
                    PawnMoves direction = (color == PieceColor.White) ? PawnMoves.Up : PawnMoves.Down;
                    return new Pawn(row, col, color, direction);
                case "Rook":
                    return new Rook(row, col, color);
                case "Knight":
                    return new Knight(row, col, color);
                case "Bishop":
                    return new Bishop(row, col, color);
                case "Queen":
                    return new Queen(row, col, color);
                case "King":
                    return new King(row, col, color);
                default:
                    throw new ArgumentException($"Unknown piece type: {typeName}");
            }
        }
    }
}
