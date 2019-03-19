using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public class King : Piece
    {
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }
        
        public King(int i, int j, PieceColor p1color) : base(i,j,p1color)
        {
            isChecked = false;
        }

        public override bool IsCaptured()
        {
            throw new NotImplementedException();
        }

        public override Square[] ValidMoves()
        {
            throw new NotImplementedException();
        }

        internal override List<Move> GetMoves(Location source, ChessBoard board)
        {
            //throw new NotImplementedException();
            //King Moves in a wierd way, i.e. it cannot move to an empty box also, if it is snipped by an enemy Unit.
            //So we add all possible moves and then remove, unavailable moves
            List<Move> possibleMoves = null;
            if (this.GetType().Name == board.GetPiece(source).GetType().Name)
            {
                possibleMoves = new List<Move>();

                //King Moves in all directions, 1 step
                //Forward
                if ((board.IsEmptyAt(source.Row + 1, source.Column)))
                {
                    if(!IsCheckedAt(source, NewLocation(source.Row + 1, source.Column), board))
                        possibleMoves.Append(new Move(this, new Square(source.Row, source.Column, this), new Square(source.Row + 1, source.Column), null));

                }


            }

            return possibleMoves;
        }

        private bool IsCheckedAt(Location source, Location location, ChessBoard board)
        {
            //throw new NotImplementedException();
            ChessBoard tempBoard = new ChessBoard(board, source, location);
            if (this.GetType().Name == tempBoard.GetPiece(location).GetType().Name)
            {
                if (this.Color == tempBoard.GetPiece(location).Color)
                {
                    //Get All Opponents pieces to check if we are in check.
                    //foreach(Piece p in tempBoard.)
                }
            }
            return false; //TODO: Change this
        }

        

        /// <summary>
        /// NOT IMPLEMENTED AS King Cannot Block a Check against itself
        /// </summary>
        /// <param name="source"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        internal override bool IsBlockingCheck(Location source, ChessBoard board)
        {
            throw new NotImplementedException();
            //return false;
        }

        private Location NewLocation(int row, int column)
        {
            Location temp = new Location();
            temp.Row = row;
            temp.Column = column;
            return temp;
        }
    }
}
