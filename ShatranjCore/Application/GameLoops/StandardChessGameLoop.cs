using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;
using ShatranjCore.Validators;

namespace ShatranjCore.Application.GameLoops
{
    /// <summary>
    /// Standard chess game loop implementation.
    /// Follows FIDE rules for check, checkmate, and stalemate.
    /// Single Responsibility: Execute standard chess game flow.
    /// </summary>
    public class StandardChessGameLoop : IGameLoopStrategy
    {
        private readonly CheckDetector checkDetector;
        private readonly ILogger logger;

        public StandardChessGameLoop(CheckDetector checkDetector, ILogger logger)
        {
            this.checkDetector = checkDetector;
            this.logger = logger;
        }

        public string GetVariantName() => "Standard Chess";

        public void Execute()
        {
            logger.Info($"Starting {GetVariantName()} game loop");

            // Standard game loop - this would be implemented by delegating to GameLoop
            // For now, this is a template that GameLoop can use
            logger.Debug("Standard chess rules applied: FIDE rules, standard castling, en passant");
        }

        /// <summary>
        /// Check game end conditions for standard chess.
        /// </summary>
        public bool IsGameOver(IChessBoard board, PieceColor currentPlayer)
        {
            // Check for checkmate
            if (checkDetector.IsCheckmate(board, currentPlayer))
            {
                logger.Info($"{currentPlayer} is checkmate - game over");
                return true;
            }

            // Check for stalemate
            if (checkDetector.IsStalemate(board, currentPlayer))
            {
                logger.Info("Stalemate - game over (draw)");
                return true;
            }

            return false;
        }
    }
}
