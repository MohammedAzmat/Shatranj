using System;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Commands;
using ShatranjCore.Interfaces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Application.CommandHandlers
{
    /// <summary>
    /// Handles Castle commands (e.g., "castle king").
    /// Single Responsibility: Only handles castling move validation and execution.
    /// </summary>
    public class CastleCommandHandler : ICommandHandler
    {
        private readonly ConsoleBoardRenderer renderer;
        private readonly IChessBoard board;
        private readonly CastlingValidator castlingValidator;
        private readonly ILogger logger;

        // Delegates for actions that require board state
        private Action switchTurnsDelegate;
        private Action waitForKeyDelegate;
        private Func<GameCommand, CastlingSide?> promptForCastlingSideDelegate;

        private PieceColor currentPlayer;

        public CastleCommandHandler(
            ConsoleBoardRenderer renderer,
            IChessBoard board,
            CastlingValidator castlingValidator,
            ILogger logger)
        {
            this.renderer = renderer;
            this.board = board;
            this.castlingValidator = castlingValidator;
            this.logger = logger;
        }

        /// <summary>
        /// Set delegates for game state manipulation.
        /// </summary>
        public void SetDelegates(
            Action switchTurns,
            Action waitForKey,
            Func<GameCommand, CastlingSide?> promptForCastlingSide,
            PieceColor currentPlayer)
        {
            this.switchTurnsDelegate = switchTurns;
            this.waitForKeyDelegate = waitForKey;
            this.promptForCastlingSideDelegate = promptForCastlingSide;
            this.currentPlayer = currentPlayer;
        }

        public bool CanHandle(GameCommand command)
        {
            return command.Type == CommandType.Castle;
        }

        public void Handle(GameCommand command)
        {
            if (!CanHandle(command))
                throw new InvalidOperationException($"CastleCommandHandler cannot handle {command.Type}");

            try
            {
                logger.Debug($"Handling castle command for {currentPlayer}");

                CastlingSide? side = command.CastleSide;

                bool canKingside = castlingValidator.CanCastleKingside(board, currentPlayer);
                bool canQueenside = castlingValidator.CanCastleQueenside(board, currentPlayer);

                // Validation: At least one castling side is available
                if (!canKingside && !canQueenside)
                {
                    renderer.DisplayError("Castling is not available.");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // If side not specified, prompt user
                if (side == null)
                {
                    side = promptForCastlingSideDelegate?.Invoke(command);
                    if (side == null)
                    {
                        renderer.DisplayInfo("Castling cancelled.");
                        waitForKeyDelegate?.Invoke();
                        return;
                    }
                }

                // Validation: Requested side is available
                if (side == CastlingSide.Kingside && !canKingside)
                {
                    renderer.DisplayError("Kingside castling is not available.");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                if (side == CastlingSide.Queenside && !canQueenside)
                {
                    renderer.DisplayError("Queenside castling is not available.");
                    waitForKeyDelegate?.Invoke();
                    return;
                }

                // Execute castle
                castlingValidator.ExecuteCastle(board, currentPlayer, side.Value);

                string castleType = side == CastlingSide.Kingside ? "kingside" : "queenside";
                renderer.DisplayInfo($"{currentPlayer} castles {castleType}!");
                logger.Info($"{currentPlayer} castled {castleType}");

                switchTurnsDelegate?.Invoke();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing castle: {ex.Message}");
                logger.Error("Castle command failed", ex);
                waitForKeyDelegate?.Invoke();
            }
        }
    }
}
