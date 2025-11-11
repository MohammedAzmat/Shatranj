using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Game;

namespace ShatranjCore.Domain
{
    /// <summary>
    /// Manages player turns and turn state
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class TurnManager : ITurnManager
    {
        private readonly IEnPassantTracker _enPassantTracker;
        private readonly IGameStateManager _stateManager;
        private readonly ILogger _logger;

        private PieceColor _currentPlayer;
        private Player[] _players;

        public PieceColor CurrentPlayer => _currentPlayer;

        public TurnManager(
            IEnPassantTracker enPassantTracker,
            IGameStateManager stateManager,
            ILogger logger)
        {
            _enPassantTracker = enPassantTracker;
            _stateManager = stateManager;
            _logger = logger;
            _currentPlayer = PieceColor.White;
        }

        /// <summary>
        /// Initialize players array
        /// </summary>
        public void SetPlayers(Player[] players)
        {
            _players = players;
        }

        /// <summary>
        /// Set current player
        /// </summary>
        public void SetCurrentPlayer(PieceColor color)
        {
            _currentPlayer = color;
        }

        /// <summary>
        /// Switches to the next player's turn
        /// </summary>
        public void SwitchTurns()
        {
            _currentPlayer = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

            if (_players != null && _players.Length == 2)
            {
                _players[0].HasTurn = !_players[0].HasTurn;
                _players[1].HasTurn = !_players[1].HasTurn;
            }

            _enPassantTracker.NextTurn();

            // Clear redo stack on new move (can't redo after making a new move)
            _stateManager.ClearRedoStack();

            _logger.Debug($"Turn switched to {_currentPlayer}");
        }
    }
}
