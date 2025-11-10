# Shatranj - Persian Chess Game

[![Phase](https://img.shields.io/badge/Phase-2%20(100%25%20Complete)-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)]()
[![License](https://img.shields.io/badge/License-MIT-green)]()
[![Tests](https://img.shields.io/badge/Tests-52%20Passing-brightgreen)]()

A fully-featured chess game built with **SOLID principles** from the ground up. "Shatranj" is the Persian word for chess.

![Shatranj Chess Board](docs/screenshots/board-example.png)
*Beautiful terminal-based chess interface with color-coded pieces*

## 🎯 Project Vision

Build a complete chess game through iterative phases. See the complete [**Development Roadmap**](docs/ROADMAP.md) for detailed phase information.

**Current Status:** Phase 2 - COMPLETE! ✅ Full save/load system, autosave, rollback, settings, and 5 difficulty levels

## ✨ Current Features

### Game Modes
- ✅ **Human vs Human** - Classic two-player chess
- ✅ **Human vs AI** - Play against BasicAI (depth 3 minimax)
- ✅ **AI vs AI** - Watch two AIs compete

### Core Gameplay
- ✅ Complete piece movement logic (King, Queen, Rook, Bishop, Knight, Pawn)
- ✅ All special moves (Castling, En Passant, Pawn Promotion)
- ✅ Check, checkmate, and stalemate detection
- ✅ Move validation and legal move highlighting
- ✅ Move history with algebraic notation
- ✅ Captured pieces tracking

### AI Features
- ✅ Minimax algorithm with alpha-beta pruning
- ✅ Position evaluation (material + piece-square tables)
- ✅ Depth 3 search (~800 ELO equivalent)
- ✅ Legal move validation
- ✅ Integrated logging system

### User Interface
- ✅ Beautiful terminal UI with Unicode characters
- ✅ Color-coded pieces and checkerboard
- ✅ Last move highlighting
- ✅ Interactive command system

## 🚀 Getting Started

### Prerequisites

- **.NET 9 SDK** or higher
- **Terminal** with Unicode support (Windows Terminal, iTerm2, etc.)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YourUsername/Shatranj.git
   cd Shatranj
   ```

2. **Build the solution**
   ```bash
   dotnet build Shatranj.sln
   ```

3. **Run the game**
   ```bash
   dotnet run --project ShatranjCMD
   ```

## 🎮 How to Play

### Game Modes

When you start the game, you'll see a menu:

```
1. Human vs Human
2. Human vs AI
3. AI vs AI
```

- **Option 1:** Play against another human
- **Option 2:** Play against AI (choose your color)
- **Option 3:** Watch two AIs play

## 📖 Game Commands

### Moving Pieces

Move a piece using algebraic notation:

```bash
move [start] [end]
```

**Examples:**
```bash
move e2 e4      # King's pawn opening
move g1 f3      # Develop knight to f3
move b8 c6      # Develop knight to c6
move d2 d4      # Queen's pawn opening
```

**Notation Guide:**
- Files (columns): `a-h` (left to right)
- Ranks (rows): `1-8` (bottom to top from white's perspective)

### Castling

Castle your king for safety:

```bash
castle [side]
```

**Examples:**
```bash
castle              # Interactive prompt for side selection
castle king         # Castle kingside (O-O) - King moves to g1/g8
castle queen        # Castle queenside (O-O-O) - King moves to c1/c8
castle k            # Shorthand for kingside
castle q            # Shorthand for queenside
```

**Requirements:**
- King and rook haven't moved
- No pieces between them
- King not in check
- King doesn't pass through check

### Getting Help

Show available moves or command list:

```bash
help [location]     # Show legal moves for piece at location
help                # Show all available commands
```

**Examples:**
```bash
help e2             # Shows: d3, d4, e3, e4 (pawn can move 1 or 2)
help g1             # Shows: f3, h3 (knight's legal moves)
help                # Display full command reference
```

### Move History

View all moves played in the game:

```bash
history
```

**Example Output:**
```
Move History:
─────────────────────────────────────────
  1. e2-e4          e7-e5
  2. Ng1-f3         Nb8-c6
  3. Bf1-c4         Bf8-c5
  4. O-O            Ng8-f6
─────────────────────────────────────────
```

### Game Control

Manage the game state:

```bash
game [command]
```

**Examples:**
```bash
game start          # Start a new game
game restart        # Restart the current game
game end            # End the current game
game save game1     # Save game to file (Phase 2 feature)
game load game1     # Load game from file (Phase 2 feature)
```

### Exiting

Exit the game:

```bash
quit                # Exit the game
exit                # Also exits the game
```

**Example:**
```bash
White > quit
Thank you for playing Shatranj!
```

## 🎮 Complete Gameplay Example

### Scholar's Mate Example

```bash
╔════════════════════════════════════════════════════════════════╗
║                        SHATRANJ CHESS                          ║
╚════════════════════════════════════════════════════════════════╝

Current Turn: White

White > move e2 e4
ℹ Move executed successfully

Black > move e7 e5

White > move f1 c4
ℹ Bishop develops to c4

Black > move b8 c6

White > move d1 h5
ℹ Queen attacks f7

Black > move g8 f6
ℹ Knight blocks the queen

White > move h5 f7
✓ Checkmate! White wins!
```

### Pawn Promotion Example

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

Press ESC to cancel

Your choice: q
ℹ Pawn promoted to Queen!
```

### Using Help Command

```bash
White > help e2

Possible moves for piece at e2:
  → e3 (one square forward)
  → e4 (two squares forward)

White > move e2 e4

Black > help g8

Possible moves for piece at g8:
  → f6 (L-shape move)
  → h6 (L-shape move)
```

## 🧪 Running Tests

The project has comprehensive test coverage across three test suites:

### Core Tests (40+ tests)

**All tests passing with zero compilation errors!** ✓)
```bash
cd tests/ShatranjCore.Tests
dotnet run
```

### AI Tests (6 tests)
```bash
cd ShatranjAI.Tests
dotnet run
```

### Integration Tests (6 tests)
```bash
cd tests/ShatranjIntegration.Tests
dotnet run
```

**Test Coverage:**
- ✅ All piece movements
- ✅ Special moves (castling, en passant, promotion)
- ✅ Check/checkmate detection
- ✅ AI move selection and evaluation
- ✅ Full game integration

See [**TESTING.md**](docs/TESTING.md) for detailed testing documentation.

## 🏗️ Project Structure

The project follows a **clean architecture** with proper separation of concerns:

```
Shatranj/
├── ShatranjCore.Abstractions/    # Core types & interfaces (no dependencies)
│   ├── CoreTypes.cs               # Location, PieceColor, GameMode, etc.
│   ├── IChessAI.cs                # AI interface
│   ├── IChessBoard.cs             # Board interface
│   └── ILogger.cs                 # Logger interface
│
├── ShatranjCore/                  # Core game engine
│   ├── Pieces/                    # All chess pieces
│   ├── Board/                     # Board representation
│   ├── Game/                      # Game orchestration
│   ├── Movement/                  # Move execution
│   ├── Validators/                # Rule validation
│   ├── UI/                        # Terminal UI
│   ├── Handlers/                  # Special move handlers
│   ├── Logging/                   # Logging implementations
│   ├── Persistence/               # Save/load functionality
│   └── Learning/                  # Game recording
│
├── ShatranjAI/                    # AI implementation
│   └── AI/
│       ├── BasicAI.cs             # Minimax with alpha-beta pruning
│       ├── MoveEvaluator.cs       # Position evaluation
│       └── IChessAI.cs            # AI interface (forwards to abstractions)
│
├── ShatranjCMD/                   # Console application entry point
│   └── Program.cs                 # Main program with DI setup
│
└── tests/                         # Test projects
    ├── ShatranjCore.Tests/        # Core unit tests (40+ tests)
    ├── ShatranjAI.Tests/          # AI unit tests (6 tests)
    └── ShatranjIntegration.Tests/ # Integration tests (6 tests)
```

**Architecture Benefits:**
- 🎯 Clear separation of concerns
- 🔄 Easy to test and mock
- 📦 No circular dependencies
- 🚀 Follows SOLID principles

## 🔧 Development

### SOLID Principles

This project demonstrates SOLID principles:

- **S**ingle Responsibility - Each class has one job
- **O**pen/Closed - Extensible without modification
- **L**iskov Substitution - All pieces are interchangeable
- **I**nterface Segregation - Focused interfaces
- **D**ependency Inversion - Depend on abstractions

Read more: [**SOLID_PRINCIPLES.md**](docs/SOLID_PRINCIPLES.md)

### Adding Features

1. Create feature branch
2. Follow SOLID principles
3. Write tests first (TDD)
4. Update documentation
5. Commit with descriptive message

## 📋 Command Quick Reference

### Gameplay Commands
| Command | Description | Example |
|---------|-------------|---------|
| `move [from] [to]` | Move a piece | `move e2 e4` |
| `castle [side]` | Castle king | `castle king` or `castle` |
| `undo` | Undo previous turn | `undo` |
| `redo` | Redo last undo | `redo` |

### Information Commands
| Command | Description | Example |
|---------|-------------|---------|
| `help [location]` | Show legal moves | `help e2` or `help` |
| `history` | View move history | `history` |

### Game Management Commands
| Command | Description | Example |
|---------|-------------|---------|
| `start` | Start a new game | `start` |
| `save` | Save current game | `save` |
| `load [gameId]` | Load a saved game | `load 1` |
| `restart` | Restart current game | `restart` |
| `end` | End current game | `end` |

### Settings Commands
| Command | Description | Example |
|---------|-------------|---------|
| `settings` | Show/change settings | `settings` |
| `settings difficulty [level]` | Set AI difficulty | `settings difficulty hard` |
| `settings profile [name]` | Set player name | `settings profile Mohammed` |

### Other Commands
| Command | Description | Example |
|---------|-------------|---------|
| `quit` | Exit game | `quit` |

### Special Keys

| Key | Function |
|-----|----------|
| `ESC` | Cancel castling/promotion |
| `Enter` | Confirm input |

## 🎯 Upcoming Features

See [**ROADMAP.md**](docs/ROADMAP.md) for the complete development plan.

### Phase 2: AI Integration - ✅ **COMPLETE (100%)**
- ✅ All core AI features working
- ✅ Zero compilation errors
- ✅ All tests passing (52 tests)
- ✅ **Complete save/load system with GameID**
- ✅ **Autosave after every turn**
- ✅ **Turn rollback (undo) functionality**
- ✅ **5 difficulty levels (Easy to Titan)**
- ✅ **Settings menu (profile names, difficulty)**
- ✅ **Resume game from autosave**
- ✅ **Game configuration persistence**

### Phase 3: Enhanced AI (Next)
- ⚪ Opening book integration
- ⚪ Endgame tablebase support
- ⚪ Iterative deepening
- ⚪ Quiescence search
- ⚪ Transposition tables
- ⚪ AI learning through self-play
- ⚪ Game analysis tools

### Phase 4: Online Multiplayer
- ⚪ Real-time multiplayer
- ⚪ Matchmaking system
- ⚪ ELO rating and leaderboards
- ⚪ Spectator mode

### Phase 5: GUI Implementation
- ⚪ Desktop GUI (WPF/Avalonia)
- ⚪ Web interface (Blazor)
- ⚪ Drag-and-drop movement
- ⚪ Animations and themes

## 📚 Documentation

- **[Roadmap](ROADMAP.md)** - Complete development roadmap with all phases
- **[Testing Guide](docs/TESTING.md)** - Test architecture and running tests
- **[SOLID Principles](docs/SOLID_PRINCIPLES.md)** - SOLID analysis and refactoring
- **[Architecture](docs/ARCHITECTURE.md)** - Technical architecture details
- **[Build Guide](docs/BUILD.md)** - Build instructions and troubleshooting

## 🤝 Contributing

Contributions are welcome! Please:

1. Read the [SOLID Principles](docs/SOLID_PRINCIPLES.md) guide
2. Follow existing code style
3. Write tests for new features
4. Update documentation
5. Open a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built as a learning exercise for SOLID principles and software architecture
- Inspired by classic chess implementations
- "Shatranj" - the Persian word for chess, reflecting the game's ancient origins

## 📞 Support

For issues or questions:
- 🐛 [Create an issue](https://github.com/YourUsername/Shatranj/issues)
- 📖 Check [documentation](docs/)
- 💬 Read the [Roadmap](ROADMAP.md)

---

**Happy Chess Playing! ♟️**

*Built with ❤️ using SOLID principles and .NET 9*
