using System;
using System.Collections.Generic;
using System.Threading;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Application;
using ShatranjCore.Board;
using ShatranjCore.Domain;
using ShatranjCore.Handlers;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;
using ShatranjCore.Logging;
using ShatranjCore.Movement;
using ShatranjCore.Persistence;
using ShatranjCore.Pieces;
using ShatranjCore.Settings;
using ShatranjCore.State;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore.Game
{
    /// <summary>
    /// Refactored Chess Game - now delegates to extracted components
    /// Reduced from 1,279 lines to 484 lines (62% reduction) by following SRP
    /// </summary>
    public class ChessGame
    {
        // Core components - now injected/delegated
        private readonly IChessBoard board;
        private readonly ConsoleBoardRenderer renderer;
        private readonly ILogger logger;
        private readonly GameRecorder recorder;

        // Extracted components
        private readonly TurnManager turnManager;
        private readonly MoveExecutor moveExecutor;
        private readonly AIHandler aiHandler;
        private readonly SnapshotManager snapshotManager;
        private readonly CommandParser commandParser;
        private readonly CommandProcessor commandProcessor;
        private readonly GameLoop gameLoop;
        private readonly GameOrchestrator orchestrator;

        // State management
        private readonly GameStateManager stateManager;
        private readonly SaveGameManager saveManager;
        private readonly SettingsManager settingsManager;
        private readonly GameConfigManager configManager;

        // Validators
        private readonly CheckDetector checkDetector;
        private readonly CastlingValidator castlingValidator;
        private readonly EnPassantTracker enPassantTracker;
        private readonly PawnPromotionHandler promotionHandler;

        // Game state
        private readonly MoveHistory moveHistory;
        private readonly List<Piece> capturedPieces;
        private Player[] players;
        private PieceColor currentPlayer;
        private GameResult gameResult;
        private bool isRunning;
        private GameMode gameMode;
        private PieceColor humanColor;
        private IChessAI whiteAI;
        private IChessAI blackAI;
        private int currentGameId;
        private DifficultyLevel difficulty;
        private string whitePlayerName;
        private string blackPlayerName;

        public ChessGame(
            GameMode mode = GameMode.HumanVsHuman,
            PieceColor humanPlayerColor = PieceColor.White,
            IChessAI whiteAI = null,
            IChessAI blackAI = null)
        {
            // Initialize core dependencies
            board = new ChessBoard(PieceColor.White);
            renderer = new ConsoleBoardRenderer();
            logger = LoggerFactory.CreateDevelopmentLogger();

            // Initialize validators and trackers
            checkDetector = new CheckDetector();
            castlingValidator = new CastlingValidator();
            enPassantTracker = new EnPassantTracker();
            promotionHandler = new PawnPromotionHandler();

            // Initialize state
            moveHistory = new MoveHistory();
            capturedPieces = new List<Piece>();
            gameResult = GameResult.InProgress;
            gameMode = mode;
            humanColor = humanPlayerColor;
            this.whiteAI = whiteAI;
            this.blackAI = blackAI;

            // Initialize infrastructure
            recorder = new GameRecorder(logger);
            configManager = new GameConfigManager(logger);
            saveManager = new SaveGameManager(logger);
            stateManager = new GameStateManager(saveManager, logger);
            settingsManager = new SettingsManager(configManager, renderer, logger);

            // Load configuration
            var config = configManager.GetConfig();
            difficulty = config.Difficulty;
            whitePlayerName = config.ProfileName;
            blackPlayerName = config.OpponentProfileName;
            currentGameId = configManager.GetNextGameId();

            // Create extracted components - DELEGATION!
            turnManager = new TurnManager(enPassantTracker, stateManager, logger);

            moveExecutor = new MoveExecutor(
                board, renderer, enPassantTracker, checkDetector,
                moveHistory, promotionHandler, logger);

            aiHandler = new AIHandler(
                renderer, enPassantTracker, checkDetector, recorder, logger);

            snapshotManager = new SnapshotManager(logger, new PieceFactory());

            commandParser = new CommandParser();

            commandProcessor = new CommandProcessor(
                commandParser, renderer, board, checkDetector,
                castlingValidator, logger, moveHistory);

            gameLoop = new GameLoop(
                board, renderer, checkDetector, moveHistory,
                recorder, logger, commandParser, stateManager);

            orchestrator = new GameOrchestrator(gameLoop, board, logger);

            logger.Info($"ChessGame (refactored) initialized - Mode: {gameMode}");
        }

        /// <summary>
        /// Starts the game - delegates to orchestrator
        /// </summary>
        public void Start()
        {
            InitializeGame();

            // Wire up delegates for components
            SetupComponentDelegates();

            // Configure and run game loop
            gameLoop.SetGameMode(gameMode);
            gameLoop.SetCurrentPlayer(currentPlayer);
            gameLoop.SetAIs(whiteAI, blackAI);

            // Start the orchestrated game
            orchestrator.Start();
        }

        /// <summary>
        /// Initialize game state
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

            // Setup turn manager
            turnManager.SetPlayers(players);
            turnManager.SetCurrentPlayer(currentPlayer);

            // Setup move executor
            moveExecutor.SetCurrentPlayer(currentPlayer);

            // Start recording
            recorder.StartNewGame(gameMode, whitePlayerType, blackPlayerType);
            if (whiteAI != null || blackAI != null)
            {
                recorder.SetAIMetadata("BasicAI_v1.0", 3);
            }

            logger.Info($"New game initialized - Mode: {gameMode}");
        }

        /// <summary>
        /// Wire up delegates so components can communicate
        /// </summary>
        private void SetupComponentDelegates()
        {
            // Wire CommandProcessor delegates
            commandProcessor.SetDelegates(
                executeMove: ExecuteMove,
                switchTurns: SwitchTurns,
                saveGame: HandleSaveGame,
                loadGame: HandleLoadGame,
                rollback: HandleRollback,
                redo: HandleRedo,
                showSettings: HandleShowSettings,
                waitForKey: WaitForKey,
                promptForCastlingSide: PromptForCastlingSide
            );

            // Wire GameLoop delegates
            gameLoop.SetDelegates(
                processCommand: ProcessCommand,
                handleAIMove: HandleAIMove,
                cleanupGameFiles: CleanupGameFiles
            );
        }

        /// <summary>
        /// Execute move - delegates to MoveExecutor
        /// </summary>
        private void ExecuteMove(Location from, Location to, Piece piece)
        {
            moveExecutor.ExecuteMove(from, to);
        }

        /// <summary>
        /// Switch turns - delegates to TurnManager
        /// </summary>
        private void SwitchTurns()
        {
            turnManager.SwitchTurns();
            currentPlayer = turnManager.CurrentPlayer;
            moveExecutor.SetCurrentPlayer(currentPlayer);
            commandProcessor.SetCurrentPlayer(currentPlayer);
        }

        /// <summary>
        /// Process command - delegates to CommandProcessor
        /// </summary>
        private void ProcessCommand(string input)
        {
            commandProcessor.ProcessCommand(input);
        }

        /// <summary>
        /// Handle AI move - delegates to AIHandler
        /// </summary>
        private void HandleAIMove(IChessAI ai)
        {
            try
            {
                AIMove aiMove = aiHandler.SelectAIMove(ai, board, currentPlayer);
                if (aiMove == null)
                {
                    isRunning = false;
                    return;
                }

                Piece piece = board.GetPiece(aiMove.From);
                if (piece == null)
                {
                    renderer.DisplayError($"AI selected invalid move!");
                    logger.Error($"AI selected invalid move - no piece at location");
                    isRunning = false;
                    return;
                }

                // Execute move
                moveExecutor.ExecuteMove(aiMove.From, aiMove.To);

                // Record for learning
                PieceColor opponent = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                bool causedCheck = checkDetector.IsKingInCheck(board, opponent);
                bool causedCheckmate = causedCheck && checkDetector.IsCheckmate(board, opponent);

                recorder.RecordMove(
                    currentPlayer, aiMove.From, aiMove.To, piece.GetType().Name,
                    $"{LocationToAlgebraic(aiMove.From)}{LocationToAlgebraic(aiMove.To)}",
                    false, causedCheck, causedCheckmate,
                    aiMove.Evaluation, aiMove.ThinkingTimeMs
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

        private void HandleSaveGame()
        {
            try
            {
                var context = CreateGameContext();
                var snapshot = snapshotManager.CreateSnapshot(board, context);
                string filePath = saveManager.SaveGame(snapshot, currentGameId);
                renderer.DisplayInfo($"Game saved successfully! Game ID: {currentGameId}");
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

        private void HandleLoadGame(int gameId)
        {
            try
            {
                var snapshot = saveManager.LoadGame(gameId);
                snapshotManager.RestoreSnapshot(snapshot, board, out GameContext context);
                RestoreGameContext(context);
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

        private void HandleRollback()
        {
            if (!stateManager.CanRollback())
            {
                renderer.DisplayError("No moves to undo!");
                WaitForKey();
                return;
            }

            try
            {
                var snapshot = stateManager.Rollback();
                snapshotManager.RestoreSnapshot(snapshot, board, out GameContext context);
                RestoreGameContext(context);
                renderer.DisplayInfo("Move undone!");
                logger.Info("Move rolled back");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Rollback failed: {ex.Message}");
                logger.Error("Rollback failed", ex);
            }
            WaitForKey();
        }

        private void HandleRedo()
        {
            if (!stateManager.CanRedo())
            {
                renderer.DisplayError("No moves to redo!");
                WaitForKey();
                return;
            }

            try
            {
                var snapshot = stateManager.Redo();
                snapshotManager.RestoreSnapshot(snapshot, board, out GameContext context);
                RestoreGameContext(context);
                renderer.DisplayInfo("Move redone!");
                logger.Info("Move redone");
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Redo failed: {ex.Message}");
                logger.Error("Redo failed", ex);
            }
            WaitForKey();
        }

        private void HandleShowSettings()
        {
            settingsManager.ShowSettingsMenu();
        }

        private CastlingSide? PromptForCastlingSide(GameCommand command)
        {
            // Simplified for now - can be enhanced later
            return CastlingSide.Kingside;
        }

        private void CleanupGameFiles()
        {
            try
            {
                stateManager.CleanupAutosave();
                logger.Info("Autosave cleaned up");
            }
            catch (Exception ex)
            {
                logger.Warning($"Autosave cleanup failed: {ex.Message}");
            }
        }

        private void WaitForKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }

        private GameContext CreateGameContext()
        {
            return new GameContext
            {
                GameId = currentGameId,
                GameMode = gameMode,
                CurrentPlayer = currentPlayer,
                HumanColor = humanColor,
                GameResult = gameResult,
                Difficulty = difficulty,
                WhitePlayerName = whitePlayerName,
                BlackPlayerName = blackPlayerName,
                Players = players
            };
        }

        private void RestoreGameContext(GameContext context)
        {
            currentPlayer = context.CurrentPlayer;
            gameMode = context.GameMode;
            humanColor = context.HumanColor;
            gameResult = context.GameResult;
            difficulty = context.Difficulty;
            whitePlayerName = context.WhitePlayerName;
            blackPlayerName = context.BlackPlayerName;

            turnManager.SetCurrentPlayer(currentPlayer);
            moveExecutor.SetCurrentPlayer(currentPlayer);
            commandProcessor.SetCurrentPlayer(currentPlayer);
        }
    }
}
