using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Handlers;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;
using ShatranjCore.Logging;
using ShatranjCore.Movement;
using ShatranjCore.Persistence;
using ShatranjCore.Pieces;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Game
{
    /// <summary>
    /// Chess Game with terminal UI and command system.
    /// Follows SOLID principles with dependency injection and separation of concerns.
    /// </summary>
    public class ChessGame
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
        private GameMode gameMode;
        private PieceColor humanColor;

        // AI and infrastructure
        private IChessAI whiteAI;
        private IChessAI blackAI;
        private readonly ILogger logger;
        private readonly GameRecorder recorder;
        private readonly GameSerializer serializer;

        // Game configuration and save management
        private readonly GameConfigManager configManager;
        private readonly SaveGameManager saveManager;
        private int currentGameId;
        private DifficultyLevel difficulty;
        private string whitePlayerName;
        private string blackPlayerName;

        // State history for rollback/redo functionality
        private readonly List<GameStateSnapshot> stateHistory;
        private readonly Stack<GameStateSnapshot> redoStack;

        public ChessGame(
            GameMode mode = GameMode.HumanVsHuman,
            PieceColor humanPlayerColor = PieceColor.White,
            IChessAI whiteAI = null,
            IChessAI blackAI = null)
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
            gameMode = mode;
            humanColor = humanPlayerColor;

            // Initialize logging and infrastructure
            logger = new CompositeLogger(
                new FileLogger(),
                new ConsoleLogger(includeTimestamp: false)
            );
            recorder = new GameRecorder(logger);
            serializer = new GameSerializer(logger);

            // Initialize configuration and save management
            configManager = new GameConfigManager(logger);
            saveManager = new SaveGameManager(logger);
            stateHistory = new List<GameStateSnapshot>();
            redoStack = new Stack<GameStateSnapshot>();

            // Load configuration
            var config = configManager.GetConfig();
            difficulty = config.Difficulty;
            whitePlayerName = config.ProfileName;
            blackPlayerName = config.OpponentProfileName;
            currentGameId = configManager.GetNextGameId();

            // Set AI instances from constructor parameters
            this.whiteAI = whiteAI;
            this.blackAI = blackAI;

            // Log game mode
            if (mode == GameMode.HumanVsAI)
            {
                if (humanPlayerColor == PieceColor.White)
                {
                    logger.Info("Game mode: Human (White) vs AI (Black)");
                }
                else
                {
                    logger.Info("Game mode: AI (White) vs Human (Black)");
                }
            }
            else if (mode == GameMode.AIVsAI)
            {
                logger.Info("Game mode: AI vs AI");
            }
            else
            {
                logger.Info("Game mode: Human vs Human");
            }
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

            // Initialize players based on game mode
            players = new Player[2];

            string whitePlayerType = "Human";
            string blackPlayerType = "Human";

            switch (gameMode)
            {
                case GameMode.HumanVsHuman:
                    players[0] = new Player(PieceColor.White, PlayerType.Human) { HasTurn = true };
                    players[1] = new Player(PieceColor.Black, PlayerType.Human) { HasTurn = false };
                    renderer.DisplayInfo("New game started! White moves first.");
                    break;

                case GameMode.HumanVsAI:
                    // Set up human and AI based on chosen color
                    if (humanColor == PieceColor.White)
                    {
                        players[0] = new Player(PieceColor.White, PlayerType.Human) { HasTurn = true };
                        players[1] = new Player(PieceColor.Black, PlayerType.AI) { HasTurn = false };
                        blackPlayerType = "AI:BasicAI";
                        renderer.DisplayInfo("New game started! You are White. You move first.");
                    }
                    else
                    {
                        players[0] = new Player(PieceColor.White, PlayerType.AI) { HasTurn = true };
                        players[1] = new Player(PieceColor.Black, PlayerType.Human) { HasTurn = false };
                        whitePlayerType = "AI:BasicAI";
                        renderer.DisplayInfo("New game started! You are Black. AI moves first.");
                    }
                    break;

                case GameMode.AIVsAI:
                    players[0] = new Player(PieceColor.White, PlayerType.AI) { HasTurn = true };
                    players[1] = new Player(PieceColor.Black, PlayerType.AI) { HasTurn = false };
                    whitePlayerType = "AI:BasicAI";
                    blackPlayerType = "AI:BasicAI";
                    renderer.DisplayInfo("New game started! AI vs AI. White AI moves first.");
                    break;
            }

            // Start recording the game
            recorder.StartNewGame(gameMode, whitePlayerType, blackPlayerType);
            if (whiteAI != null || blackAI != null)
            {
                recorder.SetAIMetadata("BasicAI_v1.0", 3);
            }

            logger.Info($"New game initialized - Mode: {gameMode}");
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

                    // Record game end
                    recorder.EndGame(winner.ToString(), "Checkmate");
                    logger.Info($"Game ended - Winner: {winner} by Checkmate");

                    // Cleanup autosave
                    CleanupGameFiles();

                    WaitForKey();
                    break;
                }

                if (checkDetector.IsStalemate(board, currentPlayer))
                {
                    renderer.RenderBoard(board, null, null);
                    renderer.DisplayGameOver(GameResult.Stalemate);
                    gameResult = GameResult.Stalemate;
                    isRunning = false;

                    // Record game end
                    recorder.EndGame("Draw", "Stalemate");
                    logger.Info("Game ended - Draw by Stalemate");

                    // Cleanup autosave
                    CleanupGameFiles();

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

                // Determine if current player is AI
                IChessAI currentAI = currentPlayer == PieceColor.White ? whiteAI : blackAI;

                if (currentAI != null)
                {
                    // AI turn
                    HandleAIMove(currentAI);

                    // Add delay for AI vs AI mode for visibility
                    if (gameMode == GameMode.AIVsAI)
                    {
                        Thread.Sleep(1000); // 1 second delay
                    }
                }
                else
                {
                    // Human turn
                    Console.Write($"{currentPlayer} > ");
                    string input = Console.ReadLine();
                    ProcessCommand(input);
                }
            }

            // Record game end if not already recorded (e.g., user quit)
            if (gameResult == GameResult.InProgress)
            {
                recorder.EndGame("None", "Incomplete");
                logger.Info("Game ended - Incomplete");
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
                    HandleSaveCommand(command);
                    break;

                case CommandType.LoadGame:
                    HandleLoadCommand(command);
                    break;

                case CommandType.Rollback:
                    HandleRollbackCommand();
                    break;

                case CommandType.Redo:
                    HandleRedoCommand();
                    break;

                case CommandType.ShowSettings:
                    HandleShowSettingsCommand();
                    break;

                case CommandType.ResetSettings:
                    HandleResetSettingsCommand();
                    break;

                case CommandType.SetProfile:
                    HandleSetProfileCommand(command);
                    break;

                case CommandType.SetOpponent:
                    HandleSetOpponentCommand(command);
                    break;

                case CommandType.SetDifficulty:
                    HandleSetDifficultyCommand(command);
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

            // Clear redo stack on new move (can't redo after making a new move)
            redoStack.Clear();

            // Autosave after each turn
            try
            {
                GameStateSnapshot snapshot = CreateSnapshot();

                // Add to state history for rollback (keep last 10 states)
                stateHistory.Add(snapshot);
                if (stateHistory.Count > 10)
                {
                    stateHistory.RemoveAt(0);
                }

                // Autosave
                saveManager.SaveAutosave(snapshot);
                logger.Debug($"Autosave completed. Turn {snapshot.MoveCount}");
            }
            catch (Exception ex)
            {
                logger.Warning($"Autosave failed: {ex.Message}");
                // Don't interrupt gameplay for autosave failures
            }
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

        /// <summary>
        /// Handles AI move selection and execution.
        /// </summary>
        private void HandleAIMove(IChessAI ai)
        {
            try
            {
                renderer.DisplayInfo($"{currentPlayer} (AI) is thinking...");

                Location? enPassantTarget = enPassantTracker.GetEnPassantTarget();
                AIMove aiMove = ai.SelectMove(board, currentPlayer, enPassantTarget);

                if (aiMove == null)
                {
                    renderer.DisplayError("AI failed to select a move!");
                    logger.Error($"AI failed to select move for {currentPlayer}");
                    isRunning = false;
                    return;
                }

                string fromAlg = LocationToAlgebraic(aiMove.From);
                string toAlg = LocationToAlgebraic(aiMove.To);
                renderer.DisplayInfo($"{currentPlayer} moves: {fromAlg} -> {toAlg} (Eval: {aiMove.Evaluation:F2})");
                logger.Info($"AI move: {fromAlg} -> {toAlg}, Eval: {aiMove.Evaluation:F2}, Nodes: {aiMove.NodesEvaluated}, Time: {aiMove.ThinkingTimeMs}ms");

                Piece piece = board.GetPiece(aiMove.From);
                if (piece == null)
                {
                    renderer.DisplayError($"No piece at {fromAlg}!");
                    logger.Error($"AI selected invalid move - no piece at {fromAlg}");
                    isRunning = false;
                    return;
                }

                // Get move info before executing
                Piece capturedPiece = board.GetPiece(aiMove.To);
                bool wasCapture = capturedPiece != null;

                // Execute the move
                ExecuteMove(aiMove.From, aiMove.To, piece);

                // Check game state after move
                PieceColor opponent = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                bool causedCheck = checkDetector.IsKingInCheck(board, opponent);
                bool causedCheckmate = causedCheck && checkDetector.IsCheckmate(board, opponent);

                // Record the move for learning
                recorder.RecordMove(
                    currentPlayer,
                    aiMove.From,
                    aiMove.To,
                    piece.GetType().Name,
                    $"{fromAlg}{toAlg}",
                    wasCapture,
                    causedCheck,
                    causedCheckmate,
                    aiMove.Evaluation,
                    aiMove.ThinkingTimeMs
                );

                // Switch turns
                SwitchTurns();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"AI error: {ex.Message}");
                logger.Error("AI move execution failed", ex);
                isRunning = false;
            }
        }

        /// <summary>
        /// Handles save game command.
        /// </summary>
        private void HandleSaveCommand(GameCommand command)
        {
            try
            {
                GameStateSnapshot snapshot = CreateSnapshot();
                string filePath = saveManager.SaveGame(snapshot, currentGameId);
                renderer.DisplayInfo($"Game saved successfully!");
                renderer.DisplayInfo($"Game ID: {currentGameId}");
                renderer.DisplayInfo($"Location: {filePath}");
                logger.Info($"Game {currentGameId} saved to {filePath}");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to save game: {ex.Message}");
                logger.Error("Game save failed", ex);
            }
            WaitForKey();
        }

        /// <summary>
        /// Handles load game command.
        /// </summary>
        private void HandleLoadCommand(GameCommand command)
        {
            try
            {
                string gameIdStr = command.FileName;

                // If no game ID provided, list saved games with metadata
                if (string.IsNullOrEmpty(gameIdStr))
                {
                    var savedGames = saveManager.ListSavedGames();
                    if (savedGames.Count == 0)
                    {
                        renderer.DisplayInfo("No saved games found.");
                        WaitForKey();
                        return;
                    }

                    renderer.DisplayInfo("════════════════════════════════════════════════════════════════");
                    renderer.DisplayInfo("                      SAVED GAMES");
                    renderer.DisplayInfo("════════════════════════════════════════════════════════════════");

                    foreach (var game in savedGames)
                    {
                        // Determine game type label
                        string gameType = game.GameMode == "AIVsAI" ? "Sim" : "Game";
                        string saveType = string.IsNullOrEmpty(game.SaveType) ? "Manual" : game.SaveType;

                        renderer.DisplayInfo($"{gameType} #{game.GameId} ({saveType}):");
                        renderer.DisplayInfo($"  Mode: {game.GameMode}");
                        renderer.DisplayInfo($"  Players: {game.WhitePlayerName} vs {game.BlackPlayerName}");
                        renderer.DisplayInfo($"  Turn {game.TurnCount} - {game.CurrentPlayer}'s move");
                        renderer.DisplayInfo($"  Difficulty: {game.Difficulty}");
                        renderer.DisplayInfo($"  Saved: {game.SavedAt:yyyy-MM-dd HH:mm:ss}");
                        renderer.DisplayInfo("");
                    }

                    renderer.DisplayInfo("Usage: load [gameId]");
                    renderer.DisplayInfo("Example: load 1");
                    WaitForKey();
                    return;
                }

                // Parse game ID
                if (!int.TryParse(gameIdStr, out int gameId))
                {
                    renderer.DisplayError("Invalid game ID. Please enter a number.");
                    WaitForKey();
                    return;
                }

                // Load the game by ID
                GameStateSnapshot snapshot = saveManager.LoadGame(gameId);
                RestoreFromSnapshot(snapshot);
                renderer.DisplayInfo($"Game #{gameId} loaded successfully!");
                logger.Info($"Game {gameId} loaded");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to load game: {ex.Message}");
                logger.Error("Game load failed", ex);
            }
            WaitForKey();
        }

        /// <summary>
        /// Creates a snapshot of the current game state.
        /// </summary>
        private GameStateSnapshot CreateSnapshot()
        {
            var snapshot = new GameStateSnapshot
            {
                GameId = currentGameId,
                GameMode = gameMode.ToString(),
                CurrentPlayer = currentPlayer.ToString(),
                HumanColor = humanColor.ToString(),
                GameResult = gameResult.ToString(),
                WhitePlayerType = players[0].Type.ToString(),
                BlackPlayerType = players[1].Type.ToString(),
                WhitePlayerName = whitePlayerName,
                BlackPlayerName = blackPlayerName,
                Difficulty = difficulty.ToString(),
                MoveCount = moveHistory.GetMoves().Count
            };

            // Save all pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.GetPiece(new Location(row, col));
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

            // Save captured pieces
            foreach (var piece in capturedPieces)
            {
                snapshot.CapturedPieces.Add(new PieceData
                {
                    Type = piece.GetType().Name,
                    Color = piece.Color.ToString(),
                    Row = 0,
                    Column = 0,
                    HasMoved = true
                });
            }

            // Save move history
            foreach (var move in moveHistory.GetMoves())
            {
                snapshot.MoveHistory.Add(move.AlgebraicNotation);
            }

            // Save en passant state
            Location? enPassantTarget = enPassantTracker.GetEnPassantTarget();
            if (enPassantTarget.HasValue)
            {
                snapshot.EnPassantTargetRow = enPassantTarget.Value.Row;
                snapshot.EnPassantTargetColumn = enPassantTarget.Value.Column;
            }

            return snapshot;
        }

        /// <summary>
        /// Restores game state from a snapshot.
        /// </summary>
        private void RestoreFromSnapshot(GameStateSnapshot snapshot)
        {
            try
            {
                logger.Info("Starting game state restoration...");

                // 1. Clear current game state
                capturedPieces.Clear();
                moveHistory.Clear();
                enPassantTracker.Reset();

                // 2. Clear the board
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

                // 3. Restore pieces
                foreach (var pieceData in snapshot.Pieces)
                {
                    PieceColor color = (PieceColor)Enum.Parse(typeof(PieceColor), pieceData.Color);
                    Piece piece = PieceFactory.CreatePiece(pieceData.Type, color, pieceData.Row, pieceData.Column);
                    piece.isMoved = pieceData.HasMoved;
                    board.PlacePiece(piece, new Location(pieceData.Row, pieceData.Column));
                }

                // 4. Restore captured pieces
                foreach (var capturedData in snapshot.CapturedPieces)
                {
                    PieceColor color = (PieceColor)Enum.Parse(typeof(PieceColor), capturedData.Color);
                    Piece piece = PieceFactory.CreatePiece(capturedData.Type, color, 0, 0);
                    capturedPieces.Add(piece);
                }

                // 5. Restore move history
                // Note: Move history is restored as strings only
                // Full move reconstruction would require more complex logic
                logger.Info($"Move history has {snapshot.MoveHistory.Count} moves");

                // 6. Restore en passant state
                if (snapshot.EnPassantTargetRow.HasValue && snapshot.EnPassantTargetColumn.HasValue)
                {
                    int targetRow = snapshot.EnPassantTargetRow.Value;
                    int targetCol = snapshot.EnPassantTargetColumn.Value;

                    if (snapshot.EnPassantPawnRow.HasValue && snapshot.EnPassantPawnColumn.HasValue)
                    {
                        Location pawnLocation = new Location(
                            snapshot.EnPassantPawnRow.Value,
                            snapshot.EnPassantPawnColumn.Value
                        );

                        // Calculate the from/to locations for RecordPawnDoubleMove
                        // If target is row 2, pawn moved from row 1 to row 3 (white)
                        // If target is row 5, pawn moved from row 6 to row 4 (black)
                        Location fromLocation = (targetRow == 2)
                            ? new Location(1, targetCol)
                            : new Location(6, targetCol);

                        enPassantTracker.RecordPawnDoubleMove(fromLocation, pawnLocation);
                    }
                }

                // 7. Restore game state
                currentPlayer = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.CurrentPlayer);
                gameResult = (GameResult)Enum.Parse(typeof(GameResult), snapshot.GameResult);
                gameMode = (GameMode)Enum.Parse(typeof(GameMode), snapshot.GameMode);
                humanColor = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.HumanColor);

                // 8. Restore players
                PlayerType whitePlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), snapshot.WhitePlayerType);
                PlayerType blackPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), snapshot.BlackPlayerType);

                players = new Player[2];
                players[0] = new Player(PieceColor.White, whitePlayerType)
                {
                    HasTurn = (currentPlayer == PieceColor.White)
                };
                players[1] = new Player(PieceColor.Black, blackPlayerType)
                {
                    HasTurn = (currentPlayer == PieceColor.Black)
                };

                // 9. Note about castling rights
                // Castling rights are implicitly restored via the HasMoved flag on Kings and Rooks
                // The castling validator will check these flags when validating castling moves

                logger.Info($"Game state restored successfully. Current player: {currentPlayer}");
                logger.Info($"Pieces on board: {snapshot.Pieces.Count}, Captured: {capturedPieces.Count}");
            }
            catch (Exception ex)
            {
                logger.Error("Failed to restore game state", ex);
                renderer.DisplayError("Failed to restore game state. The save file may be corrupted.");
                throw;
            }
        }

        /// <summary>
        /// Rolls back the game state to the previous turn
        /// </summary>
        private void HandleRollbackCommand()
        {
            try
            {
                if (stateHistory.Count < 2)
                {
                    renderer.DisplayInfo("Cannot rollback - no previous state available");
                    logger.Info("Rollback failed - insufficient state history");
                    WaitForKey();
                    return;
                }

                // Get the current and previous states
                GameStateSnapshot currentState = stateHistory[stateHistory.Count - 1];
                GameStateSnapshot previousState = stateHistory[stateHistory.Count - 2];

                // Push current state to redo stack before rolling back
                redoStack.Push(currentState);

                // Remove current state from history
                stateHistory.RemoveAt(stateHistory.Count - 1);

                // Restore the previous state
                RestoreFromSnapshot(previousState);

                renderer.DisplayInfo("Turn undone. Use 'redo' to restore.");
                logger.Info($"Game rolled back to turn {previousState.MoveCount}");
                WaitForKey();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to undo: {ex.Message}");
                logger.Error("Rollback failed", ex);
                WaitForKey();
            }
        }

        /// <summary>
        /// Redoes the last undone turn
        /// </summary>
        private void HandleRedoCommand()
        {
            try
            {
                if (redoStack.Count == 0)
                {
                    renderer.DisplayInfo("Cannot redo - no undone turns available");
                    logger.Info("Redo failed - redo stack is empty");
                    WaitForKey();
                    return;
                }

                // Pop the state from redo stack
                GameStateSnapshot redoState = redoStack.Pop();

                // Add it back to state history
                stateHistory.Add(redoState);

                // Restore the state
                RestoreFromSnapshot(redoState);

                renderer.DisplayInfo("Turn redone successfully");
                logger.Info($"Game redone to turn {redoState.MoveCount}");
                WaitForKey();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to redo: {ex.Message}");
                logger.Error("Redo failed", ex);
                WaitForKey();
            }
        }

        /// <summary>
        /// Shows the settings menu
        /// </summary>
        private void HandleShowSettingsCommand()
        {
            try
            {
                var config = configManager.GetConfig();

                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo("           GAME SETTINGS");
                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo($"  Profile Name: {config.ProfileName}");
                renderer.DisplayInfo($"  Opponent Name: {config.OpponentProfileName}");
                renderer.DisplayInfo($"  Difficulty: {config.Difficulty} (Depth {(int)config.Difficulty})");
                renderer.DisplayInfo("════════════════════════════════════════");
                renderer.DisplayInfo("");
                renderer.DisplayInfo("Commands:");
                renderer.DisplayInfo("  settings profile [name]     - Set your name");
                renderer.DisplayInfo("  settings opponent [name]    - Set opponent name");
                renderer.DisplayInfo("  settings difficulty [level] - Set AI difficulty");
                renderer.DisplayInfo("    Difficulty options: easy, medium, hard, veryhard, titan");
                renderer.DisplayInfo("    Or use numbers: 1 (Easy) to 5 (Titan)");
                renderer.DisplayInfo("  settings reset              - Reset to defaults");
                renderer.DisplayInfo("");

                logger.Info("Settings menu displayed");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to display settings: {ex.Message}");
                logger.Error("Settings display failed", ex);
            }

            WaitForKey();
        }

        /// <summary>
        /// Resets settings to defaults
        /// </summary>
        private void HandleResetSettingsCommand()
        {
            try
            {
                configManager.ResetToDefaults();
                var config = configManager.GetConfig();

                // Update local values
                difficulty = config.Difficulty;
                whitePlayerName = config.ProfileName;
                blackPlayerName = config.OpponentProfileName;

                renderer.DisplayInfo("Settings reset to defaults:");
                renderer.DisplayInfo($"  Profile: {config.ProfileName}");
                renderer.DisplayInfo($"  Opponent: {config.OpponentProfileName}");
                renderer.DisplayInfo($"  Difficulty: {config.Difficulty}");

                logger.Info("Settings reset to defaults");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to reset settings: {ex.Message}");
                logger.Error("Settings reset failed", ex);
            }

            WaitForKey();
        }

        /// <summary>
        /// Sets the player profile name
        /// </summary>
        private void HandleSetProfileCommand(GameCommand command)
        {
            try
            {
                string newName = command.FileName; // Reusing FileName field
                configManager.SetProfileName(newName);
                whitePlayerName = newName;

                renderer.DisplayInfo($"Profile name set to: {newName}");
                logger.Info($"Profile name changed to: {newName}");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set profile name: {ex.Message}");
                logger.Error("Set profile failed", ex);
            }

            WaitForKey();
        }

        /// <summary>
        /// Sets the opponent profile name
        /// </summary>
        private void HandleSetOpponentCommand(GameCommand command)
        {
            try
            {
                string newName = command.FileName; // Reusing FileName field
                configManager.SetOpponentProfileName(newName);
                blackPlayerName = newName;

                renderer.DisplayInfo($"Opponent name set to: {newName}");
                logger.Info($"Opponent name changed to: {newName}");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set opponent name: {ex.Message}");
                logger.Error("Set opponent failed", ex);
            }

            WaitForKey();
        }

        /// <summary>
        /// Sets the AI difficulty level
        /// </summary>
        private void HandleSetDifficultyCommand(GameCommand command)
        {
            try
            {
                string difficultyStr = command.FileName; // Reusing FileName field
                DifficultyLevel newDifficulty;

                // Try parsing as number first (1-5)
                if (int.TryParse(difficultyStr, out int difficultyNum))
                {
                    // Map 1-5 to difficulty levels
                    switch (difficultyNum)
                    {
                        case 1:
                            newDifficulty = DifficultyLevel.Easy;
                            break;
                        case 2:
                            newDifficulty = DifficultyLevel.Medium;
                            break;
                        case 3:
                            newDifficulty = DifficultyLevel.Hard;
                            break;
                        case 4:
                            newDifficulty = DifficultyLevel.VeryHard;
                            break;
                        case 5:
                            newDifficulty = DifficultyLevel.Titan;
                            break;
                        default:
                            renderer.DisplayError("Difficulty must be between 1-5");
                            WaitForKey();
                            return;
                    }
                }
                else
                {
                    // Try parsing as difficulty name
                    if (!Enum.TryParse<DifficultyLevel>(difficultyStr, true, out newDifficulty))
                    {
                        renderer.DisplayError("Invalid difficulty. Use: easy, medium, hard, veryhard, titan, or 1-5");
                        WaitForKey();
                        return;
                    }
                }

                configManager.SetDifficulty(newDifficulty);
                difficulty = newDifficulty;

                renderer.DisplayInfo($"Difficulty set to: {newDifficulty} (Depth {(int)newDifficulty})");
                logger.Info($"Difficulty changed to: {newDifficulty}");

                // Note: This will take effect for new games or newly created AI instances
                renderer.DisplayInfo("Note: Difficulty change will apply to new games");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Failed to set difficulty: {ex.Message}");
                logger.Error("Set difficulty failed", ex);
            }

            WaitForKey();
        }

        /// <summary>
        /// Cleans up save files when game concludes
        /// </summary>
        private void CleanupGameFiles()
        {
            try
            {
                // Delete autosave since game is over
                if (saveManager.DeleteAutosave())
                {
                    logger.Info("Autosave file deleted - game concluded");
                }

                // Note: We keep the numbered save files for game history
                // Users can manually delete them if needed
            }
            catch (Exception ex)
            {
                logger.Warning($"Failed to cleanup game files: {ex.Message}");
                // Don't interrupt game ending for cleanup failures
            }
        }

        /// <summary>
        /// Provides access to configuration for external management
        /// </summary>
        public GameConfigManager GetConfigManager() => configManager;

        /// <summary>
        /// Provides access to save manager for external operations
        /// </summary>
        public SaveGameManager GetSaveManager() => saveManager;
    }
}
