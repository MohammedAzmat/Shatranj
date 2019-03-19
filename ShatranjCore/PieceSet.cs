using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class PieceSet
    {
        private PieceColor color;
        public List<Piece> Pieces;
        public PieceColor PieceSetColor { get { return color; } }

        public PieceSet(PieceColor color)
        {
            this.color = color;
            this.Pieces = new List<Piece>();
        }
    }
}
