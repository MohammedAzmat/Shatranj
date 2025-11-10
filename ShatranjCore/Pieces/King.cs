using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Pieces
{
    public class King : Piece
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public King(int i, int j, PieceColor p1color) : base(i,j,p1color)
        {
            isChecked = false;
        }

        public override List<Move> GetMoves(Location source, IChessBoard board)
        {
            List<Move> possibleMoves = new List<Move>();

            // Verify this piece is at the source location
            Piece pieceAtSource = board.GetPiece(source);
            if (pieceAtSource == null || pieceAtSource.GetType() != this.GetType())
                return possibleMoves;

            // King moves one square in any direction (8 possible moves)
            int[,] directions = {
                {-1, -1},  // Up-Left
                {-1, 0},   // Up
                {-1, 1},   // Up-Right
                {0, -1},   // Left
                {0, 1},    // Right
                {1, -1},   // Down-Left
                {1, 0},    // Down
                {1, 1}     // Down-Right
            };

            for (int i = 0; i < 8; i++)
            {
                int newRow = source.Row + directions[i, 0];
                int newCol = source.Column + directions[i, 1];

                // Check if out of bounds
                if (!board.IsInBounds(newRow, newCol))
                    continue;

                Location targetLocation = new Location(newRow, newCol);
                Piece targetPiece = board.GetPiece(targetLocation);

                // Can't move to a square occupied by own piece
                if (targetPiece != null && targetPiece.Color == this.Color)
                    continue;

                // TODO: Add check detection - King cannot move into check
                // For now, add all legal squares (will enhance with check detection later)

                if (targetPiece == null)
                {
                    // Empty square
                    possibleMoves.Add(new Move(
                        this,
                        new Square(source.Row, source.Column, this),
                        new Square(newRow, newCol),
                        null
                    ));
                }
                else
                {
                    // Enemy piece - can capture
                    possibleMoves.Add(new Move(
                        this,
                        new Square(source.Row, source.Column, this),
                        new Square(newRow, newCol, targetPiece),
                        targetPiece
                    ));
                }
            }

            // TODO: Add castling logic (requires checking if King/Rook have moved, no pieces between, not in check)

            return possibleMoves;
        }

        public override bool CanMove(Location source, Location destination, IChessBoard board)
        {
            List<Move> validMoves = GetMoves(source, board);
            return validMoves.Any(m =>
                m.To.Location.Row == destination.Row &&
                m.To.Location.Column == destination.Column);
        }

        /// <summary>
        /// NOT IMPLEMENTED - King cannot block a check against itself
        /// </summary>
        public override bool IsBlockingCheck(Location source, IChessBoard board)
        {
            // King cannot block check against itself - it must move
            return false;
        }

        /// <summary>
        /// Helper method to check if King would be in check at a given location
        /// TODO: Implement this when adding check detection system
        /// </summary>
        private bool WouldBeInCheckAt(Location targetLocation, IChessBoard board)
        {
            // This will be implemented with the check detection system
            // For now, return false (no check detection)
            return false;
        }
    }
}
