# Phase 3 - Test Plan for Refactored Components

> **Last Updated**: November 11, 2025
> **Version**: Phase 3 Test Planning
> **Status**: Planning & Implementation

---

## Overview

Phase 3 refactoring introduced 8 tasks across 30+ new files. Each refactored component requires comprehensive testing to verify functionality and prevent regressions.

**Refactoring Summary**:
- Command Handlers: 9 handlers + factory
- Export Strategies: 4 exporters (PGN, FEN)
- DI Container: ServiceRegistration with ServiceContainer
- Game Loop Variants: IGameLoopStrategy interface
- Move Validators: 3 pluggable validators
- Serializers: Format-specific serializers + factory
- State Query/Modifier: ISP implementation
- Learning Infrastructure: 4 interfaces + 8 data classes

---

## Test Categories

### 1. CommandHandler Tests

**Purpose**: Verify each handler processes commands correctly

**Files to Create**:
- `tests/ShatranjCore.Tests/Application/CommandHandlers/CommandHandlerTests.cs`
- `MoveCommandHandlerTests.cs`
- `CastleCommandHandlerTests.cs`
- `UICommandHandlerTests.cs`
- `PersistenceCommandHandlerTests.cs`
- `GameControlCommandHandlerTests.cs`
- `SettingsCommandHandlerTests.cs`
- `InvalidCommandHandlerTests.cs`

**Test Cases for MoveCommandHandler**:
```csharp
[Fact]
public void CanHandle_ValidMoveCommand_ReturnsTrue()
{
    // Arrange
    var handler = new MoveCommandHandler(...);
    var command = new GameCommand { Type = CommandType.Move };

    // Act
    var result = handler.CanHandle(command);

    // Assert
    Assert.True(result);
}

[Fact]
public void Handle_ValidMove_ExecutesMove()
{
    // Test normal piece movement
}

[Fact]
public void Handle_InvalidMove_ReturnsError()
{
    // Test illegal move handling
}

[Fact]
public void Handle_MoveExposesKing_ReturnsError()
{
    // Test king safety validation
}
```

**Test Cases for CommandHandlerFactory**:
```csharp
[Fact]
public void GetHandler_ValidCommandType_ReturnsCorrectHandler()
{
    // Test handler routing
}

[Fact]
public void RegisterHandler_NewHandler_CanBeRetrieved()
{
    // Test dynamic handler registration
}
```

**Expected Tests**: 8 handlers × 3 tests + factory tests = ~27 tests

---

### 2. Export Strategy Tests

**Purpose**: Verify PGN and FEN export functionality

**Files to Create**:
- `tests/ShatranjCore.Tests/Persistence/Exporters/PGNExporterTests.cs`
- `FENExporterTests.cs`

**Test Cases for PGNExporter**:
```csharp
[Fact]
public void Export_SingleMove_ProducesValidPGN()
{
    // Arrange
    var exporter = new PGNExporter();
    var moves = new List<MoveRecord>
    {
        new MoveRecord { From = "e2", To = "e4", Notation = "e4" }
    };
    var metadata = new PGNMetadata { Event = "Test" };

    // Act
    var pgn = exporter.Export(moves, metadata);

    // Assert
    Assert.Contains("1. e4", pgn);
    Assert.Contains("[Event \"Test\"]", pgn);
}

[Fact]
public void Export_CaptureMove_IncludesXNotation()
{
    // Test capture notation (exd5, Nxf3, etc.)
}

[Fact]
public void Export_CastlingMove_IncludesProperNotation()
{
    // Test castling notation (O-O, O-O-O)
}

[Fact]
public void Export_MetadataIncluded_ValidPGNFormat()
{
    // Test PGN tags (Event, Site, Date, White, Black, Result, etc.)
}
```

**Test Cases for FENExporter**:
```csharp
[Fact]
public void Export_InitialPosition_ProducesInitialFEN()
{
    // Arrange
    var board = new ChessBoard();
    var exporter = new FENExporter();

    // Act
    var fen = exporter.Export(board);

    // Assert
    Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", fen);
}

[Fact]
public void Export_CustomPosition_ProducesCorrectFEN()
{
    // Test various board positions
}

[Fact]
public void Export_CastlingRightsEncoded_Correct()
{
    // Test KQkq notation in FEN
}

[Fact]
public void Export_EnPassantEncoded_Correct()
{
    // Test en passant target encoding
}
```

**Expected Tests**: 4 exporters × 4 tests = ~16 tests

---

### 3. DI Container Tests

**Purpose**: Verify ServiceContainer registration and retrieval

**Files to Create**:
- `tests/ShatranjCore.Tests/ServiceContainerTests.cs`

**Test Cases**:
```csharp
[Fact]
public void RegisterService_Singleton_ReturnsInstance()
{
    // Arrange
    var container = new ServiceContainer();
    var logger = new ConsoleLogger();
    container.Register(typeof(ILogger), logger);

    // Act
    var retrieved = container.GetService(typeof(ILogger));

    // Assert
    Assert.Same(logger, retrieved);
}

[Fact]
public void RegisterService_TypeMapping_CreatesInstance()
{
    // Test type-based registration with auto-creation
}

[Fact]
public void GetService_NotRegistered_ReturnsNull()
{
    // Test missing service handling
}

[Fact]
public void GetNamedService_RegisteredByName_ReturnsCorrectInstance()
{
    // Test named service retrieval
}

[Fact]
public void RegisterCoreServices_AllRequired_AreRegistered()
{
    // Test complete setup via factory method
}

[Fact]
public void RegisterWithAI_AIServices_AreAvailable()
{
    // Test AI registration
}
```

**Expected Tests**: ~6-8 tests

---

### 4. GameLoop Strategy Tests

**Purpose**: Verify game loop strategy pattern works correctly

**Files to Create**:
- `tests/ShatranjCore.Tests/Application/GameLoops/GameLoopStrategyTests.cs`

**Test Cases**:
```csharp
[Fact]
public void StandardChessGameLoop_ImplementsInterface_Correctly()
{
    // Test interface compliance
}

[Fact]
public void Execute_GameFlow_WorksCorrectly()
{
    // Test complete game loop execution
}

[Fact]
public void IsGameOver_Checkmate_ReturnsTrue()
{
    // Test game over detection
}

[Fact]
public void AddStrategy_NewVariant_CanBeRegistered()
{
    // Test extensibility
}
```

**Expected Tests**: ~4 tests

---

### 5. Move Validator Tests

**Purpose**: Verify validator strategy implementations

**Files to Create**:
- `tests/ShatranjCore.Tests/Domain/Validators/PieceMoveValidatorTests.cs`
- `KingSafetyValidatorTests.cs`

**Test Cases for PieceMoveValidator**:
```csharp
[Fact]
public void Validate_LegalMove_ReturnsNull()
{
    // Test valid move passes
}

[Fact]
public void Validate_IllegalMove_ReturnsErrorMessage()
{
    // Test invalid move fails
}

[Fact]
public void Validate_PieceNotExist_ReturnsError()
{
    // Test source square empty
}

[Fact]
public void Validate_WrongPlayersPiece_ReturnsError()
{
    // Test opponent's piece
}
```

**Test Cases for KingSafetyValidator**:
```csharp
[Fact]
public void Validate_MoveExposesKing_ReturnsError()
{
    // Test king safety violation
}

[Fact]
public void Validate_KingStaySsSafe_ReturnsNull()
{
    // Test safe move passes
}

[Fact]
public void Validate_BlockingCheck_ReturnsNull()
{
    // Test blocking move
}
```

**Expected Tests**: 6 validators × 3 tests = ~18 tests

---

### 6. Serializer Tests

**Purpose**: Verify serialization strategy implementations

**Files to Create**:
- `tests/ShatranjCore.Tests/Persistence/Serializers/JsonGameSerializerTests.cs`
- `GameSerializerFactoryTests.cs`

**Test Cases for JsonGameSerializer**:
```csharp
[Fact]
public void Serialize_GameState_ProducesValidJson()
{
    // Test JSON output format
}

[Fact]
public void Deserialize_ValidJson_ReconstructsGameState()
{
    // Test round-trip serialization
}

[Fact]
public void Serialize_ComplexBoard_Handles Correctly()
{
    // Test complex board state
}

[Fact]
public void GetFormat_ReturnsJsonIdentifier()
{
    // Test format identification
}
```

**Test Cases for GameSerializerFactory**:
```csharp
[Fact]
public void Create_JsonFormat_ReturnsJsonSerializer()
{
    // Test factory routing
}

[Fact]
public void CreateDefault_ReturnsJsonByDefault()
{
    // Test default format
}

[Fact]
public void Create_AllFormats_ReturnAppropriateSerializer()
{
    // Test all supported formats
}
```

**Expected Tests**: ~7 tests

---

### 7. State Query/Modifier Tests

**Purpose**: Verify interface segregation

**Files to Create**:
- `tests/ShatranjCore.Tests/State/GameStateQueryTests.cs`
- `GameStateModifierTests.cs`
- `GameStateManagerIntegrationTests.cs`

**Test Cases**:
```csharp
[Fact]
public void Query_CanRollback_DetectsAvailableStates()
{
    // Test query functionality
}

[Fact]
public void Modifier_RecordState_AddsToHistory()
{
    // Test write functionality
}

[Fact]
public void Query_ReadOnlyAccess_NoWriteMethods()
{
    // Test interface segregation
}

[Fact]
public void Modifier_WriteOnlyAccess_NoReadMethods()
{
    // Test segregation
}

[Fact]
public void Manager_ImplementsBoth_AllMethodsAvailable()
{
    // Test combined interface
}

[Fact]
public void Rollback_UndoesState_QueryReflectsChange()
{
    // Test query after modification
}

[Fact]
public void Autosave_SavesState_CanBeRestored()
{
    // Test persistence
}
```

**Expected Tests**: ~8 tests

---

### 8. Learning Infrastructure Tests

**Purpose**: Verify learning component interfaces

**Files to Create**:
- `tests/ShatranjCore.Tests/Learning/GameRecorderTests.cs`
- `GameAnalyzerTests.cs` (mocked)
- `GameDatabaseTests.cs` (mocked)
- `AILearningEngineTests.cs` (mocked)

**Test Cases for IGameRecorder**:
```csharp
[Fact]
public void StartGame_InitializesRecording_IsRecordingReturnsTrue()
{
    // Test recording initialization
}

[Fact]
public void RecordMove_ValidMove_AddedToMoveList()
{
    // Test move recording
}

[Fact]
public void EndGame_WithResult_RecordsGameData()
{
    // Test game completion
}

[Fact]
public void GetRecordedGame_ReturnsCompleteGameRecord()
{
    // Test game retrieval
}
```

**Test Cases for IGameAnalyzer** (mocked):
```csharp
[Fact]
public void Analyze_GameRecord_ReturnsMoveCount()
{
    // Test basic analysis
}

[Fact]
public void FindMistakes_PoorMoves_IdentifiedCorrectly()
{
    // Test mistake detection
}

[Fact]
public void AnalyzeOpening_OpeningMoves_CategorizedCorrectly()
{
    // Test opening identification
}
```

**Expected Tests**: ~12 tests

---

## Regression Test Suite

### Existing Tests to Review

The following existing tests should be reviewed for obsolescence or updates:

1. **Piece Movement Tests** (existing - should still work)
   - BishopMovementTests.cs
   - KingMovementTests.cs
   - KnightMovementTests.cs
   - PawnMovementTests.cs
   - QueenMovementTests.cs
   - RookMovementTests.cs

   **Status**: ✅ These test domain logic that hasn't changed
   **Action**: Run and verify still passing

2. **Integration Tests** (existing)
   - AIIntegrationTests.cs
   - GameFlowTests.cs
   - SaveLoadTests.cs

   **Status**: ✅ These test high-level flows
   **Action**: Run and verify still passing

3. **Helper Classes** (existing)
   - MockFactory.cs
   - TestBoardFactory.cs

   **Status**: ✅ These support other tests
   **Action**: Verify still compatible with new architecture

---

## Test Statistics

### New Tests to Add

| Component | Test Count | Category |
|-----------|-----------|----------|
| CommandHandlers | 27 | Application |
| Exporters | 16 | Persistence |
| DI Container | 8 | Infrastructure |
| GameLoop Strategy | 4 | Application |
| Move Validators | 18 | Domain |
| Serializers | 7 | Persistence |
| State Query/Modifier | 8 | State |
| Learning Infrastructure | 12 | Learning |
| **Total New** | **100** | **All** |

### Existing Tests (to Verify)

| Category | Test Count |
|----------|-----------|
| Piece Movement | 36+ |
| Integration | 6 |
| AI | 6 |
| Logging | 6 |
| Movement History | 8 |
| Persistence | 8 |
| **Total Existing** | **70+** |

### Grand Total

- **Existing Tests**: 70+ (should still pass)
- **New Tests**: 100 (to be added)
- **Total**: 170+ comprehensive tests

---

## Test Execution Plan

### Phase 1: Verify Existing Tests
1. Run all existing tests
2. Verify no regressions from refactoring
3. Document any updates needed

### Phase 2: Implement New Tests
1. CommandHandler tests (highest priority - core functionality)
2. Exporter tests (game history feature)
3. Serializer tests (persistence)
4. Validator tests (move validation)
5. DI Container tests (infrastructure)
6. GameLoop tests (extensibility)
7. State tests (undo/redo)
8. Learning tests (Phase 3 foundation)

### Phase 3: Integration Testing
1. Test refactored components together
2. Verify no circular dependencies
3. Test DI container with real components
4. End-to-end game flow

### Phase 4: Performance Testing
1. Handler dispatch performance
2. Serialization speed
3. Validator execution time
4. Memory usage

---

## Success Criteria

- ✅ All 70+ existing tests pass
- ✅ 100+ new tests pass
- ✅ Code coverage: 80%+ for refactored components
- ✅ Zero build errors
- ✅ Zero warnings (except pre-existing obsolete deprecations)
- ✅ No circular dependencies
- ✅ All SOLID principles verified
- ✅ Performance benchmarks maintained or improved

---

## Next Steps

1. Start with CommandHandler tests (most critical)
2. Implement exporter tests (game history readiness)
3. Add serializer tests (persistence verification)
4. Create validator tests (move validation correctness)
5. Verify all existing tests still pass
6. Document test results

---

**Prepared for**: Phase 3 Modularization
**Test Framework**: xUnit
**Coverage Target**: 80%+ for new components
