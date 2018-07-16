using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class Bishop : Piece
    {
        public Bishop(int i, int j, PieceColor pc) : base(i,j,pc)
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
    }
}
