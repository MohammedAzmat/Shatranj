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

        internal override bool CanMove(Location source, Location destination, ChessBoard board)
        {
            throw new NotImplementedException();
        }

        internal override List<Move> GetMoves(Location source, ChessBoard board)
        {
            throw new NotImplementedException();
            
        }

        private List<Location> GetMoves(Location source, ChessBoard board)
        {
            List<Location> moves = new List<Location>();
            GetMoves(source, board, moves);
            return moves;
        }

        private void GetMoves(Location location, ChessBoard board, List<Location> moves)
        {
            //throw new NotImplementedException();
            if (location.Column == 0 || location.Column == 7 || location.Row == 0 || location.Row == 7 || board.GetPiece(location) != null)
            {
                if (board.GetPiece(location) != null && board.GetPiece(location).Color != this.Color)
                {
                    moves.Add(new Move(this, location));
                }
            }
        }

        internal override bool IsBlockingCheck(Location source, ChessBoard board)
        {
            throw new NotImplementedException();
        }

        //private Move GetMove
    }
}
