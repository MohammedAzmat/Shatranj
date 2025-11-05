# SOLID Principles in Shatranj

This document explains how SOLID principles are applied throughout the Shatranj chess game codebase, where we adhere to them, and where we consciously deviate for practical reasons.

---

## Table of Contents
1. [Single Responsibility Principle (SRP)](#single-responsibility-principle-srp)
2. [Open/Closed Principle (OCP)](#openclosed-principle-ocp)
3. [Liskov Substitution Principle (LSP)](#liskov-substitution-principle-lsp)
4. [Interface Segregation Principle (ISP)](#interface-segregation-principle-isp)
5. [Dependency Inversion Principle (DIP)](#dependency-inversion-principle-dip)
6. [Pragmatic Deviations](#pragmatic-deviations)
7. [Guidelines for Future Development](#guidelines-for-future-development)

---

## Single Responsibility Principle (SRP)
> "A class should have one, and only one, reason to change."

### ✅ Adherence

#### **Piece Classes** (`King.cs`, `Queen.cs`, `Rook.cs`, etc.)
**Responsibility**: Define movement rules for a specific piece type.
```csharp
// Each piece class has ONE responsibility: defining valid moves for that piece
public class Knight : Piece
{
    public override List<Move> ValidMoves(ChessBoard board)
    {
        // Only handles knight-specific movement logic
    }
}
```
**Why it's good**: Easy to modify knight movement without affecting other pieces.

#### **Square.cs**
**Responsibility**: Represent a single board square with location and piece.
```csharp
public class Square
{
    public Location Location { get; set; }
    public Piece Piece { get; set; }
}
```
**Why it's good**: Simple, focused data structure with clear purpose.

#### **ChessBoard.cs**
**Responsibility**: Manage the 8x8 board state and piece positions.
- Initializes board
- Places and retrieves pieces
- Provides board display

**Why it's good**: Centralized board state management, doesn't handle game rules or move validation.

#### **Player.cs**
**Responsibility**: Represent player information (color, type, turn status).
```csharp
public class Player
{
    public PieceColor Color { get; set; }
    public PlayerType Type { get; set; }  // Human or AI
    public bool isTurn { get; set; }
}
```
**Why it's good**: Pure data holder for player state, no game logic mixed in.

### ❌ Current Violations

#### **ChessGame.cs** ⚠️ VIOLATION
**Current Issue**: Has multiple responsibilities:
- Game loop orchestration
- Move input parsing
- Move validation
- Board display
- Turn management

**Impact**: High coupling, difficult to test individual components.

**Recommended Refactor**:
```csharp
// Split into multiple classes:
public class ChessGame         // Orchestrates the game flow
public class InputParser       // Parses algebraic notation
public class MoveValidator     // Validates legal moves
public class GameRenderer      // Displays board (could be interface)
public class TurnManager       // Manages whose turn it is
```

**Status**: 🔧 TO BE REFACTORED in Phase 1

#### **ChessBoard.cs** ⚠️ MINOR VIOLATION
**Current Issue**: Handles both board state AND console display (`DisplayBoard()`).

**Impact**: Medium - Makes testing harder, couples board logic to console output.

**Recommended Refactor**:
```csharp
// Extract display logic
public interface IBoardRenderer
{
    void Render(ChessBoard board);
}

public class ConsoleBoardRenderer : IBoardRenderer
{
    public void Render(ChessBoard board) { /* console display */ }
}
```

**Status**: 🔧 TO BE REFACTORED in Phase 1 or Phase 5 (when adding GUI)

---

## Open/Closed Principle (OCP)
> "Software entities should be open for extension, but closed for modification."

### ✅ Adherence

#### **Piece Hierarchy**
**Implementation**: Abstract `Piece` base class with virtual/abstract methods.
```csharp
public abstract class Piece
{
    public abstract List<Move> ValidMoves(ChessBoard board);
    // Common properties and methods
}

public class Bishop : Piece
{
    public override List<Move> ValidMoves(ChessBoard board)
    {
        // Bishop-specific implementation
    }
}
```

**Why it's good**:
- Can add new piece types (e.g., for chess variants) WITHOUT modifying existing pieces
- Adheres to polymorphism
- Each piece extends behavior without changing base class

**Example Extension** (Future):
```csharp
// For Chess960 or variants - no modification to existing pieces needed
public class Archbishop : Piece  // Bishop + Knight hybrid
{
    public override List<Move> ValidMoves(ChessBoard board)
    {
        // Combine bishop and knight moves
    }
}
```

#### **PlayerType Enumeration**
**Current**:
```csharp
public enum PlayerType
{
    Human,
    AI
}
```

**Future Extension** (Phase 2-3):
```csharp
public interface IPlayerController
{
    Move GetNextMove(ChessBoard board, PieceColor color);
}

public class HumanPlayerController : IPlayerController { }
public class BasicAIController : IPlayerController { }
public class AdvancedAIController : IPlayerController { }
public class RemotePlayerController : IPlayerController { }  // Phase 4
```

**Why it's good**: New player types added without modifying existing game logic.

### ⚠️ Planned Improvements

#### **Move Validation** (TO BE IMPLEMENTED)
**Current State**: Move validation scattered across pieces and game logic.

**Planned OCP-Compliant Design**:
```csharp
public interface IMoveValidator
{
    bool IsValidMove(Move move, ChessBoard board);
}

// Can add validators without modifying existing ones
public class BasicMoveValidator : IMoveValidator { }
public class CheckValidator : IMoveValidator { }
public class CastlingValidator : IMoveValidator { }

public class CompositeValidator : IMoveValidator
{
    private List<IMoveValidator> _validators;

    public bool IsValidMove(Move move, ChessBoard board)
    {
        return _validators.All(v => v.IsValidMove(move, board));
    }
}
```

**Status**: 🔧 TO BE IMPLEMENTED in Phase 1

---

## Liskov Substitution Principle (LSP)
> "Derived classes must be substitutable for their base classes."

### ✅ Adherence

#### **Piece Substitutability**
**Current State**: All piece subclasses can be used interchangeably as `Piece` type.

```csharp
// Client code works with Piece abstraction
public void MovePiece(Piece piece, Location to, ChessBoard board)
{
    List<Move> validMoves = piece.ValidMoves(board);  // Works for ANY piece
    // ... rest of logic
}

// Can pass any piece type
MovePiece(new Knight(), destination, board);
MovePiece(new Queen(), destination, board);
```

**Why it's good**:
- Code expects `Piece` behavior, gets correct behavior from all subclasses
- No special casing needed
- Polymorphism works correctly

### ⚠️ Potential Violations to Watch

#### **Special Piece Behaviors**
**Risk**: King and Pawn have unique behaviors that might leak into base class.

**Current Example**:
```csharp
public abstract class Piece
{
    public bool isMoved { get; set; }  // Needed for King (castling) and Pawn
}
```

**Analysis**:
- ✅ Acceptable: Many pieces care about first move (Rook for castling, Pawn for double move)
- ❌ Risky: If we add King-specific properties to base `Piece`

**Rule**: If a property/method is only used by 1-2 piece types, it should NOT be in base class.

**Future-Proof Pattern**:
```csharp
// Use composition for special behaviors
public interface ICastleable
{
    bool CanCastle(ChessBoard board);
}

public class King : Piece, ICastleable
{
    public bool CanCastle(ChessBoard board) { /* ... */ }
}
```

**Status**: ✅ Currently OK, 🔍 Monitor during Phase 1 development

---

## Interface Segregation Principle (ISP)
> "Clients should not be forced to depend on interfaces they do not use."

### ⚠️ Current State: Needs Improvement

**Issue**: Currently using abstract base class (`Piece`) instead of focused interfaces.

### 🔧 Recommended Improvements (Phase 1)

#### **Segregated Interfaces for Pieces**
```csharp
// Core movement interface - ALL pieces need this
public interface IMovable
{
    List<Move> GetValidMoves(ChessBoard board);
}

// Special abilities - only relevant pieces implement
public interface ICastleable
{
    bool CanCastle(ChessBoard board, CastlingSide side);
}

public interface IPromotable
{
    List<Type> GetPromotionOptions();
}

public interface IEnPassantCapture
{
    bool CanCaptureEnPassant(Square targetSquare, ChessBoard board);
}

// Piece implementations
public class King : Piece, IMovable, ICastleable
{
    public List<Move> GetValidMoves(ChessBoard board) { /* ... */ }
    public bool CanCastle(ChessBoard board, CastlingSide side) { /* ... */ }
}

public class Pawn : Piece, IMovable, IPromotable, IEnPassantCapture
{
    public List<Move> GetValidMoves(ChessBoard board) { /* ... */ }
    public List<Type> GetPromotionOptions() { /* ... */ }
    public bool CanCaptureEnPassant(Square targetSquare, ChessBoard board) { /* ... */ }
}

public class Knight : Piece, IMovable
{
    public List<Move> GetValidMoves(ChessBoard board) { /* ... */ }
    // No special abilities - doesn't implement unnecessary interfaces
}
```

**Benefits**:
- Knight doesn't have unused `CanCastle()` methods
- Clear which pieces support which special moves
- Easier to query capabilities: `if (piece is ICastleable castleable)`

**Status**: 🔧 TO BE IMPLEMENTED in Phase 1

#### **AI Player Interfaces**
```csharp
// Phase 2-3: Don't force all AI to implement advanced features
public interface IChessAI
{
    Move SelectMove(ChessBoard board, PieceColor color);
}

public interface IConfigurableAI : IChessAI
{
    void SetDifficulty(DifficultyLevel level);
    void SetThinkingTime(TimeSpan maxTime);
}

public interface IAnalysisAI : IChessAI
{
    PositionEvaluation AnalyzePosition(ChessBoard board);
    List<Move> GetBestMoves(ChessBoard board, int count);
}

// Simple AI only implements basic interface
public class RandomAI : IChessAI { }

// Advanced AI implements all features
public class MinimaxAI : IChessAI, IConfigurableAI, IAnalysisAI { }
```

**Status**: 📅 PLANNED for Phase 2

---

## Dependency Inversion Principle (DIP)
> "Depend on abstractions, not concretions."

### ⚠️ Current Violations

#### **ChessGame → ChessBoard** ❌ VIOLATION
**Current**:
```csharp
public class ChessGame
{
    private ChessBoard board;  // Direct dependency on concrete class

    public ChessGame()
    {
        board = new ChessBoard();  // Tightly coupled
    }
}
```

**Problem**:
- Cannot easily test with mock board
- Cannot swap board implementations (e.g., for variants)
- Hard to unit test game logic in isolation

**Recommended Fix**:
```csharp
public interface IChessBoard
{
    Piece GetPieceAt(Location location);
    void PlacePiece(Piece piece, Location location);
    bool IsSquareEmpty(Location location);
    List<Piece> GetAllPieces(PieceColor color);
}

public class ChessBoard : IChessBoard
{
    // Implementation
}

public class ChessGame
{
    private readonly IChessBoard _board;

    // Dependency injection
    public ChessGame(IChessBoard board)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
    }
}

// Usage
var game = new ChessGame(new ChessBoard());

// Testing
var mockBoard = new MockChessBoard();
var testGame = new ChessGame(mockBoard);
```

**Benefits**:
- Testable with mocks
- Can create variants (e.g., `HexagonalChessBoard`, `Chess960Board`)
- Loose coupling

**Status**: 🔧 HIGH PRIORITY - Refactor in Phase 1

#### **Piece → ChessBoard Dependency** ⚠️ ACCEPTABLE
**Current**:
```csharp
public abstract class Piece
{
    public abstract List<Move> ValidMoves(ChessBoard board);  // Depends on concrete ChessBoard
}
```

**Analysis**:
- ✅ Acceptable for now: Pieces need board state to calculate moves
- 🔧 Could improve: Use `IChessBoard` interface instead

**Recommended (Lower Priority)**:
```csharp
public abstract class Piece
{
    public abstract List<Move> ValidMoves(IChessBoard board);
}
```

**Status**: 🔧 NICE TO HAVE - Refactor in Phase 1 alongside ChessGame changes

### ✅ Future Best Practices (Phase 2+)

#### **AI Dependency Injection**
```csharp
public interface IChessAI
{
    Move SelectMove(IChessBoard board, PieceColor color);
}

public class ChessGame
{
    private readonly IChessBoard _board;
    private readonly IChessAI _whitePlayerAI;
    private readonly IChessAI _blackPlayerAI;

    public ChessGame(
        IChessBoard board,
        IChessAI whiteAI = null,  // null = human player
        IChessAI blackAI = null)
    {
        _board = board;
        _whitePlayerAI = whiteAI;
        _blackPlayerAI = blackAI;
    }
}

// Usage flexibility
var humanVsHuman = new ChessGame(new ChessBoard());
var humanVsAI = new ChessGame(new ChessBoard(), blackAI: new BasicAI());
var aiVsAI = new ChessGame(new ChessBoard(), new AdvancedAI(), new BasicAI());
```

**Status**: 📅 PLANNED for Phase 2

---

## Pragmatic Deviations

Sometimes strict adherence to SOLID principles creates unnecessary complexity. Here are conscious deviations with justification.

### 1. **Square as Simple Data Structure** ✅ ACCEPTABLE

**What We're Doing**:
```csharp
public class Square
{
    public Location Location { get; set; }
    public Piece Piece { get; set; }
}
```

**SOLID Concern**:
- Public setters violate encapsulation
- Could argue for more methods/behavior

**Why We Deviate**:
- Square is a pure data holder (DTO pattern)
- No complex logic needed
- Setters make board manipulation simple
- Performance: No overhead from getters/setters

**Decision**: ✅ Keep as-is. Simplicity > theoretical purity for DTOs.

---

### 2. **Location as Struct** ✅ ACCEPTABLE

**What We're Doing**:
```csharp
public struct Location
{
    public int Row { get; set; }
    public int Column { get; set; }
}
```

**SOLID Concern**:
- Mutable struct (generally discouraged in C#)

**Why We Deviate**:
- Value semantics appropriate for coordinates
- Performance: Avoids heap allocation
- Chess board: 64 locations created frequently
- Simple enough that mutability won't cause issues

**Alternative Considered**:
```csharp
public readonly struct Location  // Immutable
{
    public int Row { get; }
    public int Column { get; }

    public Location(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
```

**Decision**: 🔧 CONSIDER making immutable in Phase 1 refactor for safety.

---

### 3. **Move as Data Class** ✅ ACCEPTABLE

**Current**:
```csharp
public class Move
{
    public Location From { get; set; }
    public Location To { get; set; }
    public Piece Piece { get; set; }
    public Piece CapturedPiece { get; set; }
}
```

**Why This is Fine**:
- Represents a simple move transaction
- No business logic belongs here
- Easy to serialize (for save/load, network play)

**Future Enhancement** (Phase 4 - Network Play):
```csharp
// Add minimal behavior for serialization
public class Move
{
    // ... existing properties ...

    public string ToAlgebraicNotation() { /* e.g., "e2-e4" */ }
    public static Move FromAlgebraicNotation(string notation) { /* parse */ }
}
```

**Decision**: ✅ Keep simple now, enhance minimally as needed.

---

### 4. **ChessBoard Display Method** ⚠️ TEMPORARY VIOLATION

**Current Violation**:
```csharp
public class ChessBoard
{
    // ... board logic ...

    public void DisplayBoard()  // Mixing concerns
    {
        Console.WriteLine(/* ... */);
    }
}
```

**Why We're Allowing It Temporarily**:
- Phase 1 focus: Get game rules working
- Console display needed for testing/debugging
- Will extract when adding GUI (Phase 5)

**Planned Fix Timeline**:
- Phase 1: Keep as-is for rapid development
- Phase 5: Extract to `IChessBoardRenderer` when adding GUI

**Decision**: ⚠️ TECHNICAL DEBT - Document and fix in Phase 5.

---

## Guidelines for Future Development

### When Adding New Features

#### ✅ DO:
1. **Ask**: "What is this class's single responsibility?"
2. **Check**: Can existing code be extended without modification? (OCP)
3. **Verify**: Are all dependencies injected as interfaces? (DIP)
4. **Test**: Can I unit test this in isolation?
5. **Review**: Would this work in a chess variant? (extensibility check)

#### ❌ DON'T:
1. Add methods to `Piece` base class that only 1-2 pieces need
2. Create "God classes" that do everything
3. Use `new` keyword inside classes for dependencies (breaks DIP)
4. Make interfaces with 10+ methods (violates ISP)
5. Write untestable code

### Code Review Checklist

Before merging any PR, verify:
- [ ] Each class has one clear responsibility
- [ ] Dependencies are interfaces, not concrete classes
- [ ] New piece types extend `Piece` without modifying it
- [ ] Unit tests exist and pass
- [ ] No SOLID violations introduced (or documented if necessary)

### Refactoring Priorities

**Phase 1 Must-Fix**:
1. ✅ Extract `IChessBoard` interface
2. ✅ Implement dependency injection in `ChessGame`
3. ✅ Split `ChessGame` responsibilities (parser, validator, renderer)
4. ✅ Add segregated interfaces for special piece abilities

**Phase 2-3 Must-Fix**:
1. ✅ Create `IChessAI` interface hierarchy
2. ✅ Dependency-inject AI implementations

**Phase 5 Must-Fix**:
1. ✅ Extract board rendering to `IChessBoardRenderer`
2. ✅ Implement MVVM or similar pattern for GUI

---

## Summary

### Current SOLID Health Score: 6/10

| Principle | Score | Status |
|-----------|-------|--------|
| Single Responsibility | 6/10 | ⚠️ ChessGame violates, pieces are good |
| Open/Closed | 8/10 | ✅ Piece hierarchy excellent, needs validators |
| Liskov Substitution | 9/10 | ✅ Working well, monitor for violations |
| Interface Segregation | 4/10 | ❌ No interfaces yet, needs implementation |
| Dependency Inversion | 3/10 | ❌ Concrete dependencies throughout |

### Post-Phase 1 Target: 9/10

**Focus Areas**:
- Dependency injection
- Interface extraction
- Responsibility separation

### Philosophy
> "SOLID principles guide us toward maintainable code, but pragmatism prevents over-engineering. When in doubt, favor simplicity and testability over theoretical purity."

---

**Last Updated**: 2025-11-05
**Next Review**: After Phase 1 completion
