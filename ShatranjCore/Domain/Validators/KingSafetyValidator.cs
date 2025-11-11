using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;
using ShatranjCore.Validators;

namespace ShatranjCore.Domain.Validators
{
    /// <summary>
    /// Validates that a move doesn't put own king in check.
    /// Ensures move legality from a king safety perspective.
    /// Single Responsibility: Only check king safety.
    /// </summary>
    public class KingSafetyValidator : IMoveValidator
    {
        private readonly CheckDetector checkDetector;

        public KingSafetyValidator(CheckDetector checkDetector)
        {
            this.checkDetector = checkDetector;
        }

        public string GetName() => "King Safety";

        public string Validate(Location from, Location to, PieceColor currentPlayer, IChessBoard board)
        {
            // Check if move would put own king in check
            if (checkDetector.WouldMoveCauseCheck(board, from, to, currentPlayer))
            {
                return "That move would leave your King in check!";
            }

            return null;  // Valid
        }
    }
}
