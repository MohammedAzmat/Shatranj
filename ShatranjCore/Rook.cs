using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class Rook : Piece
    {

        public Rook(int i, int j, PieceColor pcolor) : base(i, j, pcolor)
        {

        }

        public override bool IsCaptured()
        {
            throw new NotImplementedException();
        }

        public override Square[] ValidMoves()
        {
            throw new NotImplementedException();
        }

        internal override List<Move> GetMoves(Location source, ChessBoard board)
        {
            //Check again if this piece is at source
            List<Move> possibleMoves = new List<Move>();
            if (this.GetType().Name == board.GetPiece(source).GetType().Name)
            {
                //14 max moves, 7 verticle and 7 horizontal

            }
            return possibleMoves;
        }
    }
}
