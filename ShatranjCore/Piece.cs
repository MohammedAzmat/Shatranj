using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public enum PieceColor
    {
        White, Black
    }

    public enum PawnMoves { Up, Down}
    public struct Location
    {
        int row, column;
        public int Row
        {
            get { return row; }
            set
            {
                if (value > 7)
                    row = 7;
                else if (value < 0)
                    row = 0;
                else
                    row = value;
            }
        }
        public int Column
        {
            get { return column; }
            set
            {
                if (value > 7)
                    column = 7;
                else if (value < 0)
                    column = 0;
                else
                    column = value;
            }
        }
    }
    public abstract class Piece
    {
        private PieceColor color;
        public Location location;
        public bool isMoved;
        private readonly string notation;

        public string Notation { get { return notation; } }
        public PieceColor Color { get { return color; } }

        public Piece(int row, int column, PieceColor pieceColor)
        {
            
            location.Row = row;
            location.Column = column;
            color = pieceColor;
            isMoved = false;
            notation = color.ToString().Substring(0, 1).ToLower() + ((this.GetType().Name == "Knight")?this.GetType().Name.Substring(1,1).ToUpper():this.GetType().Name.Substring(0, 1));
        }

        abstract public Square[] ValidMoves();
        abstract public bool IsCaptured();
        internal abstract List<Move> GetMoves(Location source, ChessBoard board);
    }
}
