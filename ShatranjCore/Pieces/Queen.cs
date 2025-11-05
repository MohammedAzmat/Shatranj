using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Pieces
{
    public class Queen : Piece
    {
        public Queen(int i, int j, PieceColor p1color) : base(i,j,p1color)
        {
        }

        public override List<Move> GetMoves(Location source, IChessBoard board)
        {
            List<Move> possibleMoves = new List<Move>();

            // Verify this piece is at the source location
            Piece pieceAtSource = board.GetPiece(source);
            if (pieceAtSource == null || pieceAtSource.GetType() != this.GetType())
                return possibleMoves;

            // Queen moves like both Rook and Bishop
            // 8 directions: 4 straight (like Rook) + 4 diagonal (like Bishop)
            int[,] directions = {
                {-1, 0},   // Up (Rook)
                {1, 0},    // Down (Rook)
                {0, -1},   // Left (Rook)
                {0, 1},    // Right (Rook)
                {-1, -1},  // Up-Left (Bishop)
                {-1, 1},   // Up-Right (Bishop)
                {1, -1},   // Down-Left (Bishop)
                {1, 1}     // Down-Right (Bishop)
            };

            for (int dir = 0; dir < 8; dir++)
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
