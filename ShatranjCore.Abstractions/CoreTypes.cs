using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore.Abstractions
{
    /// <summary>
    /// Enum representing the color of chess pieces
    /// </summary>
    public enum PieceColor
    {
        White,
        Black
    }

    /// <summary>
    /// Enum representing pawn movement direction
    /// </summary>
    public enum PawnMoves
    {
        Up,
        Down
    }

    /// <summary>
    /// Struct representing a location on the chess board
    /// </summary>
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

        public Location(int r, int c)
        {
            row = r;
            column = c;
        }
    }

    /// <summary>
    /// Enum representing player type
    /// </summary>
    public enum PlayerType
    {
        Human,
        AI
    }

    /// <summary>
    /// Enum representing game mode
    /// </summary>
    public enum GameMode
    {
        HumanVsHuman,
        HumanVsAI,
        AIVsAI
    }

    /// <summary>
    /// Enum representing AI difficulty levels
    /// </summary>
    public enum DifficultyLevel
    {
        Easy = 2,       // Depth 2
        Medium = 3,     // Depth 3
        Hard = 4,       // Depth 4
        VeryHard = 5,   // Depth 5
        Titan = 6       // Depth 6
    }

    /// <summary>
    /// Enum representing game result
    /// </summary>
    public enum GameResult
    {
        InProgress,
        WhiteWins,
        BlackWins,
        Checkmate,
        Stalemate,
        Draw,
        Resignation
    }

    /// <summary>
    /// Enum representing save type
    /// </summary>
    public enum SaveType
    {
        Manual,     // User-initiated save
        Auto        // Automatic save after each turn
    }
}
