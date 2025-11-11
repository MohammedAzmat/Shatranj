# Shatranj Test Suite Plan & Coverage Strategy

> **Last Updated**: November 11, 2025
> **Current Status**: 52 tests passing (100%)
> **Coverage Estimate**: ~60-70% (estimated)
> **Objective**: Achieve 80%+ coverage with comprehensive test suite

---

## Executive Summary

This document outlines a comprehensive test strategy for the Shatranj chess project, identifying gaps in coverage and establishing a plan for improvements.

**Key Findings:**
- ✅ **52 Tests Passing:** All current tests pass without failures
- ⚠️ **Critical Gap:** No logging tests (despite logging system in place)
- ⚠️ **Missing Coverage:** Persistence layer, Command parsing, UI renderers
- 🎯 **Target:** 80%+ code coverage with 100+ tests

---

## Current Test Suite Status

### Test Distribution

| Test Project | Tests | Status | Coverage |
|---|---|---|---|
| ShatranjCore.Tests | 40+ | ✅ Passing | ~70% |
| ShatranjAI.Tests | 6 | ✅ Passing | ~60% |
| ShatranjIntegration.Tests | 6 | ✅ Passing | ~50% |
| **TOTAL** | **52+** | **✅ 100%** | **~60-70%** |

---

## Detailed Test Analysis

### 1. ShatranjCore.Tests (40+ tests)

**Current Coverage:**

#### Piece Movement Tests (28 tests)
```
PawnTests.cs               [10 tests]
├── Single square forward
├── Double square (first move)
├── Diagonal captures
├── En passant
├── Promotion
└── Edge cases

RookTests.cs               [6 tests]
├── Horizontal movement
├── Vertical movement
├── Capture logic
└── Blocking logic

KnightTests.cs             [6 tests]
├── All 8 possible L-shaped moves
├── Jump over pieces
├── Capture
└── Board boundary

BishopTests.cs             [6 tests]
├── Diagonal movement
├── Capture
├── Blocking
└── Multiple diagonal directions

QueenTests.cs              [6 tests]
├── Combined rook + bishop movement
├── All 8 directions
├── Mobility tests

KingTests.cs               [6 tests]
├── Single square movement
├── All 8 directions
├── Cannot move into check
└── Castling validation
```

#### Special Move Tests (8 tests)
```
├── Castling (2 tests)
│   ├── Kingside castling
│   └── Queenside castling
├── En Passant (2 tests)
│   ├── En passant capture
│   └── En passant validation
├── Pawn Promotion (2 tests)
│   ├── Promotion options
│   └── Promotion validation
└── Check/Checkmate/Stalemate (2 tests)
    ├── Check detection
    └── Checkmate/stalemate
```

#### Board & Initialization Tests (4+ tests)
```
├── Board initialization
├── Piece placement
├── Move validation
└── Game state management
```

**Gaps Identified:**
- ❌ No CommandParser tests
- ❌ No ConsoleBoardRenderer tests
- ❌ No MoveHistory tests
- ❌ No CastlingValidator tests (covered by King tests partially)
- ❌ No Logging tests
- ❌ No error handling tests

---

### 2. ShatranjAI.Tests (6 tests)

**Current Coverage:**

#### AI Tests (3 tests)
```
BasicAITests.cs            [3 tests]
├── AI initialization
├── Legal move selection
└── Game rule compliance
```

#### Evaluation Tests (3 tests)
```
MoveEvaluatorTests.cs      [3 tests]
├── Material evaluation
├── Position scoring
└── Piece-square tables
```

**Gaps Identified:**
- ❌ No alpha-beta pruning tests
- ❌ No minimax depth tests
- ❌ No AI difficulty level tests (600/800/1000/1200/1400 ELO)
- ❌ No AI performance benchmarks
- ❌ No AI vs AI game tests

---

### 3. ShatranjIntegration.Tests (6 tests)

**Current Coverage:**

#### Full Game Flow Tests (3 tests)
```
GameFlowTests.cs           [3 tests]
├── Check detection in game
├── Castling in real game
└── En passant in real game
```

#### AI Integration Tests (3 tests)
```
AIIntegrationTests.cs      [3 tests]
├── AI full game playthrough
├── AI move validation
└── AI logging integration
```

**Gaps Identified:**
- ❌ No Human vs Human game tests
- ❌ No Human vs AI game tests
- ❌ No AI vs AI game tests (complete)
- ❌ No game result tests (checkmate, stalemate, draw)
- ❌ No edge case game tests

---

## Coverage Gap Analysis

### High Priority Gaps (Required for Phase 0-2)

#### 1. Logging Tests ❌ CRITICAL
**Lines of Code Untested:** ~300+ lines
**Priority:** CRITICAL
**Impact:** Essential for modularization debugging

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/Logging/LoggingTests.cs
public class LoggingTests
{
    [Test] TestConsoleLoggerOutput()          // Verify console output
    [Test] TestFileLoggerCreation()           // Verify file creation
    [Test] TestCompositeLoggerMultipleLogs()  // Test composite pattern
    [Test] TestLoggerErrorHandling()          // Test error scenarios
    [Test] TestLogLevelFiltering()            // If filtering implemented
    [Test] TestLogFileContents()              // Verify actual file content
}
```

**Target:** 6 tests

---

#### 2. CommandParser Tests ❌ HIGH
**Lines of Code Untested:** ~400+ lines
**Priority:** HIGH
**Impact:** Core user interaction

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/UI/CommandParserTests.cs
public class CommandParserTests
{
    [Test] TestParseMove()                    // e2 e4 format
    [Test] TestParseMoveFull()                // move e2 e4 format
    [Test] TestParseCastle()                  // castle king/queen
    [Test] TestParseHelp()                    // help [location]
    [Test] TestParseHistory()                 // history command
    [Test] TestParseSettings()                // settings command
    [Test] TestParseSave()                    // save command
    [Test] TestParseLoad()                    // load command
    [Test] TestParseInvalid()                 // Error handling
    [Test] TestParseEdgeCases()               // Empty, whitespace, etc.
}
```

**Target:** 10 tests

---

#### 3. MoveHistory Tests ❌ HIGH
**Lines of Code Untested:** ~150+ lines
**Priority:** HIGH
**Impact:** Game state tracking

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/Movement/MoveHistoryTests.cs
public class MoveHistoryTests
{
    [Test] TestAddMove()                      // Record move
    [Test] TestGetMoves()                     // Retrieve move list
    [Test] TestGetLastMove()                  // Last move only
    [Test] TestClear()                        // Clear history
    [Test] TestAlgebraicNotation()            // Move notation format
    [Test] TestMultipleMoves()                // Sequence of moves
    [Test] TestCaptureTracking()              // Track captures
    [Test] TestCheckTracking()                // Track checks
}
```

**Target:** 8 tests

---

#### 4. Persistence Tests ❌ HIGH
**Lines of Code Untested:** ~400+ lines
**Priority:** HIGH
**Impact:** Save/Load system

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/Persistence/PersistenceTests.cs
public class PersistenceTests
{
    [Test] TestSaveGameState()                // Save snapshot
    [Test] TestLoadGameState()                // Load snapshot
    [Test] TestGameStateRoundTrip()           // Save → Load
    [Test] TestBoardRestoration()             // Board state restored
    [Test] TestMoveSerialization()            // Move list preserved
    [Test] TestInvalidSaveFile()              // Error handling
    [Test] TestMultipleSaves()                // Handle multiple saves
    [Test] TestAutosaveCreation()             // Autosave works
}
```

**Target:** 8 tests

---

### Medium Priority Gaps (Recommended for Phase 3)

#### 5. Validator Tests ⚠️ MEDIUM
**Lines of Code Untested:** ~300+ lines
**Priority:** MEDIUM
**Impact:** Rule enforcement

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/Validators/ValidatorTests.cs
public class CastlingValidatorTests
{
    [Test] TestCanCastleKingside()            // Valid kingside
    [Test] TestCanCastleQueenside()           // Valid queenside
    [Test] TestCannotCastleIfMoved()          // King/Rook moved
    [Test] TestCannotCastleInCheck()          // King in check
    [Test] TestCannotCastleThruCheck()        // Pass through check
    [Test] TestCannotCastleIntoCheck()        // Land in check
    [Test] TestCannotCastleWithPieces()       // Pieces in way
}

public class EnPassantTrackerTests
{
    [Test] TestRecordPawnDoubleMove()         // Record move
    [Test] TestGetEnPassantTarget()           // Get target
    [Test] TestResetAfterOtherMoves()         // Reset on other moves
    [Test] TestMultipleEnPassantOpportunities()
}
```

**Target:** 11 tests

---

#### 6. UI Renderer Tests ⚠️ MEDIUM
**Lines of Code Untested:** ~350+ lines
**Priority:** MEDIUM
**Impact:** User experience

**Tests to Create:**

```csharp
// tests/ShatranjCore.Tests/UI/BoardRendererTests.cs
public class BoardRendererTests
{
    [Test] TestRenderEmptyBoard()             // Basic rendering
    [Test] TestRenderInitialPosition()        // Initial board
    [Test] TestRenderAfterMove()              // Position update
    [Test] TestHighlightLastMove()            // Move highlight
    [Test] TestDisplayCapturedPieces()        // Captures display
    [Test] TestCoordinateDisplay()            // Board labels
    [Test] TestUnicodeCharacters()            // Piece symbols
}
```

**Target:** 7 tests

---

#### 7. AI Enhancement Tests ⚠️ MEDIUM
**Lines of Code Untested:** ~200+ lines
**Priority:** MEDIUM
**Impact:** AI strength

**Tests to Create:**

```csharp
// tests/ShatranjAI.Tests/AIEnhancementTests.cs
public class AIEnhancementTests
{
    [Test] TestAIDepthVariation()             // Different search depths
    [Test] TestAIDifficultyLevels()           // All 5 difficulty levels
    [Test] TestAlphaBetaPruning()             // Pruning effectiveness
    [Test] TestMoveOrdering()                 // Move selection order
    [Test] TestEvaluationAccuracy()           // Scoring correctness
    [Test] TestAIPerformance()                // Speed benchmarks
}
```

**Target:** 6 tests

---

### Low Priority Gaps (Nice to Have)

#### 8. Edge Case & Error Tests ⚠️ LOW
**Priority:** LOW
**Impact:** Robustness

**Tests to Create:**
- Invalid move scenarios
- Boundary conditions
- Resource cleanup
- Exception handling

**Target:** 10+ tests

---

## Test Implementation Schedule

### Phase 0: Critical Tests (This Week)
**Target:** 20-30 new tests
- [ ] Logging Tests (6 tests) - CRITICAL
- [ ] CommandParser Tests (10 tests) - HIGH
- [ ] MoveHistory Tests (8 tests) - HIGH

**Effort:** 8-10 hours
**Tools Needed:** xUnit (optional but recommended)

### Phase 1: High Priority Tests (Week 2)
**Target:** 16-20 new tests
- [ ] Persistence Tests (8 tests)
- [ ] Validator Tests (11 tests) - subset

**Effort:** 6-8 hours

### Phase 2: Medium Priority Tests (Week 3+)
**Target:** 20+ new tests
- [ ] Validator Tests (complete)
- [ ] UI Renderer Tests (7 tests)
- [ ] AI Enhancement Tests (6 tests)

**Effort:** 10-12 hours

### Phase 3: Edge Cases & Polish (As Needed)
**Target:** 10+ tests
- Edge cases, error scenarios, performance

---

## Test Framework Recommendations

### Current State:
- ❌ No formal test framework (using custom TestRunner.cs)
- ✅ Manual test execution works
- ⚠️ No test discovery/automation
- ⚠️ No parameterized tests

### Recommendation: Migrate to xUnit
```bash
dotnet add ShatranjCore.Tests package xunit
dotnet add ShatranjCore.Tests package xunit.runner.console
```

**Benefits:**
- ✅ Industry standard
- ✅ Attribute-based test discovery
- ✅ Parameterized tests
- ✅ Test output reporting
- ✅ CI/CD integration ready

**Migration Effort:** 4-6 hours

---

## Coverage Metrics

### Current Estimated Coverage

| Component | Tested Lines | Total Lines | Coverage % |
|---|---|---|---|
| Pieces | 280 | 350 | ~80% |
| Board | 120 | 200 | ~60% |
| Validators | 150 | 350 | ~43% |
| Movement | 80 | 200 | ~40% |
| AI | 100 | 250 | ~40% |
| UI | 50 | 500 | ~10% |
| Logging | 0 | 300 | ~0% ❌ |
| Persistence | 50 | 400 | ~12% |
| **TOTAL** | **~830** | **~2,550** | **~33%** ❌ |

### Target Coverage After Tests

| Component | Target % | Strategy |
|---|---|---|
| Pieces | 95% | Comprehensive move tests |
| Board | 85% | State management tests |
| Validators | 90% | Rule validation tests |
| Movement | 85% | Move recording tests |
| AI | 85% | Algorithm + evaluation tests |
| UI | 80% | Rendering tests |
| Logging | 95% | All logging tests ✅ |
| Persistence | 90% | Save/load round-trip tests |
| **OVERALL TARGET** | **85%** | **100+ tests** |

---

## Test Execution Strategy

### Before Each Commit:
```bash
# Run all tests
cd tests/ShatranjCore.Tests && dotnet run
cd ../.. && cd ShatranjAI.Tests && dotnet run
cd .. && cd tests/ShatranjIntegration.Tests && dotnet run
```

### Automated Testing (Future):
```bash
dotnet test Shatranj.sln
```

### Coverage Reporting (Future):
```bash
dotnet add OpenCover
opencover.console.exe -target:"dotnet.exe" -targetargs:"test Shatranj.sln"
```

---

## Testing Best Practices

### For New Tests:
1. **Descriptive Names:** `Test[Component][Scenario][Expected]`
   - ✅ `TestPawnCanMoveForwardOneSquare`
   - ❌ `TestPawn`

2. **Isolation:** Each test independent
   - ✅ Create fresh board for each test
   - ❌ Depend on previous test state

3. **Assertions:** Clear pass/fail criteria
   - ✅ Assert.True(result)
   - ❌ Assert.NotNull(result)

4. **Edge Cases:** Test boundaries
   - ✅ Corner squares (a1, h8)
   - ✅ Empty board
   - ✅ Full board

5. **Documentation:** Comment complex tests
   ```csharp
   // Test that en passant only valid immediately after opponent's double move
   // If any other move happens, en passant target should be cleared
   ```

---

## Risk Mitigation

### Risk: Tests Take Too Long
**Mitigation:** Focus on critical tests first (Phase 0)

### Risk: Tests Are Brittle
**Mitigation:** Use mocks, dependency injection, isolated boards

### Risk: Tests Don't Catch Regressions
**Mitigation:** Add tests when bugs found (TDD approach)

---

## Success Criteria

### Phase 0 (Critical):
- [ ] 20+ new tests created
- [ ] Logging coverage 95%+
- [ ] CommandParser coverage 90%+
- [ ] All tests passing
- [ ] Overall coverage 45%+ (from 33%)

### Phase 1:
- [ ] 50+ total tests
- [ ] Persistence coverage 90%+
- [ ] Validator coverage 90%+
- [ ] Overall coverage 60%+

### Phase 2+:
- [ ] 80+ total tests
- [ ] Overall coverage 80%+
- [ ] All components >85% coverage
- [ ] Zero failing tests

---

## Next Steps

1. **Immediate:** Create Logging Tests
2. **This Week:** Create CommandParser Tests
3. **Next Week:** Create Persistence Tests
4. **Ongoing:** Add tests as features added

---

**Document Status:** Active Implementation Plan
**Last Updated:** November 11, 2025
**Next Review:** After Phase 0 completion
