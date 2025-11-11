using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;

namespace ShatranjCore.Application
{
    /// <summary>
    /// Top-level orchestrator that coordinates all game components
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class GameOrchestrator : IGameOrchestrator
    {
        private readonly IGameLoop _gameLoop;
        private readonly IChessBoard _board;
        private readonly ILogger _logger;

        public GameOrchestrator(
            IGameLoop gameLoop,
            IChessBoard board,
            ILogger logger)
        {
            _gameLoop = gameLoop;
            _board = board;
            _logger = logger;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public void Start()
        {
            _logger.Info("Game orchestrator starting game");
            _gameLoop.Run();
            _logger.Info("Game orchestrator - game ended");
        }
    }
}
