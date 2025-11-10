using System;
using ShatranjCore.Abstractions;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;

namespace ShatranjCore.Validators
{
    /// <summary>
    /// Detects check, checkmate, and stalemate conditions.
    /// Follows Single Responsibility Principle - only handles check detection.
    /// </summary>
    public class CheckDetector
    {
        /// <summary>
        /// Determines if the specified king is currently in check.
        /// </summary>
        public bool IsKingInCheck(IChessBoard board, PieceColor kingColor)
        {
            King king = board.FindKing(kingColor);
            if (king == null)
                return false;

            return IsSquareUnderAttack(board, king.location, kingColor);
        }

        /// <summary>
        /// Determines if a square is under attack by the opponent.
        /// </summary>
        public bool IsSquareUnderAttack(IChessBoard board, Location square, PieceColor defendingColor)
        {
            PieceColor attackingColor = defendingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Get all opponent pieces
            List<Piece> opponentPieces = board.GetOpponentPieces(defendingColor);

            // Check if any opponent piece can attack this square
            foreach (Piece opponentPiece in opponentPieces)
            {
                if (opponentPiece == null)
                    continue;

                // Get all possible moves for this opponent piece
                List<Move> moves = opponentPiece.GetMoves(opponentPiece.location, board);

                // Check if any move targets our square
                foreach (Move move in moves)
                {
                    if (move.To.Location.Row == square.Row && move.To.Location.Column == square.Column)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a move would leave the moving player's king in check.
        /// </summary>
        public bool WouldMoveCauseCheck(IChessBoard board, Location from, Location to, PieceColor movingColor)
        {
            // Simulate the move
            Piece movingPiece = board.GetPiece(from);
            Piece capturedPiece = board.GetPiece(to);

            if (movingPiece == null)
                return false;

            // Temporarily make the move
            board.RemovePiece(from);
            board.PlacePiece(movingPiece, to);

            // Check if king is in check after this move
            bool kingInCheck = IsKingInCheck(board, movingColor);

            // Undo the move
            board.RemovePiece(to);
            board.PlacePiece(movingPiece, from);
            if (capturedPiece != null)
            {
                board.PlacePiece(capturedPiece, to);
            }

            return kingInCheck;
        }

        /// <summary>
        /// Determines if the specified color is in checkmate.
        /// </summary>
        public bool IsCheckmate(IChessBoard board, PieceColor color)
        {
            // Must be in check first
            if (!IsKingInCheck(board, color))
                return false;

            // Check if any legal move can get out of check
            return !HasAnyLegalMoves(board, color);
        }

        /// <summary>
        /// Determines if the specified color is in stalemate.
        /// </summary>
        public bool IsStalemate(IChessBoard board, PieceColor color)
        {
            // Must NOT be in check
            if (IsKingInCheck(board, color))
                return false;

            // But have no legal moves
            return !HasAnyLegalMoves(board, color);
        }

        /// <summary>
        /// Checks if the specified color has any legal moves.
        /// </summary>
        private bool HasAnyLegalMoves(IChessBoard board, PieceColor color)
        {
            List<Piece> pieces = board.GetPiecesOfColor(color);

            foreach (Piece piece in pieces)
            {
                if (piece == null)
                    continue;

                List<Move> moves = piece.GetMoves(piece.location, board);

                foreach (Move move in moves)
                {
                    // Check if this move would leave king in check
                    if (!WouldMoveCauseCheck(board, piece.location, move.To.Location, color))
                    {
                        return true; // Found at least one legal move
                    }
                }
            }

            return false; // No legal moves found
        }

        /// <summary>
        /// Gets all legal moves for a piece (moves that don't leave king in check).
        /// </summary>
        public List<Move> GetLegalMoves(IChessBoard board, Location pieceLocation, PieceColor color, Location? enPassantTarget = null)
        {
            Piece piece = board.GetPiece(pieceLocation);
            if (piece == null || piece.Color != color)
                return new List<Move>();

            List<Move> allMoves;

            // For pawns, include en passant moves if available
            if (piece is Pawn pawn && enPassantTarget.HasValue)
            {
                allMoves = pawn.GetMovesWithEnPassant(pieceLocation, board, enPassantTarget);
            }
            else
            {
                allMoves = piece.GetMoves(pieceLocation, board);
            }

            List<Move> legalMoves = new List<Move>();

            foreach (Move move in allMoves)
            {
                if (!WouldMoveCauseCheck(board, pieceLocation, move.To.Location, color))
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        /// <summary>
        /// Validates if a move is legal (doesn't leave king in check).
        /// </summary>
        public bool IsMoveLegal(IChessBoard board, Location from, Location to, PieceColor color)
        {
            Piece piece = board.GetPiece(from);
            if (piece == null || piece.Color != color)
                return false;

            // Check if the piece can make this move
            if (!piece.CanMove(from, to, board))
                return false;

            // Check if this would leave king in check
            return !WouldMoveCauseCheck(board, from, to, color);
        }
    }
}
