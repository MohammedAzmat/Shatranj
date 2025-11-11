using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjCore.Domain.Validators
{
    /// <summary>
    /// Validates that a piece CAN move to the destination.
    /// Checks piece movement rules (not legality regarding king safety).
    /// Single Responsibility: Only check if piece movement is valid.
    /// </summary>
    public class PieceMoveValidator : IMoveValidator
    {
        public string GetName() => "Piece Movement";

        public string Validate(Location from, Location to, PieceColor currentPlayer, IChessBoard board)
        {
            // Get piece at source
            var piece = board.GetPiece(from);

            if (piece == null)
                return $"No piece at {LocationToAlgebraic(from)}";

            if (piece.Color != currentPlayer)
                return $"That piece belongs to {piece.Color}, not {currentPlayer}";

            // Check if piece can move to destination
            if (!piece.CanMove(from, to, board))
                return $"Illegal move for {piece.GetType().Name}";

            return null;  // Valid
        }

        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
