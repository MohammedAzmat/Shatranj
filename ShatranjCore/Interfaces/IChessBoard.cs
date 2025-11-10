using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Pieces;

namespace ShatranjCore.Interfaces
{
    /// <summary>
    /// Interface for chess board implementations.
    /// Follows Dependency Inversion Principle - depend on abstractions, not concretions.
    /// Extends IBoardState and provides strongly-typed methods for Piece objects.
    /// </summary>
    public interface IChessBoard : IBoardState
    {
        /// <summary>
        /// Gets the piece at the specified location.
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>The piece at that location, or null if empty</returns>
        new Piece GetPiece(Location location);

        /// <summary>
        /// Gets all pieces of the specified color that are currently on the board.
        /// </summary>
        /// <param name="color">The piece color to filter by</param>
        /// <returns>List of pieces of the specified color</returns>
        new List<Piece> GetPiecesOfColor(PieceColor color);

        /// <summary>
        /// Gets all opponent pieces for the given color.
        /// </summary>
        /// <param name="color">The friendly piece color</param>
        /// <returns>List of opponent pieces</returns>
        new List<Piece> GetOpponentPieces(PieceColor color);

        /// <summary>
        /// Places a piece at the specified location.
        /// </summary>
        /// <param name="piece">The piece to place</param>
        /// <param name="location">The location to place it</param>
        void PlacePiece(Piece piece, Location location);

        /// <summary>
        /// Removes the piece at the specified location.
        /// </summary>
        /// <param name="location">The location to clear</param>
        /// <returns>The piece that was removed, or null if empty</returns>
        new Piece RemovePiece(Location location);

        /// <summary>
        /// Finds the King of the specified color.
        /// </summary>
        /// <param name="color">The color of the King to find</param>
        /// <returns>The King piece, or null if empty</returns>
        new King FindKing(PieceColor color);
    }
}
