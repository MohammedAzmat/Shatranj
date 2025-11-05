using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Pieces;

namespace ShatranjCore.Board
{
    public class Square
    {
        //private int row, column;
        //private Piece piece;
        public Location Location { get; set; }
        //public int Row { get; set; }
        //public int Column { get; set; }
        public Piece Piece { get; set; }

        public bool IsEmpty() { return (this.Piece == null)?true:false; }
        public Piece GetPiece() { return Piece; }

        public Square(int i, int j, Piece p = null)
        {
            Location = new Location(i, j);
            this.Piece = p;
        }

        public Square(Location l, Piece p = null)
        {
            Location = l;
            this.Piece = p;
        }

        //internal void Initialize(int i, int j)
        //{
        //    //throw new NotImplementedException();
        //    this.Row = i;
        //    this.Column = j;
        //    this.Piece = null;
        //}

        //internal void Initialize(int i, int j, Piece p = null)
        //{
        //    this.Row = i;
        //    this.Column = j;
        //    this.Piece = p;
        //}
    }
}
