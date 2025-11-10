using System;
using ShatranjCore.Abstractions;

namespace ShatranjCore.Validators
{
    /// <summary>
    /// Tracks en passant opportunities.
    /// Follows Single Responsibility Principle - only tracks en passant state.
    /// </summary>
    public class EnPassantTracker
    {
        private Location? enPassantTarget;
        private int enPassantValidTurn;
        private int currentTurn;

        public EnPassantTracker()
        {
            enPassantTarget = null;
            enPassantValidTurn = -1;
            currentTurn = 0;
        }

        /// <summary>
        /// Records a pawn double move that enables en passant.
        /// </summary>
        public void RecordPawnDoubleMove(Location from, Location to)
        {
            // Check if this was a two-square pawn move
            int rowDiff = Math.Abs(to.Row - from.Row);
            if (rowDiff == 2)
            {
                // The en passant target is the square the pawn passed over
                int passedOverRow = (from.Row + to.Row) / 2;
                enPassantTarget = new Location(passedOverRow, to.Column);
                enPassantValidTurn = currentTurn + 1; // Valid for next turn only
            }
            else
            {
                // Not a double move - clear en passant opportunity
                ClearEnPassant();
            }
        }

        /// <summary>
        /// Clears the en passant opportunity (called after any non-pawn-double move).
        /// </summary>
        public void ClearEnPassant()
        {
            enPassantTarget = null;
            enPassantValidTurn = -1;
        }

        /// <summary>
        /// Checks if en passant is available at the given target square.
        /// </summary>
        public bool IsEnPassantAvailable(Location targetSquare)
        {
            if (!enPassantTarget.HasValue)
                return false;

            if (currentTurn != enPassantValidTurn)
                return false;

            return enPassantTarget.Value.Row == targetSquare.Row &&
                   enPassantTarget.Value.Column == targetSquare.Column;
        }

        /// <summary>
        /// Gets the current en passant target square (if any).
        /// </summary>
        public Location? GetEnPassantTarget()
        {
            if (currentTurn != enPassantValidTurn)
                return null;

            return enPassantTarget;
        }

        /// <summary>
        /// Gets the location of the pawn that can be captured via en passant.
        /// </summary>
        public Location? GetEnPassantCaptureLocation()
        {
            if (!enPassantTarget.HasValue || currentTurn != enPassantValidTurn)
                return null;

            // The capturable pawn is on the same file as the target, but on the adjacent row
            // If target is row 2 (white capturing), pawn is on row 3
            // If target is row 5 (black capturing), pawn is on row 4
            int pawnRow = enPassantTarget.Value.Row == 2 ? 3 : 4;
            return new Location(pawnRow, enPassantTarget.Value.Column);
        }

        /// <summary>
        /// Advances to the next turn.
        /// </summary>
        public void NextTurn()
        {
            currentTurn++;
        }

        /// <summary>
        /// Resets the tracker for a new game.
        /// </summary>
        public void Reset()
        {
            enPassantTarget = null;
            enPassantValidTurn = -1;
            currentTurn = 0;
        }
    }
}
