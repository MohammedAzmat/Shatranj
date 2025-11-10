using System.Collections.Generic;

namespace ShatranjCore.Abstractions
{
    /// <summary>
    /// Interface for chess board implementations.
    /// Follows Dependency Inversion Principle - depend on abstractions, not concretions.
    /// Note: This interface uses object types for pieces to avoid circular dependencies.
    /// The actual implementation will use concrete Piece types.
    /// </summary>
    public interface IChessBoard
    {
        /// <summary>
        /// Gets the piece at the specified location.
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>The piece at that location, or null if empty</returns>
        object GetPiece(Location location);

        /// <summary>
        /// Checks if a square at the given position is empty.
        /// </summary>
        /// <param name="row">Row index (0-7)</param>
        /// <param name="column">Column index (0-7)</param>
        /// <returns>True if empty or out of bounds, false otherwise</returns>
        bool IsEmptyAt(int row, int column);

        /// <summary>
        /// Gets all pieces of the specified color that are currently on the board.
        /// </summary>
        /// <param name="color">The piece color to filter by</param>
        /// <returns>List of pieces of the specified color</returns>
        List<object> GetPiecesOfColor(PieceColor color);

        /// <summary>
        /// Gets all opponent pieces for the given color.
        /// </summary>
        /// <param name="color">The friendly piece color</param>
        /// <returns>List of opponent pieces</returns>
        List<object> GetOpponentPieces(PieceColor color);

        /// <summary>
        /// Places a piece at the specified location.
        /// </summary>
        /// <param name="piece">The piece to place</param>
        /// <param name="location">The location to place it</param>
        void PlacePiece(object piece, Location location);

        /// <summary>
        /// Removes the piece at the specified location.
        /// </summary>
        /// <param name="location">The location to clear</param>
        /// <returns>The piece that was removed, or null if empty</returns>
        object RemovePiece(Location location);

        /// <summary>
        /// Finds the King of the specified color.
        /// </summary>
        /// <param name="color">The color of the King to find</param>
        /// <returns>The King piece, or null if not found</returns>
        object FindKing(PieceColor color);

        /// <summary>
        /// Checks if a location is within the board boundaries.
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>True if within bounds (0-7), false otherwise</returns>
        bool IsInBounds(int row, int column);

        /// <summary>
        /// Checks if a location is within the board boundaries.
        /// </summary>
        /// <param name="location">The location to check</param>
        /// <returns>True if within bounds, false otherwise</returns>
        bool IsInBounds(Location location);
    }
}
