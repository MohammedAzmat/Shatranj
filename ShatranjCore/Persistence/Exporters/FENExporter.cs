using System;
using System.Text;
using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjCore.Persistence.Exporters
{
    /// <summary>
    /// Exports board positions to FEN (Forsyth-Edwards Notation) format.
    /// FEN is a standard notation for describing a board position.
    ///
    /// Format: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
    ///
    /// Components:
    /// 1. Piece placement (white/black pieces)
    /// 2. Active color (w/b)
    /// 3. Castling availability (KQkq)
    /// 4. En passant target square (- or square)
    /// 5. Halfmove clock (50-move rule)
    /// 6. Fullmove number
    ///
    /// Single Responsibility: Only exports board state to FEN string.
    /// </summary>
    public class FENExporter : IFENExporter
    {
        public string Export(IChessBoard board)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            var fen = new StringBuilder();

            // 1. Piece placement
            fen.Append(ExportPiecePlacement(board));
            fen.Append(" ");

            // Note: The following FEN components require game state that isn't in IChessBoard
            // In a full implementation, these would come from GameState:

            // 2. Active color (would need current turn from game state)
            fen.Append("w");  // Default to white for now
            fen.Append(" ");

            // 3. Castling availability (would need king/rook moved flags)
            fen.Append("KQkq");  // Default to all castling available for now
            fen.Append(" ");

            // 4. En passant target square
            fen.Append("-");  // No en passant target
            fen.Append(" ");

            // 5. Halfmove clock (50-move rule)
            fen.Append("0");
            fen.Append(" ");

            // 6. Fullmove number
            fen.Append("1");

            return fen.ToString();
        }

        /// <summary>
        /// Exports the piece placement (first component of FEN).
        /// Encodes pieces row by row from rank 8 to rank 1.
        /// </summary>
        private string ExportPiecePlacement(IChessBoard board)
        {
            var placement = new StringBuilder();

            for (int row = 0; row < 8; row++)
            {
                int emptyCount = 0;

                for (int col = 0; col < 8; col++)
                {
                    var piece = board.GetPiece(new Location(row, col));

                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        // Flush empty square count
                        if (emptyCount > 0)
                        {
                            placement.Append(emptyCount);
                            emptyCount = 0;
                        }

                        // Add piece symbol
                        placement.Append(GetPieceFENSymbol(piece));
                    }
                }

                // Flush empty square count at end of row
                if (emptyCount > 0)
                {
                    placement.Append(emptyCount);
                }

                // Add row separator (except after last row)
                if (row < 7)
                {
                    placement.Append("/");
                }
            }

            return placement.ToString();
        }

        /// <summary>
        /// Gets the FEN symbol for a piece (uppercase for white, lowercase for black).
        /// </summary>
        private char GetPieceFENSymbol(Piece piece)
        {
            char symbol = piece switch
            {
                Pawn => 'P',
                Knight => 'N',
                Bishop => 'B',
                Rook => 'R',
                Queen => 'Q',
                King => 'K',
                _ => '?'
            };

            // Lowercase for black pieces
            if (piece.Color == PieceColor.Black)
            {
                symbol = char.ToLower(symbol);
            }

            return symbol;
        }
    }
}
