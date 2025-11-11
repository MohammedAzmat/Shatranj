using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Board;
using ShatranjCore.Pieces;

namespace ShatranjCore.Movement
{
    /// <summary>
    /// Tracks the history of moves in a chess game.
    /// Follows Single Responsibility Principle - only manages move history.
    /// NOTE: MoveHistory has a local MoveRecord class with extended properties.
    /// For UI rendering, use ConsoleMoveHistoryRenderer with the DisplayHistory method.
    /// </summary>
    public class MoveHistory
    {
        private List<MoveRecord> moves = new List<MoveRecord>();

        /// <summary>
        /// Adds a move to the history.
        /// </summary>
        public void AddMove(Move move, PieceColor player, bool wasCapture, bool wasCheck = false, bool wasCheckmate = false)
        {
            var record = new MoveRecord
            {
                Move = move,
                Player = player,
                MoveNumber = (moves.Count / 2) + 1,
                WasCapture = wasCapture,
                WasCheck = wasCheck,
                WasCheckmate = wasCheckmate,
                Timestamp = DateTime.Now,
                AlgebraicNotation = ConvertToAlgebraic(move, wasCapture, wasCheck, wasCheckmate)
            };

            moves.Add(record);
        }

        /// <summary>
        /// Gets the last move played.
        /// </summary>
        public MoveRecord GetLastMove()
        {
            return moves.Count > 0 ? moves[moves.Count - 1] : null;
        }

        /// <summary>
        /// Gets all moves in the game.
        /// </summary>
        public List<MoveRecord> GetAllMoves()
        {
            return new List<MoveRecord>(moves);
        }

        /// <summary>
        /// Gets all moves in the game (alias for GetAllMoves).
        /// </summary>
        public List<MoveRecord> GetMoves()
        {
            return GetAllMoves();
        }

        /// <summary>
        /// Gets the total number of moves.
        /// </summary>
        public int MoveCount => moves.Count;

        /// <summary>
        /// Clears the move history (for new game).
        /// </summary>
        public void Clear()
        {
            moves.Clear();
        }

        /// <summary>
        /// Converts a move to algebraic notation.
        /// </summary>
        private string ConvertToAlgebraic(Move move, bool wasCapture, bool wasCheck, bool wasCheckmate)
        {
            string from = LocationToAlgebraic(move.From.Location);
            string to = LocationToAlgebraic(move.To.Location);

            string pieceSymbol = GetPieceNotation(move.Piece);
            string captureSymbol = wasCapture ? "x" : "-";
            string checkSymbol = wasCheckmate ? "#" : (wasCheck ? "+" : "");

            return $"{pieceSymbol}{from}{captureSymbol}{to}{checkSymbol}";
        }

        /// <summary>
        /// Gets the notation for a piece type.
        /// </summary>
        private string GetPieceNotation(Piece piece)
        {
            if (piece is Pawn) return "";
            if (piece is Knight) return "N";
            if (piece is Bishop) return "B";
            if (piece is Rook) return "R";
            if (piece is Queen) return "Q";
            if (piece is King) return "K";
            return "";
        }

        /// <summary>
        /// Converts a Location to algebraic notation.
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }

        /// <summary>
        /// Displays the move history to console.
        /// DEPRECATED: Use IMoveHistoryRenderer.DisplayHistory instead.
        /// </summary>
        [Obsolete("Use IMoveHistoryRenderer.DisplayHistory instead", false)]
        public void DisplayHistory()
        {
            Console.WriteLine("Move History:");
            Console.WriteLine("─────────────────────────────────────────");

            int moveNum = 1;
            for (int i = 0; i < moves.Count; i += 2)
            {
                string whiteMove = moves[i].AlgebraicNotation;
                string blackMove = i + 1 < moves.Count ? moves[i + 1].AlgebraicNotation : "";

                Console.WriteLine($"{moveNum,3}. {whiteMove,-15} {blackMove}");
                moveNum++;
            }

            Console.WriteLine("─────────────────────────────────────────");
        }
    }

    /// <summary>
    /// Represents a single move record in the game history.
    /// </summary>
    public class MoveRecord
    {
        public Move Move { get; set; }
        public PieceColor Player { get; set; }
        public int MoveNumber { get; set; }
        public bool WasCapture { get; set; }
        public bool WasCheck { get; set; }
        public bool WasCheckmate { get; set; }
        public DateTime Timestamp { get; set; }
        public string AlgebraicNotation { get; set; }
    }
}
