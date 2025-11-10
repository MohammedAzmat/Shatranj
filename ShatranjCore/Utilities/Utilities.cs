using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShatranjCore.Pieces;

namespace ShatranjCore.Utilities
{
    public class Utilities
    {
        public void PrintEmptyBoard()
        {
            String[,] squares = new String[8,8];
            string dashed_line = "---------------------------------";
            Console.WriteLine(dashed_line);
            string blank = "|    ";
            string piece = "";
            //Console.WriteLine("| X | X | X | X | X | X | X | X |");
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write("| Xw ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(dashed_line);
        }
        public void PrintInitialBoard()
        {
            String[,] squares = new String[8, 8];
            string dashed_line = "---------------------------------";
            Console.WriteLine(dashed_line);
            string blank = "|    ";
            string piece = "";
        }

        public void PrintSquare(Piece piece)
        { }


       
    }
}
