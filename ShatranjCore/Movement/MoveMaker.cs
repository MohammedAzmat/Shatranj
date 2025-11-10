using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Board;
using ShatranjCore.Pieces;

namespace ShatranjCore.Movement
{
    interface MoveMaker
    {
        Move MakeMove();
    }

    public class Move
    {
        private Square from, to;
        private Piece piece, capturedPiece;
        public Square From { get { return from; } }
        public Square To { get { return to; } }
        public Piece Piece { get { return piece; } }
        public Piece CapturedPiece { get { return capturedPiece; } }

        public Move(Piece _piece, Square _from, Square _to, Piece _captured = null)
        {
            this.from = _from;
            this.to = _to;
            this.piece = _piece;
            this.capturedPiece = _captured;
        }

        public List<Move> GetVerticleMoves(Square source, ChessBoard board)
        {
            List<Move> possibleMoves = new List<Move>();
            Square temp = new Square(source.Location.Row, source.Location.Column);
            int i = 1;
            while ((temp.Location.Row+i) < 8 && board.IsEmptyAt(temp.Location.Row+i,temp.Location.Column))
            {
                temp.Location = new Location(temp.Location.Row + 1, temp.Location.Column);
                possibleMoves.Add(new Move(source.Piece, source, temp));
            }
            return possibleMoves;
        }
    }
}
