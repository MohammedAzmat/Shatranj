using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    /// <summary>
    /// Class for Top Most functionality of the game, it sets up the game and it's environment.
    /// </summary>
    public class ChessGame
    {
        #region Variables
        //Game Players
        Player[] players;
        //Game Board
        ChessBoard board;
        //Other Stuff
        #endregion

        #region Constructors
        
        public ChessGame(PlayerType[] types, PieceColor _p1Color = PieceColor.White, List<Move> moves = null)
        {
            players = new Player[2];
            PieceColor[] pieceColors = new PieceColor[2];
            pieceColors[0] = _p1Color;
            pieceColors[1] = (PieceColor.Black == _p1Color) ? PieceColor.White : PieceColor.Black;
            for (int i = 0; i < players.Length; i++)
                    players[i] = new Player(pieceColors[i], types[i]);
            //Check Board
            if (moves == null)
            {
                //Set turn for Player1
                players[0].HasTurn = true;
                //New Game; board is not set
                board = new ChessBoard(players[0].Color);
                this.Play();
            }

        }

        private void Play()
        {
            bool play = true;
            while (play)
            {
                //((players[0].HasTurn) ? players[0].Color : players[1].Color)
                board.DisplayCurrentBoard();
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine(((players[0].HasTurn) ? players[0].Color : players[1].Color).ToString() + " to play!!\t Input Format: From-To  a2-a4\nEnter a Valid Choice: ");
                        //If Valid Move is entered Redraw the board after moving
                        if (ValidMove(Console.ReadLine()))
                            MakeMove();
                        else
                        {
                            //INVALID move
                            bool stuck = true;
                            while (stuck)
                            {
                                Console.WriteLine("INVALID MOVE!!!!! Please Enter a Legal Move\t Press Enter to Continue....");
                                if (Console.ReadKey().Key == ConsoleKey.Enter)
                                    stuck = false;
                            }
                        }
                        break;
                    case ConsoleKey.Escape:
                        play = false;
                        break;
                }
            }
            Console.WriteLine("Thank you for playing fairly!");

        }

        private void MakeMove()
        {
            throw new NotImplementedException();
        }

        private bool ValidMove(string v)
        {
            string[] split = v.Split('-');
            Location source = DecodeInput(split[0]), destination = DecodeInput(split[1]);
            if (source.Row == -1 || source.Column == -1 || destination.Row == -1 || destination.Column == -1) return false;
            //source = DecodeInput(split[0]);
            //destination = DecodeInput(split[1]);
            Piece sourcePiece = board.GetPiece(source);
            if (sourcePiece == null) return false;
            List<Move> moves = sourcePiece.GetMoves(source, board);
            //TODO: Finish Valid moves.
            return false;
        }

        private Location DecodeInput(string v)
        {
            Location location = new Location();
            v = v.Trim().ToLower();
            switch (v[0])
            {
                case 'a':
                    location.Column = 0;
                    break;
                case 'b':
                    location.Column = 1;
                    break;
                case 'c':
                    location.Column = 2;
                    break;
                case 'd':
                    location.Column = 3;
                    break;
                case 'e':
                    location.Column = 4;
                    break;
                case 'f':
                    location.Column = 5;
                    break;
                case 'g':
                    location.Column = 6;
                    break;
                case 'h':
                    location.Column = 7;
                    break;
                default:
                    location.Column = -1;
                    break;
            }
            switch (v[1])
            {
                case '8':
                    location.Row = 0;
                    break;
                case '7':
                    location.Row = 1;
                    break;
                case '6':
                    location.Row = 2;
                    break;
                case '5':
                    location.Row = 3;
                    break;
                case '4':
                    location.Row = 4;
                    break;
                case '3':
                    location.Row = 5;
                    break;
                case '2':
                    location.Row = 6;
                    break;
                case '1':
                    location.Row = 7;
                    break;
                default:
                    location.Row = -1;
                    break;
            }

            return location;

        }


        #endregion
    }
}
