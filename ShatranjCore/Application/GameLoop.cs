using System;
using System.Threading;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;
using ShatranjCore.Movement;
using ShatranjCore.State;
using ShatranjCore.UI;
using ShatranjCore.Validators;
using GameStatus = ShatranjCore.UI.GameStatus;
using MoveRecord = ShatranjCore.Movement.MoveRecord;

namespace ShatranjCore.Application
{
    /// <summary>
    /// Main game loop - orchestrates game flow
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class GameLoop : IGameLoop
    {
        private readonly IChessBoard _board;
        private readonly ConsoleBoardRenderer _renderer;
        private readonly CheckDetector _checkDetector;
        private readonly MoveHistory _moveHistory;
        private readonly GameRecorder _recorder;
        private readonly ILogger _logger;
        private readonly CommandParser _commandParser;
        private readonly GameStateManager _stateManager;

        private bool _isRunning;
        private PieceColor _currentPlayer;
        private GameMode _gameMode;
        private GameResult _gameResult;
        private IChessAI _whiteAI;
        private IChessAI _blackAI;

        // Delegates for command processing and AI handling
        private Action<string> _processCommandDelegate;
        private Action<IChessAI> _handleAIMoveDelegate;
        private Action _cleanupGameFilesDelegate;

        public GameLoop(
            IChessBoard board,
            ConsoleBoardRenderer renderer,
            CheckDetector checkDetector,
            MoveHistory moveHistory,
            GameRecorder recorder,
            ILogger logger,
            CommandParser commandParser,
            GameStateManager stateManager)
        {
            _board = board;
            _renderer = renderer;
            _checkDetector = checkDetector;
            _moveHistory = moveHistory;
            _recorder = recorder;
            _logger = logger;
            _commandParser = commandParser;
            _stateManager = stateManager;
            _isRunning = false;
            _currentPlayer = PieceColor.White;
            _gameResult = GameResult.InProgress;
        }

        public void SetGameMode(GameMode mode)
        {
            _gameMode = mode;
        }

        public void SetCurrentPlayer(PieceColor color)
        {
            _currentPlayer = color;
        }

        public void SetAIs(IChessAI whiteAI, IChessAI blackAI)
        {
            _whiteAI = whiteAI;
            _blackAI = blackAI;
        }

        public void SetDelegates(
            Action<string> processCommand,
            Action<IChessAI> handleAIMove,
            Action cleanupGameFiles)
        {
            _processCommandDelegate = processCommand;
            _handleAIMoveDelegate = handleAIMove;
            _cleanupGameFilesDelegate = cleanupGameFiles;
        }

        public void Run()
        {
            _isRunning = true;

            while (_isRunning)
            {
                // Check for checkmate
                if (_checkDetector.IsCheckmate(_board, _currentPlayer))
                {
                    PieceColor winner = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    _renderer.RenderBoard(_board, null, null);
                    _renderer.DisplayGameOver(GameResult.Checkmate, winner);
                    _gameResult = GameResult.Checkmate;
                    _isRunning = false;

                    _recorder.EndGame(winner.ToString(), "Checkmate");
                    _logger.Info($"Game ended - Winner: {winner} by Checkmate");

                    _cleanupGameFilesDelegate?.Invoke();
                    WaitForKey();
                    break;
                }

                // Check for stalemate
                if (_checkDetector.IsStalemate(_board, _currentPlayer))
                {
                    _renderer.RenderBoard(_board, null, null);
                    _renderer.DisplayGameOver(GameResult.Stalemate);
                    _gameResult = GameResult.Stalemate;
                    _isRunning = false;

                    _recorder.EndGame("Draw", "Stalemate");
                    _logger.Info("Game ended - Draw by Stalemate");

                    _cleanupGameFilesDelegate?.Invoke();
                    WaitForKey();
                    break;
                }

                // Render board
                var lastMove = _moveHistory.GetLastMove();
                Location? lastFrom = lastMove != null ? (Location?)lastMove.Move.From.Location : null;
                Location? lastTo = lastMove != null ? (Location?)lastMove.Move.To.Location : null;

                _renderer.RenderBoard(_board, lastFrom, lastTo);

                // Display status
                bool isCheck = _checkDetector.IsKingInCheck(_board, _currentPlayer);
                var status = new GameStatus
                {
                    CurrentPlayer = _currentPlayer,
                    IsCheck = isCheck
                };
                _renderer.DisplayGameStatus(status);

                // Handle turn
                IChessAI currentAI = _currentPlayer == PieceColor.White ? _whiteAI : _blackAI;

                if (currentAI != null)
                {
                    _handleAIMoveDelegate?.Invoke(currentAI);

                    if (_gameMode == GameMode.AIVsAI)
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.Write($"{_currentPlayer} > ");
                    string input = Console.ReadLine();
                    _processCommandDelegate?.Invoke(input);
                }
            }

            // Record incomplete games
            if (_gameResult == GameResult.InProgress)
            {
                _recorder.EndGame("None", "Incomplete");
                _logger.Info("Game ended - Incomplete");
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private void WaitForKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
