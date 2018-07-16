using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class Square
    {
        private int row, column;
        private Piece piece;
        public int Row { get; set; }
        public int Column { get; set; }
        public Piece Piece { get; set; }

        public bool IsEmpty() { return true; }
        public Piece GetPiece() { return piece; }

        public Square(int i, int j, Piece p = null)
        {
            this.Row = i;
            this.Column = j;
            this.Piece = p;
        }

        //internal void Initialize(int i, int j)
        //{
        //    //throw new NotImplementedException();
        //    this.Row = i;
        //    this.Column = j;
        //    this.Piece = null;
        //}

        internal void Initialize(int i, int j, Piece p = null)
        {
            this.Row = i;
            this.Column = j;
            this.Piece = p;
        }
    }
}
