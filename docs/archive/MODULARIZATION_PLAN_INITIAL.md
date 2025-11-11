# Shatranj Modularization Plan

## Overview

This document outlines opportunities for code modularization to improve maintainability, testability, and adherence to SOLID principles before beginning Phase 3 development.

---

## Current State Analysis

### ChessGame.cs - 1384 Lines (TOO LARGE ⚠️)

**Current Responsibilities:**
1. Game initialization and setup
2. Main game loop orchestration
3. Command parsing and routing (duplicates CommandParser)
4. Move execution and validation
5. AI move handling
6. **Save/Load operations** ← NEW in Phase 2
7. **Settings management** ← NEW in Phase 2
8. **State history & rollback** ← NEW in Phase 2
9. UI rendering coordination
10. Turn management
11. Check/checkmate detection coordination
12. Castle move handling

**Single Responsibility Principle Violations:**
- ChessGame should orchestrate gameplay, not handle persistence
- Settings management should be separate
- State history could be its own service
- Command handling could be more modular

---

## Proposed Modularization

### 1. Extract `GameStateManager` Class

**Purpose:** Manage game state snapshots, autosave, rollback, and redo

**File:** `/ShatranjCore/State/GameStateManager.cs`

**Responsibilities:**
- Maintain state history (last 10 turns)
- Maintain redo stack
- Autosave after each turn
- Rollback to previous state
- Redo to next state
- Cleanup autosave on game end

**Methods to Extract from ChessGame.cs:**
```csharp
public class GameStateManager
{
    private readonly SaveGameManager saveManager;
    private readonly List<GameStateSnapshot> stateHistory;
    private readonly Stack<GameStateSnapshot> redoStack;
    private readonly ILogger logger;

    // Constructor
    public GameStateManager(SaveGameManager saveManager, ILogger logger)

    // State management
    public void RecordState(GameStateSnapshot snapshot)
    public GameStateSnapshot Rollback()
    public GameStateSnapshot Redo()
    public void ClearRedoStack()
    public void Autosave(GameStateSnapshot snapshot)
    public void CleanupAutosave()
    public bool CanRollback()
    public bool CanRedo()
    public int GetStateCount()
}
```

**Lines Reduced:** ~150 lines from ChessGame.cs

---

### 2. Extract `SettingsManager` Class

**Purpose:** Handle all settings-related operations

**File:** `/ShatranjCore/Settings/SettingsManager.cs`

**Responsibilities:**
- Display settings menu
- Update profile names
- Update opponent name
- Update difficulty
- Reset settings to defaults

**Methods to Extract from ChessGame.cs:**
```csharp
public class SettingsManager
{
    private readonly GameConfigManager configManager;
    private readonly IRenderer renderer;

    // Constructor
    public SettingsManager(GameConfigManager configManager, IRenderer renderer)

    // Settings operations
    public void ShowSettingsMenu()
    public void SetProfileName(string name)
    public void SetOpponentName(string name)
    public void SetDifficulty(string difficulty)
    public void ResetToDefaults()

    // Display helpers
    private void DisplayCurrentSettings()
    private void DisplaySettingsOptions()
}
```

**Lines Reduced:** ~120 lines from ChessGame.cs

---

### 3. Extract `CommandHandler` Class

**Purpose:** Route commands to appropriate handlers

**File:** `/ShatranjCore/Handlers/CommandHandler.cs`

**Responsibilities:**
- Receive parsed commands
- Route to appropriate handler methods
- Return results/errors

**Methods to Extract from ChessGame.cs:**
```csharp
public class CommandHandler
{
    private readonly ChessGame game;
    private readonly GameStateManager stateManager;
    private readonly SettingsManager settingsManager;
    private readonly SaveGameManager saveManager;

    // Constructor
    public CommandHandler(ChessGame game, ...)

    // Command routing
    public void HandleCommand(GameCommand command)

    // Individual handlers
    public void HandleMove(GameCommand command)
    public void HandleCastle(GameCommand command)
    public void HandleShowMoves(GameCommand command)
    public void HandleSave(GameCommand command)
    public void HandleLoad(GameCommand command)
    public void HandleRollback()
    public void HandleRedo()
    public void HandleSettings(GameCommand command)
    public void HandleQuit()
}
```

**Lines Reduced:** ~200 lines from ChessGame.cs

---

### 4. Extract `AIMovementCoordinator` Class

**Purpose:** Handle AI move execution and display

**File:** `/ShatranjCore/AI/AIMovementCoordinator.cs`

**Responsibilities:**
- Request move from AI
- Execute AI move
- Display AI move information
- Handle AI move errors

**Methods to Extract from ChessGame.cs:**
```csharp
public class AIMovementCoordinator
{
    private readonly ChessBoard board;
    private readonly IRenderer renderer;
    private readonly ILogger logger;

    // Constructor
    public AIMovementCoordinator(ChessBoard board, IRenderer renderer, ILogger logger)

    // AI move handling
    public void ExecuteAIMove(IChessAI ai, PieceColor color)
    private void DisplayAIMoveInfo(ChessMove move, PieceColor color)
    private string LocationToAlgebraic(Location location)
}
```

**Lines Reduced:** ~100 lines from ChessGame.cs

---

## Impact Summary

### Before Modularization:
- **ChessGame.cs:** 1,384 lines
- **Responsibilities:** 12+ concerns

### After Modularization:
- **ChessGame.cs:** ~814 lines (41% reduction)
- **GameStateManager.cs:** ~150 lines (NEW)
- **SettingsManager.cs:** ~120 lines (NEW)
- **CommandHandler.cs:** ~200 lines (NEW)
- **AIMovementCoordinator.cs:** ~100 lines (NEW)

**Total Lines:** Same, but properly separated
**Maintainability:** Significantly improved
**Testability:** Each component can be tested independently
**SOLID Compliance:** Much better adherence to Single Responsibility Principle

---

## Additional Modularization Opportunities

### SaveGameManager.cs (310 lines) - GOOD SIZE ✓
Currently well-structured, no immediate changes needed.

### CommandParser.cs (392 lines) - ACCEPTABLE SIZE ✓
Could benefit from minor refactoring:
- Extract parsing helpers for different command types
- Not urgent, but consider for future

### ConsoleBoardRenderer.cs (350 lines) - ACCEPTABLE SIZE ✓
Well-structured, focused responsibility.

---

## Implementation Priority

### High Priority (Before Phase 3):
1. **Extract GameStateManager** (150 lines)
   - Most self-contained
   - Clearly defined boundaries
   - Critical for Phase 3 learning features

2. **Extract SettingsManager** (120 lines)
   - Self-contained
   - No complex dependencies
   - Improves testability

### Medium Priority:
3. **Extract CommandHandler** (200 lines)
   - More complex dependencies
   - Requires careful interface design
   - Can be done incrementally

### Low Priority:
4. **Extract AIMovementCoordinator** (100 lines)
   - Smallest impact
   - Low complexity
   - Can be deferred to Phase 3

---

## Recommended Approach

### Step 1: Extract GameStateManager
**Effort:** 2-3 hours
**Risk:** Low
**Benefit:** High (enables Phase 3 features)

### Step 2: Extract SettingsManager
**Effort:** 1-2 hours
**Risk:** Low
**Benefit:** Medium (improves clarity)

### Step 3: Update Tests
**Effort:** 2-3 hours
**Risk:** Medium
**Benefit:** High (ensures no regressions)

### Step 4: Consider CommandHandler (Optional)
**Effort:** 3-4 hours
**Risk:** Medium
**Benefit:** Medium (better organization)

---

## Benefits for Phase 3

### Game History & AI Learning:
- **GameStateManager** provides clean interface for recording all states
- Easy to extend for game history persistence
- State snapshots already contain all needed data

### Enhanced AI:
- **AIMovementCoordinator** can be extended with evaluation metrics
- Clean separation allows different AI implementations
- Easy to add learning feedback loops

### Testing:
- Each component can be unit tested independently
- Mock interfaces simplify integration tests
- Regression testing becomes more manageable

---

## Decision

**Recommendation:** Execute Step 1 (GameStateManager) and Step 2 (SettingsManager) before Phase 3.

**Rationale:**
- Combined effort: 3-5 hours
- Low risk
- High benefit for Phase 3 features
- Reduces ChessGame.cs by ~270 lines (20% reduction)
- Both are self-contained with clear boundaries

**Alternative:** Defer all modularization to Phase 4 cleanup
- Pros: Faster Phase 3 start
- Cons: Technical debt accumulates, harder to implement learning features

---

## Next Steps

1. Review and approve this plan
2. Create GameStateManager interface and implementation
3. Create SettingsManager interface and implementation
4. Update ChessGame.cs to use new managers
5. Update tests to cover new classes
6. Update documentation

---

**Last Updated:** 2025-11-10
**Status:** Pending Approval
