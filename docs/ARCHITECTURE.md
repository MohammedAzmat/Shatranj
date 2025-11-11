# Shatranj - Architecture Documentation

> **Last Updated**: November 2025
> **Version**: Phase 2 - AI Integration (100% Complete) ✅
> **SOLID Score**: 9/10

This document provides a comprehensive technical overview of the Shatranj chess project architecture.

---

## Table of Contents
1. [Architectural Overview](#architectural-overview)
2. [Project Structure](#project-structure)
3. [Abstractions Layer](#abstractions-layer)
4. [Core Abstractions](#core-abstractions)
5. [Design Patterns](#design-patterns)
6. [Data Flow](#data-flow)
7. [Key Algorithms](#key-algorithms)
8. [Extensibility](#extensibility)

---

## Architectural Overview

### Architecture Style
Shatranj follows a **Layered Architecture** with proper **Dependency Inversion**:

```
┌─────────────────────────────────────────────────────────────┐
│                   Presentation Layer                        │
│              (ShatranjCMD with DI setup)                    │
└─────────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┴──────────────────┐
        │                                      │
        ↓                                      ↓
┌─────────────────────┐            ┌──────────────────────┐
│    AI Layer         │            │  Game Logic Layer    │
│   (ShatranjAI)      │            │   (ShatranjCore)     │
│  - BasicAI          │            │  - ChessGame         │
│  - MoveEvaluator    │            │  - Validators        │
│                     │            │  - Movement          │
└─────────────────────┘            └──────────────────────┘
        │                                      │
        └───────────────────┬──────────────────┘
                            │
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                 Abstractions Layer                          │
│            (ShatranjCore.Abstractions)                      │
│  - IBoardState (minimal board interface)                   │
│  - IChessAI (AI interface)                                 │
│  - ILogger (logging interface)                             │
│  - Core Types (Location, PieceColor, GameMode)             │
│  NO DEPENDENCIES - Pure interfaces and types               │
└─────────────────────────────────────────────────────────────┘
                            │
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   Domain Model Layer                        │
│  - Pieces (King, Queen, Rook, Bishop, Knight, Pawn)        │
│  - Board (ChessBoard implements IChessBoard + IBoardState)  │
│  - IChessBoard (extends IBoardState with Piece types)      │
└─────────────────────────────────────────────────────────────┘
```

### Design Principles
1. **Separation of Concerns**: Each layer has a distinct responsibility
2. **Dependency Inversion**: High-level modules depend on abstractions (IChessBoard)
3. **Single Responsibility**: Each class has one clear purpose
4. **Testability**: Core logic is decoupled from UI and infrastructure
5. **Extensibility**: New pieces, validators, or UI can be added without modifying existing code

---

## Project Structure

### Complete Solution Organization

```
Shatranj/
├── ShatranjCore.Abstractions/   # 🔷 Core abstractions (NO DEPENDENCIES)
│   ├── CoreTypes.cs              # Location, PieceColor, GameMode, PlayerType, DifficultyLevel, SaveType
│   ├── IBoardState.cs            # Minimal board interface (object-based)
│   ├── IChessAI.cs               # AI interface
│   ├── ILogger.cs                # Logging interface
│   ├── Commands/                 # GameCommand, CommandType enums
│   └── Interfaces/               # 30+ abstraction interfaces
│       ├── IGameOrchestrator.cs  # Game startup
│       ├── IGameLoop.cs          # Main game loop
│       ├── ICommandProcessor.cs  # Command routing
│       ├── IMoveExecutor.cs      # Move execution
│       ├── ITurnManager.cs       # Turn management
│       ├── IAIHandler.cs         # AI coordination
│       ├── ICastlingValidator.cs # Castling rules
│       ├── ICastlingExecutor.cs  # Castling execution
│       ├── ICheckDetector.cs     # Check/checkmate detection
│       ├── IEnPassantTracker.cs  # En passant tracking
│       ├── IPromotionRule.cs     # Promotion rules
│       ├── IMoveHistory.cs       # Move history
│       ├── ISaveGameManager.cs   # Save/load games
│       ├── ISnapshotManager.cs   # Game state snapshots
│       ├── ISettingsManager.cs   # Settings management
│       ├── IGameRecorder.cs      # Game recording
│       ├── IRenderer.cs          # Rendering abstraction
│       └── ... (15+ more interfaces for extensibility)
│
├── ShatranjCore/                 # 🎮 Core game engine (organized in 12 modules)
│   │
│   ├── Application/              # 🚀 Application Layer (new Phase 3.0+)
│   │   ├── GameOrchestrator.cs   # Entry point for game startup
│   │   ├── GameLoop.cs           # Main game loop execution
│   │   ├── CommandProcessor.cs   # Route and handle user commands
│   │   └── AIHandler.cs          # Coordinate AI move selection
│   │
│   ├── Domain/                   # 📊 Domain Layer (new Phase 3.0+)
│   │   ├── MoveExecutor.cs       # Execute moves on board
│   │   ├── TurnManager.cs        # Manage player turns
│   │   ├── CastlingExecutor.cs   # Execute castling moves
│   │   └── PromotionRule.cs      # Pawn promotion rules
│   │
│   ├── Validators/               # ✅ Business Rule Validation
│   │   ├── CastlingValidator.cs  # Validate castling legality (6 rules)
│   │   ├── CheckDetector.cs      # Check/checkmate/stalemate detection
│   │   └── EnPassantTracker.cs   # En passant state tracking
│   │
│   ├── Pieces/                   # ♟️ Piece Implementations
│   │   ├── Piece.cs              # Abstract base class (isMoved, color, location)
│   │   ├── Pawn.cs               # Complex: forward moves, captures, en passant, promotion
│   │   ├── Rook.cs               # Castling support, linear moves
│   │   ├── Knight.cs             # L-shaped moves, jumping
│   │   ├── Bishop.cs             # Diagonal moves
│   │   ├── Queen.cs              # Rook + Bishop combined
│   │   └── King.cs               # Single square + castling
│   │
│   ├── Board/                    # 🎲 Board Representation
│   │   ├── ChessBoard.cs         # 8x8 array, implements IChessBoard + IBoardState
│   │   ├── Square.cs             # Individual square state
│   │   └── Move.cs               # Move representation (piece, from, to, captured)
│   │
│   ├── Interfaces/               # 📋 Core Game Interfaces
│   │   └── IChessBoard.cs        # Strongly-typed board interface (extends IBoardState)
│   │
│   ├── Movement/                 # 🔄 Move Data Structures & History
│   │   ├── MoveHistory.cs        # Tracks all moves, enables undo/redo
│   │   ├── MoveMaker.cs          # Legacy - mostly replaced by MoveExecutor
│   │   └── Move.cs               # Move data structure
│   │
│   ├── Game/                     # 🎮 Main Game Orchestration
│   │   ├── ChessGame.cs          # Main game coordinator (refactored, 484 lines)
│   │   └── Player.cs             # Player state
│   │
│   ├── State/                    # 💾 State Management (new Phase 3.0+)
│   │   ├── GameStateManager.cs   # Game state stack for undo/redo
│   │   ├── SnapshotManager.cs    # Create/restore game snapshots
│   │   └── GameStateSnapshot.cs  # Serializable game state
│   │
│   ├── Persistence/              # 💾 Save/Load Functionality
│   │   ├── SaveGameManager.cs    # Save/load complete games
│   │   ├── GameSerializer.cs     # Serialize game state
│   │   ├── GameConfig.cs         # Game configuration
│   │   ├── PieceFactory.cs       # Piece creation from serialized data
│   │   └── GameStateSnapshot.cs  # Serializable game state
│   │
│   ├── UI/                       # 🖥️ User Interface Layer
│   │   ├── ConsoleBoardRenderer.cs # Render 8x8 board (ASCII/Unicode)
│   │   ├── CommandParser.cs      # Parse user input → GameCommand
│   │   ├── ConsolePromotionUI.cs # Pawn promotion selection dialog
│   │   ├── ConsoleMoveHistoryRenderer.cs # Display move history
│   │   └── GameMenuHandler.cs    # Main menu system
│   │
│   ├── Handlers/                 # 🎯 Special Move Handlers
│   │   └── PawnPromotionHandler.cs # Promotion logic
│   │
│   ├── Learning/                 # 🧠 Game Recording & Analysis
│   │   ├── GameRecorder.cs       # Record games for AI learning
│   │   └── GameRecord.cs         # Recorded game data
│   │
│   ├── Logging/                  # 📝 Logging System
│   │   ├── ConsoleLogger.cs      # Log to console
│   │   ├── FileLogger.cs         # Log to file
│   │   ├── RollingFileLogger.cs  # Log with file rotation
│   │   ├── ErrorTraceLogger.cs   # Log errors with stack traces
│   │   ├── CompositeLogger.cs    # Log to multiple targets
│   │   └── LoggerFactory.cs      # Create logger instances
│   │
│   ├── Settings/                 # ⚙️ Configuration Management
│   │   └── SettingsManager.cs    # Game settings (difficulty, player names)
│   │
│   └── Utilities/                # 🛠️ Helper Classes
│       ├── Utilities.cs          # General utility functions
│       └── PieceSet.cs           # Piece collection management
│
├── ShatranjAI/                   # 🤖 AI Implementation
│   └── AI/
│       ├── BasicAI.cs            # Minimax with alpha-beta pruning (depth 3)
│       ├── MoveEvaluator.cs      # Position evaluation (material + piece-square tables)
│       └── IChessAI.cs           # AI interface forwarding to abstractions
│
├── ShatranjCMD/                  # 💻 Console Application
│   └── Program.cs                # Entry point with dependency injection setup
│
├── ShatranjMain/                 # 🖼️ WinForms GUI (legacy)
│   └── Program.cs
│
└── tests/                        # 🧪 Test Projects (70+ tests total)
    ├── Shatranj.Tests/           # xUnit piece movement tests (20+ tests)
    ├── ShatranjCore.Tests/       # xUnit core tests (40+ tests)
    │   ├── Logging/              # Logger implementation tests (6 tests)
    │   ├── UI/                   # CommandParser & UIRenderer tests (10+7 tests)
    │   ├── Movement/             # MoveHistory tests (8 tests)
    │   ├── Persistence/          # File I/O tests (8 tests)
    │   ├── Validators/           # Validator framework tests (11 tests)
    │   ├── PieceTests/           # Individual piece movement tests
    │   └── TestRunner.cs         # Main test orchestrator
    ├── ShatranjAI.Tests/         # xUnit AI tests (6+ tests)
    │   ├── BasicAITests.cs       # AI initialization and move selection
    │   ├── MoveEvaluatorTests.cs # Move evaluation tests
    │   ├── AIEnhancementTests.cs # AI enhancement framework (6 tests)
    │   └── TestRunner.cs         # Main test orchestrator
    └── ShatranjIntegration.Tests/ # Integration tests (6+ tests)
        ├── AIIntegrationTests.cs # AI in real game scenarios
        ├── GameFlowTests.cs      # Complete game flows
        └── TestRunner.cs         # Main test orchestrator
```

### Namespace Strategy

**Abstractions Namespace** (`ShatranjCore.Abstractions`)
- **Pure interfaces and types** - NO dependencies on any other project
- Core types: `Location`, `PieceColor`, `GameMode`, `PlayerType`, `PawnMoves`
- Interfaces: `IBoardState`, `IChessAI`, `ILogger`
- Purpose: Enable dependency inversion without circular references

**Root Namespace** (`ShatranjCore`)
- Contains `Models.cs` with type aliases for backward compatibility
- Forwards to types in `ShatranjCore.Abstractions`

**Module Namespaces** (Suffix pattern)
- `ShatranjCore.Pieces` - All piece classes
- `ShatranjCore.Board` - Board and square
- `ShatranjCore.Interfaces` - IChessBoard (extends IBoardState)
- `ShatranjCore.Game` - Game logic
- `ShatranjCore.Movement` - Move execution
- `ShatranjCore.Validators` - Validation rules
- `ShatranjCore.UI` - User interface
- `ShatranjCore.Handlers` - Special handlers
- `ShatranjCore.Logging` - Logging implementations
- `ShatranjCore.Persistence` - Save/load functionality
- `ShatranjCore.Learning` - Game recording
- `ShatranjCore.Utilities` - Utilities

**AI Namespace** (`ShatranjAI.AI`)
- AI implementations that depend only on Abstractions
- Can evaluate board state through IBoardState interface

**Benefits**:
- **No Circular Dependencies**: Abstractions layer breaks dependency cycles
- **Clear Organization**: Related classes are grouped together
- **Reduced Coupling**: Each module has minimal dependencies
- **Easy Testing**: Mock implementations through interfaces
- **Scalability**: New modules can be added without conflicts

---

## Abstractions Layer

### Purpose and Design

The **ShatranjCore.Abstractions** project is a critical architectural component that enables **dependency inversion** without creating circular dependencies.

```
┌─────────────────────────────────────────────────────────────┐
│          ShatranjCore.Abstractions                          │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  IBoardState (minimal board interface)             │   │
│  │  - Uses 'object' instead of 'Piece'                │   │
│  │  - Enables AI to work with board without           │   │
│  │    depending on ShatranjCore                       │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  IChessAI                                           │   │
│  │  - Depends on IBoardState (not IChessBoard)        │   │
│  │  - Can be implemented without circular deps        │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  Core Types (Location, PieceColor, GameMode)       │   │
│  │  - Shared across all projects                      │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  NO DEPENDENCIES - Pure interfaces and value types         │
└─────────────────────────────────────────────────────────────┘
                    ↑                    ↑
                    │                    │
        ┌───────────┴─────┐    ┌────────┴─────────┐
        │  ShatranjCore   │    │   ShatranjAI     │
        │  (implements)   │    │   (consumes)     │
        └─────────────────┘    └──────────────────┘
```

### IBoardState Interface

The key abstraction that breaks circular dependencies:

```csharp
namespace ShatranjCore.Abstractions
{
    /// <summary>
    /// Minimal board state interface for AI evaluation.
    /// Uses object for pieces to avoid circular dependencies.
    /// </summary>
    public interface IBoardState
    {
        object GetPiece(Location location);
        bool IsEmptyAt(int row, int column);
        List<object> GetPiecesOfColor(PieceColor color);
        List<object> GetOpponentPieces(PieceColor color);
        void PlacePiece(object piece, Location location);
        object RemovePiece(Location location);
        object FindKing(PieceColor color);
        bool IsInBounds(int row, int column);
        bool IsInBounds(Location location);
    }
}
```

**Why `object` instead of `Piece`?**
- `Piece` is defined in ShatranjCore
- If IBoardState used `Piece`, Abstractions would depend on ShatranjCore
- This would create a circular dependency: ShatranjCore → Abstractions → ShatranjCore ❌
- Using `object` keeps Abstractions independent ✅

### IChessBoard Extends IBoardState

The full board interface in ShatranjCore adds type safety:

```csharp
namespace ShatranjCore.Interfaces
{
    /// <summary>
    /// Full chess board interface with strongly-typed Piece methods.
    /// Extends IBoardState for AI compatibility.
    /// </summary>
    public interface IChessBoard : IBoardState
    {
        // Strongly-typed versions (hides IBoardState methods)
        new Piece GetPiece(Location location);
        new List<Piece> GetPiecesOfColor(PieceColor color);
        new List<Piece> GetOpponentPieces(PieceColor color);
        void PlacePiece(Piece piece, Location location);  // Different signature
        new Piece RemovePiece(Location location);
        new King FindKing(PieceColor color);
    }
}
```

**The `new` keyword:**
- Hides the base interface method with a more specific version
- When accessed as `IChessBoard` → returns `Piece`
- When accessed as `IBoardState` → returns `object`

### ChessBoard Implementation

ChessBoard implements both interfaces using **explicit interface implementation**:

```csharp
public class ChessBoard : IChessBoard
{
    // Public strongly-typed methods (for IChessBoard)
    public Piece GetPiece(Location location)
    {
        return squares[location.Row, location.Column].Piece;
    }

    public List<Piece> GetPiecesOfColor(PieceColor color)
    {
        int index = (color == PieceColor.Black) ? 0 : 1;
        return boardSet[index].Pieces.Where(p => p != null).ToList();
    }

    // Explicit IBoardState implementation (only accessible when cast to IBoardState)
    object IBoardState.GetPiece(Location location)
    {
        return GetPiece(location);  // Calls the public method
    }

    List<object> IBoardState.GetPiecesOfColor(PieceColor color)
    {
        return GetPiecesOfColor(color).Cast<object>().ToList();
    }

    // ... other explicit implementations
}
```

**How it works:**
1. **Normal usage** (game logic): Uses `IChessBoard` → gets `Piece` objects
2. **AI usage**: Uses `IBoardState` → gets `object` (cast internally to Piece)
3. **No circular dependency**: AI depends only on Abstractions, not ShatranjCore

### Dependency Flow

```
Clean dependency flow (no cycles):

ShatranjCore.Abstractions
    ↑               ↑
    │               │
    │               └─────── ShatranjAI (depends on Abstractions only)
    │                            ↑
    │                            │
    └─── ShatranjCore ───────────┘ (depends on both)
            ↑
            │
        ShatranjCMD (depends on all)
```

**Before Abstractions Layer:**
- ShatranjAI needed IChessBoard
- IChessBoard is in ShatranjCore
- Circular dependency: AI → Core → AI ❌

**After Abstractions Layer:**
- ShatranjAI needs only IBoardState
- IBoardState is in Abstractions (no dependencies)
- No circular dependency ✅

---

## Application and Domain Layers (Phase 3.0+)

### Architecture Evolution

The codebase has evolved through modularization phases:

**Phase 0-2**: Monolithic ChessGame class (1,279 lines)
- All game logic in one file
- Hard to test
- Hard to extend
- Coupling between concerns

**Phase 3.0+**: Extracted Application and Domain layers
- ChessGame reduced to 484 lines (62% reduction)
- Clear separation: Orchestration vs. Business Logic
- Single Responsibility Principle applied
- SOLID score: 9/10

### Application Layer Architecture

The **Application Layer** handles game flow orchestration:

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│                 (ShatranjCore.Application)                  │
│                                                             │
│  ┌────────────────────────────────────────────────────┐   │
│  │  GameOrchestrator                                  │   │
│  │  • Entry point for game startup                    │   │
│  │  • Initializes all components                      │   │
│  │  • Delegates to GameLoop                           │   │
│  └────────────────────────────────────────────────────┘   │
│                          │                                  │
│                          ↓                                  │
│  ┌────────────────────────────────────────────────────┐   │
│  │  GameLoop                                          │   │
│  │  • Main game loop execution                        │   │
│  │  • Turn-by-turn control flow                       │   │
│  │  • Check/checkmate detection per turn             │   │
│  │  • Delegates to components for specific actions    │   │
│  └────────────────────────────────────────────────────┘   │
│         │                        │                          │
│         ↓                        ↓                          │
│  ┌─────────────┐         ┌──────────────┐                 │
│  │ CommandProc │         │   AIHandler  │                 │
│  │ - Routes    │         │ - AI turn    │                 │
│  │ - Validates │         │ - Recording  │                 │
│  └─────────────┘         └──────────────┘                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│                 (ShatranjCore.Domain)                       │
│  • MoveExecutor: Execute moves on board                    │
│  • TurnManager: Manage player turns                        │
│  • CastlingExecutor: Execute castling moves                │
│  • PromotionRule: Pawn promotion logic                     │
└─────────────────────────────────────────────────────────────┘
```

### Application Layer Components

**GameOrchestrator.cs**
```csharp
public class GameOrchestrator : IGameOrchestrator
{
    // Entry point to start game
    public void StartGame(GameConfig config)
    {
        // Initialize board, players, validators, etc.
        // Start game loop
        gameLoop.ExecuteGameLoop(gameState);
    }
}
```

**GameLoop.cs**
```csharp
public class GameLoop : IGameLoop
{
    public void ExecuteGameLoop(GameState state)
    {
        while (!IsGameOver())
        {
            // Render current state
            renderer.Render(board);

            // Get player move (human or AI)
            Move move = GetPlayerMove();

            // Execute move via domain layer
            moveExecutor.ExecuteMove(move);

            // Check game state
            if (detector.IsCheckmate())
                EndGame("Checkmate");

            // Switch turns
            turnManager.SwitchTurns();
        }
    }
}
```

**CommandProcessor.cs**
```csharp
public class CommandProcessor : ICommandProcessor
{
    public void ProcessCommand(GameCommand command)
    {
        switch (command.Type)
        {
            case CommandType.Move:
                HandleMoveCommand((MoveCommand)command);
                break;
            case CommandType.Castle:
                HandleCastleCommand((CastleCommand)command);
                break;
            case CommandType.Save:
                HandleSaveCommand((SaveCommand)command);
                break;
            // ... etc
        }
    }
}
```

**AIHandler.cs**
```csharp
public class AIHandler : IAIHandler
{
    public Move SelectAIMove(PieceColor color, IChessBoard board)
    {
        // Get AI evaluation
        Move bestMove = ai.SelectMove(board, color);

        // Record for learning
        gameRecorder.RecordMove(bestMove, evaluation);

        return bestMove;
    }
}
```

### Domain Layer Architecture

The **Domain Layer** contains pure business logic:

```
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│                 (ShatranjCore.Domain)                       │
│                                                             │
│  ┌────────────────────────────────────────────────────┐   │
│  │  MoveExecutor                                      │   │
│  │  • Execute moves on board                          │   │
│  │  • Handle captures (normal and en passant)         │   │
│  │  • Trigger pawn promotion                          │   │
│  │  • Update board state                              │   │
│  │  • Record move in history                          │   │
│  └────────────────────────────────────────────────────┘   │
│                          │                                  │
│         ┌────────────────┼────────────────┐                │
│         ↓                ↓                ↓                 │
│   ┌───────────┐  ┌────────────┐  ┌──────────────┐         │
│   │ TurnMgr   │  │ CastlingEx │  │ PromotionRule│         │
│   │ Manage    │  │ Execute    │  │ Check/apply  │         │
│   │ turns     │  │ castling   │  │ promotion    │         │
│   └───────────┘  └────────────┘  └──────────────┘         │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Domain Layer Components

**MoveExecutor.cs** (Core business logic)
```csharp
public class MoveExecutor : IMoveExecutor
{
    public void ExecuteMove(Move move)
    {
        // 1. Validate move
        if (!IsValidMove(move)) return;

        // 2. Handle special cases
        if (move.IsEnPassant)
            HandleEnPassant(move);
        else if (move.Piece is Pawn && DestinationIsPromotionRank(move))
            HandlePromotion(move);

        // 3. Update board
        board.RemovePiece(move.From);
        board.PlacePiece(move.Piece, move.To);

        // 4. Record move
        moveHistory.RecordMove(move);
    }
}
```

**TurnManager.cs**
```csharp
public class TurnManager : ITurnManager
{
    private PieceColor currentPlayer = PieceColor.White;

    public void SwitchTurns()
    {
        currentPlayer = (currentPlayer == PieceColor.White)
            ? PieceColor.Black
            : PieceColor.White;
    }

    public PieceColor GetCurrentPlayer() => currentPlayer;
}
```

**CastlingExecutor.cs** (Phase 3.4 extracted)
```csharp
public class CastlingExecutor : ICastlingExecutor
{
    public void ExecuteCastling(PieceColor color, CastlingSide side)
    {
        // Move King
        King king = board.FindKing(color);
        Location kingTarget = GetKingTarget(color, side);
        board.PlacePiece(king, kingTarget);
        king.isMoved = true;

        // Move Rook
        Rook rook = GetRook(color, side);
        Location rookTarget = GetRookTarget(color, side);
        board.PlacePiece(rook, rookTarget);
        rook.isMoved = true;
    }
}
```

**PromotionRule.cs**
```csharp
public class PromotionRule : IPromotionRule
{
    public bool ShouldPromote(Move move)
    {
        if (!(move.Piece is Pawn)) return false;

        int targetRank = move.To.Location.Row;
        return (move.Piece.Color == PieceColor.White && targetRank == 0)
            || (move.Piece.Color == PieceColor.Black && targetRank == 7);
    }

    public void ApplyPromotion(Pawn pawn, PieceType newType)
    {
        // Replace pawn with new piece
        Piece promoted = pieceFactory.Create(newType, pawn.Color, pawn.location);
        board.PlacePiece(promoted, pawn.location);
    }
}
```

### Separation of Concerns

| Layer | Responsibility | Example Classes |
|-------|---|---|
| **Application** | Control flow, orchestration | GameOrchestrator, GameLoop, CommandProcessor, AIHandler |
| **Domain** | Business logic execution | MoveExecutor, TurnManager, CastlingExecutor, PromotionRule |
| **Validators** | Rule checking (not execution) | CheckDetector, CastlingValidator, EnPassantTracker |
| **Pieces** | Movement strategy per piece | Pawn, Rook, Bishop, Knight, Queen, King |
| **Board** | State management | ChessBoard, Square, Move |
| **UI** | User interaction | ConsoleBoardRenderer, CommandParser, ConsolePromotionUI |
| **Persistence** | Save/load functionality | SaveGameManager, GameSerializer, SnapshotManager |
| **Logging** | Logging infrastructure | ConsoleLogger, FileLogger, RollingFileLogger, etc. |

### Benefits of Modularization

1. **Testability**: Each component can be tested independently
2. **Maintainability**: Clear responsibilities make code easier to understand
3. **Extensibility**: New features add with minimal changes to existing code
4. **Reusability**: Components can be reused in other projects (AI, web UI, mobile)
5. **SOLID Compliance**: Follows all five SOLID principles

---

## Core Abstractions

### 1. Piece Hierarchy

```csharp
// Base abstraction for all chess pieces
namespace ShatranjCore.Pieces
{
    public abstract class Piece
    {
        // Properties
        public PieceColor Color { get; }
        public Location location;
        public bool isMoved;              // For castling/pawn first move
        public string Notation { get; }   // e.g., "wK" = white King

        // Core contract
        abstract public List<Move> GetMoves(Location source, IChessBoard board);
        abstract public bool CanMove(Location source, Location destination, IChessBoard board);
        abstract public bool IsBlockingCheck(Location source, IChessBoard board);

        // Legacy methods (to be removed)
        abstract public Square[] ValidMoves();
        abstract public bool IsCaptured();
    }
}
```

**Key Design Decisions:**

1. **Abstract Base Class (not interface)**
   - Shared state: `color`, `location`, `isMoved`, `notation`
   - Common logic can be added to base class
   - All pieces share the same contract

2. **GetMoves() returns List<Move>**
   - Contains all possible moves from a given position
   - Includes normal moves AND captures
   - Does NOT filter illegal moves (putting own King in check) yet

3. **CanMove() for quick validation**
   - Lightweight check: Can piece move from A to B?
   - Used for user input validation
   - Internally calls `GetMoves()` and searches for match

4. **isMoved flag**
   - Critical for castling (King and Rook must not have moved)
   - Important for pawn (2-square first move)
   - Set to true after first move

### 2. Board Abstraction (IBoardState + IChessBoard)

#### Two-Level Board Abstraction

The board is abstracted through two interfaces:

**Level 1: IBoardState (in Abstractions)**
```csharp
namespace ShatranjCore.Abstractions
{
    public interface IBoardState
    {
        // Type-agnostic methods (uses object)
        object GetPiece(Location location);
        bool IsEmptyAt(int row, int column);
        bool IsInBounds(int row, int column);
        List<object> GetPiecesOfColor(PieceColor color);
        List<object> GetOpponentPieces(PieceColor color);
        void PlacePiece(object piece, Location location);
        object RemovePiece(Location location);
        object FindKing(PieceColor color);
    }
}
```

**Level 2: IChessBoard (in ShatranjCore.Interfaces)**
```csharp
namespace ShatranjCore.Interfaces
{
    public interface IChessBoard : IBoardState
    {
        // Strongly-typed methods (uses Piece)
        new Piece GetPiece(Location location);
        new List<Piece> GetPiecesOfColor(PieceColor color);
        new List<Piece> GetOpponentPieces(PieceColor color);
        void PlacePiece(Piece piece, Location location);  // Different signature
        new Piece RemovePiece(Location location);
        new King FindKing(PieceColor color);
        // Inherits: IsEmptyAt, IsInBounds from IBoardState
    }
}
```

**Why two interfaces?**

1. **Break Circular Dependencies**
   - AI can depend on `IBoardState` (in Abstractions) without depending on ShatranjCore
   - Enables clean layered architecture

2. **Type Safety Where Needed**
   - Game logic uses `IChessBoard` → strongly-typed `Piece` objects
   - AI uses `IBoardState` → flexible `object` (cast internally)

3. **Dependency Inversion Principle**
   - High-level modules depend on abstractions
   - Future: Could implement `BitBoard`, `MailboxBoard`, etc.
   - Testability: Can create mock boards for testing

4. **Encapsulation**
   - Board implementation details hidden from consumers
   - Can change internal representation without affecting pieces or AI

5. **Flexibility**
   - Different board types for different scenarios:
     - `ChessBoard` - Standard 8x8 array (implements both interfaces)
     - `TestBoard` - Minimal board for unit tests
     - `BitBoard` - Future optimization using bitwise operations

### 3. Move Representation

```csharp
namespace ShatranjCore.Board
{
    public class Move
    {
        public Piece Piece { get; set; }
        public Square From { get; set; }
        public Square To { get; set; }
        public Piece CapturedPiece { get; set; }

        public Move(Piece piece, Square from, Square to, Piece capturedPiece)
        {
            Piece = piece;
            From = from;
            To = to;
            CapturedPiece = capturedPiece;
        }
    }
}
```

**Move contains:**
- **Piece**: What piece is moving
- **From**: Source square (contains piece, location)
- **To**: Destination square (may contain enemy piece)
- **CapturedPiece**: null for normal moves, enemy piece for captures

**Special Moves:**
- **Castling**: Represented as King move (Rook move happens in `MoveMaker`)
- **En Passant**: `CapturedPiece` is the pawn being captured (not at `To` square!)
- **Promotion**: Handled after move execution by `PawnPromotionHandler`

---

## Design Patterns

### 1. Strategy Pattern (Piece Movement)

Each piece implements its own movement strategy:

```csharp
// Each piece defines HOW it moves
class Rook : Piece {
    override List<Move> GetMoves(Location source, IChessBoard board) {
        // Rook-specific: horizontal + vertical
        List<Move> moves = new List<Move>();
        AddDirectionalMoves(moves, source, board, 0, 1);   // Right
        AddDirectionalMoves(moves, source, board, 0, -1);  // Left
        AddDirectionalMoves(moves, source, board, 1, 0);   // Down
        AddDirectionalMoves(moves, source, board, -1, 0);  // Up
        return moves;
    }
}

class Bishop : Piece {
    override List<Move> GetMoves(Location source, IChessBoard board) {
        // Bishop-specific: diagonals only
        List<Move> moves = new List<Move>();
        AddDirectionalMoves(moves, source, board, 1, 1);    // Down-Right
        AddDirectionalMoves(moves, source, board, 1, -1);   // Down-Left
        AddDirectionalMoves(moves, source, board, -1, 1);   // Up-Right
        AddDirectionalMoves(moves, source, board, -1, -1);  // Up-Left
        return moves;
    }
}
```

**Benefits:**
- Adding a new piece type (e.g., "Amazon" = Queen + Knight) requires only implementing `GetMoves()`
- Each piece's logic is isolated
- Easy to test individual piece behavior

### 2. Template Method (Game Flow)

`EnhancedChessGame` defines the game loop skeleton:

```csharp
public class EnhancedChessGame
{
    public void Start()
    {
        InitializeBoard();

        while (!IsGameOver())
        {
            // Template steps
            RenderBoard();
            PromptForMove();
            string input = GetPlayerInput();

            if (ValidateInput(input))
            {
                ExecuteMove(input);
                CheckGameState();
                SwitchTurns();
            }
            else
            {
                DisplayError();
            }
        }

        DisplayResult();
    }

    // Subclasses or extensions can override specific steps
    protected virtual void RenderBoard() { /* default implementation */ }
    protected virtual string GetPlayerInput() { /* default implementation */ }
}
```

### 3. Dependency Inversion (IChessBoard)

High-level classes depend on abstraction, not implementation:

```csharp
// ❌ BAD: Direct dependency on concrete class
class Knight : Piece {
    List<Move> GetMoves(Location source, ChessBoard board) { ... }
}

// ✅ GOOD: Dependency on abstraction
class Knight : Piece {
    List<Move> GetMoves(Location source, IChessBoard board) { ... }
}
```

**Benefits:**
- Knight doesn't care about board implementation details
- Can test Knight with a simple mock board
- Can swap board implementation without changing pieces

### 4. Single Responsibility (Specialized Validators)

Before refactoring, all validation was in one massive class:

```csharp
// ❌ BAD: One class doing everything (500+ lines)
class ChessGame {
    bool ValidateMove() { ... }
    bool IsInCheck() { ... }
    bool IsCheckmate() { ... }
    bool CanCastle() { ... }
    bool CanEnPassant() { ... }
    void PromotePawn() { ... }
    void RenderBoard() { ... }
}
```

After refactoring:

```csharp
// ✅ GOOD: Each class has ONE responsibility
class CheckDetector {
    bool IsInCheck(PieceColor kingColor, IChessBoard board) { ... }
    bool IsCheckmate(PieceColor kingColor, IChessBoard board) { ... }
    bool IsStalemate(PieceColor currentColor, IChessBoard board) { ... }
}

class CastlingValidator {
    bool CanCastleKingside(PieceColor color, IChessBoard board) { ... }
    bool CanCastleQueenside(PieceColor color, IChessBoard board) { ... }
}

class EnPassantTracker {
    Location? GetEnPassantTarget() { ... }
    void UpdateAfterMove(Move move) { ... }
}
```

**Result**: SOLID score improved from 6/10 → 9/10

---

## Data Flow

### Move Execution Flow

```
User Input: "e2-e4"
     │
     ↓
┌────────────────────────────────────────┐
│  CommandParser.ParseMove()             │
│  • Converts "e2" → Location(6, 4)      │
│  • Converts "e4" → Location(4, 4)      │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  EnhancedChessGame                     │
│  .ValidateAndExecuteMove()             │
└────────────────────────────────────────┘
     │
     ├──→ Is source square valid? ──────────→ No → Error
     │
     ├──→ Is there a piece at source? ──────→ No → Error
     │
     ├──→ Is it current player's piece? ────→ No → Error
     │
     ├──→ Can piece move to destination? ───→ No → Error
     │     (calls Piece.CanMove())
     │
     ├──→ Is it a special move?
     │     ├─→ Castling? → CastlingValidator
     │     ├─→ En Passant? → EnPassantTracker
     │     └─→ Promotion? → (handled after move)
     │
     ↓
┌────────────────────────────────────────┐
│  MoveMaker.MakeMove()                  │
│  • Updates board array                 │
│  • Captures enemy piece if applicable  │
│  • Sets piece.isMoved = true           │
│  • Handles special moves (castling)    │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  MoveHistory.RecordMove()              │
│  • Stores move for undo/PGN export     │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  Check for Pawn Promotion              │
│  If pawn reached rank 8/1:             │
│  • PawnPromotionHandler.PromotePawn()  │
│  • User chooses new piece (Q/R/B/N)    │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  CheckDetector.CheckGameState()        │
│  • Is opponent in check?               │
│  • Is opponent in checkmate? → GAME OVER│
│  • Is it stalemate? → GAME OVER (draw) │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  EnPassantTracker.UpdateAfterMove()    │
│  • Track if pawn moved 2 squares       │
│  • Set en passant target for next turn │
└────────────────────────────────────────┘
     │
     ↓
┌────────────────────────────────────────┐
│  Switch Turns                          │
│  • player1.HasTurn = !player1.HasTurn  │
│  • player2.HasTurn = !player2.HasTurn  │
└────────────────────────────────────────┘
     │
     ↓
   Loop continues...
```

### Check Detection Flow

```
CheckDetector.IsInCheck(PieceColor kingColor, IChessBoard board)
     │
     ↓
┌──────────────────────────────────────┐
│  1. Find King of given color         │
│     kingLocation = board.FindKing()  │
└──────────────────────────────────────┘
     │
     ↓
┌──────────────────────────────────────┐
│  2. Get all enemy pieces             │
│     enemies = board.GetAllPieces(    │
│         opponentColor)               │
└──────────────────────────────────────┘
     │
     ↓
┌──────────────────────────────────────┐
│  3. For each enemy piece:            │
│     • Get possible moves             │
│     • Check if any move targets King │
└──────────────────────────────────────┘
     │
     ├──→ If ANY enemy can capture King → ✅ CHECK
     │
     └──→ Otherwise → ❌ NOT IN CHECK
```

### Checkmate Detection Flow

```
CheckDetector.IsCheckmate(PieceColor kingColor, IChessBoard board)
     │
     ↓
┌──────────────────────────────────────┐
│  1. Is King in check?                │
└──────────────────────────────────────┘
     │
     ├──→ No → ❌ NOT CHECKMATE (can't be mate if not in check)
     │
     ↓
┌──────────────────────────────────────┐
│  2. Get all friendly pieces          │
└──────────────────────────────────────┘
     │
     ↓
┌──────────────────────────────────────┐
│  3. For each friendly piece:         │
│     • Get all possible moves         │
│     • For each move:                 │
│       - Simulate move on board       │
│       - Check if King still in check │
│       - Undo simulation              │
└──────────────────────────────────────┘
     │
     ├──→ If ANY move gets out of check → ❌ NOT CHECKMATE
     │
     └──→ If NO moves escape check → ✅ CHECKMATE
```

---

## Control Flow Diagrams

### Piece Movement Control Flow

Complete flow from user input to move execution:

```
┌─────────────────────────────────────────────────────────────────────┐
│                      PIECE MOVEMENT FLOW                            │
└─────────────────────────────────────────────────────────────────────┘

1. USER INPUT
   ┌─────────────────────────────────┐
   │ Player enters: "move e2 e4"     │
   └──────────────┬──────────────────┘
                  │
                  ↓
2. COMMAND PARSING (CommandParser)
   ┌──────────────────────────────────────┐
   │ Parse algebraic notation              │
   │ • "e2" → Location(6, 4)               │
   │ • "e4" → Location(4, 4)               │
   │ Creates GameCommand object            │
   │ Type: Move                            │
   └──────────────┬───────────────────────┘
                  │
                  ↓
3. COMMAND ROUTING (CommandProcessor)
   ┌──────────────────────────────────────┐
   │ Route command to handler              │
   │ HandlemoveCommand() called            │
   └──────────────┬───────────────────────┘
                  │
                  ↓
4. INITIAL VALIDATION
   ┌──────────────────────────────────────┐
   │ ✓ Source square on board?            │
   │ ✓ Destination square on board?       │
   │ ✓ Piece exists at source?            │
   │ ✓ Piece belongs to current player?   │
   └──────────────┬───────────────────────┘
                  │
           NO ←───┴───→ YES
           │             │
           ↓             ↓
       ERROR        ┌─────────────────────┐
       Display      │ Piece.CanMove()     │
       Message      │ (calls GetMoves())  │
           │        │ Check destination   │
           │        │ in possible moves   │
           └─→ ←───┘                      │
               │    │ Return true/false    │
               │    └─────────────────────┘
               │        │
         NO ←──┴────────→ YES
               │        │
               ↓        ↓
           ERROR    ┌────────────────────────┐
           Message  │ SPECIAL MOVE CHECK     │
               │    │                        │
               │    │ • Castling?            │
               │    │   → CastlingValidator  │
               │    │   → CastlingExecutor   │
               │    │                        │
               │    │ • En Passant?          │
               │    │   → EnPassantTracker   │
               │    │                        │
               │    │ • Pawn Promotion?     │
               │    │   (Check after move)   │
               │    └────────────┬───────────┘
               │                 │
               └────────────←────┘
                    │
                    ↓
5. MOVE EXECUTION (MoveExecutor)
   ┌──────────────────────────────────────┐
   │ • Check for en passant capture       │
   │ • Check for regular capture          │
   │ • Remove piece from source:          │
   │   board.RemovePiece(from)            │
   │ • Place piece at destination:        │
   │   board.PlacePiece(piece, to)        │
   │ • Mark piece as moved:               │
   │   piece.isMoved = true               │
   │ • Handle pawn promotion:             │
   │   - At rank 8/1? Create new piece    │
   │ • Create Move object                 │
   │ • Add to history                     │
   └──────────────┬───────────────────────┘
                  │
                  ↓
6. BOARD STATE UPDATE
   ┌──────────────────────────────────────┐
   │ Board rendered with new position     │
   │ Last move highlighted                │
   │ Captured pieces displayed            │
   └──────────────┬───────────────────────┘
                  │
                  ↓
7. GAME STATE CHECK (CheckDetector)
   ┌──────────────────────────────────────┐
   │ • Is opponent in checkmate?          │
   │   → Game Over (Current player wins)  │
   │ • Is opponent in stalemate?          │
   │   → Game Over (Draw)                 │
   │ • Is opponent in check?              │
   │   → Display "Check!" warning         │
   │ • Update en passant tracker:         │
   │   → Record pawn double moves         │
   └──────────────┬───────────────────────┘
                  │
                  ↓
8. TURN SWITCH (TurnManager)
   ┌──────────────────────────────────────┐
   │ currentPlayer = opposite color       │
   │ Update game state                    │
   │ Loop continues...                    │
   └──────────────────────────────────────┘

KEY DECISION POINTS:
─────────────────────
❌ Invalid square → Error message → Repeat input
❌ No piece → Error message → Repeat input
❌ Not player's piece → Error message → Repeat input
❌ Illegal move → Error message → Repeat input
✓ Valid move → Execute → Check special cases → Update board → Check game state → Switch turns
```

### Piece Capture Control Flow

Detailed flow for handling piece captures (including en passant):

```
┌──────────────────────────────────────────────────────────────────────┐
│                    PIECE CAPTURE FLOW                                │
└──────────────────────────────────────────────────────────────────────┘

CAPTURE DETECTION
┌──────────────────────────────────────────────────────────────────────┐
│ When executing move from (source) → (destination)                    │
└──────────────────────────────────────────────────────────────────────┘
                           │
                           ↓
        ┌──────────────────────────────────┐
        │ Piece at destination?            │
        │ AND different color?             │
        └──────────┬───────────────────────┘
                   │
           NO ←────┴────→ YES
           │        │
           ↓        ↓
   ┌──────────────┐  ┌─────────────────────────────────┐
   │ No capture   │  │ NORMAL CAPTURE (99% of cases)  │
   │              │  │                                 │
   │ Empty square │  │ capturedPiece = board.GetPiece  │
   │ Empty/no cap │  │     (destination)               │
   │              │  │ board.RemovePiece(destination)  │
   │              │  │ capturedPieces.Add(captured)    │
   │              │  │                                 │
   │              │  │ Display: "{Piece} captures      │
   │              │  │           {CapturedPiece}!"     │
   │              │  └────────────┬────────────────────┘
   │              │               │
   │              │               ↓
   │              │     ┌─────────────────────────────┐
   │              │     │ SPECIAL: EN PASSANT?        │
   │              │     │                             │
   │              │     │ Destination empty BUT       │
   │              │     │ en passant target matches?  │
   │              │     │                             │
   │              │     │ YES ↓                       │
   │              │     │ Get actual capture location:│
   │              │     │ capturePos = enPassant      │
   │              │     │   .GetCaptureLocation()    │
   │              │     │ (Different from dest!)      │
   │              │     │                             │
   │              │     │ capturedPawn = board        │
   │              │     │   .GetPiece(capturePos)    │
   │              │     │ board.RemovePiece(        │
   │              │     │   capturePos)              │
   │              │     │ capturedPieces.Add(pawn)    │
   │              │     │                             │
   │              │     │ Display: "Pawn captures     │
   │              │     │   Pawn en passant!"         │
   │              │     └────────────┬────────────────┘
   │              │                  │
   └──────────────┴──────────────────┘
                  │
                  ↓
     ┌────────────────────────────────┐
     │ Move piece to destination       │
     │ board.RemovePiece(source)       │
     │ board.PlacePiece(piece, dest)   │
     │ piece.isMoved = true            │
     └────────────┬───────────────────┘
                  │
                  ↓
    ┌─────────────────────────────────┐
    │ RECORD CAPTURE IN HISTORY       │
    │                                 │
    │ moveHistory.RecordMove(         │
    │   move,                         │
    │   wasCapture: true,             │
    │   capturedPiece: piece)         │
    │                                 │
    │ Used for:                       │
    │ • Undo/Redo functionality       │
    │ • PGN notation (pgn shows 'x')  │
    │ • Material count (AI eval)      │
    │ • Game analysis                 │
    └────────────┬───────────────────┘
                 │
                 ↓
┌───────────────────────────────────────────────┐
│ UPDATE CAPTURED PIECES LIST                   │
│                                               │
│ Displayed at end of each turn:                │
│ White captured: ♟ ♟ ♞                        │
│ Black captured: ♟ ♞ ♗                        │
└───────────────────────────────────────────────┘

EN PASSANT SPECIAL CASE (1% of captures)
──────────────────────────────────────────

Position before move:
  5: . ♟ . .    (Black pawn at e5)
  4: . . . .
  3: . ♙ . .    (White pawn at d3)

Move: e3-e4 (White pawn moves forward)

Position after move:
  5: . . . .
  4: . ♙ . .    (White pawn now at e4)
  3: . . . .

But wait! En passant next turn:
  5: . ♟ . .    (Still at e5)
  4: . ♙ . .

Black can now play: d5-d4 (diagonal) and CAPTURE the white pawn at e4!

Why? The white pawn moved 2 squares, "jumping over" the black pawn.

En passant detection:
  1. Move is diagonal by pawn
  2. Destination empty (not normal capture)
  3. EnPassantTracker.HasTarget() == true
  4. Destination matches en passant target
  → EN PASSANT CAPTURE!

Key difference: Captured piece at (e4), destination (e5)!
```

### Class Structure Diagram

Overall class relationships and dependencies:

```
┌────────────────────────────────────────────────────────────────────────┐
│                      CLASS STRUCTURE & DEPENDENCIES                    │
└────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                      ABSTRACTIONS LAYER (No Dependencies)              │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────┐               │
│  │   IBoardState │  │  IChessAI    │  │  ILogger     │               │
│  └───────────────┘  └──────────────┘  └──────────────┘               │
│         ↑                   ↑                   ↑                       │
│         │ Used by          │ Used by          │ Used by                │
│         └─ ShatranjCore    └─ ShatranjAI     └─ All modules           │
│           AI                                                           │
│  ┌─────────────────────────────────────────────────────────┐         │
│  │  30+ Interfaces for Dependency Inversion               │         │
│  │  • IGameLoop, IGameOrchestrator, ICommandProcessor     │         │
│  │  • IMoveExecutor, ITurnManager, IAIHandler             │         │
│  │  • ICastlingValidator, ICastlingExecutor               │         │
│  │  • ICheckDetector, IEnPassantTracker                   │         │
│  │  • IMoveHistory, ISaveGameManager                      │         │
│  │  • ISnapshotManager, ISettingsManager                  │         │
│  │  • IGameRecorder, IRenderer                            │         │
│  └─────────────────────────────────────────────────────────┘         │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                              ↑ Depends on
                              │
┌─────────────────────────────────────────────────────────────────────────┐
│                      SHATRANJ CORE (ShatranjCore)                      │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  APPLICATION LAYER                                     │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌──────────────────────┐   ┌──────────────────┐    │          │
│  │  │ GameOrchestrator     │   │ GameLoop         │    │          │
│  │  │ (Start game)         │──→│ (Main loop)      │    │          │
│  │  │ [IGameOrchestrator]  │   │ [IGameLoop]      │    │          │
│  │  └──────────────────────┘   └────────┬─────────┘    │          │
│  │                                       │               │          │
│  │         ┌─────────────────────────────┼──────┐       │          │
│  │         ↓                             ↓      ↓       │          │
│  │  ┌─────────────────┐   ┌──────────────────────┐     │          │
│  │  │CommandProcessor │   │    AIHandler         │     │          │
│  │  │(Route commands) │   │ (AI move selection)  │     │          │
│  │  │[ICommandProc]   │   │ [IAIHandler]         │     │          │
│  │  └─────────────────┘   └──────────────────────┘     │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  DOMAIN LAYER                                          │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌──────────────┐  ┌──────────────┐  ┌────────────┐ │          │
│  │  │ MoveExecutor │  │ TurnManager  │  │CastlingExec│ │          │
│  │  │(Execute      │  │(Manage turns)│  │(Execute    │ │          │
│  │  │ moves)       │  │              │  │ castling)  │ │          │
│  │  │[IMoveExec]   │  │[ITurnManager]│  │[ICastling] │ │          │
│  │  └──────┬───────┘  └──────────────┘  └────────────┘ │          │
│  │         │                                             │          │
│  │         └──→ ┌────────────────┐                       │          │
│  │              │ PromotionRule  │                       │          │
│  │              │(Promotion logic)                       │          │
│  │              │[IPromotionRule]│                       │          │
│  │              └────────────────┘                       │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  VALIDATORS                                            │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌─────────────────┐  ┌──────────────┐ ┌───────────┐│          │
│  │  │ CheckDetector   │  │CastlingValid │ │EnPassant  ││          │
│  │  │(Check/mate/     │  │(Castling     │ │Tracker    ││          │
│  │  │ stalemate)      │  │ rules)       │ │(En passant││          │
│  │  │[ICheckDetector] │  │[ICastlingVal]│ │tracking)  ││          │
│  │  └─────────────────┘  └──────────────┘ │[IEnPassant││          │
│  │                                          └───────────┘│          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  PIECES (Strategy Pattern)                             │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │      ┌──────────────────┐                             │          │
│  │      │ Piece (Abstract) │                             │          │
│  │      ├──────────────────┤                             │          │
│  │      │ - color          │                             │          │
│  │      │ - location       │                             │          │
│  │      │ - isMoved        │                             │          │
│  │      │ + GetMoves()     │ ← Abstract method (each     │          │
│  │      │ + CanMove()      │   piece implements!)        │          │
│  │      │ + IsBlockingCheck│                             │          │
│  │      └─────┬────────────┘                             │          │
│  │            │                                          │          │
│  │  ┌─────────┴────────────┬─────────┬─────────┐        │          │
│  │  ↓          ↓            ↓         ↓         ↓        │          │
│  │ Pawn    Rook         Bishop    Knight    Queen King  │          │
│  │ (Complex)(Linear)   (Diagonal)(Fixed)  (Combined)   │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  BOARD & STATE                                         │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌──────────────────┐     ┌──────────────────┐       │          │
│  │  │ChessBoard        │     │Move              │       │          │
│  │  │(8x8 board)       │     │(Piece, from,     │       │          │
│  │  │Implements:       │     │ to, captured)    │       │          │
│  │  │ - IChessBoard    │     │                  │       │          │
│  │  │ - IBoardState    │     └──────────────────┘       │          │
│  │  │                  │                                 │          │
│  │  │Contains: Square[]│                                 │          │
│  │  │                  │                                 │          │
│  │  └──────────────────┘                                 │          │
│  │                                                        │          │
│  │  ┌──────────────────┐                                 │          │
│  │  │MoveHistory       │                                 │          │
│  │  │(Track all moves) │                                 │          │
│  │  │[IMoveHistory]    │                                 │          │
│  │  └──────────────────┘                                 │          │
│  │                                                        │          │
│  │  ┌──────────────────┐                                 │          │
│  │  │SnapshotManager   │                                 │          │
│  │  │(Game state       │                                 │          │
│  │  │ snapshots)       │                                 │          │
│  │  │[ISnapshotManager]│                                 │          │
│  │  └──────────────────┘                                 │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  UI LAYER                                              │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌────────────────────┐  ┌────────────────────┐      │          │
│  │  │ConsoleBoardRenderer│  │CommandParser       │      │          │
│  │  │(Render board)      │  │(Parse input)       │      │          │
│  │  │[IRenderer]         │  │[ICommandParser]    │      │          │
│  │  └────────────────────┘  └────────────────────┘      │          │
│  │                                                        │          │
│  │  ┌────────────────────┐  ┌────────────────────┐      │          │
│  │  │ConsolePromotionUI  │  │ConsoleMoveHistory  │      │          │
│  │  │(Promotion dialog)  │  │Renderer            │      │          │
│  │  │[IPromotionUI]      │  │(Display history)   │      │          │
│  │  └────────────────────┘  └────────────────────┘      │          │
│  │                                                        │          │
│  │  ┌────────────────────┐                              │          │
│  │  │GameMenuHandler     │                              │          │
│  │  │(Main menu)         │                              │          │
│  │  │[IMenuHandler]      │                              │          │
│  │  └────────────────────┘                              │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  PERSISTENCE                                           │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌────────────────────────────────────────────┐      │          │
│  │  │SaveGameManager (Save/load games)           │      │          │
│  │  │GameSerializer (Serialize state)            │      │          │
│  │  │GameStateSnapshot (Serializable state)      │      │          │
│  │  │[ISaveGameManager, IGameSerializer]         │      │          │
│  │  └────────────────────────────────────────────┘      │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  LOGGING                                               │          │
│  ├────────────────────────────────────────────────────────┤          │
│  │                                                        │          │
│  │  ┌──────────────────────────────────────────┐         │          │
│  │  │ConsoleLogger                             │         │          │
│  │  │FileLogger                                │         │          │
│  │  │RollingFileLogger                         │         │          │
│  │  │ErrorTraceLogger                          │         │          │
│  │  │CompositeLogger                           │         │          │
│  │  │(Implements ILogger)                      │         │          │
│  │  └──────────────────────────────────────────┘         │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
                              ↑ Depends on
                              │
┌─────────────────────────────────────────────────────────────────────────┐
│                      SHATRANJ AI (ShatranjAI)                           │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌────────────────────────────────────────────────────────┐          │
│  │  ┌──────────────────┐  ┌──────────────────┐           │          │
│  │  │BasicAI           │  │MoveEvaluator     │           │          │
│  │  │(Minimax + alpha- │  │(Position eval)   │           │          │
│  │  │ beta pruning)    │  │(Material count)  │           │          │
│  │  │Implements:       │  │(Piece-square     │           │          │
│  │  │ IChessAI         │  │ tables)          │           │          │
│  │  │Uses: IBoardState │  │                  │           │          │
│  │  │(NOT IChessBoard!)│  └──────────────────┘           │          │
│  │  └──────────────────┘                                 │          │
│  │                                                        │          │
│  └────────────────────────────────────────────────────────┘          │
│                                                                         │
│  NOTE: AI depends ONLY on Abstractions                                │
│        NOT on ShatranjCore concrete classes                           │
│        This breaks circular dependencies!                             │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

KEY ARCHITECTURAL PRINCIPLES:
──────────────────────────────

1. DEPENDENCY INVERSION
   ✓ All components depend on interfaces, not implementations
   ✓ Swappable implementations (MockBoard for testing, etc.)

2. ABSTRACTION LAYER BREAKS CIRCULAR DEPENDENCIES
   ✓ AI uses IBoardState (in Abstractions) not IChessBoard (in Core)
   ✓ No circular: AI → Abstractions → Core is safe

3. SEPARATION OF CONCERNS
   ✓ Application: Flow control
   ✓ Domain: Business logic
   ✓ Validators: Rule checking
   ✓ Pieces: Movement strategies
   ✓ Board: State management
   ✓ UI: User interaction
   ✓ Persistence: Save/load
   ✓ Logging: Infrastructure

4. SINGLE RESPONSIBILITY
   ✓ Each class has ONE reason to change
   ✓ 35+ focused classes (vs 1 monolithic class)

5. TESTABILITY
   ✓ Each component tested independently
   ✓ Interfaces enable mocking
   ✓ 70+ tests, 100% passing
```

---

## Key Algorithms

### 1. Directional Move Generation (Rook, Bishop, Queen)

```csharp
private void AddDirectionalMoves(
    List<Move> moves,
    Location source,
    IChessBoard board,
    int rowDelta,    // -1, 0, or 1
    int colDelta)    // -1, 0, or 1
{
    int currentRow = source.Row + rowDelta;
    int currentCol = source.Column + colDelta;

    // Keep moving in direction until blocked
    while (board.IsInBounds(currentRow, currentCol))
    {
        Piece targetPiece = board.GetPiece(new Location(currentRow, currentCol));

        if (targetPiece == null)
        {
            // Empty square - can move here
            moves.Add(CreateMove(source, currentRow, currentCol, null));
        }
        else if (targetPiece.Color != this.Color)
        {
            // Enemy piece - can capture
            moves.Add(CreateMove(source, currentRow, currentCol, targetPiece));
            break; // Can't continue past capture
        }
        else
        {
            // Friendly piece - blocked
            break;
        }

        // Continue in same direction
        currentRow += rowDelta;
        currentCol += colDelta;
    }
}
```

**Usage:**
- **Rook**: Calls 4 times (up, down, left, right)
- **Bishop**: Calls 4 times (4 diagonals)
- **Queen**: Calls 8 times (Rook + Bishop directions)

### 2. Knight Move Generation (Fixed Offsets)

```csharp
override List<Move> GetMoves(Location source, IChessBoard board)
{
    List<Move> moves = new List<Move>();

    // All 8 possible L-shaped moves
    int[,] offsets = {
        {-2, -1}, {-2, 1},  // 2 up, 1 left/right
        {-1, -2}, {-1, 2},  // 1 up, 2 left/right
        {1, -2},  {1, 2},   // 1 down, 2 left/right
        {2, -1},  {2, 1}    // 2 down, 1 left/right
    };

    for (int i = 0; i < 8; i++)
    {
        int newRow = source.Row + offsets[i, 0];
        int newCol = source.Column + offsets[i, 1];

        if (board.IsInBounds(newRow, newCol))
        {
            Piece targetPiece = board.GetPiece(new Location(newRow, newCol));

            // Can move if empty or enemy piece
            if (targetPiece == null || targetPiece.Color != this.Color)
            {
                moves.Add(CreateMove(source, newRow, newCol, targetPiece));
            }
        }
    }

    return moves;
}
```

**Why this works:**
- Knight has exactly 8 possible destinations (if on empty board in center)
- No need for loops - just check each offset once
- Knight is the ONLY piece that can jump over others

### 3. Castling Validation

```csharp
bool CanCastleKingside(PieceColor color, IChessBoard board)
{
    // 1. Find King and Rook
    Location kingLocation = board.FindKing(color);
    Location rookLocation = new Location(kingLocation.Row, 7); // h-file

    King king = board.GetPiece(kingLocation) as King;
    Rook rook = board.GetPiece(rookLocation) as Rook;

    // 2. Both must exist and not have moved
    if (king == null || rook == null || king.isMoved || rook.isMoved)
        return false;

    // 3. Check squares between are empty
    for (int col = kingLocation.Column + 1; col < rookLocation.Column; col++)
    {
        if (!board.IsEmptyAt(kingLocation.Row, col))
            return false;
    }

    // 4. King is not in check
    if (CheckDetector.IsInCheck(color, board))
        return false;

    // 5. King doesn't pass through check
    Location middleSquare = new Location(kingLocation.Row, kingLocation.Column + 1);
    // Simulate King on middle square
    board.PlacePiece(king, middleSquare);
    bool passesThruCheck = CheckDetector.IsInCheck(color, board);
    board.PlacePiece(king, kingLocation); // Restore
    if (passesThruCheck)
        return false;

    // 6. King doesn't land in check
    Location finalSquare = new Location(kingLocation.Row, kingLocation.Column + 2);
    board.PlacePiece(king, finalSquare);
    bool landsInCheck = CheckDetector.IsInCheck(color, board);
    board.PlacePiece(king, kingLocation); // Restore
    if (landsInCheck)
        return false;

    // All conditions met!
    return true;
}
```

**6 Conditions for Legal Castling:**
1. King has not moved (`king.isMoved == false`)
2. Rook has not moved (`rook.isMoved == false`)
3. No pieces between King and Rook
4. King is not currently in check
5. King does not pass through check
6. King does not land in check

---

## Extensibility

### Adding a New Piece Type

To add a new piece (e.g., "Archbishop" = Bishop + Knight):

1. **Create new class in `Pieces/` folder:**
```csharp
namespace ShatranjCore.Pieces
{
    public class Archbishop : Piece
    {
        public Archbishop(int row, int column, PieceColor pieceColor)
            : base(row, column, pieceColor) { }

        override public List<Move> GetMoves(Location source, IChessBoard board)
        {
            List<Move> moves = new List<Move>();

            // Combine Bishop moves
            AddDiagonalMoves(moves, source, board);

            // Combine Knight moves
            AddKnightMoves(moves, source, board);

            return moves;
        }

        override public bool CanMove(Location source, Location destination, IChessBoard board)
        {
            List<Move> validMoves = GetMoves(source, board);
            return validMoves.Any(m =>
                m.To.Location.Row == destination.Row &&
                m.To.Location.Column == destination.Column);
        }

        // ... implement other abstract methods
    }
}
```

2. **Update `ChessBoard` initialization** (if standard piece)
3. **Add unit tests** in `tests/ShatranjCore.Tests/PieceTests/ArchbishopTests.cs`
4. **Done!** No changes needed to validators, UI, or game logic.

### Adding a New Validator

To add a new rule (e.g., "Fifty-Move Rule"):

1. **Create new class in `Validators/` folder:**
```csharp
namespace ShatranjCore.Validators
{
    public class FiftyMoveRuleDetector
    {
        private int movesSinceLastCaptureOrPawn = 0;

        public void UpdateAfterMove(Move move)
        {
            // Reset counter if capture or pawn move
            if (move.CapturedPiece != null || move.Piece is Pawn)
                movesSinceLastCaptureOrPawn = 0;
            else
                movesSinceLastCaptureOrPawn++;
        }

        public bool IsFiftyMoveRule()
        {
            return movesSinceLastCaptureOrPawn >= 50;
        }
    }
}
```

2. **Integrate into `EnhancedChessGame`:**
```csharp
class EnhancedChessGame
{
    private FiftyMoveRuleDetector fiftyMoveDetector = new FiftyMoveRuleDetector();

    private void CheckGameState()
    {
        // Existing checks...

        // New check
        if (fiftyMoveDetector.IsFiftyMoveRule())
        {
            EndGame("Draw by fifty-move rule");
        }
    }
}
```

3. **Done!** Clean, isolated addition following SRP.

### Adding a New UI

To add a web UI (Phase 3):

1. **ShatranjCore remains unchanged** (pure domain logic)
2. **Create new project:** `ShatranjWeb`
3. **Reference ShatranjCore:** `<ProjectReference Include="..\ShatranjCore\ShatranjCore.csproj" />`
4. **Use existing classes:**
```csharp
// ASP.NET Core Controller
public class ChessController : Controller
{
    private EnhancedChessGame game;

    [HttpPost("move")]
    public IActionResult MakeMove(string from, string to)
    {
        // Reuse existing game logic
        bool success = game.ValidateAndExecuteMove(from, to);

        if (success)
            return Ok(new { board = game.GetBoardState() });
        else
            return BadRequest("Invalid move");
    }
}
```

**No changes to core library needed!** This is the power of good architecture.

---

## Performance Considerations

### Current Optimizations
- **2D Array Board**: Fast O(1) access to any square
- **Early Exit in Move Generation**: Stop when blocked
- **Move Validation Cache**: (future) Cache legal moves per turn

### Future Optimizations (Phase 2+)
1. **Bitboard Representation**: Use 64-bit integers for board state (dramatic speed increase)
2. **Move Generation Optimization**: Pre-calculate attack tables
3. **Alpha-Beta Pruning**: For AI move search (reduces search tree by ~90%)
4. **Transposition Table**: Cache evaluated positions
5. **Opening Book**: Pre-computed optimal openings

---

## Testing Strategy

### Three-Tier Test Architecture

The project follows a comprehensive testing pyramid with three distinct test projects:

```
                    ┌──────────────────────────┐
                    │   Integration Tests      │  (6 tests)
                    │   Full game scenarios    │
                    └──────────────────────────┘
                            ↑
                ┌───────────┴───────────┐
                │      AI Tests         │  (6 tests)
                │   AI-specific tests   │
                └───────────────────────┘
                        ↑
            ┌───────────┴───────────┐
            │    Core Tests         │  (40+ tests)
            │  Foundation tests     │
            └───────────────────────┘
```

### 1. ShatranjCore.Tests (40+ tests)

**Purpose:** Unit tests for core game logic

**Test Coverage:**
- **Piece Movement** (28 tests): Each piece has 5-9 tests
  - Pawn: Basic moves, double move, captures, en passant
  - Rook: Horizontal/vertical movement, blocking
  - Bishop: Diagonal movement, blocking
  - Knight: L-shaped moves, jumping
  - Queen: Combined movement
  - King: Single square movement, check detection
- **Edge Cases**: Board boundaries, blocking, captures
- **Special Moves**: Castling (6 tests), en passant (2 tests), promotion (2 tests)
- **Check Detection** (4 tests): Check, checkmate, stalemate
- **Move Validation** (6+ tests): Legal move filtering

**Run Command:**
```bash
cd tests/ShatranjCore.Tests
dotnet run
```

### 2. ShatranjAI.Tests (6 tests)

**Purpose:** Unit tests for AI components

**Test Coverage:**
- **BasicAI Tests** (3 tests):
  - AI can select valid moves
  - AI makes legal moves
  - AI respects game rules
- **MoveEvaluator Tests** (3 tests):
  - Position evaluation accuracy
  - Material counting
  - Piece-square table application

**Run Command:**
```bash
cd ShatranjAI.Tests
dotnet run
```

### 3. ShatranjIntegration.Tests (6 tests)

**Purpose:** Integration tests for complete game scenarios

**Test Coverage:**
- **AI Integration** (3 tests):
  - AI can play a full game
  - AI makes only valid moves throughout game
  - AI logging integration works
- **Game Flow** (3 tests):
  - Check detection in real game
  - Castling in real game
  - En passant in real game

**Run Command:**
```bash
cd tests/ShatranjIntegration.Tests
dotnet run
```

### Test Isolation and Dependencies

**Dependency Graph:**
```
ShatranjIntegration.Tests
    ├── → ShatranjCore.Abstractions
    ├── → ShatranjCore
    └── → ShatranjAI

ShatranjAI.Tests
    ├── → ShatranjCore.Abstractions
    └── → ShatranjAI

ShatranjCore.Tests
    ├── → ShatranjCore.Abstractions
    └── → ShatranjCore
```

**Benefits:**
- **Clear separation**: Unit tests don't depend on AI, AI tests don't depend on integration
- **Fast feedback**: Core tests run quickly without AI overhead
- **Comprehensive coverage**: Integration tests verify complete scenarios
- **Easy debugging**: Failures isolated to specific layers

### Performance Tests (Future - Phase 3)
- Benchmark move generation (target: <1ms for full board)
- AI search depth (target: depth 6 in <3 seconds)
- Position evaluation speed (target: 100k positions/second)
- Memory profiling for long games

---

## Conclusion

Shatranj's architecture prioritizes:
1. **SOLID Principles** → Maintainable, testable code
2. **Modular Design** → Easy to extend and modify
3. **Clear Abstractions** → Loose coupling, high cohesion
4. **Separation of Concerns** → Each class has one job
5. **Future-Proof** → Ready for AI, web, mobile phases

This architecture allows the project to grow from a simple command-line game to a full-featured chess platform without major refactoring.

---

**Last Updated**: November 2025
**Maintained by**: Mohammed Azmat
**Architecture Status**: Phase 2 - AI Integration (100% Complete) ✅
**Key Achievement**: Clean layered architecture with zero circular dependencies through Abstractions layer
