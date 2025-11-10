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
    public class Rook : Piece
    {
        public Rook(int i, int j, PieceColor pcolor) : base(i, j, pcolor)
        {
        }

        public override List<Move> GetMoves(Location source, IChessBoard board)
        {
            List<Move> possibleMoves = new List<Move>();

            // Verify this piece is at the source location
            Piece pieceAtSource = board.GetPiece(source);
            if (pieceAtSource == null || pieceAtSource.GetType() != this.GetType())
                return possibleMoves;

            // Rook moves in 4 directions: up, down, left, right
            // Direction vectors: (row_delta, col_delta)
            int[,] directions = {
                {-1, 0},  // Up
                {1, 0},   // Down
                {0, -1},  // Left
                {0, 1}    // Right
            };

            for (int dir = 0; dir < 4; dir++)
            {
                int rowDelta = directions[dir, 0];
                int colDelta = directions[dir, 1];

                // Keep moving in this direction until we hit a piece or board edge
                for (int step = 1; step < 8; step++)
                {
                    int newRow = source.Row + (step * rowDelta);
                    int newCol = source.Column + (step * colDelta);

                    // Check if out of bounds
                    if (!board.IsInBounds(newRow, newCol))
                        break;

                    Location targetLocation = new Location(newRow, newCol);
                    Piece targetPiece = board.GetPiece(targetLocation);

                    if (targetPiece == null)
                    {
                        // Empty square - valid move
                        possibleMoves.Add(new Move(
                            this,
                            new Square(source.Row, source.Column, this),
                            new Square(newRow, newCol),
                            null
                        ));
                    }
                    else
                    {
                        // Square occupied
                        if (targetPiece.Color != this.Color)
                        {
                            // Enemy piece - can capture
                            possibleMoves.Add(new Move(
                                this,
                                new Square(source.Row, source.Column, this),
                                new Square(newRow, newCol, targetPiece),
                                targetPiece
                            ));
                        }
                        // Can't move past this piece (friendly or enemy)
                        break;
                    }
                }
            }

            return possibleMoves;
        }

        public override bool CanMove(Location source, Location destination, IChessBoard board)
        {
            List<Move> validMoves = GetMoves(source, board);
            return validMoves.Any(m =>
                m.To.Location.Row == destination.Row &&
                m.To.Location.Column == destination.Column);
        }

        public override bool IsBlockingCheck(Location source, IChessBoard board)
        {
            // TODO: Implement check blocking logic in Phase 1
            // A piece blocks check if removing it would put its own King in check
            throw new NotImplementedException();
        }
    }
}
