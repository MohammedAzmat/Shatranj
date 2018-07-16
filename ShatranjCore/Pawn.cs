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

        internal override List<Move> GetMoves(Location source, ChessBoard board)
        {
            //throw new NotImplementedException();
            //Check again if this piece is at source
            List<Move> possibleMoves = new List<Move>();
            if (this.GetType().Name == board.GetPiece(source).GetType().Name)
            {
                // |||| Pawn Moves ||||
                //Empty block(s) ahead
                int dirMult = GetDirectionMultiplier();
                if (board.IsEmptyAt(source.Row + 1 * dirMult, source.Column))
                {
                    possibleMoves.Append(new Move(this, new Square(source.Row, source.Column, this), new Square(source.Row + 1 * dirMult, source.Column), null));
                    if(!this.isMoved && (board.IsEmptyAt(source.Row + 2 * dirMult, source.Column)))
                        possibleMoves.Append(new Move(this, new Square(source.Row, source.Column, this), new Square(source.Row + 2 * dirMult, source.Column), null));
                }
                //Capture
                Location temp = new Location();
                temp.Row = source.Row + 1 * dirMult;
                temp.Column = source.Column - 1;
                if (temp.Column >= 0 && ((!board.IsEmptyAt(temp.Row, temp.Column))&&(board.GetPiece(temp).Color != this.Color)))
                    possibleMoves.Append(new Move(this, new Square(source.Row, source.Column, this), new Square(temp.Row, temp.Column, board.GetPiece(temp)), board.GetPiece(temp)));
                //Similarly Right Capture
                temp.Column = source.Column + 1;
                if (temp.Column < 8 && ((!board.IsEmptyAt(temp.Row, temp.Column)) && (board.GetPiece(temp).Color != this.Color)))
                    possibleMoves.Append(new Move(this, new Square(source.Row, source.Column, this), new Square(temp.Row, temp.Column, board.GetPiece(temp)), board.GetPiece(temp)));

            }

            return possibleMoves;
        }

        private int GetDirectionMultiplier()
        {
            //throw new NotImplementedException();
            return (direction == PawnMoves.Up) ? 1 : -1;
        }
    }
}
