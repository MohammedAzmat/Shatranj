# Shatranj - Architecture Documentation

> **Last Updated**: November 2025
> **Version**: Phase 1 Complete
> **SOLID Score**: 9/10

This document provides a comprehensive technical overview of the Shatranj chess project architecture.

---

## Table of Contents
1. [Architectural Overview](#architectural-overview)
2. [Modular Structure](#modular-structure)
3. [Core Abstractions](#core-abstractions)
4. [Design Patterns](#design-patterns)
5. [Data Flow](#data-flow)
6. [Key Algorithms](#key-algorithms)
7. [Extensibility](#extensibility)

---

## Architectural Overview

### Architecture Style
Shatranj follows a **Layered Architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (ShatranjCMD, ShatranjMain - future)   │
└─────────────────────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────┐
│          Game Logic Layer               │
│    (EnhancedChessGame, Player)          │
└─────────────────────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────┐
│       Validation & Rules Layer          │
│  (CheckDetector, CastlingValidator,     │
│   EnPassantTracker, MoveMaker)          │
└─────────────────────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────┐
│          Domain Model Layer             │
│  (Pieces, Board, Square, Location)      │
└─────────────────────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────┐
│         Infrastructure Layer            │
│  (IChessBoard interface, utilities)     │
└─────────────────────────────────────────┘
```

### Design Principles
1. **Separation of Concerns**: Each layer has a distinct responsibility
2. **Dependency Inversion**: High-level modules depend on abstractions (IChessBoard)
3. **Single Responsibility**: Each class has one clear purpose
4. **Testability**: Core logic is decoupled from UI and infrastructure
5. **Extensibility**: New pieces, validators, or UI can be added without modifying existing code

---

## Modular Structure

### ShatranjCore Library Organization

```
ShatranjCore/
├── Models.cs                    # Base types & enums (root namespace)
├── Pieces/                      # ♟️ Piece implementations
│   ├── Piece.cs                 # Abstract base class
│   ├── Pawn.cs                  # Most complex piece
│   ├── Rook.cs                  # Castling support
│   ├── Knight.cs                # Jump ability
│   ├── Bishop.cs                # Diagonal movement
│   ├── Queen.cs                 # Combined movement
│   └── King.cs                  # Castling + restricted movement
│
├── Board/                       # 🎲 Board representation
│   ├── ChessBoard.cs            # 8x8 array, implements IChessBoard
│   └── Square.cs                # Individual square state
│
├── Interfaces/                  # 📋 Abstractions
│   └── IChessBoard.cs           # Board contract for DI
│
├── Game/                        # 🎮 Game orchestration
│   ├── ChessGame.cs             # Original simple implementation
│   ├── EnhancedChessGame.cs    # Refactored with all features
│   └── Player.cs                # Player state & turn management
│
├── Movement/                    # 🔄 Move handling
│   ├── MoveMaker.cs             # Executes moves
│   └── MoveHistory.cs           # Tracks game history
│
├── Validators/                  # ✅ Rule validation
│   ├── CastlingValidator.cs     # Castling rules
│   ├── CheckDetector.cs         # Check/checkmate/stalemate
│   └── EnPassantTracker.cs      # En passant state
│
├── UI/                          # 🖥️ User interaction
│   ├── ConsoleBoardRenderer.cs  # ASCII/Unicode board display
│   └── CommandParser.cs         # Input parsing (e2-e4 format)
│
├── Handlers/                    # 🎯 Special move handlers
│   └── PawnPromotionHandler.cs  # Promotion logic
│
└── Utilities/                   # 🛠️ Helpers
    ├── Utilities.cs             # General utilities
    └── PieceSet.cs              # Piece collection management
```

### Namespace Strategy

**Root Namespace** (`ShatranjCore`)
- Contains only `Models.cs` with base types used across all modules:
  - `PieceColor` enum (White, Black)
  - `PawnMoves` enum (Up, Down)
  - `Location` struct (row, column)
  - `Move` struct (From, To)
  - `PlayerType` enum (Human, AI)

**Module Namespaces** (Suffix pattern)
- `ShatranjCore.Pieces` - All piece classes
- `ShatranjCore.Board` - Board and square
- `ShatranjCore.Interfaces` - IChessBoard
- `ShatranjCore.Game` - Game logic
- `ShatranjCore.Movement` - Move execution
- `ShatranjCore.Validators` - Validation rules
- `ShatranjCore.UI` - User interface
- `ShatranjCore.Handlers` - Special handlers
- `ShatranjCore.Utilities` - Utilities

**Benefits**:
- **Clear Organization**: Related classes are grouped together
- **Reduced Coupling**: Each module has minimal dependencies
- **Easy Navigation**: Find functionality by category
- **Scalability**: New modules can be added without conflicts

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

### 2. Board Abstraction (IChessBoard)

```csharp
namespace ShatranjCore.Interfaces
{
    public interface IChessBoard
    {
        // Piece queries
        Piece GetPiece(Location location);
        bool IsEmptyAt(int row, int column);
        bool IsInBounds(int row, int column);

        // Piece manipulation
        void PlacePiece(Piece piece, Location location);
        void RemovePiece(Location location);

        // Game state
        Location? FindKing(PieceColor color);
        List<Piece> GetAllPieces(PieceColor? color = null);
    }
}
```

**Why an interface?**

1. **Dependency Inversion Principle**
   - Pieces depend on `IChessBoard`, not concrete `ChessBoard`
   - Future: Could implement `BitBoard`, `MailboxBoard`, etc.
   - Testability: Can create mock boards for testing

2. **Encapsulation**
   - Board implementation details hidden from pieces
   - Can change internal representation without affecting pieces

3. **Flexibility**
   - Different board types for different scenarios:
     - `ChessBoard` - Standard 8x8 array
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

### Unit Tests (Current)
- **Piece Movement**: Each piece has 5-9 tests
- **Edge Cases**: Board boundaries, blocking, captures
- **Special Moves**: Castling, en passant, promotion

### Integration Tests (Future)
- Full game scenarios (e.g., Scholar's Mate in 4 moves)
- Check/checkmate combinations
- Stalemate scenarios

### Performance Tests (Future)
- Benchmark move generation (target: <1ms for full board)
- AI search depth (target: depth 6 in <3 seconds)

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
**Architecture Status**: Phase 1 Complete ✅
