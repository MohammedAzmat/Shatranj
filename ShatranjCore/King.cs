using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class King : Piece
    {
        readonly char notation = 'K';

        public char Notation { get { return notation; } }
        
        public King(int i, int j, PieceColor p1color) : base(i,j,p1color)
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
