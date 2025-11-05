using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;
using ShatranjCore.Utilities;

namespace ShatranjCore.Board
{
    public class ChessBoard : IChessBoard
    {
        #region Class members
        private Square[,] squares;
        private PieceSet[] boardSet, fallenSet;
        private PieceColor p1color;
        #endregion

        /* Board Noations and Sample Board
         * R = Rook
         * N = Knight
         * B = Bishop
         * Q = Queen
         * K = King
         * P = Pawn
         * S = Blank / Empty Space
         * -----------------
         * |R|N|B|Q|K|B|N|R|
         * |P|P|P|P|P|P|P|P|
         * |S|S|S|S|S|S|S|S|
         * |S|S|S|S|S|S|S|S|
         * |S|S|S|S|S|S|S|S|
         * |S|S|S|S|S|S|S|S|
         * |P|P|P|P|P|P|P|P|
         * |R|N|B|Q|K|B|N|R|
         */

        #region Constructors
        /// <summary>
        /// Constructor for a new game.
        /// </summary>
        /// <param name="color">It signifies color of player going first</param>
        public ChessBoard(PieceColor color = PieceColor.White)
        {
            //Initialize Board
            p1color = color;
            boardSet = new PieceSet[2];
            // For board and Fallen Piece Sets 0 is always black and 1 is always white. Since these are just Lists we are maintaining.
            boardSet[0] = new PieceSet(PieceColor.Black);
            boardSet[1] = new PieceSet(PieceColor.White);

            fallenSet = new PieceSet[2];
            fallenSet[0] = new PieceSet(PieceColor.Black);
            fallenSet[1] = new PieceSet(PieceColor.White);
            InitializeBoard(p1color);
            
            //if (p1color == PieceColor.Black)
            //{
            //    //Collection of Pieces on Board
            //    boardSet[0] = new PieceSet();
            //    boardSet[0].PieceSetColor = p1color;
            //}

            
        }

        public ChessBoard(ChessBoard board, Location source, Location destination)
        {
            this.squares = board.squares;
            this.squares[destination.Row, destination.Column].Piece = board.GetPiece(source);
            this.squares[source.Row, source.Column].Piece = null;
        }

        internal bool IsEmptyAt(int row, int column)
        {
            //throw new NotImplementedException();
            if (row > 7 || row < 0) return false;
            if (column > 7 || column < 0) return false;
            return (squares[row, column].Piece == null) ? true : false;
        }



        #endregion

        #region Internal and Private Functions

        #region Board Initilization Functions

        private void InitializeBoard(PieceColor p1color)
        {
            //throw new NotImplementedException();
            squares = new Square[8,8];

            /* Each Line of the board should be like this.
             * |R|N|B|Q|K|B|N|R|    //Spl Forces
             * |P|P|P|P|P|P|P|P|    //Infantry
             * |S|S|S|S|S|S|S|S|    //Rest
             * |S|S|S|S|S|S|S|S|    //Rest
             * |S|S|S|S|S|S|S|S|    //Rest
             * |S|S|S|S|S|S|S|S|    //Rest
             * |P|P|P|P|P|P|P|P|    //Infantry
             * |R|N|B|Q|K|B|N|R|    //Spl Forces
             */
            
            InitializeRest();

            InitializeSplForces(p1color);
            InitializeInfantry(p1color);
        }

        private void InitializeRest()
        {
            //throw new NotImplementedException();
            //Row 3 - 6 should be empty
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    squares[i, j] = new Square(i,j);
        }

        private void InitializeInfantry(PieceColor p1color)
        {
            //throw new NotImplementedException();
            // Row 2 and 7
            PawnMoves direction = PawnMoves.Down;
            for (int i = 1; i < 8; i+=5)
            {
                p1color = ChangePlayerColor(p1color);
                int index = (p1color == PieceColor.Black) ? 0 : 1;

                for (int j = 0; j < 8; j++)
                {
                    //squares[i, j] = new Square(i, j, new Pawn(i, j, ChangePlayerColor(p1color),direction));
                    squares[i, j].Piece = new Pawn(i, j, p1color, direction);
                    boardSet[index].Pieces.Append(squares[i, j].Piece);

                }
                direction = ChangeDirection(direction);
            }
        }

        internal Piece GetPiece(Location source)
        {
            //throw new NotImplementedException();
            return squares[source.Row, source.Column].Piece;
        }

        private void InitializeSplForces(PieceColor p1color)
        {
            //throw new NotImplementedException();
            

            for (int i = 0; i < 8; i+=7)
            {
                //Because we set player 2 first, from chess board perspective
                p1color = ChangePlayerColor(p1color);
                int index = (p1color == PieceColor.Black) ? 0 : 1;
                for (int j = 0; j < 8; j++)
                    switch (j)
                    {
                        //Rook
                        case 0:
                        case 7:
                            //squares[i, j] = new Square(i, j, new Rook(i, j, p1color));
                            squares[i, j].Piece = new Rook(i, j, p1color);
                            boardSet[index].Pieces.Append(squares[i, j].Piece);
                            break;
                        //Knight
                        case 1:
                        case 6:
                            //squares[i, j] = new Square(i, j, new Knight(i, j, p1color));
                            squares[i, j].Piece = new Knight(i, j, p1color);
                            boardSet[index].Pieces.Append(squares[i, j].Piece);

                            break;
                        //Bishop
                        case 2:
                        case 5:
                            //squares[i, j] = new Square(i, j, new Bishop(i, j, p1color));
                            squares[i, j].Piece = new Bishop(i, j, p1color);
                            boardSet[index].Pieces.Append(squares[i, j].Piece);

                            break;
                        //Queen
                        case 3:
                            //squares[i, j] = new Square(i, j, new Queen(i, j, p1color));
                            squares[i, j].Piece = new Queen(i, j, p1color);
                            boardSet[index].Pieces.Append(squares[i, j].Piece);

                            break;
                        //King
                        case 4:
                            //squares[i, j] = new Square(i, j, new King(i, j, p1color));
                            squares[i, j].Piece = new King(i, j, p1color);
                            boardSet[index].Pieces.Append(squares[i, j].Piece);

                            break;
                    }
            }
        }

        #endregion

        private PieceColor ChangePlayerColor(PieceColor p1color)
        {
            //throw new NotImplementedException();
            return (p1color == PieceColor.Black) ? PieceColor.White : PieceColor.Black;
        }

        private PawnMoves ChangeDirection(PawnMoves direction)
        { return (direction == PawnMoves.Down) ? PawnMoves.Up : PawnMoves.Down; }

        internal void DisplayCurrentBoard()
        {

            string dash = "|--------------------------------------------------------------|";
            string colLable = "\t | aA\t| bB\t| cC\t| dD\t| eE\t| fF\t| gG\t| hH\t|";
            string footer = "";

            Console.Clear();
            for (int i = 0; i < 8; i++)
            {
                Console.Write("\t "+dash+"\n\t ");
                for (int j = 0; j < 8; j++)
                    Console.Write("|\t");
                Console.Write("|\n\t"+(8-i));
                for (int j = 0; j < 8; j++)
                    Console.Write("| " + ((squares[i, j].Piece == null) ? "__" : squares[i, j].Piece.Notation) + "\t");
                Console.Write("|\n\t ");
                for (int j = 0; j < 8; j++)
                    Console.Write("|\t");
                Console.WriteLine("|");

            }
            Console.WriteLine("\t "+dash+"\n"+ colLable);
            Console.WriteLine("\tPlease Enter to play, Press ESC to quite playing.");



        }

        public List<Piece> GetOppPieces(PieceColor color) { return new List<Piece>(); }

        #endregion

        #region IChessBoard Implementation

        /// <summary>
        /// Gets all pieces of the specified color currently on the board.
        /// </summary>
        public List<Piece> GetPiecesOfColor(PieceColor color)
        {
            int index = (color == PieceColor.Black) ? 0 : 1;
            return boardSet[index].Pieces.Where(p => p != null && !p.IsCaptured()).ToList();
        }

        /// <summary>
        /// Gets all opponent pieces for the given color.
        /// </summary>
        public List<Piece> GetOpponentPieces(PieceColor color)
        {
            PieceColor opponentColor = (color == PieceColor.Black) ? PieceColor.White : PieceColor.Black;
            return GetPiecesOfColor(opponentColor);
        }

        /// <summary>
        /// Places a piece at the specified location.
        /// </summary>
        public void PlacePiece(Piece piece, Location location)
        {
            if (!IsInBounds(location))
                throw new ArgumentOutOfRangeException(nameof(location), "Location is out of bounds");

            squares[location.Row, location.Column].Piece = piece;
            if (piece != null)
            {
                piece.location = location;
            }
        }

        /// <summary>
        /// Removes the piece at the specified location.
        /// </summary>
        public Piece RemovePiece(Location location)
        {
            if (!IsInBounds(location))
                throw new ArgumentOutOfRangeException(nameof(location), "Location is out of bounds");

            Piece piece = squares[location.Row, location.Column].Piece;
            squares[location.Row, location.Column].Piece = null;
            return piece;
        }

        /// <summary>
        /// Finds the King of the specified color.
        /// </summary>
        public King FindKing(PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = squares[row, col].Piece;
                    if (piece is King king && king.Color == color)
                    {
                        return king;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a location is within board boundaries (0-7).
        /// </summary>
        public bool IsInBounds(int row, int column)
        {
            return row >= 0 && row < 8 && column >= 0 && column < 8;
        }

        /// <summary>
        /// Checks if a location is within board boundaries.
        /// </summary>
        public bool IsInBounds(Location location)
        {
            return IsInBounds(location.Row, location.Column);
        }

        #endregion
    }
}
