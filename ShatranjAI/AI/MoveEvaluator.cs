using System;
using System.Collections.Generic;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjAI.AI
{
    /// <summary>
    /// Evaluates chess positions for AI decision-making
    /// </summary>
    public class MoveEvaluator
    {
        // Material values (in centipawns, 1 pawn = 100)
        private static readonly Dictionary<string, int> PieceValues = new Dictionary<string, int>
        {
            { "Pawn", 100 },
            { "Knight", 320 },
            { "Bishop", 330 },
            { "Rook", 500 },
            { "Queen", 900 },
            { "King", 20000 }
        };

        // Piece-square tables for positional evaluation (pawn example)
        // Values are from white's perspective (flip for black)
        private static readonly int[,] PawnTable = new int[8, 8]
        {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            {  5,  5, 10, 25, 25, 10,  5,  5 },
            {  0,  0,  0, 20, 20,  0,  0,  0 },
            {  5, -5,-10,  0,  0,-10, -5,  5 },
            {  5, 10, 10,-20,-20, 10, 10,  5 },
            {  0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] KnightTable = new int[8, 8]
        {
            { -50,-40,-30,-30,-30,-30,-40,-50 },
            { -40,-20,  0,  0,  0,  0,-20,-40 },
            { -30,  0, 10, 15, 15, 10,  0,-30 },
            { -30,  5, 15, 20, 20, 15,  5,-30 },
            { -30,  0, 15, 20, 20, 15,  0,-30 },
            { -30,  5, 10, 15, 15, 10,  5,-30 },
            { -40,-20,  0,  5,  5,  0,-20,-40 },
            { -50,-40,-30,-30,-30,-30,-40,-50 }
        };

        private static readonly int[,] BishopTable = new int[8, 8]
        {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5, 10, 10,  5,  0,-10 },
            { -10,  5,  5, 10, 10,  5,  5,-10 },
            { -10,  0, 10, 10, 10, 10,  0,-10 },
            { -10, 10, 10, 10, 10, 10, 10,-10 },
            { -10,  5,  0,  0,  0,  0,  5,-10 },
            { -20,-10,-10,-10,-10,-10,-10,-20 }
        };

        private static readonly int[,] RookTable = new int[8, 8]
        {
            {  0,  0,  0,  0,  0,  0,  0,  0 },
            {  5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            {  0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static readonly int[,] QueenTable = new int[8, 8]
        {
            { -20,-10,-10, -5, -5,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5,  5,  5,  5,  0,-10 },
            {  -5,  0,  5,  5,  5,  5,  0, -5 },
            {   0,  0,  5,  5,  5,  5,  0, -5 },
            { -10,  5,  5,  5,  5,  5,  0,-10 },
            { -10,  0,  5,  0,  0,  0,  0,-10 },
            { -20,-10,-10, -5, -5,-10,-10,-20 }
        };

        private static readonly int[,] KingMiddlegameTable = new int[8, 8]
        {
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -20,-30,-30,-40,-40,-30,-30,-20 },
            { -10,-20,-20,-20,-20,-20,-20,-10 },
            {  20, 20,  0,  0,  0,  0, 20, 20 },
            {  20, 30, 10,  0,  0, 10, 30, 20 }
        };

        /// <summary>
        /// Evaluates a board position from the perspective of the given color
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <param name="color">The color to evaluate for (positive = good for this color)</param>
        /// <returns>Evaluation score in centipawns</returns>
        public double Evaluate(IChessBoard board, PieceColor color)
        {
            double score = 0;

            // Material evaluation
            score += EvaluateMaterial(board, color);

            // Positional evaluation
            score += EvaluatePosition(board, color);

            // Mobility evaluation (simplified - count legal moves)
            // score += EvaluateMobility(board, color);

            return score;
        }

        /// <summary>
        /// Evaluates material balance
        /// </summary>
        private double EvaluateMaterial(IChessBoard board, PieceColor color)
        {
            double score = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.GetPiece(new Location(row, col));
                    if (piece != null)
                    {
                        int value = GetPieceValue(piece);
                        if (piece.Color == color)
                            score += value;
                        else
                            score -= value;
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Evaluates piece positioning using piece-square tables
        /// </summary>
        private double EvaluatePosition(IChessBoard board, PieceColor color)
        {
            double score = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.GetPiece(new Location(row, col));
                    if (piece != null)
                    {
                        int positionValue = GetPositionValue(piece, row, col);
                        if (piece.Color == color)
                            score += positionValue;
                        else
                            score -= positionValue;
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the material value of a piece
        /// </summary>
        private int GetPieceValue(Piece piece)
        {
            string typeName = piece.GetType().Name;
            return PieceValues.ContainsKey(typeName) ? PieceValues[typeName] : 0;
        }

        /// <summary>
        /// Gets the positional value of a piece based on piece-square tables
        /// </summary>
        private int GetPositionValue(Piece piece, int row, int col)
        {
            string typeName = piece.GetType().Name;
            int[,] table = null;

            switch (typeName)
            {
                case "Pawn":
                    table = PawnTable;
                    break;
                case "Knight":
                    table = KnightTable;
                    break;
                case "Bishop":
                    table = BishopTable;
                    break;
                case "Rook":
                    table = RookTable;
                    break;
                case "Queen":
                    table = QueenTable;
                    break;
                case "King":
                    table = KingMiddlegameTable;
                    break;
                default:
                    return 0;
            }

            // Flip table for black pieces
            if (piece.Color == PieceColor.Black)
            {
                row = 7 - row;
            }

            return table[row, col];
        }

        /// <summary>
        /// Gets piece value by type name (static method for external use)
        /// </summary>
        public static int GetPieceValueByType(string typeName)
        {
            return PieceValues.ContainsKey(typeName) ? PieceValues[typeName] : 0;
        }
    }
}
