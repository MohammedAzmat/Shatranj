using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;
using GameCommand = ShatranjCore.UI.GameCommand;
using CastlingSide = ShatranjCore.Validators.CastlingSide;

namespace ShatranjCore.Application
{
    /// <summary>
    /// Processes game commands and routes them to appropriate handlers
    /// Extracted from ChessGame for Single Responsibility Principle
    /// </summary>
    public class CommandProcessor : ICommandProcessor
    {
        private readonly CommandParser _commandParser;
        private readonly IRenderer _renderer;
        private readonly IChessBoard _board;
        private readonly ICheckDetector _checkDetector;
        private readonly CastlingValidator _castlingValidator;
        private readonly ILogger _logger;
        private readonly IMoveHistory _moveHistory;

        private PieceColor _currentPlayer;
        private bool _isRunning;

        // Delegates for actions that require access to ChessGame internals
        private Action<Location, Location, Piece> _executeMoveDelegate;
        private Action _switchTurnsDelegate;
        private Action _saveGameDelegate;
        private Action<int> _loadGameDelegate;
        private Action _rollbackDelegate;
        private Action _redoDelegate;
        private Action _showSettingsDelegate;
        private Action _waitForKeyDelegate;
        private Func<GameCommand, CastlingSide?> _promptForCastlingSideDelegate;

        public CommandProcessor(
            CommandParser commandParser,
            IRenderer renderer,
            IChessBoard board,
            ICheckDetector checkDetector,
            CastlingValidator castlingValidator,
            ILogger logger,
            IMoveHistory moveHistory)
        {
            _commandParser = commandParser;
            _renderer = renderer;
            _board = board;
            _checkDetector = checkDetector;
            _castlingValidator = castlingValidator;
            _logger = logger;
            _moveHistory = moveHistory;
            _isRunning = true;
        }

        public void SetCurrentPlayer(PieceColor color)
        {
            _currentPlayer = color;
        }

        public void SetDelegates(
            Action<Location, Location, Piece> executeMove,
            Action switchTurns,
            Action saveGame,
            Action<int> loadGame,
            Action rollback,
            Action redo,
            Action showSettings,
            Action waitForKey,
            Func<GameCommand, CastlingSide?> promptForCastlingSide)
        {
            _executeMoveDelegate = executeMove;
            _switchTurnsDelegate = switchTurns;
            _saveGameDelegate = saveGame;
            _loadGameDelegate = loadGame;
            _rollbackDelegate = rollback;
            _redoDelegate = redo;
            _showSettingsDelegate = showSettings;
            _waitForKeyDelegate = waitForKey;
            _promptForCastlingSideDelegate = promptForCastlingSide;
        }

        public void ProcessCommand(string input)
        {
            var command = _commandParser.Parse(input);

            _logger.Debug($"Processing command: {command.Type}");

            switch (command.Type)
            {
                case CommandType.Move:
                    HandleMoveCommand(command);
                    break;

                case CommandType.Castle:
                    HandleCastleCommand(command);
                    break;

                case CommandType.ShowMoves:
                    HandleShowMovesCommand(command);
                    break;

                case CommandType.ShowHelp:
                    _logger.Debug("Displaying help menu");
                    _renderer.DisplayCommands();
                    _waitForKeyDelegate?.Invoke();
                    break;

                case CommandType.ShowHistory:
                    _logger.Debug("Displaying move history");
                    _moveHistory.DisplayHistory();
                    _waitForKeyDelegate?.Invoke();
                    break;

                case CommandType.SaveGame:
                    _logger.Info("Save game command received");
                    _saveGameDelegate?.Invoke();
                    break;

                case CommandType.LoadGame:
                    if (int.TryParse(command.FileName, out int gameId))
                    {
                        _loadGameDelegate?.Invoke(gameId);
                    }
                    break;

                case CommandType.Rollback:
                    _rollbackDelegate?.Invoke();
                    break;

                case CommandType.Redo:
                    _redoDelegate?.Invoke();
                    break;

                case CommandType.ShowSettings:
                    _showSettingsDelegate?.Invoke();
                    break;

                case CommandType.Quit:
                    _logger.Info($"{_currentPlayer} player quit the game");
                    _renderer.DisplayInfo("Thanks for playing Shatranj!");
                    _isRunning = false;
                    break;

                case CommandType.Invalid:
                    _renderer.DisplayError(command.ErrorMessage);
                    _waitForKeyDelegate?.Invoke();
                    break;
            }
        }

        private void HandleMoveCommand(GameCommand command)
        {
            try
            {
                _logger.Debug($"Handling move command: {LocationToAlgebraic(command.From)} -> {LocationToAlgebraic(command.To)}");

                Piece piece = _board.GetPiece(command.From);

                if (piece == null)
                {
                    _renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                    _waitForKeyDelegate?.Invoke();
                    return;
                }

                if (piece.Color != _currentPlayer)
                {
                    _renderer.DisplayError($"That piece belongs to {piece.Color}, not {_currentPlayer}!");
                    _waitForKeyDelegate?.Invoke();
                    return;
                }

                if (!piece.CanMove(command.From, command.To, _board))
                {
                    _renderer.DisplayError($"Illegal move for {piece.GetType().Name}");
                    _waitForKeyDelegate?.Invoke();
                    return;
                }

                if (_checkDetector.WouldMoveCauseCheck(_board, command.From, command.To, _currentPlayer))
                {
                    _renderer.DisplayError("That move would leave your King in check!");
                    _waitForKeyDelegate?.Invoke();
                    return;
                }

                _executeMoveDelegate?.Invoke(command.From, command.To, piece);
                _switchTurnsDelegate?.Invoke();

                _logger.Info($"Move executed: {piece.GetType().Name} {LocationToAlgebraic(command.From)} -> {LocationToAlgebraic(command.To)}");
            }
            catch (Exception ex)
            {
                _renderer.DisplayError($"Error processing move: {ex.Message}");
                _logger.Error("Move command failed", ex);
                _waitForKeyDelegate?.Invoke();
            }
        }

        private void HandleCastleCommand(GameCommand command)
        {
            try
            {
                _logger.Debug($"Handling castle command for {_currentPlayer}");

                CastlingSide? side = command.CastleSide;

                bool canKingside = _castlingValidator.CanCastleKingside(_board, _currentPlayer);
                bool canQueenside = _castlingValidator.CanCastleQueenside(_board, _currentPlayer);

                if (!canKingside && !canQueenside)
                {
                    _renderer.DisplayError("Castling is not available.");
                    _waitForKeyDelegate?.Invoke();
                    return;
                }

                if (side == null)
                {
                    side = _promptForCastlingSideDelegate?.Invoke(command);
                    if (side == null)
                    {
                        _renderer.DisplayInfo("Castling cancelled.");
                        _waitForKeyDelegate?.Invoke();
                        return;
                    }
                }

                _castlingValidator.ExecuteCastle(_board, _currentPlayer, side.Value);

                string castleType = side == CastlingSide.Kingside ? "kingside" : "queenside";
                _renderer.DisplayInfo($"{_currentPlayer} castles {castleType}!");
                _logger.Info($"{_currentPlayer} castled {castleType}");

                _switchTurnsDelegate?.Invoke();
            }
            catch (Exception ex)
            {
                _renderer.DisplayError($"Error processing castle: {ex.Message}");
                _logger.Error("Castle command failed", ex);
                _waitForKeyDelegate?.Invoke();
            }
        }

        private void HandleShowMovesCommand(GameCommand command)
        {
            // TODO: Implement show moves functionality
            _renderer.DisplayInfo("Show moves not yet implemented in CommandProcessor");
            _waitForKeyDelegate?.Invoke();
        }

        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
