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
    public class Knight : Piece
    {
        public Knight(int i, int j, PieceColor pcolor) : base(i, j, pcolor)
        {
        }

        public override bool IsCaptured()
        {
            // A piece is captured when it's been removed from the board
            // This will be implemented when we add captured piece tracking
            return false;
        }

        public override Square[] ValidMoves()
        {
            throw new NotImplementedException();
        }

        public override List<Move> GetMoves(Location source, IChessBoard board)
        {
            List<Move> possibleMoves = new List<Move>();

            // Verify this piece is at the source location
            Piece pieceAtSource = board.GetPiece(source);
            if (pieceAtSource == null || pieceAtSource.GetType() != this.GetType())
                return possibleMoves;

            // Knight moves in an L-shape: 2 squares in one direction, 1 square perpendicular
            // 8 possible L-shaped moves
            int[,] knightMoves = {
                {-2, -1},  // 2 up, 1 left
                {-2, 1},   // 2 up, 1 right
                {-1, -2},  // 1 up, 2 left
                {-1, 2},   // 1 up, 2 right
                {1, -2},   // 1 down, 2 left
                {1, 2},    // 1 down, 2 right
                {2, -1},   // 2 down, 1 left
                {2, 1}     // 2 down, 1 right
            };

            for (int i = 0; i < 8; i++)
            {
                int newRow = source.Row + knightMoves[i, 0];
                int newCol = source.Column + knightMoves[i, 1];

                // Check if out of bounds
                if (!board.IsInBounds(newRow, newCol))
                    continue;

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
                else if (targetPiece.Color != this.Color)
                {
                    // Enemy piece - can capture
                    possibleMoves.Add(new Move(
                        this,
                        new Square(source.Row, source.Column, this),
                        new Square(newRow, newCol, targetPiece),
                        targetPiece
                    ));
                }
                // If friendly piece, can't move there (skip)
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
