# Shatranj Testing Documentation

This document describes the testing architecture and how to run tests for the Shatranj chess game.

## Test Project Structure

The project has three separate test projects to maintain clear separation of concerns:

### 1. ShatranjCore.Tests (`tests/ShatranjCore.Tests/`)

**Purpose:** Unit tests for core game functionality

**Scope:**
- Piece movement logic (Pawn, Knight, Bishop, Rook, Queen, King)
- Board state management
- Game rules validation
- Initial game setup
- Move validation

**Dependencies:**
- ShatranjCore.Abstractions
- ShatranjCore

**Run Tests:**
```bash
cd tests/ShatranjCore.Tests
dotnet run
```

**Test Files:**
- `TestRunner.cs` - Main test orchestrator
- `PieceTests/PawnTests.cs` - Pawn movement tests
- `PieceTests/KnightTests.cs` - Knight movement tests
- `PieceTests/BishopTests.cs` - Bishop movement tests
- `PieceTests/RookTests.cs` - Rook movement tests
- `PieceTests/QueenTests.cs` - Queen movement tests
- `PieceTests/KingTests.cs` - King movement tests
- `AllPiecesMovementTest.cs` - Comprehensive movement test
- `InitialGameMoveTest.cs` - Initial game state test

### 2. ShatranjAI.Tests (`ShatranjAI.Tests/`)

**Purpose:** Unit tests for AI functionality

**Scope:**
- AI move selection algorithms
- Position evaluation
- Minimax algorithm with alpha-beta pruning
- Move evaluator logic

**Dependencies:**
- ShatranjCore.Abstractions
- ShatranjCore
- ShatranjAI

**Run Tests:**
```bash
cd ShatranjAI.Tests
dotnet run
```

**Test Files:**
- `TestRunner.cs` - Main test orchestrator
- `BasicAITests.cs` - AI initialization and move selection tests
- `MoveEvaluatorTests.cs` - Position evaluation tests

### 3. ShatranjIntegration.Tests (`tests/ShatranjIntegration.Tests/`)

**Purpose:** Integration tests for complete system functionality

**Scope:**
- AI integration with core game engine
- Full game flow from start to finish
- Inter-component communication
- Check detection integration
- Castling integration
- En passant integration

**Dependencies:**
- ShatranjCore.Abstractions
- ShatranjCore
- ShatranjAI

**Run Tests:**
```bash
cd tests/ShatranjIntegration.Tests
dotnet run
```

**Test Files:**
- `TestRunner.cs` - Main test orchestrator
- `AIIntegrationTests.cs` - AI integration tests
- `GameFlowTests.cs` - Complete game flow tests

## Running All Tests

To run all test suites in sequence:

```bash
# Core tests
cd tests/ShatranjCore.Tests && dotnet run
cd ../..

# AI tests
cd ShatranjAI.Tests && dotnet run
cd ..

# Integration tests
cd tests/ShatranjIntegration.Tests && dotnet run
cd ../..
```

## Test Architecture

```
┌─────────────────────────────────────────────────────┐
│                 Test Pyramid                        │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Integration Tests (ShatranjIntegration.Tests)     │
│  ┌───────────────────────────────────────────┐     │
│  │  Full game flows, AI vs Core, etc        │     │
│  └───────────────────────────────────────────┘     │
│                                                     │
│  Unit Tests (ShatranjAI.Tests)                     │
│  ┌───────────────────────────────────────────┐     │
│  │  AI algorithms, Move evaluation           │     │
│  └───────────────────────────────────────────┘     │
│                                                     │
│  Unit Tests (ShatranjCore.Tests)                   │
│  ┌───────────────────────────────────────────┐     │
│  │  Piece logic, Board state, Rules          │     │
│  └───────────────────────────────────────────┘     │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## Test Coverage

### Core Tests (40+ tests)
- ✅ All piece movements (legal and illegal)
- ✅ Castling (kingside and queenside)
- ✅ En passant captures
- ✅ Pawn promotion
- ✅ Check detection
- ✅ Checkmate detection
- ✅ Stalemate detection
- ✅ Initial board setup

### AI Tests (6 tests)
- ✅ AI initialization
- ✅ Legal move selection
- ✅ Material evaluation
- ✅ Position evaluation
- ✅ Capture detection
- ✅ Piece value constants

### Integration Tests (6 tests)
- ✅ AI full game playthrough
- ✅ AI move validation
- ✅ AI with logging integration
- ✅ Check detection in game
- ✅ Castling rights tracking
- ✅ En passant integration

## Adding New Tests

### For Core Functionality
1. Add test file to `tests/ShatranjCore.Tests/`
2. Follow existing test pattern
3. Update `TestRunner.cs` to include new test

### For AI Functionality
1. Add test file to `ShatranjAI.Tests/`
2. Follow existing test pattern
3. Update `TestRunner.cs` to include new test

### For Integration
1. Add test file to `tests/ShatranjIntegration.Tests/`
2. Test cross-component functionality
3. Update `TestRunner.cs` to include new test

## Test Best Practices

1. **Naming:** Use descriptive test method names (e.g., `TestPawnCanMoveForwardOneSquare`)
2. **Isolation:** Each test should be independent
3. **Clear Output:** Use colored console output (Green = Pass, Red = Fail, Yellow = Partial)
4. **Exception Handling:** Wrap tests in try-catch blocks
5. **Documentation:** Add XML comments to test classes and methods

## Continuous Testing

While we don't have CI/CD set up yet, you should run all tests:
- Before committing changes
- After adding new features
- When refactoring code
- When fixing bugs

## Future Enhancements

- [ ] Add xUnit/NUnit for automated test running
- [ ] Implement test coverage reporting
- [ ] Add performance benchmarks
- [ ] Create mock objects for better isolation
- [ ] Add parameterized tests
- [ ] Implement CI/CD pipeline with automated testing
