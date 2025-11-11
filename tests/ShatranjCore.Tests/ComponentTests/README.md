# Component Test Suite

This directory contains unit tests for the 7 extracted components from Phase 2 refactoring.

## Test Coverage

### ✅ Completed Tests (4/7 - 26 test cases):

1. **TurnManagerTests.cs** (10 tests)
   - Turn switching between White/Black
   - Player state management
   - Integration with EnPassantTracker and StateManager
   - Comprehensive logging verification

2. **AIHandlerTests.cs** (6 tests)
   - AI move selection and validation
   - Error handling for null/failed moves
   - Logging of AI decisions and metrics
   - Integration with EnPassantTracker

3. **GameOrchestratorTests.cs** (4 tests)
   - Game loop coordination
   - Start/end sequence validation
   - Logging of orchestration events

4. **SnapshotManagerTests.cs** (6 tests)
   - Snapshot creation from game state
   - State restoration with validation
   - Error handling for invalid snapshots
   - Context preservation

### ⏳ Pending Tests (3/7):

5. **MoveExecutorTests** - Complex component requiring:
   - Move validation tests
   - Special move tests (en passant, castling, promotion)
   - Capture logic tests
   - Check/checkmate detection tests

6. **CommandProcessorTests** - Large component requiring:
   - Command routing tests for all command types
   - Move validation tests
   - Castle command tests
   - Delegate invocation tests

7. **GameLoopTests** - Integration-heavy component requiring:
   - Game flow tests
   - Win condition detection tests
   - Turn sequence tests
   - AI vs Human turn handling

## Test Framework

- **Framework**: xUnit
- **Mocking**: Moq
- **Coverage**: Unit tests with mocked dependencies

## Running Tests

```bash
dotnet test tests/ShatranjCore.Tests/
```

## Future Enhancements

The 3 pending test files require integration test setup due to their complex
dependencies. These will be added as part of Phase 6 (Testing & Validation).
