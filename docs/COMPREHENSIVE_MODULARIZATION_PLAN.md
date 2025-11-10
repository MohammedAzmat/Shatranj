# Comprehensive Modularization & Decoupling Plan

## Executive Summary

After thorough analysis of the entire Shatranj codebase, we've identified critical architectural issues that prevent testability, reusability, and extensibility. This plan outlines a complete refactoring strategy to transform the codebase into a clean, decoupled, SOLID-compliant architecture.

**Current State:** Tightly-coupled monolith with God classes, no DI, mixed concerns
**Target State:** Clean architecture with proper layering, DI, and interface-based design

---

## Critical Issues Identified

### 1. God Class Problem (CRITICAL)
- **ChessGame.cs** (1,279 lines): 19+ responsibilities
- Violates Single Responsibility Principle fundamentally
- Impossible to test, maintain, or extend

### 2. Missing Abstractions (HIGH)
- 15+ services without interfaces
- Concrete implementations tightly coupled throughout
- Cannot swap implementations or mock for testing

### 3. No Dependency Injection (HIGH)
- Constructor instantiation everywhere (`new` keyword abuse)
- Violates Dependency Inversion Principle
- Hard dependencies make testing impossible

### 4. UI Coupled to Business Logic (HIGH)
- `Console.WriteLine` calls in business classes
- Cannot reuse logic for GUI, web, or mobile
- Mixed responsibilities (validation + UI, logic + display)

### 5. Duplicated Logic (MEDIUM)
- **Program.cs** repeats AI creation logic 3 times
- Game initialization duplicated
- Settings UI duplicated

### 6. Mixed Validation & Execution (MEDIUM)
- CastlingValidator does both validation AND execution
- PawnPromotionHandler does rules AND UI prompting
- MoveHistory stores data AND displays UI

---

## Architecture Vision

### Current Architecture (Problematic)
```
Program.cs
    ↓
ChessGame (GOD CLASS - 1,279 lines, 19 responsibilities)
    ├─ Creates all dependencies
    ├─ Game loop
    ├─ Command routing
    ├─ Move execution
    ├─ AI handling
    ├─ Save/load
    ├─ Settings
    ├─ UI rendering
    └─ Everything else...
```

### Target Architecture (Clean Layers)
```
┌──────────────────────────────────────────┐
│   Presentation Layer (UI)                │
│   - IRenderer implementations            │
│   - IMenuHandler implementations         │
│   - Console-specific UI                  │
└───────────────┬──────────────────────────┘
                │
┌───────────────▼──────────────────────────┐
│   Application Layer (Orchestration)      │
│   - IGameOrchestrator                    │
│   - IGameLoop                            │
│   - ICommandProcessor                    │
│   - IAIHandler                           │
│   - IGameCommands                        │
└───────────────┬──────────────────────────┘
                │
┌───────────────▼──────────────────────────┐
│   Domain Layer (Business Logic)          │
│   - IMoveExecutor                        │
│   - IMoveValidator                       │
│   - ITurnManager                         │
│   - ICheckDetector                       │
│   - ICastlingValidator/Executor          │
│   - IPromotionRule                       │
│   - IChessBoard                          │
│   - Pieces                               │
└───────────────┬──────────────────────────┘
                │
┌───────────────▼──────────────────────────┐
│   Infrastructure Layer (Services)        │
│   - ISaveGameManager                     │
│   - IGameSerializer                      │
│   - IGameRecorder                        │
│   - ILogger implementations              │
│   - ISnapshotManager                     │
└──────────────────────────────────────────┘
```

---

## Phase-by-Phase Implementation Plan

---

## PHASE 1: Create Interface Abstractions (FOUNDATION)
**Priority:** CRITICAL
**Effort:** 4-6 hours
**Risk:** Low
**Dependencies:** None

### 1.1 Create Core Interfaces

Create new folder: `/ShatranjCore/Abstractions/Interfaces/`

#### UI Interfaces
```csharp
// IRenderer.cs
public interface IRenderer
{
    void RenderBoard(IChessBoard board, Location? lastFrom, Location? lastTo);
    void DisplayGameStatus(GameStatus status);
    void DisplayError(string message);
    void DisplayInfo(string message);
    void DisplayPossibleMoves(Location from, List<Move> legalMoves);
    void DisplayGameOver(GameResult result, PieceColor? winner = null);
    void DisplayCommands();
}

// ICommandParser.cs
public interface ICommandParser
{
    GameCommand Parse(string input);
}

// IMenuHandler.cs
public interface IMenuHandler
{
    GameMode ShowGameModeMenu();
    PieceColor ShowColorSelectionMenu();
    MainMenuChoice ShowMainMenu(bool hasAutosave);
}

// IPromotionUI.cs
public interface IPromotionUI
{
    Type PromptForPromotion(PieceColor color);
}
```

#### Game Logic Interfaces
```csharp
// IGameOrchestrator.cs
public interface IGameOrchestrator
{
    void Start();
}

// IGameLoop.cs
public interface IGameLoop
{
    void Run();
}

// ICommandProcessor.cs
public interface ICommandProcessor
{
    void ProcessCommand(string input);
}

// IMoveExecutor.cs
public interface IMoveExecutor
{
    void ExecuteMove(Location from, Location to, Piece piece);
}

// IMoveValidator.cs
public interface IMoveValidator
{
    bool IsValid(Location from, Location to, PieceColor currentPlayer);
    ValidationResult ValidateMove(Location from, Location to, PieceColor currentPlayer);
}

// ITurnManager.cs
public interface ITurnManager
{
    void SwitchTurns();
    PieceColor CurrentPlayer { get; }
}

// IGameCommands.cs
public interface IGameCommands
{
    void SaveGame();
    void LoadGame(int gameId);
    void ShowSettings();
    void Rollback();
    void Redo();
}

// IAIHandler.cs
public interface IAIHandler
{
    void HandleAITurn(IChessAI ai, PieceColor color);
}
```

#### Validator Interfaces
```csharp
// ICheckDetector.cs
public interface ICheckDetector
{
    bool IsKingInCheck(IChessBoard board, PieceColor color);
    bool IsCheckmate(IChessBoard board, PieceColor color);
    bool IsStalemate(IChessBoard board, PieceColor color);
    bool WouldMoveCauseCheck(IChessBoard board, Location from, Location to, PieceColor color);
}

// ICastlingValidator.cs
public interface ICastlingValidator
{
    bool CanCastleKingside(IChessBoard board, PieceColor color);
    bool CanCastleQueenside(IChessBoard board, PieceColor color);
}

// ICastlingExecutor.cs
public interface ICastlingExecutor
{
    void ExecuteCastle(IChessBoard board, PieceColor color, CastlingSide side);
}

// IEnPassantTracker.cs
public interface IEnPassantTracker
{
    void RecordPawnDoubleMove(Location from, Location to);
    Location? GetEnPassantTarget();
    Location? GetEnPassantCaptureLocation();
    void NextTurn();
    void Reset();
}

// IMoveGenerator.cs (NEW)
public interface IMoveGenerator
{
    List<Move> GetLegalMoves(IChessBoard board, Location from, PieceColor color, Location? enPassantTarget);
}

// IPromotionRule.cs
public interface IPromotionRule
{
    bool NeedsPromotion(Piece piece, Location location);
}
```

#### State & Persistence Interfaces
```csharp
// IGameStateManager.cs
public interface IGameStateManager
{
    void RecordState(GameStateSnapshot snapshot);
    GameStateSnapshot Rollback();
    GameStateSnapshot Redo();
    void ClearRedoStack();
    void Autosave(GameStateSnapshot snapshot);
    void CleanupAutosave();
    bool CanRollback();
    bool CanRedo();
}

// ISaveGameManager.cs
public interface ISaveGameManager
{
    string SaveGame(GameStateSnapshot snapshot, int gameId);
    GameStateSnapshot LoadGame(int gameId);
    GameStateSnapshot LoadAutosave();
    bool AutosaveExists();
    List<GameMetadata> ListSavedGames();
    void DeleteGame(int gameId);
}

// IGameSerializer.cs
public interface IGameSerializer
{
    string Serialize(GameStateSnapshot snapshot);
    GameStateSnapshot Deserialize(string json);
}

// ISnapshotManager.cs (NEW)
public interface ISnapshotManager
{
    GameStateSnapshot CreateSnapshot(IChessBoard board, GameContext context);
    void RestoreSnapshot(GameStateSnapshot snapshot, IChessBoard board, out GameContext context);
}

// ISettingsManager.cs
public interface ISettingsManager
{
    string SetProfileName(string name);
    string SetOpponentName(string name);
    DifficultyLevel SetDifficulty(string difficultyStr);
    GameConfig ResetToDefaults();
    GameConfig GetCurrentConfig();
}
```

#### AI & Learning Interfaces
```csharp
// IMoveEvaluator.cs
public interface IMoveEvaluator
{
    double Evaluate(IChessBoard board, PieceColor color);
}

// IGameRecorder.cs
public interface IGameRecorder
{
    void StartNewGame(GameMode mode, string whitePlayerType, string blackPlayerType);
    void RecordMove(PieceColor player, Location from, Location to, string pieceName,
                    string notation, bool wasCapture, bool causedCheck, bool causedCheckmate,
                    double evaluation = 0, long thinkingTimeMs = 0);
    void EndGame(string winner, string endReason);
    void SaveGame(string fileName);
}
```

#### Factory Interfaces
```csharp
// IPieceFactory.cs
public interface IPieceFactory
{
    Piece CreatePiece(string type, PieceColor color, int row, int column);
    Piece CreatePiece(Type type, PieceColor color, Location location);
}

// IAIFactory.cs (NEW)
public interface IAIFactory
{
    IChessAI CreateAI(DifficultyLevel difficulty);
}

// IGameFactory.cs (NEW)
public interface IGameFactory
{
    IGameOrchestrator CreateGame(GameMode mode, PieceColor humanColor);
}
```

#### History Interface
```csharp
// IMoveHistory.cs
public interface IMoveHistory
{
    void AddMove(Move move, PieceColor player, bool wasCapture, bool wasCheck, bool wasCheckmate);
    MoveRecord GetLastMove();
    List<MoveRecord> GetAllMoves();
    void Clear();
}

// IMoveHistoryRenderer.cs (NEW - separate display concern)
public interface IMoveHistoryRenderer
{
    void DisplayHistory(IMoveHistory history);
}
```

### 1.2 Update Existing Classes to Implement Interfaces

Minimal changes:
```csharp
// Update each existing class:
public class ConsoleBoardRenderer : IRenderer { ... }
public class CommandParser : ICommandParser { ... }
public class GameMenuHandler : IMenuHandler { ... }
public class CheckDetector : ICheckDetector { ... }
public class CastlingValidator : ICastlingValidator { ... }
public class EnPassantTracker : IEnPassantTracker { ... }
public class GameStateManager : IGameStateManager { ... }
public class SaveGameManager : ISaveGameManager { ... }
public class GameSerializer : IGameSerializer { ... }
public class MoveHistory : IMoveHistory { ... }
public class GameRecorder : IGameRecorder { ... }
public class MoveEvaluator : IMoveEvaluator { ... }
```

---

## PHASE 2: Break Apart ChessGame God Class (CRITICAL)
**Priority:** CRITICAL
**Effort:** 12-16 hours
**Risk:** Medium-High
**Dependencies:** Phase 1 complete

### 2.1 Extract Command Processing (450 lines)

**New File:** `/ShatranjCore/Application/CommandProcessor.cs`

```csharp
public class CommandProcessor : ICommandProcessor
{
    private readonly ICommandParser _parser;
    private readonly IMoveExecutor _moveExecutor;
    private readonly IGameCommands _gameCommands;
    private readonly IRenderer _renderer;
    private readonly ILogger _logger;

    public CommandProcessor(
        ICommandParser parser,
        IMoveExecutor moveExecutor,
        IGameCommands gameCommands,
        IRenderer renderer,
        ILogger logger)
    {
        _parser = parser;
        _moveExecutor = moveExecutor;
        _gameCommands = gameCommands;
        _renderer = renderer;
        _logger = logger;
    }

    public void ProcessCommand(string input)
    {
        var command = _parser.Parse(input);

        switch (command.Type)
        {
            case CommandType.Move:
                HandleMoveCommand(command);
                break;
            case CommandType.Castle:
                HandleCastleCommand(command);
                break;
            // ... all other command types
        }
    }

    // All Handle*Command methods from ChessGame
    private void HandleMoveCommand(GameCommand command) { ... }
    private void HandleCastleCommand(GameCommand command) { ... }
    // etc...
}
```

**Extracted from ChessGame.cs:**
- `ProcessCommand()` method
- All `Handle*Command()` methods (12 methods)
- Command routing logic

---

### 2.2 Extract Move Execution (250 lines)

**New File:** `/ShatranjCore/Domain/MoveExecutor.cs`

```csharp
public class MoveExecutor : IMoveExecutor
{
    private readonly IChessBoard _board;
    private readonly IPromotionRule _promotionRule;
    private readonly IPromotionUI _promotionUI;
    private readonly IPieceFactory _pieceFactory;
    private readonly IEnPassantTracker _enPassantTracker;
    private readonly ICheckDetector _checkDetector;
    private readonly IMoveHistory _moveHistory;
    private readonly IRenderer _renderer;
    private readonly ILogger _logger;
    private readonly List<Piece> _capturedPieces;

    public MoveExecutor(
        IChessBoard board,
        IPromotionRule promotionRule,
        IPromotionUI promotionUI,
        IPieceFactory pieceFactory,
        IEnPassantTracker enPassantTracker,
        ICheckDetector checkDetector,
        IMoveHistory moveHistory,
        IRenderer renderer,
        ILogger logger)
    {
        _board = board;
        _promotionRule = promotionRule;
        _promotionUI = promotionUI;
        _pieceFactory = pieceFactory;
        _enPassantTracker = enPassantTracker;
        _checkDetector = checkDetector;
        _moveHistory = moveHistory;
        _renderer = renderer;
        _logger = logger;
        _capturedPieces = new List<Piece>();
    }

    public void ExecuteMove(Location from, Location to, Piece piece)
    {
        // All move execution logic from ChessGame.ExecuteMove()
        // - Capture handling
        // - En passant
        // - Pawn promotion
        // - Move recording
        // - Check detection
    }

    public List<Piece> GetCapturedPieces() => _capturedPieces;
}
```

**Extracted from ChessGame.cs:**
- `ExecuteMove()` method (100+ lines)
- Capture handling logic
- En passant logic
- Pawn promotion logic

---

### 2.3 Extract Snapshot Management (200 lines)

**New File:** `/ShatranjCore/State/SnapshotManager.cs`

```csharp
public class SnapshotManager : ISnapshotManager
{
    private readonly ILogger _logger;

    public SnapshotManager(ILogger logger)
    {
        _logger = logger;
    }

    public GameStateSnapshot CreateSnapshot(IChessBoard board, GameContext context)
    {
        var snapshot = new GameStateSnapshot
        {
            GameId = context.GameId,
            GameMode = context.GameMode.ToString(),
            CurrentPlayer = context.CurrentPlayer.ToString(),
            // ... all snapshot creation logic
        };

        // Save all pieces
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece piece = board.GetPiece(new Location(row, col));
                if (piece != null)
                {
                    snapshot.Pieces.Add(new PieceData { ... });
                }
            }
        }

        return snapshot;
    }

    public void RestoreSnapshot(GameStateSnapshot snapshot, IChessBoard board, out GameContext context)
    {
        // All restoration logic from ChessGame.RestoreFromSnapshot()
    }
}

// NEW: Context object to hold game state
public class GameContext
{
    public int GameId { get; set; }
    public GameMode GameMode { get; set; }
    public PieceColor CurrentPlayer { get; set; }
    public PieceColor HumanColor { get; set; }
    public GameResult GameResult { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string WhitePlayerName { get; set; }
    public string BlackPlayerName { get; set; }
    public Player[] Players { get; set; }
}
```

**Extracted from ChessGame.cs:**
- `CreateSnapshot()` method
- `RestoreFromSnapshot()` method

---

### 2.4 Extract AI Handling (100 lines)

**New File:** `/ShatranjCore/Application/AIHandler.cs`

```csharp
public class AIHandler : IAIHandler
{
    private readonly IMoveExecutor _moveExecutor;
    private readonly IGameRecorder _recorder;
    private readonly ICheckDetector _checkDetector;
    private readonly IEnPassantTracker _enPassantTracker;
    private readonly IRenderer _renderer;
    private readonly ILogger _logger;

    public AIHandler(
        IMoveExecutor moveExecutor,
        IGameRecorder recorder,
        ICheckDetector checkDetector,
        IEnPassantTracker enPassantTracker,
        IRenderer renderer,
        ILogger logger)
    {
        _moveExecutor = moveExecutor;
        _recorder = recorder;
        _checkDetector = checkDetector;
        _enPassantTracker = enPassantTracker;
        _renderer = renderer;
        _logger = logger;
    }

    public void HandleAITurn(IChessAI ai, PieceColor color)
    {
        // All logic from ChessGame.HandleAIMove()
    }
}
```

**Extracted from ChessGame.cs:**
- `HandleAIMove()` method

---

### 2.5 Extract Turn Management (30 lines)

**New File:** `/ShatranjCore/Domain/TurnManager.cs`

```csharp
public class TurnManager : ITurnManager
{
    private readonly IGameStateManager _stateManager;
    private readonly IEnPassantTracker _enPassantTracker;
    private readonly ISnapshotManager _snapshotManager;
    private readonly IChessBoard _board;
    private readonly ILogger _logger;

    private PieceColor _currentPlayer;
    private Player[] _players;
    private GameContext _gameContext;

    public PieceColor CurrentPlayer => _currentPlayer;

    public TurnManager(
        IGameStateManager stateManager,
        IEnPassantTracker enPassantTracker,
        ISnapshotManager snapshotManager,
        IChessBoard board,
        ILogger logger)
    {
        _stateManager = stateManager;
        _enPassantTracker = enPassantTracker;
        _snapshotManager = snapshotManager;
        _board = board;
        _logger = logger;
    }

    public void SwitchTurns()
    {
        _currentPlayer = _currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
        _players[0].HasTurn = !_players[0].HasTurn;
        _players[1].HasTurn = !_players[1].HasTurn;
        _enPassantTracker.NextTurn();

        // Clear redo stack
        _stateManager.ClearRedoStack();

        // Autosave
        try
        {
            GameStateSnapshot snapshot = _snapshotManager.CreateSnapshot(_board, _gameContext);
            _stateManager.RecordState(snapshot);
            _stateManager.Autosave(snapshot);
        }
        catch (Exception ex)
        {
            _logger.Warning($"State management failed: {ex.Message}");
        }
    }
}
```

**Extracted from ChessGame.cs:**
- `SwitchTurns()` method
- Turn management logic

---

### 2.6 Extract Game Loop (200 lines)

**New File:** `/ShatranjCore/Application/GameLoop.cs`

```csharp
public class GameLoop : IGameLoop
{
    private readonly IChessBoard _board;
    private readonly IRenderer _renderer;
    private readonly ICheckDetector _checkDetector;
    private readonly IMoveHistory _moveHistory;
    private readonly ICommandProcessor _commandProcessor;
    private readonly IAIHandler _aiHandler;
    private readonly ITurnManager _turnManager;
    private readonly IGameRecorder _recorder;
    private readonly ILogger _logger;
    private readonly GameContext _context;

    private bool _isRunning;

    public GameLoop(
        IChessBoard board,
        IRenderer renderer,
        ICheckDetector checkDetector,
        IMoveHistory moveHistory,
        ICommandProcessor commandProcessor,
        IAIHandler aiHandler,
        ITurnManager turnManager,
        IGameRecorder recorder,
        ILogger logger,
        GameContext context)
    {
        _board = board;
        _renderer = renderer;
        _checkDetector = checkDetector;
        _moveHistory = moveHistory;
        _commandProcessor = commandProcessor;
        _aiHandler = aiHandler;
        _turnManager = turnManager;
        _recorder = recorder;
        _logger = logger;
        _context = context;
    }

    public void Run()
    {
        _isRunning = true;

        while (_isRunning)
        {
            // Check for checkmate/stalemate
            // Render board
            // Display status
            // Handle turn (AI or human)
        }
    }
}
```

**Extracted from ChessGame.cs:**
- `GameLoop()` method
- Main game loop logic

---

### 2.7 Create Game Orchestrator (150 lines)

**New File:** `/ShatranjCore/Application/GameOrchestrator.cs`

```csharp
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

    public void Start()
    {
        _logger.Info("Starting new game");
        _gameLoop.Run();
    }
}
```

---

### 2.8 What Remains in ChessGame.cs?

After extraction, ChessGame.cs becomes a **lightweight facade** (< 200 lines):

```csharp
public class ChessGame
{
    private readonly IGameOrchestrator _orchestrator;

    public ChessGame(IGameOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public void Start()
    {
        _orchestrator.Start();
    }
}
```

Or ChessGame.cs can be **removed entirely** and replaced with IGameOrchestrator!

---

## PHASE 3: Separate UI from Business Logic (HIGH PRIORITY)
**Priority:** HIGH
**Effort:** 6-8 hours
**Risk:** Low-Medium
**Dependencies:** Phase 2 in progress

### 3.1 Split PawnPromotionHandler

**Current:** Mixes rules with UI
**Solution:** Split into two classes

**New File:** `/ShatranjCore/Domain/PromotionRule.cs`
```csharp
public class PromotionRule : IPromotionRule
{
    public bool NeedsPromotion(Piece piece, Location location)
    {
        if (piece is Pawn)
        {
            return (piece.Color == PieceColor.White && location.Row == 0) ||
                   (piece.Color == PieceColor.Black && location.Row == 7);
        }
        return false;
    }
}
```

**New File:** `/ShatranjCore/UI/ConsolePromotionUI.cs`
```csharp
public class ConsolePromotionUI : IPromotionUI
{
    public Type PromptForPromotion(PieceColor color)
    {
        // All Console.WriteLine/ReadKey logic
    }
}
```

**Update:** `/ShatranjCore/Handlers/PawnPromotionHandler.cs`
```csharp
// This class can be REMOVED entirely, replaced by:
// - IPromotionRule (business logic)
// - IPromotionUI (user interaction)
// - IPieceFactory (piece creation)
```

---

### 3.2 Split MoveHistory

**Current:** Stores data AND displays UI
**Solution:** Keep storage, extract display

**Update:** `/ShatranjCore/Movement/MoveHistory.cs`
```csharp
public class MoveHistory : IMoveHistory
{
    // Remove DisplayHistory() method entirely
    // Keep only:
    // - AddMove()
    // - GetLastMove()
    // - GetAllMoves()
    // - Clear()
}
```

**New File:** `/ShatranjCore/UI/ConsoleMoveHistoryRenderer.cs`
```csharp
public class ConsoleMoveHistoryRenderer : IMoveHistoryRenderer
{
    public void DisplayHistory(IMoveHistory history)
    {
        // Move DisplayHistory() logic here
    }
}
```

---

### 3.3 Update SettingsManager

**Current:** Manages settings AND displays UI
**Solution:** Remove UI from SettingsManager

**Update:** `/ShatranjCore/Settings/SettingsManager.cs`
```csharp
public class SettingsManager : ISettingsManager
{
    private readonly GameConfigManager _configManager;
    private readonly ILogger _logger;

    // Remove renderer dependency entirely
    // Remove ShowSettingsMenu() - move to UI layer

    public SettingsManager(
        GameConfigManager configManager,
        ILogger logger)
    {
        _configManager = configManager;
        _logger = logger;
    }

    // Keep only business logic:
    public string SetProfileName(string name) { ... }
    public string SetOpponentName(string name) { ... }
    public DifficultyLevel SetDifficulty(string difficultyStr) { ... }
    public GameConfig ResetToDefaults() { ... }
    public GameConfig GetCurrentConfig() { ... }
}
```

**New File:** `/ShatranjCore/UI/SettingsMenuUI.cs`
```csharp
public class SettingsMenuUI
{
    private readonly ISettingsManager _settingsManager;
    private readonly IRenderer _renderer;

    public SettingsMenuUI(ISettingsManager settingsManager, IRenderer renderer)
    {
        _settingsManager = settingsManager;
        _renderer = renderer;
    }

    public void ShowSettingsMenu()
    {
        // All UI logic from SettingsManager.ShowSettingsMenu()
    }

    private void DisplayCurrentSettings() { ... }
    private void DisplaySettingsOptions() { ... }
}
```

---

### 3.4 Split CastlingValidator

**Current:** Validates AND executes
**Solution:** Split into two classes

**Update:** `/ShatranjCore/Validators/CastlingValidator.cs`
```csharp
public class CastlingValidator : ICastlingValidator
{
    private readonly ICheckDetector _checkDetector;

    public CastlingValidator(ICheckDetector checkDetector)
    {
        _checkDetector = checkDetector;
    }

    public bool CanCastleKingside(IChessBoard board, PieceColor color) { ... }
    public bool CanCastleQueenside(IChessBoard board, PieceColor color) { ... }

    // Remove ExecuteCastle() method
}
```

**New File:** `/ShatranjCore/Domain/CastlingExecutor.cs`
```csharp
public class CastlingExecutor : ICastlingExecutor
{
    public void ExecuteCastle(IChessBoard board, PieceColor color, CastlingSide side)
    {
        // Move ExecuteCastle() logic here
    }
}
```

---

## PHASE 4: Implement Dependency Injection (CRITICAL)
**Priority:** CRITICAL
**Effort:** 8-10 hours
**Risk:** Medium
**Dependencies:** Phases 1-3 complete

### 4.1 Add DI Container

**Install:** `Microsoft.Extensions.DependencyInjection`

```bash
dotnet add ShatranjCMD/ShatranjCMD.csproj package Microsoft.Extensions.DependencyInjection
```

---

### 4.2 Create DI Composition Root

**New File:** `/ShatranjCMD/DependencyInjection/ServiceRegistration.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using ShatranjCore.*;
// ... all imports

public static class ServiceRegistration
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // === UI Layer ===
        services.AddSingleton<IRenderer, ConsoleBoardRenderer>();
        services.AddSingleton<ICommandParser, CommandParser>();
        services.AddSingleton<IMenuHandler, GameMenuHandler>();
        services.AddSingleton<IPromotionUI, ConsolePromotionUI>();
        services.AddSingleton<IMoveHistoryRenderer, ConsoleMoveHistoryRenderer>();

        // === Application Layer ===
        services.AddScoped<IGameOrchestrator, GameOrchestrator>();
        services.AddScoped<IGameLoop, GameLoop>();
        services.AddScoped<ICommandProcessor, CommandProcessor>();
        services.AddScoped<IAIHandler, AIHandler>();
        services.AddScoped<IGameCommands, GameCommands>();

        // === Domain Layer ===
        services.AddScoped<IMoveExecutor, MoveExecutor>();
        services.AddScoped<IMoveValidator, MoveValidator>();
        services.AddScoped<ITurnManager, TurnManager>();

        // Validators (Singleton - stateless)
        services.AddSingleton<ICheckDetector, CheckDetector>();
        services.AddSingleton<ICastlingValidator, CastlingValidator>();
        services.AddSingleton<ICastlingExecutor, CastlingExecutor>();
        services.AddSingleton<IEnPassantTracker, EnPassantTracker>();
        services.AddSingleton<IPromotionRule, PromotionRule>();
        services.AddSingleton<IMoveGenerator, MoveGenerator>();

        // Board (Scoped - per game)
        services.AddScoped<IChessBoard, ChessBoard>();

        // === Infrastructure Layer ===
        // State & Persistence
        services.AddScoped<IGameStateManager, GameStateManager>();
        services.AddScoped<ISnapshotManager, SnapshotManager>();
        services.AddSingleton<ISaveGameManager, SaveGameManager>();
        services.AddSingleton<IGameSerializer, GameSerializer>();
        services.AddSingleton<GameConfigManager>();

        // Settings
        services.AddSingleton<ISettingsManager, SettingsManager>();

        // Factories
        services.AddSingleton<IPieceFactory, PieceFactory>();
        services.AddSingleton<IAIFactory, AIFactory>();
        services.AddTransient<IGameFactory, GameFactory>();

        // AI (Scoped - per game)
        services.AddScoped<IMoveEvaluator, MoveEvaluator>();
        // AI instances created by AIFactory

        // Learning
        services.AddSingleton<IGameRecorder, GameRecorder>();

        // Logging
        services.AddSingleton<ILogger>(sp => new CompositeLogger(
            new FileLogger(),
            new ConsoleLogger(includeTimestamp: false)
        ));

        // History
        services.AddScoped<IMoveHistory, MoveHistory>();

        return services.BuildServiceProvider();
    }
}
```

---

### 4.3 Create Factory Classes

#### AI Factory
**New File:** `/ShatranjAI/AI/AIFactory.cs`

```csharp
public class AIFactory : IAIFactory
{
    private readonly IMoveEvaluator _evaluator;
    private readonly ICheckDetector _checkDetector;
    private readonly ILogger _logger;

    public AIFactory(
        IMoveEvaluator evaluator,
        ICheckDetector checkDetector,
        ILogger logger)
    {
        _evaluator = evaluator;
        _checkDetector = checkDetector;
        _logger = logger;
    }

    public IChessAI CreateAI(DifficultyLevel difficulty)
    {
        int depth = (int)difficulty;
        return new BasicAI(_evaluator, _checkDetector, depth, _logger);
    }
}
```

#### Update BasicAI Constructor
```csharp
public class BasicAI : IChessAI
{
    private readonly IMoveEvaluator _evaluator;
    private readonly ICheckDetector _checkDetector;
    private readonly ILogger _logger;

    // Inject dependencies instead of creating them!
    public BasicAI(
        IMoveEvaluator evaluator,
        ICheckDetector checkDetector,
        int depth,
        ILogger logger)
    {
        _evaluator = evaluator;
        _checkDetector = checkDetector;
        Depth = depth;
        _logger = logger;
    }
}
```

#### Game Factory
**New File:** `/ShatranjCore/Application/GameFactory.cs`

```csharp
public class GameFactory : IGameFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAIFactory _aiFactory;
    private readonly ILogger _logger;

    public GameFactory(
        IServiceProvider serviceProvider,
        IAIFactory aiFactory,
        ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _aiFactory = aiFactory;
        _logger = logger;
    }

    public IGameOrchestrator CreateGame(GameMode mode, PieceColor humanColor)
    {
        // Create new scope for the game
        var scope = _serviceProvider.CreateScope();

        // Create AI instances if needed
        IChessAI whiteAI = null;
        IChessAI blackAI = null;

        var configManager = scope.ServiceProvider.GetRequiredService<GameConfigManager>();
        var config = configManager.GetConfig();

        if (mode == GameMode.HumanVsAI)
        {
            IChessAI ai = _aiFactory.CreateAI(config.Difficulty);
            if (humanColor == PieceColor.White)
                blackAI = ai;
            else
                whiteAI = ai;
        }
        else if (mode == GameMode.AIVsAI)
        {
            whiteAI = _aiFactory.CreateAI(config.Difficulty);
            blackAI = _aiFactory.CreateAI(config.Difficulty);
        }

        // Create game context
        var context = new GameContext
        {
            GameId = configManager.GetNextGameId(),
            GameMode = mode,
            HumanColor = humanColor,
            Difficulty = config.Difficulty,
            WhitePlayerName = config.ProfileName,
            BlackPlayerName = config.OpponentProfileName,
            CurrentPlayer = PieceColor.White,
            GameResult = GameResult.InProgress
        };

        // Resolve orchestrator from scope
        var orchestrator = scope.ServiceProvider.GetRequiredService<IGameOrchestrator>();

        return orchestrator;
    }
}
```

#### Piece Factory (Enhanced)
**Update:** `/ShatranjCore/Persistence/PieceFactory.cs`

```csharp
public class PieceFactory : IPieceFactory
{
    public Piece CreatePiece(string type, PieceColor color, int row, int column)
    {
        return type switch
        {
            "Pawn" => new Pawn(row, column, color, GetDirection(color)),
            "Rook" => new Rook(row, column, color),
            "Knight" => new Knight(row, column, color),
            "Bishop" => new Bishop(row, column, color),
            "Queen" => new Queen(row, column, color),
            "King" => new King(row, column, color),
            _ => throw new ArgumentException($"Unknown piece type: {type}")
        };
    }

    public Piece CreatePiece(Type type, PieceColor color, Location location)
    {
        if (type == typeof(Queen))
            return new Queen(location.Row, location.Column, color);
        if (type == typeof(Rook))
            return new Rook(location.Row, location.Column, color);
        if (type == typeof(Bishop))
            return new Bishop(location.Row, location.Column, color);
        if (type == typeof(Knight))
            return new Knight(location.Row, location.Column, color);

        throw new ArgumentException($"Invalid promotion type: {type.Name}");
    }

    private PawnMoves GetDirection(PieceColor color)
    {
        return color == PieceColor.White ? PawnMoves.Up : PawnMoves.Down;
    }
}
```

---

### 4.4 Refactor Program.cs to Use DI

**Update:** `/ShatranjCMD/Program.cs`

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using ShatranjCMD.DependencyInjection;
using ShatranjCore.Abstractions;
using ShatranjCore.Application;
using ShatranjCore.UI;

namespace ShatranjCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console encoding
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Build DI container
            var serviceProvider = ServiceRegistration.ConfigureServices();

            // Show welcome message
            ShowWelcomeMessage();

            // Get required services
            var menuHandler = serviceProvider.GetRequiredService<IMenuHandler>();
            var gameFactory = serviceProvider.GetRequiredService<IGameFactory>();
            var saveManager = serviceProvider.GetRequiredService<ISaveGameManager>();
            var settingsUI = new SettingsMenuUI(
                serviceProvider.GetRequiredService<ISettingsManager>(),
                serviceProvider.GetRequiredService<IRenderer>()
            );

            // Main menu loop
            bool exitProgram = false;
            while (!exitProgram)
            {
                bool hasAutosave = saveManager.AutosaveExists();
                var menuChoice = menuHandler.ShowMainMenu(hasAutosave);

                switch (menuChoice)
                {
                    case MainMenuChoice.Resume:
                        ResumeGame(saveManager, gameFactory);
                        break;

                    case MainMenuChoice.NewGame:
                        StartNewGame(menuHandler, gameFactory);
                        break;

                    case MainMenuChoice.LoadGame:
                        LoadSavedGame(saveManager, gameFactory);
                        break;

                    case MainMenuChoice.Settings:
                        settingsUI.ShowSettingsMenu();
                        break;

                    case MainMenuChoice.Exit:
                        exitProgram = true;
                        break;
                }
            }

            // Exit
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Thank you for playing Shatranj!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ShowWelcomeMessage()
        {
            // Same as before
        }

        static void StartNewGame(IMenuHandler menuHandler, IGameFactory gameFactory)
        {
            GameMode mode = menuHandler.ShowGameModeMenu();
            PieceColor humanColor = PieceColor.White;

            if (mode == GameMode.HumanVsAI)
            {
                humanColor = menuHandler.ShowColorSelectionMenu();
            }

            // Factory handles ALL game creation logic!
            IGameOrchestrator game = gameFactory.CreateGame(mode, humanColor);
            game.Start();
        }

        static void ResumeGame(ISaveGameManager saveManager, IGameFactory gameFactory)
        {
            // Load autosave
            var snapshot = saveManager.LoadAutosave();

            // Parse snapshot metadata
            GameMode mode = (GameMode)Enum.Parse(typeof(GameMode), snapshot.GameMode);
            PieceColor humanColor = (PieceColor)Enum.Parse(typeof(PieceColor), snapshot.HumanColor);

            // Factory creates game
            IGameOrchestrator game = gameFactory.CreateGame(mode, humanColor);

            // TODO: Need to inject snapshot into game for restoration
            // This requires passing snapshot to orchestrator or implementing IGameLoader

            game.Start();
        }

        static void LoadSavedGame(ISaveGameManager saveManager, IGameFactory gameFactory)
        {
            // Similar to ResumeGame
        }
    }
}
```

**Result:** Program.cs goes from **539 lines → ~200 lines** (62% reduction!)

---

### 4.5 Update All Classes to Use Injected Dependencies

**Key changes:**

1. **Remove all `new` keywords** from constructors
2. **Inject interfaces** instead of concrete classes
3. **Use constructor injection** consistently

**Example - Update CheckDetector:**
```csharp
// BEFORE:
public class CastlingValidator : ICastlingValidator
{
    private readonly CheckDetector checkDetector;

    public CastlingValidator()
    {
        checkDetector = new CheckDetector();  // ❌ Constructor instantiation
    }
}

// AFTER:
public class CastlingValidator : ICastlingValidator
{
    private readonly ICheckDetector _checkDetector;

    public CastlingValidator(ICheckDetector checkDetector)  // ✅ Dependency injection
    {
        _checkDetector = checkDetector;
    }
}
```

**Example - Update ChessBoard:**
```csharp
// BEFORE:
public class ChessBoard : IChessBoard
{
    private void InitializeSplForces(PieceColor color)
    {
        squares[row, 0].Piece = new Rook(row, 0, color);  // ❌ Direct instantiation
    }
}

// AFTER:
public class ChessBoard : IChessBoard
{
    private readonly IPieceFactory _pieceFactory;

    public ChessBoard(IPieceFactory pieceFactory, PieceColor startingColor)
    {
        _pieceFactory = pieceFactory;
        InitializeBoard(startingColor);
    }

    private void InitializeSplForces(PieceColor color)
    {
        squares[row, 0].Piece = _pieceFactory.CreatePiece("Rook", color, row, 0);  // ✅ Factory
    }
}
```

---

## PHASE 5: Additional Improvements (MEDIUM PRIORITY)
**Priority:** MEDIUM
**Effort:** 4-6 hours
**Risk:** Low
**Dependencies:** Phases 1-4 complete

### 5.1 Extract MoveGenerator from CheckDetector

**Current:** CheckDetector does check detection AND move generation
**Solution:** Extract move generation

**New File:** `/ShatranjCore/Domain/MoveGenerator.cs`

```csharp
public class MoveGenerator : IMoveGenerator
{
    public List<Move> GetLegalMoves(IChessBoard board, Location from, PieceColor color, Location? enPassantTarget)
    {
        // Move logic from CheckDetector.GetLegalMoves()
    }
}
```

**Update CheckDetector:**
```csharp
public class CheckDetector : ICheckDetector
{
    private readonly IMoveGenerator _moveGenerator;

    public CheckDetector(IMoveGenerator moveGenerator)
    {
        _moveGenerator = moveGenerator;
    }

    // Now delegates to MoveGenerator instead of doing it itself
}
```

---

### 5.2 Create Utility Classes

**New File:** `/ShatranjCore/Utilities/AlgebraicNotation.cs`

```csharp
public static class AlgebraicNotation
{
    public static string LocationToAlgebraic(Location location)
    {
        char file = (char)('a' + location.Column);
        int rank = 8 - location.Row;
        return $"{file}{rank}";
    }

    public static Location AlgebraicToLocation(string notation)
    {
        char file = notation[0];
        int rank = int.Parse(notation[1].ToString());
        int col = file - 'a';
        int row = 8 - rank;
        return new Location(row, col);
    }
}
```

**Remove duplication:** This method appears in 5+ places currently!

---

### 5.3 Extract Board Initializer

**New File:** `/ShatranjCore/Board/BoardInitializer.cs`

```csharp
public class BoardInitializer
{
    private readonly IPieceFactory _pieceFactory;

    public BoardInitializer(IPieceFactory pieceFactory)
    {
        _pieceFactory = pieceFactory;
    }

    public void InitializeStandardChessPosition(Square[,] squares, PieceColor startingColor)
    {
        // Move initialization logic from ChessBoard
    }
}
```

---

## PHASE 6: Testing & Validation (HIGH PRIORITY)
**Priority:** HIGH
**Effort:** 10-12 hours
**Risk:** Medium
**Dependencies:** Phases 1-5 complete

### 6.1 Update Existing Tests

All existing tests need updates:
- Use interfaces instead of concrete classes
- Mock dependencies
- Test in isolation

### 6.2 Create New Unit Tests

Create tests for all new classes:
```
/tests/ShatranjCore.Tests/
├── Application/
│   ├── CommandProcessorTests.cs
│   ├── GameLoopTests.cs
│   ├── AIHandlerTests.cs
│   └── GameOrchestratorTests.cs
├── Domain/
│   ├── MoveExecutorTests.cs
│   ├── TurnManagerTests.cs
│   ├── MoveGeneratorTests.cs
│   ├── CastlingExecutorTests.cs
│   └── PromotionRuleTests.cs
├── State/
│   └── SnapshotManagerTests.cs
└── Factories/
    ├── AIFactoryTests.cs
    ├── PieceFactoryTests.cs
    └── GameFactoryTests.cs
```

### 6.3 Integration Tests

Create integration tests to verify:
- Full game flows work
- DI container resolves all dependencies
- No circular dependencies
- All commands work end-to-end

---

## Implementation Timeline

### Week 1: Foundation (Phases 1-2)
**Days 1-2:** Create all interfaces (Phase 1)
**Days 3-5:** Break apart ChessGame (Phase 2)

### Week 2: Decoupling (Phases 3-4)
**Days 1-2:** Separate UI from business logic (Phase 3)
**Days 3-5:** Implement DI (Phase 4)

### Week 3: Polish & Test (Phases 5-6)
**Days 1-2:** Additional improvements (Phase 5)
**Days 3-5:** Testing & validation (Phase 6)

**Total Estimated Effort:** 3 weeks (15 working days)

---

## Impact Summary

### Before Modularization
| File | Lines | Responsibilities | Issues |
|------|-------|------------------|--------|
| ChessGame.cs | 1,279 | 19+ | God class, no SRP |
| Program.cs | 539 | 8 | Duplicated logic |
| PawnPromotionHandler.cs | 144 | 3 | UI + Logic mixed |
| MoveHistory.cs | 150 | 2 | Display + Storage |
| SettingsManager.cs | 216 | 2 | UI + Logic mixed |
| CastlingValidator.cs | 170 | 2 | Validation + Execution |
| **Total Problems** | | | **35+ SRP violations** |

### After Modularization
| Component | Lines | Responsibilities | Compliance |
|-----------|-------|------------------|------------|
| GameOrchestrator | ~150 | 1 (Start game) | ✅ SRP |
| GameLoop | ~200 | 1 (Main loop) | ✅ SRP |
| CommandProcessor | ~450 | 1 (Route commands) | ✅ SRP |
| MoveExecutor | ~250 | 1 (Execute moves) | ✅ SRP |
| TurnManager | ~80 | 1 (Manage turns) | ✅ SRP |
| AIHandler | ~100 | 1 (AI coordination) | ✅ SRP |
| SnapshotManager | ~200 | 1 (Snapshots) | ✅ SRP |
| **Total** | ~1,430 | **7 focused classes** | **✅ SOLID** |

---

## Benefits Achieved

### ✅ Testability
- Every class can be tested in isolation
- Mock all dependencies via interfaces
- 100% code coverage achievable

### ✅ Maintainability
- Clear separation of concerns
- Changes isolated to specific classes
- Easy to locate bugs

### ✅ Extensibility
- Easy to add new features:
  - Different UIs (GUI, Web, Mobile)
  - New AI algorithms
  - Network multiplayer
  - Database persistence
  - Different game modes

### ✅ Reusability
- Business logic independent of UI
- Services can be reused in different contexts
- Pieces can be used in other chess projects

### ✅ SOLID Compliance
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ✅ Liskov Substitution Principle
- ✅ Interface Segregation Principle
- ✅ Dependency Inversion Principle

### ✅ Clean Architecture
- Clear layering (Presentation → Application → Domain → Infrastructure)
- Dependencies flow inward (UI depends on Domain, not vice versa)
- Business logic at the core, protected from external changes

---

## Risk Mitigation

### Risk: Breaking Existing Functionality
**Mitigation:**
- Implement phases incrementally
- Run existing tests after each phase
- Manual testing at each milestone

### Risk: Over-Engineering
**Mitigation:**
- Follow YAGNI principle
- Only add abstractions that solve real problems
- Review after each phase

### Risk: Performance Degradation
**Mitigation:**
- DI overhead is negligible
- Profile before/after major changes
- Optimize hot paths if needed

---

## Success Criteria

### Phase 1 Success
- [ ] All 25+ interfaces created
- [ ] Existing classes implement interfaces
- [ ] Code compiles with no errors
- [ ] All existing tests pass

### Phase 2 Success
- [ ] ChessGame.cs reduced to < 300 lines (or removed)
- [ ] 7+ new focused classes created
- [ ] Each class has single responsibility
- [ ] Code compiles with no errors

### Phase 3 Success
- [ ] No Console calls in business logic
- [ ] UI classes separated from domain
- [ ] PromotionRule, PromotionUI split
- [ ] MoveHistory only stores data

### Phase 4 Success
- [ ] DI container configured
- [ ] All dependencies injected
- [ ] No `new` keywords in business logic
- [ ] Factories create complex objects

### Phase 5 Success
- [ ] MoveGenerator extracted
- [ ] Utility classes created
- [ ] No code duplication

### Phase 6 Success
- [ ] All tests updated and passing
- [ ] 20+ new unit tests created
- [ ] Integration tests pass
- [ ] Code coverage > 80%

---

## Next Steps

1. **Review this plan** - Get approval on approach
2. **Set up branch** - Create feature branch for refactoring
3. **Start Phase 1** - Create interfaces
4. **Incremental commits** - Commit after each major change
5. **Continuous testing** - Run tests frequently
6. **Code reviews** - Review major changes before proceeding
7. **Documentation** - Update docs as we go

---

## Questions for Discussion

1. **Scope:** Should we do all phases, or prioritize specific phases?
2. **Timeline:** Is 3 weeks acceptable, or do we need faster delivery?
3. **Testing:** What level of test coverage is required?
4. **Breaking changes:** Are we okay with breaking existing API contracts?
5. **GUI consideration:** Should we plan for GUI support now or later?

---

**Document Status:** Draft for Review
**Last Updated:** 2025-11-10
**Author:** Claude (AI Assistant)
**Next Review:** After stakeholder feedback
