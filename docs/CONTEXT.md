# Shatranj - Complete Project Context

> **Last Updated**: November 10, 2025
> **Version**: Phase 2 - Complete (100%) ✅
> **Purpose**: Comprehensive reference for all project files and their responsibilities

---

## 📝 Documentation Conventions

**IMPORTANT**: All new markdown documentation files must be created in the `docs/` folder, not the root directory.

**File Organization:**
- `/docs/` - All documentation files (`.md`)
- `/docs/archive/` - Archived/historical documentation
- Root directory - Only keep: `README.md` (main project readme)

**Examples:**
- ✅ `/docs/PHASE_3_ROADMAP.md` (Correct)
- ✅ `/docs/MODULARIZATION_PLAN.md` (Correct)
- ✅ `/docs/archive/PHASE_2_SAVE_LOAD_SYSTEM.md` (Archived)
- ❌ `/PHASE_3_ROADMAP.md` (Incorrect - not in docs/)

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Solution Structure](#solution-structure)
3. [File-by-File Documentation](#file-by-file-documentation)
4. [Key Architectural Patterns](#key-architectural-patterns)
5. [Quick Reference](#quick-reference)

---

## Project Overview

**Shatranj** is a fully-featured chess game built with SOLID principles, featuring:
- Complete chess rules implementation (castling, en passant, promotion)
- AI opponent using minimax with alpha-beta pruning
- Clean layered architecture with zero circular dependencies
- Comprehensive test coverage (50+ tests)
- Cross-platform terminal-based UI

**Tech Stack**: .NET 9.0, C#, Console Application

**Development Status**: Phase 2 Complete (100%) ✅ - AI Integration

---

## Solution Structure

```
Shatranj.sln
├── ShatranjCore.Abstractions/    # Core abstractions (NO DEPENDENCIES)
├── ShatranjCore/                  # Core game engine
├── ShatranjAI/                    # AI implementation
├── ShatranjCMD/                   # Console application entry point
├── ShatranjMain/                  # Windows Forms GUI (legacy)
└── tests/
    ├── ShatranjCore.Tests/        # Core unit tests (40+ tests)
    ├── ShatranjAI.Tests/          # AI unit tests (6 tests)
    └── ShatranjIntegration.Tests/ # Integration tests (6 tests)
```

**Dependency Flow**:
```
ShatranjCore.Abstractions (no dependencies)
    ↑               ↑
    │               └─────── ShatranjAI
    │                            ↑
    └─── ShatranjCore ───────────┘
            ↑
        ShatranjCMD
```

---

## File-by-File Documentation

### 📦 Root Files

#### `Shatranj.sln`
- **Type**: Solution file
- **Purpose**: Visual Studio/Rider solution configuration
- **Contains**: All project references and build configurations

#### `README.md`
- **Type**: Documentation
- **Purpose**: Quick start guide, features, commands reference
- **Audience**: End users and developers getting started
- **Key Sections**: Installation, gameplay commands, testing instructions

#### `ROADMAP.md`
- **Type**: Documentation
- **Purpose**: Complete 6-phase development plan
- **Content**:
  - Phase 1: Core Implementation (✅ Complete)
  - Phase 2: AI Integration (85% Complete)
  - Phase 3-6: Future features (Enhanced AI, Multiplayer, GUI, Polish)

#### `BUILD.md`
- **Type**: Documentation
- **Purpose**: Build instructions, troubleshooting, platform support
- **Content**: Commands for building, running, testing, publishing

#### `DOTNET9_UPGRADE.md`
- **Type**: Documentation
- **Purpose**: Migration guide from .NET 8 to .NET 9

#### `CONTEXT.md` (this file)
- **Type**: Documentation
- **Purpose**: Complete project file reference for quick onboarding

---

### 🔷 ShatranjCore.Abstractions/ - Core Abstractions

**Purpose**: Pure interfaces and types with NO dependencies. Breaks circular dependencies.

**Project File**: `ShatranjCore.Abstractions.csproj`
- Target: .NET 9.0
- Dependencies: None
- Output: Class library

#### `CoreTypes.cs`
- **Purpose**: Fundamental types used across all projects
- **Exports**:
  - `Location` struct - Board position (row, column)
  - `PieceColor` enum - White, Black
  - `PawnMoves` enum - Up, Down (movement direction)
  - `GameMode` enum - HumanVsHuman, HumanVsAI, AIvsAI
  - `PlayerType` enum - Human, AI
  - `AIMove` class - AI move selection result
- **Why Here**: Shared by all projects, no dependencies

#### `IBoardState.cs`
- **Purpose**: Minimal board interface for AI (uses `object` instead of `Piece`)
- **Methods**:
  - `GetPiece(Location)` → object
  - `GetPiecesOfColor(PieceColor)` → List\<object\>
  - `PlacePiece(object, Location)`
  - `FindKing(PieceColor)` → object
- **Why `object`**: Avoids circular dependency (Piece is in ShatranjCore)
- **Used By**: IChessAI interface

#### `IChessAI.cs`
- **Purpose**: AI contract that all AI implementations must follow
- **Methods**:
  - `SelectMove(IBoardState, PieceColor, Location?)` → AIMove
  - `EvaluatePosition(IBoardState, PieceColor)` → double
- **Properties**: Name, Version, Depth
- **Depends On**: IBoardState (not IChessBoard - no circular dependency!)

#### `ILogger.cs`
- **Purpose**: Logging abstraction
- **Methods**: Info(), Warning(), Error()
- **Implementations**: ConsoleLogger, FileLogger, CompositeLogger

---

### 🎮 ShatranjCore/ - Core Game Engine

**Purpose**: Complete chess game logic, rules, UI, and infrastructure

**Project File**: `ShatranjCore.csproj`
- Target: .NET 9.0
- Dependencies: ShatranjCore.Abstractions
- Output: Class library

#### Root Files

##### `Models.cs`
- **Purpose**: Type aliases for backward compatibility
- **Content**: Forwards types to ShatranjCore.Abstractions
- **Example**: `using PieceColor = ShatranjCore.Abstractions.PieceColor;`

---

#### 📋 Interfaces/

##### `IChessBoard.cs`
- **Purpose**: Full chess board interface with strongly-typed Piece methods
- **Extends**: IBoardState
- **Methods** (override with `new`):
  - `GetPiece(Location)` → Piece (not object)
  - `GetPiecesOfColor(PieceColor)` → List\<Piece\>
  - `FindKing(PieceColor)` → King
- **Why Extend IBoardState**: Provides type safety while maintaining AI compatibility

---

#### 🎲 Board/

##### `ChessBoard.cs`
- **Purpose**: 8x8 chess board implementation
- **Implements**: IChessBoard (public methods) + IBoardState (explicit implementations)
- **Data Structures**:
  - `Square[,] squares` - 8x8 array
  - `PieceSet[] boardSet` - Active pieces by color [Black, White]
  - `PieceSet[] fallenSet` - Captured pieces by color
- **Key Methods**:
  - `InitializeBoard()` - Sets up initial chess position
  - `GetPiece(Location)` - Retrieves piece at location
  - `PlacePiece(Piece, Location)` - Places piece
  - `FindKing(PieceColor)` - Locates king
- **Pattern**: Explicit interface implementation for IBoardState

##### `Square.cs`
- **Purpose**: Represents a single board square
- **Properties**:
  - `Location` - Position on board
  - `Piece` - Piece occupying square (null if empty)

---

#### ♟️ Pieces/

**Pattern**: Strategy pattern - each piece defines its own movement rules

##### `Piece.cs`
- **Type**: Abstract base class
- **Purpose**: Common contract and state for all pieces
- **Properties**:
  - `Color` - PieceColor
  - `location` - Current position
  - `isMoved` - Track first move (for castling, pawn double-move)
  - `Notation` - String representation (e.g., "wK" = white King)
- **Abstract Methods**:
  - `GetMoves(Location, IChessBoard)` → List\<Move\>
  - `CanMove(Location, Location, IChessBoard)` → bool
  - `IsBlockingCheck(Location, IChessBoard)` → bool

##### `Pawn.cs`
- **Extends**: Piece
- **Movement**:
  - Forward 1 square (or 2 on first move)
  - Diagonal captures
  - En passant
- **Special**: `GetMovesWithEnPassant()` - Handles en passant capture
- **Properties**: `Direction` (Up/Down based on color)

##### `Rook.cs`
- **Extends**: Piece
- **Movement**: Horizontal and vertical lines
- **Algorithm**: Directional moves in 4 directions (up, down, left, right)
- **Special**: Used in castling

##### `Knight.cs`
- **Extends**: Piece
- **Movement**: L-shaped (2+1 or 1+2)
- **Unique**: Only piece that can jump over others
- **Algorithm**: 8 fixed offset positions

##### `Bishop.cs`
- **Extends**: Piece
- **Movement**: Diagonal lines
- **Algorithm**: Directional moves in 4 diagonal directions

##### `Queen.cs`
- **Extends**: Piece
- **Movement**: Combines Rook + Bishop (8 directions)
- **Algorithm**: Directional moves in all 8 directions

##### `King.cs`
- **Extends**: Piece
- **Movement**: One square in any direction
- **Special**: Cannot move into check
- **Properties**: Used in castling validation

---

#### 🎮 Game/

##### `ChessGame.cs`
- **Purpose**: Original simple game orchestration
- **Status**: Legacy (kept for reference)
- **Contains**: Basic game loop, move validation, game state

##### `Player.cs`
- **Purpose**: Player state and turn management
- **Properties**:
  - `Color` - PieceColor
  - `Type` - PlayerType (Human or AI)
  - `HasTurn` - Boolean turn indicator

---

#### 🔄 Movement/

##### `MoveMaker.cs`
- **Purpose**: Executes moves and updates board state
- **Responsibilities**:
  - Move piece from source to destination
  - Handle captures
  - Update `isMoved` flag
  - Execute special moves (castling rook movement)

##### `MoveHistory.cs`
- **Purpose**: Track all moves in the game
- **Methods**:
  - `AddMove(Move, PieceColor, bool wasCapture, bool wasCheck)`
  - `GetMoves()` / `GetAllMoves()` → List\<MoveRecord\>
  - `GetLastMove()` → MoveRecord
  - `DisplayHistory()` - Pretty-print move history
- **Data**: List\<MoveRecord\> with algebraic notation, timestamps

---

#### ✅ Validators/

##### `CheckDetector.cs`
- **Purpose**: Detect check, checkmate, and stalemate
- **Methods**:
  - `IsInCheck(PieceColor, IChessBoard)` → bool
  - `IsCheckmate(PieceColor, IChessBoard)` → bool
  - `IsStalemate(PieceColor, IChessBoard)` → bool
- **Algorithm**:
  - Check: Any opponent piece attacks king
  - Checkmate: In check + no legal moves escape check
  - Stalemate: Not in check + no legal moves available

##### `CastlingValidator.cs`
- **Purpose**: Validate castling legality
- **Methods**:
  - `CanCastleKingside(PieceColor, IChessBoard)` → bool
  - `CanCastleQueenside(PieceColor, IChessBoard)` → bool
- **6 Conditions**:
  1. King not moved
  2. Rook not moved
  3. No pieces between
  4. King not in check
  5. King doesn't pass through check
  6. King doesn't land in check

##### `EnPassantTracker.cs`
- **Purpose**: Track en passant opportunities
- **Methods**:
  - `UpdateAfterMove(Move)` - Update en passant target
  - `GetEnPassantTarget()` → Location?
- **Logic**: Set target when pawn moves 2 squares

---

#### 🎯 Handlers/

##### `PawnPromotionHandler.cs`
- **Purpose**: Handle pawn promotion when reaching last rank
- **Methods**:
  - `PromptForPromotion(PieceColor)` → Piece
  - Interactive UI for promotion choice (Q, R, B, N)

---

#### 🖥️ UI/

##### `ConsoleBoardRenderer.cs`
- **Purpose**: Render chess board to terminal
- **Features**:
  - Unicode piece characters (♔ ♕ ♖ ♗ ♘ ♙)
  - Checkerboard pattern
  - Last move highlighting
  - Color-coded pieces
- **Method**: `RenderBoard(IChessBoard, Location? lastMoveFrom, Location? lastMoveTo)`

##### `CommandParser.cs`
- **Purpose**: Parse user input commands
- **Handles**:
  - Move commands: "e2 e4", "move e2 e4"
  - Castle commands: "castle king", "castle queen"
  - Help commands: "help e2"
  - Game commands: "game start", "game save"
- **Output**: Parsed command type and parameters

##### `GameMenuHandler.cs`
- **Purpose**: Game mode selection menu
- **Options**:
  - Human vs Human
  - Human vs AI (choose color)
  - AI vs AI
- **Returns**: GameMode configuration

---

#### 📝 Logging/

##### `FileLogger.cs`
- **Implements**: ILogger
- **Purpose**: Log to file
- **Methods**: Info(), Warning(), Error()
- **Output**: `chess_log_YYYYMMDD.txt`

##### `ConsoleLogger.cs`
- **Implements**: ILogger
- **Purpose**: Log to console
- **Features**: Color-coded log levels

##### `CompositeLogger.cs`
- **Implements**: ILogger
- **Purpose**: Combine multiple loggers
- **Pattern**: Composite pattern
- **Usage**: Log to both file and console simultaneously

---

#### 💾 Persistence/

##### `GameSerializer.cs`
- **Purpose**: Save/load game state to/from files
- **Format**: JSON
- **Methods**:
  - `SaveGame(GameStateSnapshot, string filename)`
  - `LoadGame(string filename)` → GameStateSnapshot

##### `GameStateSnapshot.cs`
- **Purpose**: Serializable game state representation
- **Properties**:
  - Board state (piece positions)
  - Move history
  - Current turn
  - En passant target
  - Castling rights
  - Player types

---

#### 🧠 Learning/

##### `GameRecorder.cs`
- **Purpose**: Record games for AI training
- **Output**: PGN (Portable Game Notation) format
- **Methods**:
  - `StartRecording()`
  - `RecordMove(Move)`
  - `SaveGame(string filename)`

##### `GameRecord.cs`
- **Purpose**: Data structure for recorded game
- **Properties**:
  - Player names
  - Moves in algebraic notation
  - Result
  - Date/time

---

#### 🛠️ Utilities/

##### `PieceSet.cs`
- **Purpose**: Manage collection of pieces for one color
- **Properties**: `List<Piece> Pieces`, `PieceColor Color`

##### `Utilities.cs`
- **Purpose**: Miscellaneous helper functions
- **Methods**:
  - `PrintEmptyBoard()` - Debug board display
  - `PrintInitialBoard()` - Debug initial position

---

### 🤖 ShatranjAI/ - AI Implementation

**Purpose**: Chess AI using minimax algorithm with alpha-beta pruning

**Project File**: `ShatranjAI.csproj`
- Target: .NET 9.0
- Dependencies: ShatranjCore.Abstractions, ShatranjCore
- Output: Class library

#### AI/

##### `BasicAI.cs`
- **Implements**: IChessAI
- **Algorithm**: Minimax with alpha-beta pruning
- **Depth**: Configurable (default 3)
- **Methods**:
  - `SelectMove(IBoardState, PieceColor, Location?)` → AIMove
    - Casts IBoardState to IChessBoard internally
    - Returns best move with evaluation score
  - `EvaluatePosition(IBoardState, PieceColor)` → double
- **Features**:
  - Legal move generation
  - Alpha-beta pruning (reduces nodes ~90%)
  - Move ordering (best moves first)
  - Logging integration
  - Performance metrics (nodes evaluated, time)

##### `MoveEvaluator.cs`
- **Purpose**: Evaluate board positions
- **Scoring**:
  - Material value (Pawn=100, Knight/Bishop=300, Rook=500, Queen=900)
  - Piece-square tables (positional bonuses)
  - Mobility (number of legal moves)
- **Method**: `Evaluate(IChessBoard, PieceColor)` → double

##### `IChessAI.cs`
- **Purpose**: Forwarding interface to Abstractions
- **Content**: `using IChessAI = ShatranjCore.Abstractions.IChessAI;`

---

### 💻 ShatranjCMD/ - Console Application

**Purpose**: Entry point for terminal-based chess game

**Project File**: `ShatranjCMD.csproj`
- Target: .NET 9.0
- Dependencies: ShatranjCore.Abstractions, ShatranjCore, ShatranjAI
- Output: Executable console application

#### `Program.cs`
- **Purpose**: Application entry point with Dependency Injection
- **Setup**:
  - Configure DI container (Microsoft.Extensions.DependencyInjection)
  - Register services (ILogger, IChessAI, etc.)
  - Initialize game
  - Start game loop
- **Flow**:
  1. Show game mode menu
  2. Create players based on mode
  3. Initialize board and components
  4. Run game loop (input → validate → execute → render)
  5. Handle game end conditions

---

### 🖼️ ShatranjMain/ - Windows Forms GUI (Legacy)

**Purpose**: Desktop GUI application (Windows only)

**Project File**: `ShatranjMain.csproj`
- Target: .NET 9.0-windows
- Dependencies: ShatranjCore
- Output: Windows executable
- **Status**: Legacy/experimental, not actively maintained

#### Files

- `Program.cs` - WinForms entry point
- `ChessBoard.cs` / `ChessBoard.Designer.cs` - Custom board control
- `Forms/ChessGame.cs` - Main game form
- `Forms/Main.cs` - Main menu form
- `Properties/` - Assembly info, resources, settings

---

### 🧪 tests/ - Test Projects

**Architecture**: Three-tier test pyramid

#### tests/ShatranjCore.Tests/ - Core Unit Tests (40+ tests)

**Project File**: `ShatranjCore.Tests.csproj`
- Dependencies: ShatranjCore.Abstractions, ShatranjCore

##### `TestRunner.cs`
- **Purpose**: Custom test runner with formatted output
- **Features**: Colored output, test grouping, pass/fail summary

##### `AllPiecesMovementTest.cs`
- **Purpose**: Verify all pieces in initial position can move correctly
- **Tests**: All 32 pieces, verifies piece types and valid moves

##### `InitialGameMoveTest.cs`
- **Purpose**: Test legal moves from initial game state
- **Validates**: Only pawns and knights can move initially

##### PieceTests/

###### `RookTests.cs` (6 tests)
- Center position moves
- Corner position moves
- Capture enemy piece
- Blocked by friendly piece
- Blocked by enemy piece
- Horizontal/vertical only

###### `KnightTests.cs` (6 tests)
- Center position (8 moves)
- Corner position (2 moves)
- Jump over pieces
- Capture enemy pieces
- Blocked by friendly pieces
- L-shaped movement validation

###### `BishopTests.cs` (6 tests)
- Center diagonal moves
- Corner diagonal moves
- Diagonal captures
- Blocked by friendly pieces
- Blocked by enemy pieces
- Diagonal-only validation

###### `QueenTests.cs` (6 tests)
- Combined Rook + Bishop movement
- Maximum mobility tests
- Capture tests
- Blocking tests

###### `KingTests.cs` (6 tests)
- One-square movement
- All 8 directions
- Cannot move into check
- Castling validation
- Capture tests

###### `PawnTests.cs` (10 tests)
- Single square forward
- Double square on first move
- Diagonal capture
- Cannot move backward
- Blocked by pieces
- En passant capture
- Promotion to Queen
- Promotion to Knight
- Edge case tests

---

#### ShatranjAI.Tests/ - AI Unit Tests (6 tests)

**Project File**: `ShatranjAI.Tests.csproj`
- Dependencies: ShatranjCore.Abstractions, ShatranjAI

##### `TestRunner.cs`
- Custom test runner for AI tests

##### `BasicAITests.cs` (3 tests)
- AI can select valid moves
- AI makes only legal moves
- AI respects game rules (no moving into check)

##### `MoveEvaluatorTests.cs` (3 tests)
- Position evaluation accuracy
- Material counting correct
- Piece-square table bonuses applied

---

#### tests/ShatranjIntegration.Tests/ - Integration Tests (6 tests)

**Project File**: `ShatranjIntegration.Tests.csproj`
- Dependencies: ShatranjCore.Abstractions, ShatranjCore, ShatranjAI

##### `TestRunner.cs`
- Integration test runner

##### `AIIntegrationTests.cs` (3 tests)
- AI can play complete game to conclusion
- AI makes only valid moves throughout entire game
- AI logging integration works correctly

##### `GameFlowTests.cs` (3 tests)
- Check detection in real game scenarios
- Castling in real game scenarios
- En passant in real game scenarios

---

### 📚 docs/ - Documentation

#### Core Documentation

##### `ARCHITECTURE.md`
- **Purpose**: Deep technical architecture documentation
- **Content**:
  - Layered architecture explanation
  - IBoardState abstraction pattern
  - Design patterns used (Strategy, DI, etc.)
  - Data flow diagrams
  - Key algorithms (minimax, move generation)
  - Testing strategy
  - Extensibility guide

##### `TESTING.md`
- **Purpose**: Test architecture and running tests
- **Content**:
  - Three-tier test pyramid explanation
  - How to run each test suite
  - Test coverage details
  - Writing new tests

##### `SOLID_PRINCIPLES.md`
- **Purpose**: SOLID analysis and refactoring guide
- **Content**:
  - Before/after examples
  - SOLID score evolution (6/10 → 9/10)
  - Specific refactorings applied

#### Status Documentation

##### `IMPLEMENTATION_STATUS.md`
- **Purpose**: Detailed implementation status tracking
- **Content**: Feature completion percentages, TODO items

##### `PHASE2_STATUS.md`
- **Purpose**: Phase 2 specific status
- **Content**: AI integration progress, remaining tasks

##### `PROJECT_ROADMAP.md`
- **Purpose**: Original roadmap (superseded by ROADMAP.md in root)

##### `PROJECT_DETAILS.md`
- **Purpose**: Project metadata and organization

#### Analysis Documentation

##### `CHESSGAME_ANALYSIS.md`
- **Purpose**: Analysis of original ChessGame.cs implementation
- **Content**: Code smells, refactoring opportunities

##### `TERMINAL_COMMANDS.md`
- **Purpose**: Command reference for terminal UI

---

## Key Architectural Patterns

### 1. Layered Architecture

```
┌─────────────────────────────────────────┐
│   Presentation (ShatranjCMD)            │
├─────────────────────────────────────────┤
│   AI Layer (ShatranjAI)  │  Game Logic │
├──────────────────────────┴─────────────┤
│   Abstractions (No Dependencies)        │
├─────────────────────────────────────────┤
│   Domain Model (Pieces, Board)          │
└─────────────────────────────────────────┘
```

### 2. Dependency Inversion

- **High-level modules** depend on **abstractions** (interfaces)
- **IChessBoard** interface allows swapping board implementations
- **IChessAI** interface allows multiple AI implementations
- **ILogger** interface allows different logging strategies

### 3. Strategy Pattern

Each piece class implements its own movement strategy:
- `Piece.GetMoves(Location, IChessBoard)` → abstract method
- Each piece (Pawn, Rook, etc.) provides specific implementation
- Add new piece types without modifying existing code

### 4. Explicit Interface Implementation

`ChessBoard` implements both:
- `IChessBoard` (public methods, strongly-typed)
- `IBoardState` (explicit methods, object-typed)

Allows type-safe access for game logic while providing loose coupling for AI.

### 5. Composite Pattern

`CompositeLogger` combines multiple loggers:
```csharp
var logger = new CompositeLogger(
    new ConsoleLogger(),
    new FileLogger("game.log")
);
```

### 6. Template Method

Game loop defines skeleton, specific steps can be customized:
1. Render board
2. Get player input
3. Validate move
4. Execute move
5. Check game state
6. Switch turns

---

## Quick Reference

### Starting a New Feature

1. **Check ROADMAP.md** - Understand phase and priorities
2. **Read ARCHITECTURE.md** - Understand structure
3. **Review this file** - Locate relevant files
4. **Check tests/** - See existing test patterns
5. **Follow SOLID principles** - Maintain architecture quality

### Common Development Tasks

**Add new piece type:**
1. Create class in `ShatranjCore/Pieces/`
2. Extend `Piece` abstract class
3. Implement `GetMoves()` with movement rules
4. Add tests in `tests/ShatranjCore.Tests/PieceTests/`
5. Update board initialization if standard piece

**Add new AI:**
1. Create class in `ShatranjAI/AI/`
2. Implement `IChessAI` interface
3. Use `IBoardState` (cast to `IChessBoard` internally if needed)
4. Add tests in `ShatranjAI.Tests/`

**Add new validator:**
1. Create class in `ShatranjCore/Validators/`
2. Accept `IChessBoard` in methods
3. Return validation result
4. Integrate into game flow
5. Add integration tests

**Add new command:**
1. Update `CommandParser.cs` to recognize command
2. Add handler in game loop
3. Update `TERMINAL_COMMANDS.md`
4. Update `README.md` examples

### Build and Test

```bash
# Build all
dotnet build

# Run specific test suite
dotnet run --project tests/ShatranjCore.Tests
dotnet run --project ShatranjAI.Tests
dotnet run --project tests/ShatranjIntegration.Tests

# Run game
dotnet run --project ShatranjCMD
```

### File Naming Conventions

- **Interfaces**: `I` prefix (IChessBoard, IChessAI, ILogger)
- **Abstract classes**: No prefix (Piece)
- **Implementations**: Descriptive names (BasicAI, FileLogger)
- **Tests**: `*Tests.cs` suffix (RookTests.cs, BasicAITests.cs)
- **Test runners**: `TestRunner.cs` (one per test project)

### Namespace Conventions

- Abstractions: `ShatranjCore.Abstractions`
- Core modules: `ShatranjCore.{Module}` (Pieces, Board, Validators, etc.)
- AI: `ShatranjAI.AI`
- Tests: Match source namespace (e.g., `ShatranjCore.Tests`)

---

## Next Steps for Future Sessions

1. **Review ROADMAP.md** for current phase status
2. **Check this file** for relevant file locations
3. **Read ARCHITECTURE.md** for deep technical details
4. **Verify build** with `dotnet build`
5. **Run tests** to ensure clean baseline
6. **Start implementing** new features following SOLID principles

---

**Maintained by**: Mohammed Azmat
**Repository**: https://github.com/MohammedAzmat/Shatranj
**License**: MIT

---

## Recent Updates (2025-11-10)

### ✅ Compilation Errors Resolved

All 264+ compilation errors have been fixed:
- Added `using ShatranjCore.Abstractions;` to all test files
- Added `using ShatranjCore.Movement;` to BasicAI.cs  
- Removed unused variables from TestRunner.cs
- **Current Status**: Zero errors, zero warnings!

### 📁 Documentation Reorganization

- Moved BUILD.md, ROADMAP.md, CONTEXT.md, DOTNET9_UPGRADE.md to `docs/` folder
- All documentation now centralized in `docs/` except README.md
- Updated all cross-references between documentation files

### 🎯 Phase 2 Status: 100% Complete ✅

Core AI integration is complete and fully functional:
- ✅ All game modes working (Human vs Human, Human vs AI, AI vs AI)
- ✅ Minimax algorithm with alpha-beta pruning
- ✅ Position evaluation with material + piece-square tables
- ✅ Save/Load system with undo/redo functionality
- ✅ 52 tests passing (40 core, 6 AI, 6 integration)
- ✅ Zero compilation errors
- ✅ Clean architecture with proper abstractions layer

Phase 2 is officially complete. Ready to begin Phase 3: AI Learning & Game History.

---

**For complete project status, see [ROADMAP.md](ROADMAP.md)**
