using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Application
{
    /// <summary>
    /// Handles AI move selection and execution
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class AIHandler : IAIHandler
    {
        private readonly ConsoleBoardRenderer _renderer;
        private readonly EnPassantTracker _enPassantTracker;
        private readonly CheckDetector _checkDetector;
        private readonly GameRecorder _recorder;
        private readonly ILogger _logger;

        public AIHandler(
            ConsoleBoardRenderer renderer,
            EnPassantTracker enPassantTracker,
            CheckDetector checkDetector,
            GameRecorder recorder,
            ILogger logger)
        {
            _renderer = renderer;
            _enPassantTracker = enPassantTracker;
            _checkDetector = checkDetector;
            _recorder = recorder;
            _logger = logger;
        }

        /// <summary>
        /// Handles AI turn - selects move and returns it for execution
        /// </summary>
        public void HandleAITurn(IChessAI ai, PieceColor color)
        {
            // This method signature needs to be updated to return move info
            // For now, just logging
            _logger.Info($"AI turn for {color}");
        }

        /// <summary>
        /// Selects AI move
        /// </summary>
        public AIMove SelectAIMove(IChessAI ai, IChessBoard board, PieceColor currentPlayer)
        {
            try
            {
                _renderer.DisplayInfo($"{currentPlayer} (AI) is thinking...");

                Location? enPassantTarget = _enPassantTracker.GetEnPassantTarget();
                AIMove aiMove = ai.SelectMove(board, currentPlayer, enPassantTarget);

                if (aiMove == null)
                {
                    _renderer.DisplayError("AI failed to select a move!");
                    _logger.Error($"AI failed to select move for {currentPlayer}");
                    return null;
                }

                string fromAlg = LocationToAlgebraic(aiMove.From);
                string toAlg = LocationToAlgebraic(aiMove.To);
                _renderer.DisplayInfo($"{currentPlayer} moves: {fromAlg} -> {toAlg} (Eval: {aiMove.Evaluation:F2})");
                _logger.Info($"AI move: {fromAlg} -> {toAlg}, Eval: {aiMove.Evaluation:F2}, Nodes: {aiMove.NodesEvaluated}, Time: {aiMove.ThinkingTimeMs}ms");

                return aiMove;
            }
            catch (Exception ex)
            {
                _logger.Error($"AI move selection failed for {currentPlayer}", ex);
                _renderer.DisplayError($"AI error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts location to algebraic notation
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
