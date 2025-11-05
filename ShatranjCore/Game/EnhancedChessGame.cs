using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Board;
using ShatranjCore.Handlers;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Game
{
    /// <summary>
    /// Enhanced Chess Game with improved terminal UI and command system.
    /// Follows SOLID principles with dependency injection and separation of concerns.
    /// </summary>
    public class EnhancedChessGame
    {
        private readonly IChessBoard board;
        private readonly ConsoleBoardRenderer renderer;
        private readonly CommandParser commandParser;
        private readonly MoveHistory moveHistory;
        private readonly CastlingValidator castlingValidator;
        private readonly PawnPromotionHandler promotionHandler;
        private readonly CheckDetector checkDetector;
        private readonly EnPassantTracker enPassantTracker;
        private readonly List<Piece> capturedPieces;

        private Player[] players;
        private PieceColor currentPlayer;
        private GameResult gameResult;
        private bool isRunning;

        public EnhancedChessGame()
        {
            board = new ChessBoard(PieceColor.White);
            renderer = new ConsoleBoardRenderer();
            commandParser = new CommandParser();
            moveHistory = new MoveHistory();
            castlingValidator = new CastlingValidator();
            promotionHandler = new PawnPromotionHandler();
            checkDetector = new CheckDetector();
            enPassantTracker = new EnPassantTracker();
            capturedPieces = new List<Piece>();
            gameResult = GameResult.InProgress;
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void Start()
        {
            InitializeGame();
            GameLoop();
        }

        /// <summary>
        /// Initializes a new game.
        /// </summary>
        private void InitializeGame()
        {
            currentPlayer = PieceColor.White;
            isRunning = true;
            gameResult = GameResult.InProgress;
            capturedPieces.Clear();
            moveHistory.Clear();
            enPassantTracker.Reset();

            // Initialize players
            players = new Player[2];
            players[0] = new Player(PieceColor.White, PlayerType.Human) { HasTurn = true };
            players[1] = new Player(PieceColor.Black, PlayerType.Human) { HasTurn = false };

            renderer.DisplayInfo("New game started! White moves first.");
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        private void GameLoop()
        {
            while (isRunning)
            {
                // Check for checkmate or stalemate
                if (checkDetector.IsCheckmate(board, currentPlayer))
                {
                    PieceColor winner = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    renderer.RenderBoard(board, null, null);
                    renderer.DisplayGameOver(GameResult.Checkmate, winner);
                    gameResult = GameResult.Checkmate;
                    isRunning = false;
                    WaitForKey();
                    break;
                }

                if (checkDetector.IsStalemate(board, currentPlayer))
                {
                    renderer.RenderBoard(board, null, null);
                    renderer.DisplayGameOver(GameResult.Stalemate);
                    gameResult = GameResult.Stalemate;
                    isRunning = false;
                    WaitForKey();
                    break;
                }

                // Render the board
                var lastMove = moveHistory.GetLastMove();
                Location? lastFrom = lastMove != null ? (Location?)lastMove.Move.From.Location : null;
                Location? lastTo = lastMove != null ? (Location?)lastMove.Move.To.Location : null;

                renderer.RenderBoard(board, lastFrom, lastTo);

                // Display game status
                bool isCheck = checkDetector.IsKingInCheck(board, currentPlayer);
                var status = new GameStatus
                {
                    CurrentPlayer = currentPlayer,
                    IsCheck = isCheck,
                    LastMove = lastMove?.AlgebraicNotation,
                    CapturedPieces = capturedPieces
                };
                renderer.DisplayGameStatus(status);

                // Get and process command
                Console.Write($"{currentPlayer} > ");
                string input = Console.ReadLine();

                ProcessCommand(input);
            }
        }

        /// <summary>
        /// Processes a user command.
        /// </summary>
        private void ProcessCommand(string input)
        {
            var command = commandParser.Parse(input);

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
                    renderer.DisplayCommands();
                    WaitForKey();
                    break;

                case CommandType.ShowHistory:
                    moveHistory.DisplayHistory();
                    WaitForKey();
                    break;

                case CommandType.StartGame:
                    renderer.DisplayInfo("Starting new game...");
                    InitializeGame();
                    break;

                case CommandType.RestartGame:
                    renderer.DisplayInfo("Restarting game...");
                    InitializeGame();
                    break;

                case CommandType.EndGame:
                    renderer.DisplayInfo("Ending game...");
                    isRunning = false;
                    break;

                case CommandType.SaveGame:
                    renderer.DisplayInfo("Game saving not yet implemented.");
                    WaitForKey();
                    break;

                case CommandType.Quit:
                    renderer.DisplayInfo("Thanks for playing Shatranj!");
                    isRunning = false;
                    break;

                case CommandType.Invalid:
                    renderer.DisplayError(command.ErrorMessage);
                    WaitForKey();
                    break;
            }
        }

        /// <summary>
        /// Handles a move command.
        /// </summary>
        private void HandleMoveCommand(GameCommand command)
        {
            try
            {
                Piece piece = board.GetPiece(command.From);

                // Validate piece exists
                if (piece == null)
                {
                    renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                    WaitForKey();
                    return;
                }

                // Validate piece belongs to current player
                if (piece.Color != currentPlayer)
                {
                    renderer.DisplayError($"That piece belongs to {piece.Color}, not {currentPlayer}!");
                    WaitForKey();
                    return;
                }

                // Check if move is valid for this piece type
                if (!piece.CanMove(command.From, command.To, board))
                {
                    renderer.DisplayError($"Illegal move for {piece.GetType().Name}");
                    WaitForKey();
                    return;
                }

                // Check if move would leave king in check
                if (checkDetector.WouldMoveCauseCheck(board, command.From, command.To, currentPlayer))
                {
                    renderer.DisplayError("That move would leave your King in check!");
                    WaitForKey();
                    return;
                }

                // Execute the move
                ExecuteMove(command.From, command.To, piece);

                // Switch turns
                SwitchTurns();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing move: {ex.Message}");
                WaitForKey();
            }
        }

        /// <summary>
        /// Executes a move on the board.
        /// </summary>
        private void ExecuteMove(Location from, Location to, Piece piece)
        {
            Piece capturedPiece = board.GetPiece(to);
            bool wasCapture = capturedPiece != null;
            bool wasEnPassant = false;

            // Check for en passant capture
            if (piece is Pawn && capturedPiece == null)
            {
                Location? enPassantCaptureLocation = enPassantTracker.GetEnPassantCaptureLocation();
                if (enPassantCaptureLocation.HasValue)
                {
                    // Check if this move is to the en passant target square
                    Location? enPassantTarget = enPassantTracker.GetEnPassantTarget();
                    if (enPassantTarget.HasValue && to.Row == enPassantTarget.Value.Row && to.Column == enPassantTarget.Value.Column)
                    {
                        // This is an en passant capture - remove the pawn from the side square
                        capturedPiece = board.GetPiece(enPassantCaptureLocation.Value);
                        if (capturedPiece != null)
                        {
                            board.RemovePiece(enPassantCaptureLocation.Value);
                            wasCapture = true;
                            wasEnPassant = true;
                            capturedPieces.Add(capturedPiece);
                            renderer.DisplayInfo($"Pawn captures {capturedPiece.GetType().Name} en passant!");
                        }
                    }
                }
            }

            if (wasCapture && !wasEnPassant)
            {
                capturedPieces.Add(capturedPiece);
                renderer.DisplayInfo($"{piece.GetType().Name} captures {capturedPiece.GetType().Name}!");
            }

            // Move the piece
            board.RemovePiece(from);
            board.PlacePiece(piece, to);
            piece.isMoved = true;

            // Track pawn double moves for en passant
            if (piece is Pawn)
            {
                int rowDiff = Math.Abs(to.Row - from.Row);
                if (rowDiff == 2)
                {
                    enPassantTracker.RecordPawnDoubleMove(from, to);
                }
            }

            // Check for pawn promotion
            if (promotionHandler.NeedsPromotion(piece, to))
            {
                Type promotionPiece = promotionHandler.PromptForPromotion(currentPlayer);

                if (promotionPiece == null)
                {
                    // User pressed ESC - cancel the move
                    renderer.DisplayInfo("Move cancelled.");
                    board.RemovePiece(to);
                    board.PlacePiece(piece, from);
                    piece.isMoved = false;

                    if (wasCapture)
                    {
                        board.PlacePiece(capturedPiece, to);
                        capturedPieces.Remove(capturedPiece);
                    }

                    WaitForKey();
                    return;
                }

                // Create promoted piece
                Piece promotedPiece = promotionHandler.CreatePromotionPiece(promotionPiece, to, currentPlayer);
                board.RemovePiece(to);
                board.PlacePiece(promotedPiece, to);
                promotedPiece.isMoved = true;

                renderer.DisplayInfo($"Pawn promoted to {promotionPiece.Name}!");
                piece = promotedPiece; // Update piece reference for move history
            }

            // Check if opponent is now in check/checkmate
            PieceColor opponent = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            bool causedCheck = checkDetector.IsKingInCheck(board, opponent);
            bool causedCheckmate = causedCheck && checkDetector.IsCheckmate(board, opponent);

            // Record the move
            Move move = new Move(
                piece,
                new Square(from.Row, from.Column, piece),
                new Square(to.Row, to.Column, capturedPiece),
                capturedPiece
            );

            moveHistory.AddMove(move, currentPlayer, wasCapture, causedCheck, causedCheckmate);
        }

        /// <summary>
        /// Handles a castle command.
        /// </summary>
        private void HandleCastleCommand(GameCommand command)
        {
            try
            {
                // Check if user specified a side
                CastlingSide? side = command.CastleSide;

                // Check castling availability
                bool canKingside = castlingValidator.CanCastleKingside(board, currentPlayer);
                bool canQueenside = castlingValidator.CanCastleQueenside(board, currentPlayer);

                if (!canKingside && !canQueenside)
                {
                    renderer.DisplayError("Castling is not available.");
                    WaitForKey();
                    return;
                }

                // If no side specified, or both options available, prompt user
                if (side == null)
                {
                    if (!canKingside && canQueenside)
                    {
                        side = CastlingSide.Queenside;
                    }
                    else if (canKingside && !canQueenside)
                    {
                        side = CastlingSide.Kingside;
                    }
                    else
                    {
                        // Both available - prompt user
                        side = PromptForCastlingSide();
                        if (side == null)
                        {
                            renderer.DisplayInfo("Castling cancelled.");
                            WaitForKey();
                            return;
                        }
                    }
                }

                // Validate the chosen side is available
                if (side == CastlingSide.Kingside && !canKingside)
                {
                    renderer.DisplayError("Kingside castling is not available.");
                    WaitForKey();
                    return;
                }

                if (side == CastlingSide.Queenside && !canQueenside)
                {
                    renderer.DisplayError("Queenside castling is not available.");
                    WaitForKey();
                    return;
                }

                // Execute castling
                castlingValidator.ExecuteCastle(board, currentPlayer, side.Value);

                string castleType = side == CastlingSide.Kingside ? "kingside" : "queenside";
                renderer.DisplayInfo($"{currentPlayer} castles {castleType}!");

                // Record the move in history
                string notation = side == CastlingSide.Kingside ? "O-O" : "O-O-O";
                // TODO: Add proper castling move to move history

                // Switch turns
                SwitchTurns();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing castle: {ex.Message}");
                WaitForKey();
            }
        }

        /// <summary>
        /// Prompts user to choose castling side.
        /// </summary>
        private CastlingSide? PromptForCastlingSide()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Both castling options are available!");
            Console.ResetColor();
            Console.WriteLine("Choose a side:");
            Console.WriteLine("  king/k      - Castles kingside  (O-O)");
            Console.WriteLine("  queen/q     - Castle queenside (O-O-O)");
            Console.WriteLine();
            Console.WriteLine("Press ESC to cancel");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice: ");
                var keyInfo = Console.ReadKey(intercept: false);
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return null;
                }

                string input = keyInfo.KeyChar.ToString().ToLower();

                // Read rest of line if they're typing a full word
                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    string rest = Console.ReadLine();
                    input += rest.ToLower().Trim();
                }

                // Parse the input
                var tempCommand = commandParser.Parse($"castle {input.Trim()}");
                if (tempCommand.Type == CommandType.Castle && tempCommand.CastleSide.HasValue)
                {
                    return tempCommand.CastleSide.Value;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please enter: king/k or queen/q");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles showing available moves for a piece.
        /// </summary>
        private void HandleShowMovesCommand(GameCommand command)
        {
            Piece piece = board.GetPiece(command.From);

            if (piece == null)
            {
                renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                WaitForKey();
                return;
            }

            if (piece.Color != currentPlayer)
            {
                renderer.DisplayError($"That piece belongs to {piece.Color}, not {currentPlayer}!");
                WaitForKey();
                return;
            }

            // Get only legal moves (those that don't leave king in check)
            // Include en passant target if available
            Location? enPassantTarget = enPassantTracker.GetEnPassantTarget();
            List<Move> legalMoves = checkDetector.GetLegalMoves(board, command.From, currentPlayer, enPassantTarget);
            renderer.DisplayPossibleMoves(command.From, legalMoves);
            WaitForKey();
        }

        /// <summary>
        /// Switches to the next player's turn.
        /// </summary>
        private void SwitchTurns()
        {
            currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            players[0].HasTurn = !players[0].HasTurn;
            players[1].HasTurn = !players[1].HasTurn;
            enPassantTracker.NextTurn();
        }

        /// <summary>
        /// Waits for user to press a key.
        /// </summary>
        private void WaitForKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Converts a Location to algebraic notation.
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
