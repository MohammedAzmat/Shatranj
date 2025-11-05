# Shatranj Test Suite

This directory contains all automated tests for the Shatranj chess game.

## Structure

```
tests/
├── ShatranjCore.Tests/          # Unit tests for core game logic
│   ├── PieceTests/              # Tests for individual piece movements
│   │   ├── KingTests.cs
│   │   ├── QueenTests.cs
│   │   ├── RookTests.cs
│   │   ├── BishopTests.cs
│   │   ├── KnightTests.cs
│   │   └── PawnTests.cs
│   ├── BoardTests/              # Tests for board and square logic
│   │   ├── ChessBoardTests.cs
│   │   └── SquareTests.cs
│   └── GameTests/               # Tests for game rules and orchestration
│       ├── MoveValidatorTests.cs
│       ├── CheckDetectionTests.cs
│       ├── CheckmateTests.cs
│       ├── CastlingTests.cs
│       ├── EnPassantTests.cs
│       └── PromotionTests.cs
└── ShatranjIntegration.Tests/   # Integration tests for complete game flows
    ├── FullGameTests.cs
    └── SpecialMovesIntegrationTests.cs
```

## Test Framework

- **Framework**: NUnit / xUnit / MSTest (to be decided)
- **Mocking**: Moq (if needed for dependencies)
- **Coverage Target**: >80% for core business logic

## Running Tests

### Command Line
```bash
dotnet test
```

### Visual Studio
- Test Explorer → Run All Tests

### Individual Test Projects
```bash
dotnet test tests/ShatranjCore.Tests
dotnet test tests/ShatranjIntegration.Tests
```

## Writing Tests

### Test Naming Convention
```csharp
[Test]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    // Act
    // Assert
}
```

### Example
```csharp
[Test]
public void ValidMoves_KnightInCenter_Returns8Moves()
{
    // Arrange
    var board = new ChessBoard();
    var knight = new Knight(PieceColor.White, new Location(4, 4));

    // Act
    var moves = knight.ValidMoves(board);

    // Assert
    Assert.AreEqual(8, moves.Count);
}
```

## Test Categories

Use categories to organize test runs:

```csharp
[Test]
[Category("Piece")]
[Category("Knight")]
public void Knight_Movement_Test() { }

[Test]
[Category("Integration")]
public void FullGame_Test() { }
```

## Coverage Requirements

| Component | Minimum Coverage |
|-----------|-----------------|
| Piece Classes | 95% |
| ChessBoard | 90% |
| MoveValidator | 95% |
| GameState | 85% |
| UI/Rendering | 50% (lower priority) |

## Continuous Integration

Tests will run automatically on:
- Every commit to feature branches
- Pull requests to main branch
- Nightly builds

## Test Data

Use the `TestData/` directory for:
- Sample game positions (FEN notation)
- PGN files for game replay tests
- Expected move lists

## TODO

- [ ] Set up test project files (.csproj)
- [ ] Add NUnit/xUnit package references
- [ ] Configure test runner in CI/CD
- [ ] Write tests for existing piece implementations
- [ ] Add coverage reporting
- [ ] Create test data fixtures
