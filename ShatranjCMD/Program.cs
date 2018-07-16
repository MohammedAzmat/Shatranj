using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore;

namespace ShatranjCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            //ShatranjCore.Utilities ut = new Utilities();
            //ut.PrintEmptyBoard();
            Console.WriteLine("Initializing new Program....\nCreating a New Game Env");
            PlayerType[] players = new PlayerType[2];
            players[0] = PlayerType.Human;
            players[1] = PlayerType.Human;

            ChessGame game = new ChessGame(players, PieceColor.White);
            
            Console.ReadKey();
        }
    }
}
