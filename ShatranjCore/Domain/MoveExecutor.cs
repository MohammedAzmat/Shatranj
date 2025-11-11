using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Board;
using ShatranjCore.Handlers;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Domain
{
    /// <summary>
    /// Executes chess moves including special moves (en passant, promotion)
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class MoveExecutor : IMoveExecutor
    {
        private readonly IChessBoard _board;
        private readonly IRenderer _renderer;
        private readonly IEnPassantTracker _enPassantTracker;
        private readonly ICheckDetector _checkDetector;
        private readonly IMoveHistory _moveHistory;
        private readonly PawnPromotionHandler _promotionHandler;
        private readonly ILogger _logger;
        private readonly List<Piece> _capturedPieces;

        private PieceColor _currentPlayer;

        public MoveExecutor(
            IChessBoard board,
            IRenderer renderer,
            IEnPassantTracker enPassantTracker,
            ICheckDetector checkDetector,
            IMoveHistory moveHistory,
            PawnPromotionHandler promotionHandler,
            ILogger logger)
        {
            _board = board;
            _renderer = renderer;
            _enPassantTracker = enPassantTracker;
            _checkDetector = checkDetector;
            _moveHistory = moveHistory;
            _promotionHandler = promotionHandler;
            _logger = logger;
            _capturedPieces = new List<Piece>();
        }

        public void SetCurrentPlayer(PieceColor color)
        {
            _currentPlayer = color;
        }

        public List<object> GetCapturedPieces()
        {
            return new List<object>(_capturedPieces);
        }

        /// <summary>
        /// Executes a move on the board
        /// </summary>
        public void ExecuteMove(Location from, Location to)
        {
            Piece piece = _board.GetPiece(from);
            if (piece == null)
            {
                _logger.Warning($"ExecuteMove called with no piece at {from}");
                return;
            }

            Piece capturedPiece = _board.GetPiece(to);
            bool wasCapture = capturedPiece != null;
            bool wasEnPassant = false;

            // Check for en passant capture
            if (piece is Pawn && capturedPiece == null)
            {
                Location? enPassantCaptureLocation = _enPassantTracker.GetEnPassantCaptureLocation();
                if (enPassantCaptureLocation.HasValue)
                {
                    Location? enPassantTarget = _enPassantTracker.GetEnPassantTarget();
                    if (enPassantTarget.HasValue && to.Row == enPassantTarget.Value.Row && to.Column == enPassantTarget.Value.Column)
                    {
                        capturedPiece = _board.GetPiece(enPassantCaptureLocation.Value);
                        if (capturedPiece != null)
                        {
                            _board.RemovePiece(enPassantCaptureLocation.Value);
                            wasCapture = true;
                            wasEnPassant = true;
                            _capturedPieces.Add(capturedPiece);
                            _renderer.DisplayInfo($"Pawn captures {capturedPiece.GetType().Name} en passant!");
                        }
                    }
                }
            }

            if (wasCapture && !wasEnPassant)
            {
                _capturedPieces.Add(capturedPiece);
                _renderer.DisplayInfo($"{piece.GetType().Name} captures {capturedPiece.GetType().Name}!");
            }

            // Move the piece
            _board.RemovePiece(from);
            _board.PlacePiece(piece, to);
            piece.isMoved = true;

            // Track pawn double moves for en passant
            if (piece is Pawn)
            {
                int rowDiff = Math.Abs(to.Row - from.Row);
                if (rowDiff == 2)
                {
                    _enPassantTracker.RecordPawnDoubleMove(from, to);
                }
            }

            // Check for pawn promotion
            if (_promotionHandler.NeedsPromotion(piece, to))
            {
                Type promotionPiece = _promotionHandler.PromptForPromotion(_currentPlayer);

                if (promotionPiece == null)
                {
                    // User pressed ESC - cancel the move
                    _renderer.DisplayInfo("Move cancelled.");
                    _board.RemovePiece(to);
                    _board.PlacePiece(piece, from);
                    piece.isMoved = false;

                    if (wasCapture)
                    {
                        _board.PlacePiece(capturedPiece, to);
                        _capturedPieces.Remove(capturedPiece);
                    }

                    return;
                }

                // Create promoted piece
                Piece promotedPiece = _promotionHandler.CreatePromotionPiece(promotionPiece, to, _currentPlayer);
                _board.RemovePiece(to);
                _board.PlacePiece(promotedPiece, to);
                promotedPiece.isMoved = true;

                _renderer.DisplayInfo($"Pawn promoted to {promotionPiece.Name}!");
                piece = promotedPiece;
            }

            // Check if opponent is now in check/checkmate
            PieceColor opponent = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            bool causedCheck = _checkDetector.IsKingInCheck(_board, opponent);
            bool causedCheckmate = causedCheck && _checkDetector.IsCheckmate(_board, opponent);

            // Record the move
            Move move = new Move(
                piece,
                new Square(from.Row, from.Column, piece),
                new Square(to.Row, to.Column, capturedPiece),
                capturedPiece
            );

            _moveHistory.AddMove(move, _currentPlayer, wasCapture, causedCheck, causedCheckmate);
        }
    }
}
