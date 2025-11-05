using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class Pawn : Piece
    {
        private PawnMoves direction;
        private bool startpos;
        public PawnMoves Direction {get{ return direction; } }
        public Pawn(int row, int column, PieceColor pieceColor, PawnMoves _direction) : base(row, column, pieceColor)
        {
            direction = _direction;
            startpos = true;
        }

        
        public override bool IsCaptured()
        {
            throw new NotImplementedException();
        }

        public override Square[] ValidMoves()
        {
            throw new NotImplementedException();
        }

        private bool PromotionAvailable()
        {
            return ((location.Row == 0 && direction == PawnMoves.Up) || (location.Row == 7 && direction == PawnMoves.Down)) ? true : false;
        }

        internal override List<Move> GetMoves(Location source, IChessBoard board)
        {
            return GetMovesWithEnPassant(source, board, null);
        }

        /// <summary>
        /// Gets all possible moves for this pawn, including en passant if available.
        /// </summary>
        internal List<Move> GetMovesWithEnPassant(Location source, IChessBoard board, Location? enPassantTarget)
        {
            List<Move> possibleMoves = new List<Move>();

            // Verify this piece is at source
            Piece pieceAtSource = board.GetPiece(source);
            if (pieceAtSource == null || pieceAtSource.GetType() != this.GetType())
                return possibleMoves;

            int dirMult = GetDirectionMultiplier();

            // Forward moves
            int oneForward = source.Row + (1 * dirMult);
            if (board.IsInBounds(oneForward, source.Column) && board.IsEmptyAt(oneForward, source.Column))
            {
                possibleMoves.Add(new Move(
                    this,
                    new Square(source.Row, source.Column, this),
                    new Square(oneForward, source.Column),
                    null
                ));

                // Two square move from starting position
                if (!this.isMoved)
                {
                    int twoForward = source.Row + (2 * dirMult);
                    if (board.IsInBounds(twoForward, source.Column) && board.IsEmptyAt(twoForward, source.Column))
                    {
                        possibleMoves.Add(new Move(
                            this,
                            new Square(source.Row, source.Column, this),
                            new Square(twoForward, source.Column),
                            null
                        ));
                    }
                }
            }

            // Diagonal captures (left)
            int captureRow = source.Row + (1 * dirMult);
            int leftColumn = source.Column - 1;
            if (board.IsInBounds(captureRow, leftColumn))
            {
                Piece leftPiece = board.GetPiece(new Location(captureRow, leftColumn));
                if (leftPiece != null && leftPiece.Color != this.Color)
                {
                    possibleMoves.Add(new Move(
                        this,
                        new Square(source.Row, source.Column, this),
                        new Square(captureRow, leftColumn, leftPiece),
                        leftPiece
                    ));
                }
            }

            // Diagonal captures (right)
            int rightColumn = source.Column + 1;
            if (board.IsInBounds(captureRow, rightColumn))
            {
                Piece rightPiece = board.GetPiece(new Location(captureRow, rightColumn));
                if (rightPiece != null && rightPiece.Color != this.Color)
                {
                    possibleMoves.Add(new Move(
                        this,
                        new Square(source.Row, source.Column, this),
                        new Square(captureRow, rightColumn, rightPiece),
                        rightPiece
                    ));
                }
            }

            // En passant capture
            if (enPassantTarget.HasValue)
            {
                Location target = enPassantTarget.Value;

                // Check if we can capture via en passant (diagonally to the en passant target square)
                if (target.Row == captureRow)
                {
                    if (target.Column == leftColumn || target.Column == rightColumn)
                    {
                        // The capturable pawn is on the same row as us, adjacent column
                        Location capturablePawnLocation = new Location(source.Row, target.Column);
                        Piece capturablePawn = board.GetPiece(capturablePawnLocation);

                        if (capturablePawn != null && capturablePawn is Pawn && capturablePawn.Color != this.Color)
                        {
                            possibleMoves.Add(new Move(
                                this,
                                new Square(source.Row, source.Column, this),
                                new Square(target.Row, target.Column),
                                capturablePawn // The pawn being captured
                            ));
                        }
                    }
                }
            }

            return possibleMoves;
        }

        private int GetDirectionMultiplier()
        {
            //throw new NotImplementedException();
            return (direction == PawnMoves.Up) ? 1 : -1;
        }

        internal override bool CanMove(Location source, Location destination, IChessBoard board)
        {
            List<Move> validMoves = GetMoves(source, board);
            return validMoves.Any(m =>
                m.To.Location.Row == destination.Row &&
                m.To.Location.Column == destination.Column);
        }

        internal override bool IsBlockingCheck(Location source, IChessBoard board)
        {
            // TODO: Implement check blocking logic in Phase 1
            // A piece blocks check if removing it would put its own King in check
            throw new NotImplementedException();
        }
    }
}
