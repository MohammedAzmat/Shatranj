using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class Queen : Piece
    {
        public Queen(int i, int j, PieceColor p1color) : base(i,j,p1color)
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
            throw new NotImplementedException();
        }

        internal override bool IsBlockingCheck(Location source, ChessBoard board)
        {
            throw new NotImplementedException();
        }
    }
}
