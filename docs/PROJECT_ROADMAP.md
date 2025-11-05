# Shatranj - Project Roadmap

## Vision
Build a fully-featured chess game from the ground up, following SOLID principles and iterative development. Starting with a simple command-line human vs human game, progressively adding AI, difficulty levels, online play, and finally a polished UI.

## Core Philosophy
- **SOLID Principles First**: Design with clean architecture and maintainability
- **Iterative Development**: Each phase builds upon the previous, ensuring stability
- **Pragmatism Over Dogma**: Deviate from SOLID when practical benefits outweigh theoretical purity
- **Test-Driven Mindset**: Validate game rules and logic thoroughly

---

## Phase 1: Human vs Human (Command Line) ⚡ CURRENT PHASE

### Objectives
- Complete chess rule implementation
- Command-line interface for two local players
- Full move validation and game state management
- Checkmate, stalemate, and draw detection

### Key Features
- [ ] Complete all piece movement logic (Queen, Rook, Bishop, Knight)
- [ ] Implement special moves:
  - [ ] Castling (kingside and queenside)
  - [ ] En passant
  - [ ] Pawn promotion
- [ ] Check detection and validation
- [ ] Checkmate detection
- [ ] Stalemate detection
- [ ] Draw conditions:
  - [ ] Insufficient material
  - [ ] Threefold repetition
  - [ ] Fifty-move rule
- [ ] Move history tracking
- [ ] Algebraic notation input (e.g., "e2-e4" or "Nf3")
- [ ] Display captured pieces
- [ ] Undo/Redo functionality
- [ ] Save/Load game state (PGN format support)

### Technical Requirements
- Robust `ChessGame` orchestration
- Complete `Piece` hierarchy implementation
- `MoveValidator` for legal move checking
- `GameState` tracking (active, check, checkmate, stalemate, draw)
- Unit tests for all piece movements and game rules

### Success Criteria
✓ Two players can complete a full game from start to finish
✓ All chess rules correctly implemented and validated
✓ Clear, user-friendly command-line interface
✓ Comprehensive test coverage (>80%)

---

## Phase 2: AI Integration (Basic)

### Objectives
- Introduce computer opponent with basic intelligence
- Player can choose to play against AI or another human
- AI makes legal, reasonable moves

### Key Features
- [ ] AI player type implementation
- [ ] Basic move evaluation:
  - [ ] Material value assessment (Pawn=1, Knight/Bishop=3, Rook=5, Queen=9)
  - [ ] Capture prioritization
  - [ ] Basic positional awareness (center control)
- [ ] Minimax algorithm (depth 2-3)
- [ ] Random move selection as fallback
- [ ] AI response time configuration

### Technical Requirements
- `IChessAI` interface for AI implementations
- `BasicAI` class implementing simple evaluation
- `MoveEvaluator` for position scoring
- Game mode selection (Human vs Human, Human vs AI)

### Success Criteria
✓ AI makes legal moves consistently
✓ AI demonstrates basic strategic thinking
✓ Game remains playable and responsive with AI

---

## Phase 3: AI with Difficulty Levels & Self-Learning

### Objectives
- Multiple AI difficulty settings (Easy, Medium, Hard, Expert)
- Advanced chess algorithms for challenging gameplay
- Performance optimization for deeper search
- **AI vs AI mode for adversarial learning and self-improvement**

### Key Features
- [ ] Difficulty level selection
- [ ] Enhanced AI algorithms:
  - [ ] Alpha-beta pruning (depth 4-6 for hard mode)
  - [ ] Quiescence search
  - [ ] Transposition tables
  - [ ] Iterative deepening
- [ ] Advanced evaluation:
  - [ ] Piece-square tables
  - [ ] Pawn structure analysis
  - [ ] King safety evaluation
  - [ ] Mobility and development scoring
- [ ] Opening book integration (optional)
- [ ] Endgame tablebases (optional)
- [ ] **AI vs AI Mode**:
  - [ ] Automated game simulation (AI vs AI)
  - [ ] Game data collection and logging
  - [ ] Performance metrics tracking (win rate, average game length, etc.)
  - [ ] Batch simulation mode (run N games automatically)
  - [ ] Position evaluation comparison between AI versions
  - [ ] Learning from self-play (store successful strategies)
  - [ ] Adversarial training framework for AI improvement

### Technical Requirements
- `AIEngine` with configurable depth and evaluation
- `TranspositionTable` for position caching
- `OpeningBook` for early game moves
- Performance profiling and optimization
- **AI Training Infrastructure**:
  - `GameSimulator` for automated AI vs AI matches
  - `GameDataLogger` for recording game outcomes and positions
  - `PerformanceAnalyzer` for evaluating AI strength
  - `LearningEngine` for extracting patterns from self-play games
  - Data storage for training games (SQLite/JSON)

### Success Criteria
✓ Easy mode beatable by beginners
✓ Hard mode challenging for intermediate players
✓ AI moves calculated in reasonable time (<5 seconds)
✓ No illegal moves or crashes under any difficulty
✓ AI vs AI mode can simulate 100+ games unattended
✓ Measurable improvement in AI performance through self-play

---

## Phase 4: Online Multiplayer

### Objectives
- Play chess against opponents over the network
- Matchmaking and lobby system
- Real-time move synchronization

### Key Features
- [ ] Network architecture (Client-Server model)
- [ ] Player authentication and accounts
- [ ] Matchmaking system:
  - [ ] Quick match
  - [ ] Private games (invite by code)
  - [ ] ELO-based ranking
- [ ] Real-time move synchronization
- [ ] Chat system (optional)
- [ ] Spectator mode (optional)
- [ ] Game history and statistics
- [ ] Time controls (Blitz, Rapid, Classical)
- [ ] Clock synchronization
- [ ] Disconnect/reconnection handling

### Technical Requirements
- WebSocket or TCP/IP networking
- Game server implementation (ASP.NET Core or similar)
- Protocol for move transmission (JSON-based)
- Database for user accounts and game history (SQL Server/PostgreSQL)
- Security: Input validation, anti-cheating measures
- Latency handling and synchronization

### Success Criteria
✓ Stable connections with <100ms latency on good networks
✓ Graceful handling of disconnections
✓ Accurate time control enforcement
✓ Secure account and game data

---

## Phase 5: UI/GUI Implementation

### Objectives
- Beautiful, intuitive graphical interface
- Enhanced user experience with animations and themes
- Cross-platform support (Windows initially, expand later)

### Key Features
- [ ] Windows Forms enhancement OR migration to:
  - [ ] WPF (Windows Presentation Foundation)
  - [ ] Avalonia (cross-platform)
  - [ ] Blazor (web-based)
- [ ] Visual chess board with piece dragging
- [ ] Multiple board themes and piece sets
- [ ] Smooth animations:
  - [ ] Piece movement
  - [ ] Capture animations
  - [ ] Check/Checkmate highlights
- [ ] Sound effects (optional)
- [ ] Move highlighting (last move, legal moves)
- [ ] Game analysis panel:
  - [ ] Move list with navigation
  - [ ] Evaluation bar (when playing AI)
  - [ ] Captured pieces display
- [ ] Settings and preferences
- [ ] Accessibility features (keyboard navigation, screen reader support)

### Technical Requirements
- Refactor ShatranjMain to use MVVM pattern (if using WPF/Avalonia)
- `IChessBoardView` interface for UI abstraction
- Graphics rendering optimization
- Resource management for themes and assets
- Responsive layout design

### Success Criteria
✓ Smooth 60fps rendering
✓ Intuitive drag-and-drop piece movement
✓ Visually appealing and polished
✓ All Phase 1-4 features accessible via GUI

---

## Future Enhancements (Post Phase 5)

### Potential Features
- Mobile app (iOS/Android)
- Puzzle mode (tactical training)
- Game analysis engine integration (Stockfish)
- Tournament mode
- Variant support (Chess960, Three-check, etc.)
- Replay famous games
- Video tutorials and hints system

---

## Development Principles

### Code Quality
- Follow SOLID principles (see `SOLID_PRINCIPLES.md`)
- Write unit tests for all business logic
- Code reviews before merging to main branch
- Maintain >80% test coverage

### Documentation
- XML documentation comments for public APIs
- Architecture decision records (ADRs) for major decisions
- Keep `SOLID_PRINCIPLES.md` updated with deviations

### Version Control
- Feature branches for each major feature
- Meaningful commit messages
- Git tags for each phase completion

---

## Timeline (Tentative)

| Phase | Estimated Duration | Status |
|-------|-------------------|--------|
| Phase 1: Human vs Human | 4-6 weeks | 🟡 In Progress (~30% complete) |
| Phase 2: Basic AI | 2-3 weeks | ⚪ Not Started |
| Phase 3: AI Levels | 3-4 weeks | ⚪ Not Started |
| Phase 4: Online Play | 6-8 weeks | ⚪ Not Started |
| Phase 5: UI/GUI | 4-6 weeks | ⚪ Not Started |

**Total Estimated Time**: 19-27 weeks (4.5-6 months)

---

## Current Status

**Last Updated**: 2025-11-05
**Active Phase**: Phase 1 - Human vs Human
**Completion**: ~30%

### Recent Accomplishments
- Basic piece structure established
- Board initialization complete
- Pawn movement partially implemented
- Console display functional

### Immediate Next Steps
1. Complete Queen, Rook, Bishop, Knight movement logic
2. Implement check detection
3. Add special moves (castling, en passant, promotion)
4. Build comprehensive test suite
5. Implement checkmate/stalemate detection

---

## Contributing
(For future collaborators)
- Read `SOLID_PRINCIPLES.md` before making changes
- Follow existing code style and patterns
- Write tests for new features
- Update documentation as needed
