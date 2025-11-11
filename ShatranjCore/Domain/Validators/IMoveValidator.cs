using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;

namespace ShatranjCore.Domain.Validators
{
    /// <summary>
    /// Strategy interface for move validation.
    /// Each validator checks one specific aspect of move legality.
    /// Follows Strategy Pattern and Single Responsibility Principle.
    /// </summary>
    public interface IMoveValidator
    {
        /// <summary>
        /// Validate a move according to this validator's rules.
        /// Returns null if validation passes, or error message if it fails.
        /// </summary>
        string Validate(Location from, Location to, PieceColor currentPlayer, IChessBoard board);

        /// <summary>
        /// Get the name of this validator.
        /// </summary>
        string GetName();
    }
}
