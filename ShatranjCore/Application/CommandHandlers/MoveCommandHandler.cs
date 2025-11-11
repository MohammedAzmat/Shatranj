using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.Interfaces;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Move commands (e.g., "move e2 e4").
    /// Single Responsibility: Only moves pieces on the board.
    /// </summary>
    public class MoveCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly IChessBoard board;
        private readonly CheckDetector checkDetector;
        private readonly ILogger logger;

        // Delegates for actions that require board state
        private Action<Location, Location, Piece> executeMoveDelegate;
        private Action switchTurnsDelegate;
        private Action waitForKeyDelegate;

        private PieceColor currentPlayer;

        public MoveCommandHandler(
            ConsoleBoardRenderer renderer,
            IChessBoard board,
            CheckDetector checkDetector,
            ILogger logger)
        {
            this.renderer = renderer;
            this.board = board;
            this.checkDetector = checkDetector;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for game state manipulation.
        /// </summary>
        public void SetDelegates(
            Action<Location, Location, Piece> executeMove,
            Action switchTurns,
            Action waitForKey,
            PieceColor currentPlayer)
        {
            this.executeMoveDelegate = executeMove;
            this.switchTurnsDelegate = switchTurns;
            this.waitForKeyDelegate = waitForKey;
            this.currentPlayer = currentPlayer;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.Move;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"MoveCommandHandler cannot handle {command.Type}");

            try
            {
                logger.Debug($"Handling move command: {LocationToAlgebraic(command.From)} -> {LocationToAlgebraic(command.To)}");

                // Validation: Piece exists
                Piece piece = board.GetPiece(command.From);
                if (piece == null)
                {
                    renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // Validation: Piece belongs to current player
                if (piece.Color != currentPlayer)
                {
                    renderer.DisplayError($"That piece belongs to {piece.Color}, not {currentPlayer}!");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // Validation: Piece can move to destination
                if (!piece.CanMove(command.From, command.To, board))
                {
                    renderer.DisplayError($"Illegal move for {piece.GetType().Name}");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // Validation: Move doesn't put own king in check
                if (checkDetector.WouldMoveCauseCheck(board, command.From, command.To, currentPlayer))
                {
                    renderer.DisplayError("That move would leave your King in check!");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // Execute move and switch turns
                executeMoveDelegate?.Invoke(command.From, command.To, piece);
                switchTurnsDelegate?.Invoke();

                logger.Info($"Move executed: {piece.GetType().Name} {LocationToAlgebraic(command.From)} -> {LocationToAlgebraic(command.To)}");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing move: {ex.Message}");
                logger.Error("Move command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }

        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
