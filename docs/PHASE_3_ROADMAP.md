# Phase 3: AI Learning & Game History - Roadmap

## Overview

Phase 3 focuses on implementing AI learning capabilities through game history analysis, enhanced AI algorithms, and code modularization to support future development.

**Duration Estimate:** 4-6 weeks
**Priority:** High (Foundational for future AI improvements)
**Status:** Planning

---

## 🎯 Phase 3 Goals

1. **Game History System** - Persistent storage of all completed games
2. **AI Learning** - Enable AI to learn from game history
3. **Enhanced AI Algorithms** - Improve AI strength and speed
4. **Game Analysis Tools** - Provide move analysis and suggestions
5. **Code Modularization** - Extract managers from ChessGame.cs

---

## 📋 Feature Breakdown

### 1. Game History Persistence System

**Purpose:** Record and store all completed games for future analysis and AI learning

#### 1.1 GameHistory Data Model

**File:** `/ShatranjCore/Learning/GameHistory.cs`

```csharp
public class GameHistory
{
    public int HistoryId { get; set; }
    public int GameId { get; set; }
    public DateTime CompletedAt { get; set; }
    public GameMode GameMode { get; set; }
    public string WhitePlayerName { get; set; }
    public string BlackPlayerName { get; set; }
    public PlayerType WhitePlayerType { get; set; }  // Human or AI
    public PlayerType BlackPlayerType { get; set; }
    public DifficultyLevel Difficulty { get; set; }

    // Game outcome
    public GameResult Result { get; set; }  // WhiteWins, BlackWins, Draw, etc.
    public string Winner { get; set; }  // "White", "Black", or "Draw"
    public int TotalMoves { get; set; }
    public int GameDurationMinutes { get; set; }

    // Move sequence
    public List<MoveRecord> Moves { get; set; }

    // Opening classification
    public string OpeningName { get; set; }  // e.g., "Sicilian Defense", "King's Gambit"
    public string OpeningECO { get; set; }   // ECO code (e.g., "B20")

    // Statistics
    public int WhiteCapturedPieces { get; set; }
    public int BlackCapturedPieces { get; set; }
    public int CheckCount { get; set; }
    public bool HadCastling { get; set; }
    public bool HadEnPassant { get; set; }
    public bool HadPromotion { get; set; }
}

public class MoveRecord
{
    public int MoveNumber { get; set; }
    public PieceColor PlayerColor { get; set; }
    public string Move { get; set; }  // Algebraic notation (e.g., "Nf3", "e4")
    public Location From { get; set; }
    public Location To { get; set; }
    public string PieceType { get; set; }
    public bool WasCapture { get; set; }
    public bool WasCheck { get; set; }
    public bool WasCheckmate { get; set; }
    public int? EvaluationScore { get; set; }  // From AI perspective
    public TimeSpan MoveTime { get; set; }
}
```

#### 1.2 GameHistoryManager

**File:** `/ShatranjCore/Learning/GameHistoryManager.cs`

**Responsibilities:**
- Save completed games to database/file
- Query game history by filters
- Calculate statistics (win rates, common openings, etc.)
- Provide data for AI learning

**Methods:**
```csharp
public class GameHistoryManager
{
    public void SaveGameHistory(GameHistory history)
    public List<GameHistory> GetAllGames()
    public List<GameHistory> GetGamesByDifficulty(DifficultyLevel difficulty)
    public List<GameHistory> GetGamesByResult(GameResult result)
    public List<GameHistory> GetGamesByOpening(string openingName)

    // Statistics
    public Dictionary<GameResult, int> GetWinLossStats()
    public Dictionary<DifficultyLevel, double> GetWinRateByDifficulty()
    public Dictionary<string, int> GetMostCommonOpenings()
    public double GetAverageGameLength()

    // AI learning support
    public List<MoveRecord> GetAllMovesInPosition(string fen)
    public Dictionary<string, double> GetMoveSuccessRates(string position)
}
```

**Storage Options:**
1. **JSON Files** (Phase 3 MVP) - Simple, no dependencies
   - File: `game_history.json`
   - Append-only format
   - Easy to implement and debug

2. **SQLite Database** (Future Enhancement)
   - Better querying performance
   - Concurrent access support
   - SQL query capabilities

#### 1.3 Integration with ChessGame

**Changes to ChessGame.cs:**
```csharp
// Add field
private readonly GameHistoryManager historyManager;

// On game conclusion (checkmate/stalemate/draw)
private void OnGameConcluded(GameResult result)
{
    // Create game history record
    var history = BuildGameHistory(result);
    historyManager.SaveGameHistory(history);

    // Clean up autosave
    CleanupGameFiles();
}
```

**Estimated Lines:** ~200 lines (GameHistory.cs, GameHistoryManager.cs)

---

### 2. AI Learning from Game History

**Purpose:** Enable AI to improve by analyzing past games

#### 2.1 Opening Book Builder

**File:** `/ShatranjAI/Learning/OpeningBookBuilder.cs`

**Concept:**
- Analyze game history to find common opening sequences
- Build a database of opening positions and their success rates
- AI consults opening book for first 8-12 moves

```csharp
public class OpeningBookBuilder
{
    public void BuildOpeningBook(List<GameHistory> games)
    public Dictionary<string, OpeningBookEntry> GetOpeningBook()
}

public class OpeningBookEntry
{
    public string Position { get; set; }  // FEN notation
    public Dictionary<ChessMove, MoveStats> Moves { get; set; }
}

public class MoveStats
{
    public int TimesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public double SuccessRate => (Wins + Draws * 0.5) / TimesPlayed;
}
```

#### 2.2 Pattern Recognition

**File:** `/ShatranjAI/Learning/PatternRecognizer.cs`

**Concept:**
- Identify tactical patterns (pins, forks, skewers, discovered attacks)
- Learn which positions lead to successful tactics
- Weight positions similar to successful past games higher

```csharp
public class PatternRecognizer
{
    public List<TacticalPattern> FindPatterns(ChessBoard board)
    public double GetPositionSimilarity(ChessBoard current, ChessBoard historical)
}
```

#### 2.3 Learning-Enhanced AI

**File:** `/ShatranjAI/AI/LearningAI.cs`

**Extends BasicAI with:**
- Opening book consultation
- Pattern-based evaluation bonuses
- Adaptive depth based on game phase

```csharp
public class LearningAI : BasicAI
{
    private readonly OpeningBook openingBook;
    private readonly PatternRecognizer patternRecognizer;

    public override ChessMove GetMove(ChessBoard board, PieceColor color)
    {
        // Check opening book first
        if (moveCount < 12)
        {
            var bookMove = openingBook.GetBestMove(board);
            if (bookMove != null) return bookMove;
        }

        // Use enhanced evaluation with pattern recognition
        return base.GetMove(board, color);
    }
}
```

**Estimated Lines:** ~400 lines (OpeningBookBuilder, PatternRecognizer, LearningAI)

---

### 3. Enhanced AI Algorithms

#### 3.1 Iterative Deepening

**Purpose:** Search to progressively deeper depths, allowing time management

**Implementation:**
- Start with depth 1, gradually increase
- Use results from shallow search to improve move ordering
- Can be interrupted when time limit reached

**File:** `/ShatranjAI/AI/IterativeDeepeningAI.cs`

#### 3.2 Quiescence Search

**Purpose:** Avoid horizon effect by searching "quiet" positions

**Concept:**
- After reaching depth limit, continue searching capture sequences
- Prevents underestimating positions with pending captures
- Significantly improves tactical play

**Changes to:** `/ShatranjAI/AI/BasicAI.cs` (Minimax method)

#### 3.3 Transposition Tables

**Purpose:** Cache evaluated positions to avoid redundant calculations

**Implementation:**
- Use Zobrist hashing for position keys
- Store evaluation, depth, best move
- Huge performance improvement (2-3x speedup typical)

**File:** `/ShatranjAI/AI/TranspositionTable.cs`

```csharp
public class TranspositionTable
{
    private Dictionary<ulong, TranspositionEntry> table;

    public void Store(ulong hash, int depth, int eval, ChessMove bestMove)
    public TranspositionEntry Lookup(ulong hash)
    public void Clear()
}
```

#### 3.4 Move Ordering Improvements

**Purpose:** Search best moves first to maximize alpha-beta pruning

**Improvements:**
- Killer move heuristic
- History heuristic
- MVV-LVA (Most Valuable Victim - Least Valuable Attacker) for captures

**Estimated Lines:** ~500 lines (Iterative deepening, Quiescence, Transposition, Move ordering)

---

### 4. Game Analysis Tools

#### 4.1 Move Analyzer

**File:** `/ShatranjCore/Analysis/MoveAnalyzer.cs`

**Features:**
- Evaluate if move was best, good, inaccuracy, mistake, or blunder
- Suggest better alternatives
- Classify move types (tactical, positional, defensive)

```csharp
public class MoveAnalyzer
{
    public MoveEvaluation AnalyzeMove(ChessBoard board, ChessMove move, PieceColor color)
    public List<ChessMove> SuggestBetterMoves(ChessBoard board, PieceColor color)
}

public enum MoveQuality
{
    Best,           // Best move or within 10 centipawns
    Good,           // Within 50 centipawns
    Inaccuracy,     // 50-100 centipawns worse
    Mistake,        // 100-200 centipawns worse
    Blunder         // >200 centipawns worse
}
```

#### 4.2 Game Replay with Annotations

**File:** `/ShatranjCore/UI/GameReplayHandler.cs`

**Features:**
- Step through game move by move
- Show AI analysis at each position
- Highlight mistakes and suggest improvements

**Estimated Lines:** ~250 lines (MoveAnalyzer, GameReplayHandler)

---

### 5. Code Modularization

**Priority:** High (Should be done early in Phase 3)

See [**MODULARIZATION_PLAN.md**](MODULARIZATION_PLAN.md) for full details.

#### 5.1 Extract GameStateManager (Priority 1)
- Lines: ~150
- Effort: 2-3 hours
- Risk: Low

#### 5.2 Extract SettingsManager (Priority 2)
- Lines: ~120
- Effort: 1-2 hours
- Risk: Low

**Result:** ChessGame.cs reduced from 1,384 lines to ~1,114 lines (20% reduction)

---

## 🗓️ Implementation Timeline

### Week 1-2: Foundation & Modularization
- ✅ Day 1-2: Extract GameStateManager
- ✅ Day 3-4: Extract SettingsManager
- ✅ Day 5-7: Update tests, verify no regressions

### Week 2-3: Game History System
- 📋 Day 8-9: Implement GameHistory data model
- 📋 Day 10-11: Implement GameHistoryManager
- 📋 Day 12-13: Integrate with ChessGame
- 📋 Day 14: Write tests for game history

### Week 3-4: Enhanced AI Algorithms
- 📋 Day 15-16: Implement transposition tables with Zobrist hashing
- 📋 Day 17-18: Add quiescence search
- 📋 Day 19-20: Implement iterative deepening
- 📋 Day 21: Improve move ordering (killer moves, history heuristic)

### Week 4-5: AI Learning
- 📋 Day 22-23: Build OpeningBookBuilder
- 📋 Day 24-25: Implement PatternRecognizer
- 📋 Day 26-27: Create LearningAI class
- 📋 Day 28: Test AI improvements

### Week 5-6: Game Analysis & Polish
- 📋 Day 29-30: Implement MoveAnalyzer
- 📋 Day 31-32: Create GameReplayHandler
- 📋 Day 33-35: Integration testing and bug fixes
- 📋 Day 36-40: Documentation and cleanup

---

## 📊 Success Metrics

### Quantitative Metrics:
- AI strength improvement: Target 1600+ ELO (from current 1400)
- Search speed: 2-3x faster with transposition tables
- Opening play: 95% of games use book moves for first 8 moves
- Code organization: ChessGame.cs under 1,200 lines
- Test coverage: 80+ tests covering all Phase 3 features

### Qualitative Metrics:
- AI makes fewer tactical blunders
- Games feel more natural with book openings
- Code is more maintainable and testable
- Game history provides useful insights

---

## 🧪 Testing Strategy

### Unit Tests:
- GameHistory serialization/deserialization
- GameHistoryManager queries and statistics
- OpeningBook building and lookup
- TranspositionTable hash collisions
- Move analyzer accuracy

### Integration Tests:
- Complete game recording and retrieval
- AI using opening book in actual games
- Learning from game history
- Game replay functionality

### Performance Tests:
- Transposition table speedup measurement
- Iterative deepening time management
- Opening book lookup speed

---

## 📚 Documentation Updates

### Files to Create:
- `PHASE_3_ROADMAP.md` (this file) ✅
- `docs/GAME_HISTORY.md` - Game history system documentation
- `docs/AI_LEARNING.md` - AI learning algorithms explained
- `docs/GAME_ANALYSIS.md` - Using analysis tools

### Files to Update:
- `README.md` - Update Phase 3 status and features ✅
- `MODULARIZATION_PLAN.md` - Track refactoring progress ✅
- `docs/ARCHITECTURE.md` - Document new components

### Files to Archive:
- `PHASE_2_SAVE_LOAD_SYSTEM.md` → Move to `docs/archive/`

---

## 🚀 Getting Started with Phase 3

### Prerequisites:
- Phase 2 complete (100%) ✅
- All tests passing ✅
- Clean git working directory ✅

### First Steps:
1. Review and approve modularization plan
2. Create Phase 3 development branch
3. Extract GameStateManager (first refactoring)
4. Extract SettingsManager (second refactoring)
5. Begin GameHistory implementation

---

## 🔄 Dependencies

### External Libraries Needed:
None! Phase 3 uses only standard .NET libraries.

### Internal Dependencies:
- SaveGameManager (from Phase 2) ✅
- GameStateSnapshot (from Phase 2) ✅
- BasicAI and MoveEvaluator (from Phase 2) ✅

---

## ⚠️ Risks & Mitigation

### Risk 1: AI learning may not improve play significantly
**Mitigation:**
- Start with simple opening book (proven effective)
- Pattern recognition is bonus, not critical
- Iterative deepening and transposition tables are proven improvements

### Risk 2: Performance degradation with transposition tables
**Mitigation:**
- Implement incremental updates (Zobrist hashing)
- Use efficient hash table with replacement strategy
- Profile and optimize hot paths

### Risk 3: Game history file size growth
**Mitigation:**
- Compress old game history (gzip)
- Implement history pruning (keep last N games)
- Consider SQLite for better scalability

### Risk 4: Refactoring introduces bugs
**Mitigation:**
- Comprehensive test suite before refactoring
- Refactor incrementally, one manager at a time
- Extensive integration testing after each refactor

---

## 💡 Future Enhancements (Phase 4+)

- UCI protocol support for external engine analysis
- Cloud sync for game history
- Multiplayer game history tracking
- Advanced pattern recognition (neural networks)
- Real-time game coaching

---

**Last Updated:** 2025-11-10
**Status:** Planning Complete - Ready for Development
**Next Action:** Review and approve modularization plan, then begin implementation
