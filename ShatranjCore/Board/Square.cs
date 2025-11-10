using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Pieces;

namespace ShatranjCore.Board
{
    public class Square
    {
        public Location Location { get; set; }
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
    }
}
