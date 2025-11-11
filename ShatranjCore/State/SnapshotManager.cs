using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;
using ShatranjCore.Persistence;
using ShatranjCore.Pieces;

namespace ShatranjCore.State
{
    /// <summary>
    /// Manages creation and restoration of game state snapshots
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class SnapshotManager : ISnapshotManager
    {
        private readonly ILogger _logger;

        public SnapshotManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates a snapshot of the current game state
        /// </summary>
        public GameStateSnapshot CreateSnapshot(IBoardState board, GameContext context)
        {
            var snapshot = new GameStateSnapshot
            {
                GameId = context.GameId,
                GameMode = context.GameMode.ToString(),
                CurrentPlayer = context.CurrentPlayer.ToString(),
                HumanColor = context.HumanColor.ToString(),
                GameResult = context.GameResult.ToString(),
                WhitePlayerName = context.WhitePlayerName,
                BlackPlayerName = context.BlackPlayerName,
                Difficulty = context.Difficulty.ToString(),
                MoveCount = 0 // Will be set by caller
            };

            // Save all pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = board.GetPiece(new Location(row, col));
                    if (piece != null)
                    {
                        snapshot.Pieces.Add(new PieceData
                        {
                            Type = piece.GetType().Name,
                            Color = piece.Color.ToString(),
                            Row = row,
                            Column = col,
                            HasMoved = piece.isMoved
                        });
                    }
                }
            }

            return snapshot;
        }

        /// <summary>
        /// Restores game state from a snapshot
        /// </summary>
        public void RestoreSnapshot(GameStateSnapshot snapshot, IBoardState board, out GameContext context)
        {

            try
            {
                _logger.Info("Starting game state restoration...");

                // Clear the board
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        Location loc = new Location(row, col);
                        if (board.GetPiece(loc) != null)
                        {
                            board.RemovePiece(loc);
                        }
                    }
                }

                // Restore pieces
                foreach (var pieceData in snapshot.Pieces)
                {
                    PieceColor color = (PieceColor)Enum.Parse(typeof(PieceColor), pieceData.Color);
                    Piece piece = PieceFactory.CreatePiece(pieceData.Type, color, pieceData.Row, pieceData.Column);
                    piece.isMoved = pieceData.HasMoved;
                    board.PlacePiece(piece, new Location(pieceData.Row, pieceData.Column));
                }

                // Restore game context
                context = new GameContext
                {
                    GameId = snapshot.GameId,
                    GameMode = (GameMode)Enum.Parse(typeof(GameMode), snapshot.GameMode),
                    CurrentPlayer = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.CurrentPlayer),
                    HumanColor = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.HumanColor),
                    GameResult = (GameResult)Enum.Parse(typeof(GameResult), snapshot.GameResult),
                    Difficulty = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), snapshot.Difficulty),
                    WhitePlayerName = snapshot.WhitePlayerName,
                    BlackPlayerName = snapshot.BlackPlayerName
                };

                _logger.Info("Game state restored successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to restore game state", ex);
                throw;
            }
        }
    }
}
