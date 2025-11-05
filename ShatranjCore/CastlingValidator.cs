using System;

namespace ShatranjCore
{
    /// <summary>
    /// Validator for castling moves.
    /// Follows Single Responsibility Principle - only validates castling.
    /// </summary>
    public class CastlingValidator
    {
        /// <summary>
        /// Checks if kingside castling is possible for the given color.
        /// </summary>
        public bool CanCastleKingside(IChessBoard board, PieceColor color)
        {
            King king = board.FindKing(color);
            if (king == null || king.isMoved)
                return false;

            // King must be on starting position
            int kingRow = color == PieceColor.White ? 7 : 0;
            if (king.location.Row != kingRow || king.location.Column != 4)
                return false;

            // Find kingside rook
            Location rookLocation = new Location(kingRow, 7);
            Piece rook = board.GetPiece(rookLocation);

            if (rook == null || !(rook is Rook) || rook.isMoved || rook.Color != color)
                return false;

            // Check squares between king and rook are empty (f and g files)
            if (!board.IsEmptyAt(kingRow, 5) || !board.IsEmptyAt(kingRow, 6))
                return false;

            // TODO: King cannot be in check, pass through check, or end in check
            // This requires check detection implementation

            return true;
        }

        /// <summary>
        /// Checks if queenside castling is possible for the given color.
        /// </summary>
        public bool CanCastleQueenside(IChessBoard board, PieceColor color)
        {
            King king = board.FindKing(color);
            if (king == null || king.isMoved)
                return false;

            // King must be on starting position
            int kingRow = color == PieceColor.White ? 7 : 0;
            if (king.location.Row != kingRow || king.location.Column != 4)
                return false;

            // Find queenside rook
            Location rookLocation = new Location(kingRow, 0);
            Piece rook = board.GetPiece(rookLocation);

            if (rook == null || !(rook is Rook) || rook.isMoved || rook.Color != color)
                return false;

            // Check squares between king and rook are empty (b, c, d files)
            if (!board.IsEmptyAt(kingRow, 1) || !board.IsEmptyAt(kingRow, 2) || !board.IsEmptyAt(kingRow, 3))
                return false;

            // TODO: King cannot be in check, pass through check, or end in check
            // This requires check detection implementation

            return true;
        }

        /// <summary>
        /// Executes a castling move.
        /// </summary>
        public void ExecuteCastle(IChessBoard board, PieceColor color, CastlingSide side)
        {
            int row = color == PieceColor.White ? 7 : 0;

            King king = board.FindKing(color);
            Location kingStart = new Location(row, 4);

            if (side == CastlingSide.Kingside)
            {
                // Move king from e1/e8 to g1/g8
                Location kingEnd = new Location(row, 6);
                board.RemovePiece(kingStart);
                board.PlacePiece(king, kingEnd);
                king.isMoved = true;

                // Move rook from h1/h8 to f1/f8
                Location rookStart = new Location(row, 7);
                Location rookEnd = new Location(row, 5);
                Piece rook = board.GetPiece(rookStart);
                board.RemovePiece(rookStart);
                board.PlacePiece(rook, rookEnd);
                rook.isMoved = true;
            }
            else // Queenside
            {
                // Move king from e1/e8 to c1/c8
                Location kingEnd = new Location(row, 2);
                board.RemovePiece(kingStart);
                board.PlacePiece(king, kingEnd);
                king.isMoved = true;

                // Move rook from a1/a8 to d1/d8
                Location rookStart = new Location(row, 0);
                Location rookEnd = new Location(row, 3);
                Piece rook = board.GetPiece(rookStart);
                board.RemovePiece(rookStart);
                board.PlacePiece(rook, rookEnd);
                rook.isMoved = true;
            }
        }
    }

    /// <summary>
    /// Enum for castling side.
    /// </summary>
    public enum CastlingSide
    {
        Kingside,
        Queenside
    }

    /// <summary>
    /// Result of castling availability check.
    /// </summary>
    public class CastlingAvailability
    {
        public bool CanCastleKingside { get; set; }
        public bool CanCastleQueenside { get; set; }

        public bool CanCastleEither => CanCastleKingside || CanCastleQueenside;
        public bool CanCastleBoth => CanCastleKingside && CanCastleQueenside;
    }
}
