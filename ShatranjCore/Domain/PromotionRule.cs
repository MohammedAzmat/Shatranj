using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjCore.Domain
{
    /// <summary>
    /// Handles pawn promotion business logic (detection only).
    /// Responsibility: Determine if a pawn needs promotion.
    /// </summary>
    public class PromotionRule : IPromotionRule
    {
        /// <summary>
        /// Checks if a pawn at the given location needs promotion.
        /// </summary>
        /// <param name="piece">The piece to check (cast from object to Piece)</param>
        /// <param name="location">The location the piece is at</param>
        /// <returns>True if the piece is a pawn that has reached the promotion rank</returns>
        public bool NeedsPromotion(object piece, Location location)
        {
            if (!(piece is Pawn pawn))
                return false;

            // White pawns promote on row 0, black pawns on row 7
            if (pawn.Color == PieceColor.White && location.Row == 0)
                return true;

            if (pawn.Color == PieceColor.Black && location.Row == 7)
                return true;

            return false;
        }
    }
}
