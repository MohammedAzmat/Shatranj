using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Pieces
{
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
            location = new Location(row, column);
            color = pieceColor;
            isMoved = false;
            notation = color.ToString().Substring(0, 1).ToLower() + ((this.GetType().Name == "Knight")?this.GetType().Name.Substring(1,1).ToUpper():this.GetType().Name.Substring(0, 1));
        }

        abstract public Square[] ValidMoves();
        abstract public bool IsCaptured();
        public abstract List<Move> GetMoves(Location source, IChessBoard board);
        public abstract bool IsBlockingCheck(Location source, IChessBoard board);
        public abstract bool CanMove(Location source, Location destination, IChessBoard board);
    }
}
