using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;

namespace ShatranjCore.Domain
{
    /// <summary>
    /// Executes castling moves on the board.
    /// Responsibility: Update board state when a castling move is made.
    /// </summary>
    public class CastlingExecutor : ICastlingExecutor
    {
        /// <summary>
        /// Executes a castling move.
        /// </summary>
        /// <param name="board">The chess board to execute the move on</param>
        /// <param name="color">The color of the player performing the castling</param>
        /// <param name="side">The side to castle (kingside or queenside)</param>
        public void ExecuteCastle(IBoardState board, PieceColor color, CastlingSide side)
        {
            int row = color == PieceColor.White ? 7 : 0;

            // Cast to concrete type to access pieces
            if (!(board is IChessBoard chessBoard))
                throw new ArgumentException("Board must implement IChessBoard interface", nameof(board));

            King king = chessBoard.FindKing(color);
            Location kingStart = new Location(row, 4);

            if (side == CastlingSide.Kingside)
            {
                // Move king from e1/e8 to g1/g8
                Location kingEnd = new Location(row, 6);
                chessBoard.RemovePiece(kingStart);
                chessBoard.PlacePiece(king, kingEnd);
                king.isMoved = true;

                // Move rook from h1/h8 to f1/f8
                Location rookStart = new Location(row, 7);
                Location rookEnd = new Location(row, 5);
                Piece rook = chessBoard.GetPiece(rookStart);
                chessBoard.RemovePiece(rookStart);
                chessBoard.PlacePiece(rook, rookEnd);
                rook.isMoved = true;
            }
            else // Queenside
            {
                // Move king from e1/e8 to c1/c8
                Location kingEnd = new Location(row, 2);
                chessBoard.RemovePiece(kingStart);
                chessBoard.PlacePiece(king, kingEnd);
                king.isMoved = true;

                // Move rook from a1/a8 to d1/d8
                Location rookStart = new Location(row, 0);
                Location rookEnd = new Location(row, 3);
                Piece rook = chessBoard.GetPiece(rookStart);
                chessBoard.RemovePiece(rookStart);
                chessBoard.PlacePiece(rook, rookEnd);
                rook.isMoved = true;
            }
        }
    }
}
