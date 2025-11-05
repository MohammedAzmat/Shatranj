# Shatranj - Persian Chess Game

[![Phase](https://img.shields.io/badge/Phase-1%20(100%25%20Complete)-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)]()
[![License](https://img.shields.io/badge/License-MIT-green)]()
[![Tests](https://img.shields.io/badge/Tests-40%20Passing-brightgreen)]()

A fully-featured chess game built with **SOLID principles** from the ground up. "Shatranj" is the Persian word for chess.

![Shatranj Chess Board](docs/screenshots/board-example.png)
*Beautiful terminal-based chess interface with color-coded pieces*

## 🎯 Project Vision

Build a complete chess game through iterative phases:
- **Phase 1**: Human vs Human (Command Line) ✅ *Complete!*
- **Phase 2**: AI Integration (Basic) ← *Next Phase*
- **Phase 3**: AI with Difficulty Levels & AI vs AI Learning
- **Phase 4**: Online Multiplayer
- **Phase 5**: GUI Implementation

## ✨ Features (Phase 1)

### Core Gameplay
- ✅ Complete piece movement logic (King, Queen, Rook, Bishop, Knight, Pawn)
- ✅ Move validation and legal move detection
- ✅ Check, checkmate, and stalemate detection
- ✅ Castling (kingside and queenside with check validation)
- ✅ Pawn promotion with interactive piece selection
- ✅ En passant special pawn capture
- ✅ Capture detection and tracking
- ✅ Move simulation to prevent king exposure

### User Interface
- ✅ Beautiful terminal UI with Unicode box-drawing
- ✅ Color-coded pieces (White/Red)
- ✅ Checkerboard pattern
- ✅ Move highlighting (last move shown)
- ✅ Captured pieces display
- ✅ Move history tracking

### Commands
- ✅ Move pieces with algebraic notation
- ✅ Interactive castling with prompts
- ✅ Pawn promotion with ESC cancellation
- ✅ Help system showing legal moves
- ✅ Game control (start, restart, end)
- ✅ Move history viewer

### Architecture
- ✅ SOLID principles applied throughout
- ✅ Dependency Inversion (IChessBoard interface)
- ✅ Single Responsibility (separate renderers, validators, parsers)
- ✅ Unit test infrastructure
- ✅ Comprehensive documentation

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** or higher
- **Visual Studio 2022+**, **Visual Studio Code**, or **.NET CLI**
- **Windows**, **Linux**, or **macOS** (cross-platform support)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YourUsername/Shatranj.git
   cd Shatranj
   ```

2. **Build the solution**

   **Using Visual Studio:**
   - Open `Shatranj.sln`
   - Build → Build Solution (Ctrl+Shift+B)

   **Using .NET CLI:**
   ```bash
   # Build entire solution
   dotnet build Shatranj.sln

   # Or build specific project
   dotnet build ShatranjCMD/ShatranjCMD.csproj
   ```

3. **Run the game**

   **Using Visual Studio:**
   - Set `ShatranjCMD` as startup project
   - Press F5 or click Start

   **Using .NET CLI:**
   ```bash
   dotnet run --project ShatranjCMD
   ```

   **Or run the executable directly:**
   ```bash
   ./ShatranjCMD/bin/Debug/ShatranjCMD.exe
   ```

## 🧪 Running Tests

### Run All Tests

```bash
# Using dotnet CLI
dotnet test

# Or run test runner directly
dotnet run --project tests/ShatranjCore.Tests
```

### Run Specific Test Categories

```bash
# Build and run test project
cd tests/ShatranjCore.Tests
dotnet build
dotnet run
```

### Test Coverage

Complete test coverage (40 tests):
- ✅ Rook movement (6 tests)
- ✅ Knight movement (6 tests)
- ✅ Bishop movement (6 tests)
- ✅ Queen movement (6 tests)
- ✅ King movement (6 tests)
- ✅ Pawn movement (10 tests including en passant)
- ✅ All tests passing

## 📖 Game Commands

### Moving Pieces

Move a piece using algebraic notation:

```bash
move [start] [end]
```

**Examples:**
```bash
move e2 e4      # Move pawn from e2 to e4
move g1 f3      # Move knight from g1 to f3
move e1 g1      # Move king (not castling, just move)
```

**Algebraic Notation:**
- **Files** (columns): `a` through `h` (left to right)
- **Ranks** (rows): `1` through `8` (bottom to top for white)
- Example: `e2` = column E, row 2

### Castling

Castle your king for safety:

```bash
castle [side]
```

**Examples:**
```bash
castle              # Prompts you to choose side if both available
castle king         # Castle kingside (O-O)
castle queen        # Castle queenside (O-O-O)
castle k            # Shorthand for kingside
castle q            # Shorthand for queenside
```

**Castling Rules:**
- King and rook must not have moved
- No pieces between king and rook
- King cannot be in check
- King cannot pass through check
- King cannot end in check

### Getting Help

Show available moves or command list:

```bash
help [location]     # Show legal moves for piece at location
help                # Show all available commands
```

**Examples:**
```bash
help e2             # Show all legal moves for piece at e2
help d7             # Show all legal moves for piece at d7
help                # Display full command list
```

### Move History

View all moves played in the game:

```bash
history
```

**Output Format:**
```
Move History:
─────────────────────────────────────────
  1. e2-e4          e7-e5
  2. Ng1-f3         Nb8-c6
  3. Bf1-c4         Bf8-c5
─────────────────────────────────────────
```

### Game Control

Manage the game state:

```bash
game start          # Start a new game
game restart        # Restart current game
game end            # End current game
game save           # Save game (not yet implemented)
```

### Quitting

Exit the game:

```bash
quit                # Exit the game
exit                # Also exits the game
```

## 🎮 Gameplay Example

Here's a complete game session:

```bash
╔════════════════════════════════════════════════════════════════╗
║                        SHATRANJ CHESS                          ║
╚════════════════════════════════════════════════════════════════╝

     A      B      C      D      E      F      G      H
   ╔═══════╦═══════╦═══════╦═══════╦═══════╦═══════╦═══════╦═══════╗
 8 ║  r    ║  n    ║  b    ║  q    ║  k    ║  b    ║  n    ║  r    ║ 8
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 7 ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║ 7
   ...
 2 ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║ 2
 1 ║  R    ║  N    ║  B    ║  Q    ║  K    ║  B    ║  N    ║  R    ║ 1
   ╚═══════╩═══════╩═══════╩═══════╩═══════╩═══════╩═══════╩═══════╝

┌────────────────────────────────────────────────────────────────┐
│ Current Turn: White                                            │
└────────────────────────────────────────────────────────────────┘

White > move e2 e4
ℹ Move executed successfully

Black > move e7 e5

White > move g1 f3

Black > help d7
Possible moves for piece at d7:
  → d6
  → d5

Black > move d7 d6

White > castle k
ℹ White castles kingside!

Black > history
Move History:
─────────────────────────────────────────
  1. e2-e4          e7-e5
  2. Ng1-f3         d7-d6
  3. O-O
─────────────────────────────────────────
```

### Pawn Promotion Example

When a pawn reaches the end of the board:

```bash
White > move a7 a8

╔════════════════════════════════════════════════════════════════╗
║                    PAWN PROMOTION!                             ║
╚════════════════════════════════════════════════════════════════╝

Choose a piece to promote to:
  1. Queen   (Q)
  2. Rook    (R)
  3. Bishop  (B)
  4. Knight  (N)

Press ESC to cancel (pawn will not move)

Your choice: q
ℹ Pawn promoted to Queen!
```

## 🏗️ Project Structure

```
Shatranj/
├── ShatranjCore/              # Core game logic (DLL)
│   ├── Pieces/                # Piece implementations
│   │   ├── Piece.cs          # Abstract base class
│   │   ├── King.cs
│   │   ├── Queen.cs
│   │   ├── Rook.cs
│   │   ├── Bishop.cs
│   │   ├── Knight.cs
│   │   └── Pawn.cs
│   ├── IChessBoard.cs         # Board interface (DIP)
│   ├── ChessBoard.cs          # Board implementation
│   ├── EnhancedChessGame.cs   # Game orchestration
│   ├── CommandParser.cs       # Command parsing (SRP)
│   ├── ConsoleBoardRenderer.cs # Terminal display (SRP)
│   ├── MoveHistory.cs         # Move tracking (SRP)
│   ├── CastlingValidator.cs   # Castling logic (SRP)
│   ├── PawnPromotionHandler.cs # Promotion logic (SRP)
│   ├── CheckDetector.cs       # Check/checkmate detection (SRP)
│   └── EnPassantTracker.cs    # En passant tracking (SRP)
├── ShatranjCMD/               # Console application
│   └── Program.cs             # Entry point
├── ShatranjMain/              # Windows Forms GUI (Phase 5)
├── tests/                     # Unit tests
│   ├── ShatranjCore.Tests/   # Core logic tests
│   │   ├── PieceTests/
│   │   │   ├── RookTests.cs     # 6 tests
│   │   │   ├── KnightTests.cs   # 6 tests
│   │   │   ├── BishopTests.cs   # 6 tests
│   │   │   ├── QueenTests.cs    # 6 tests
│   │   │   ├── KingTests.cs     # 6 tests
│   │   │   └── PawnTests.cs     # 10 tests
│   │   └── TestRunner.cs
│   └── README.md
├── docs/                      # Documentation
│   ├── PROJECT_ROADMAP.md     # Development phases
│   ├── SOLID_PRINCIPLES.md    # Architecture guide
│   └── TERMINAL_COMMANDS.md   # Command reference
└── README.md                  # This file
```

## 🔧 Development

### SOLID Principles

This project is built with SOLID principles as a learning exercise:

- **S**ingle Responsibility: Each class has one job
  - `ConsoleBoardRenderer` only renders
  - `CommandParser` only parses
  - `MoveHistory` only tracks moves

- **O**pen/Closed: Extensible without modification
  - `Piece` hierarchy allows new piece types
  - `IChessBoard` allows different board implementations

- **L**iskov Substitution: All pieces are interchangeable
  - Any `Piece` subclass works where `Piece` is expected

- **I**nterface Segregation: Focused interfaces
  - `IChessBoard` has only essential methods
  - No unnecessary dependencies

- **D**ependency Inversion: Depend on abstractions
  - `EnhancedChessGame` depends on `IChessBoard`, not `ChessBoard`
  - Easy to test and mock

Read more in [`docs/SOLID_PRINCIPLES.md`](docs/SOLID_PRINCIPLES.md)

### Adding New Features

1. **Create feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Follow SOLID principles**
   - One responsibility per class
   - Use interfaces for dependencies
   - Write tests first (TDD)

3. **Run tests**
   ```bash
   dotnet test
   ```

4. **Update documentation**
   - Update `PROJECT_ROADMAP.md`
   - Update `SOLID_PRINCIPLES.md` if needed
   - Add command documentation if new commands

5. **Commit with descriptive message**
   ```bash
   git add .
   git commit -m "feat: Add your feature description"
   ```

## 📋 Command Quick Reference

| Command | Description | Example |
|---------|-------------|---------|
| `move [start] [end]` | Move a piece | `move e2 e4` |
| `castle [side]` | Castle king | `castle king` |
| `help [location]` | Show legal moves | `help e2` |
| `help` | Show commands | `help` |
| `history` | View move history | `history` |
| `game start` | New game | `game start` |
| `game restart` | Restart game | `game restart` |
| `game end` | End game | `game end` |
| `quit` | Exit | `quit` |

### Special Keys

| Key | Function |
|-----|----------|
| `ESC` | Cancel castling or pawn promotion |
| `Enter` | Confirm input |

## 🎯 Current Status

### Phase 1: ✅ 100% Complete!

**All Phase 1 Objectives Achieved:**
- ✅ All piece movement implementations (Rook, Knight, Bishop, Queen, King, Pawn)
- ✅ Terminal UI with colors and Unicode formatting
- ✅ Command parsing system with comprehensive command support
- ✅ Move validation and legal move detection
- ✅ Check, checkmate, and stalemate detection
- ✅ Castling (kingside and queenside with full validation)
- ✅ Pawn promotion with interactive selection and ESC cancellation
- ✅ En passant special pawn capture
- ✅ Move history tracking with algebraic notation
- ✅ Captured pieces tracking and display
- ✅ Complete unit test coverage (40 tests passing)
- ✅ Comprehensive documentation

**Ready for Phase 2: AI Integration**

**Future Enhancements (Post-Phase 1):**
- ⚪ Draw conditions (threefold repetition, fifty-move rule, insufficient material)
- ⚪ Game save/load functionality
- ⚪ Move time tracking
- ⚪ Opening book integration

## 🐛 Known Issues

**Phase 1 - None! All core chess rules implemented.**

**Future Enhancements:**
- Draw conditions not yet implemented (threefold repetition, fifty-move rule, insufficient material)
- Game save/load functionality not implemented
- No opening book or endgame tablebase integration

## 🤝 Contributing

Contributions are welcome! Please:

1. Read [`docs/SOLID_PRINCIPLES.md`](docs/SOLID_PRINCIPLES.md)
2. Follow the existing code style
3. Write tests for new features
4. Update documentation

## 📚 Documentation

- **[Project Roadmap](docs/PROJECT_ROADMAP.md)** - Development phases and timeline
- **[SOLID Principles](docs/SOLID_PRINCIPLES.md)** - Architecture and design decisions
- **[Terminal Commands](docs/TERMINAL_COMMANDS.md)** - Detailed command reference

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built as a learning exercise for SOLID principles
- Inspired by classic chess implementations
- "Shatranj" - the Persian word for chess

## 📞 Support

For issues or questions:
- Create an issue on GitHub
- Check existing documentation in `docs/`

---

**Happy Chess Playing! ♟️**

*Built with ❤️ and SOLID principles*
