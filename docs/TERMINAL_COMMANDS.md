# Shatranj Terminal Commands Guide

This document explains all available commands in the Shatranj chess game terminal interface.

## Game Display

The terminal displays:
- **Chess Board**: 8x8 grid with pieces
  - White pieces: `K` (King), `Q` (Queen), `R` (Rook), `B` (Bishop), `N` (Knight), `P` (Pawn)
  - Black pieces: `k` (king), `q` (queen), `r` (rook), `b` (bishop), `n` (knight), `p` (pawn)
- **Last Move Highlight**: Yellow background shows the last move
- **Checkerboard Pattern**: Alternating dark gray and black squares
- **Current Turn**: Shows which player's turn it is
- **Captured Pieces**: Lists all captured pieces
- **Last Move**: Displays the last move in algebraic notation

## Command Syntax

### Move Command

Move a piece from one square to another.

**Syntax**: `move [start] [end]`

**Examples**:
```
move e2 e4    - Move pawn from e2 to e4
move g1 f3    - Move knight from g1 to f3
move e1 g1    - Castle kingside (when implemented)
```

**Algebraic Notation**:
- Files (columns): `a` through `h` (left to right)
- Ranks (rows): `1` through `8` (bottom to top for white)
- Example: `e2` = column E, row 2

**Move Validation**:
- ✓ Piece must exist at start location
- ✓ Piece must belong to current player
- ✓ Move must be legal for that piece type
- ✓ Cannot move into check (TODO: Phase 1)

**Errors**:
- "No piece at [location]" - No piece exists there
- "That piece belongs to [color]" - Trying to move opponent's piece
- "Illegal move for [piece]" - Move not allowed for that piece type

### Help Command

Show available moves for a specific piece or display general help.

**Syntax**:
- `help` - Display all available commands
- `help [location]` - Show possible moves for piece at location

**Examples**:
```
help          - Show command list
help e2       - Show all legal moves for piece at e2
help d4       - Show all legal moves for piece at d4
```

**Output**:
- Lists all valid destination squares
- Marks capture moves in red
- Example output:
  ```
  Possible moves for piece at e2:
    → e3
    → e4
  ```

### Game Commands

Control the game flow.

**Syntax**: `game [action]`

**Actions**:

| Action | Description | Status |
|--------|-------------|--------|
| `game start` | Start a new game | ✓ Implemented |
| `game restart` | Restart the current game | ✓ Implemented |
| `game end` | End the current game | ✓ Implemented |
| `game save` | Save game to file | ⚠ Coming soon |

**Examples**:
```
game start      - Begin a new game
game restart    - Restart from beginning
game end        - End current game
game save       - Save game state (not yet implemented)
```

### History Command

Display the move history of the current game.

**Syntax**: `history`

**Output**:
```
Move History:
─────────────────────────────────────────
  1. e2-e4          e7-e5
  2. Ng1-f3         Nb8-c6
  3. Bf1-c4         Bf8-c5
─────────────────────────────────────────
```

**Notation**:
- Format: `[Piece][from]-[to][modifier]`
- Modifiers:
  - `x` = capture (e.g., `Rxd4`)
  - `+` = check
  - `#` = checkmate
- No piece letter = pawn move

### Quit Command

Exit the game.

**Syntax**: `quit` or `exit`

**Examples**:
```
quit    - Exit the game
exit    - Exit the game
```

## Game Status Indicators

### Turn Indicator
```
Current Turn: White
```
Shows whose turn it is. Color-coded:
- White pieces: White text
- Black pieces: Red text

### Check Warning
```
⚠ CHECK! White King is in check!
```
Displayed in yellow when a King is in check.

### Last Move Display
```
Last Move: e2-e4
```
Shows the most recent move in algebraic notation.

### Captured Pieces
```
Captured: p, p, N, b
```
Lists all captured pieces:
- Uppercase = White pieces captured
- Lowercase = Black pieces captured

## Game End Conditions

### Checkmate
```
╔════════════════════════════════════════════════════════════════╗
║                        CHECKMATE!                              ║
║              White wins the game!                              ║
╚════════════════════════════════════════════════════════════════╝
```

### Stalemate
```
╔════════════════════════════════════════════════════════════════╗
║                        STALEMATE!                              ║
║                    Game ends in a draw                         ║
╚════════════════════════════════════════════════════════════════╝
```

### Draw
```
╔════════════════════════════════════════════════════════════════╗
║                          DRAW!                                 ║
║                    Game ends in a draw                         ║
╚════════════════════════════════════════════════════════════════╝
```

### Resignation
```
╔════════════════════════════════════════════════════════════════╗
║                       RESIGNATION                              ║
║              Black wins by resignation                         ║
╚════════════════════════════════════════════════════════════════╝
```

## Example Game Session

```
═══════════════════════════════════════════════════════════════════
                        SHATRANJ CHESS
═══════════════════════════════════════════════════════════════════

     A      B      C      D      E      F      G      H
   ╔═══════╦═══════╦═══════╦═══════╦═══════╦═══════╦═══════╦═══════╗
 8 ║  r    ║  n    ║  b    ║  q    ║  k    ║  b    ║  n    ║  r    ║ 8
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 7 ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║  p    ║ 7
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 6 ║       ║       ║       ║       ║       ║       ║       ║       ║ 6
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 5 ║       ║       ║       ║       ║       ║       ║       ║       ║ 5
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 4 ║       ║       ║       ║       ║       ║       ║       ║       ║ 4
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 3 ║       ║       ║       ║       ║       ║       ║       ║       ║ 3
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 2 ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║  P    ║ 2
   ╠═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╬═══════╣
 1 ║  R    ║  N    ║  B    ║  Q    ║  K    ║  B    ║  N    ║  R    ║ 1
   ╚═══════╩═══════╩═══════╩═══════╩═══════╩═══════╩═══════╩═══════╝
     A      B      C      D      E      F      G      H

┌────────────────────────────────────────────────────────────────┐
│ Current Turn: White                                            │
└────────────────────────────────────────────────────────────────┘

White > move e2 e4
ℹ Move executed successfully

Black > help d7

Possible moves for piece at d7:
  → d6
  → d5

Press any key to continue...

Black > move d7 d5

White > move e4 d5
ℹ Pawn captures Pawn!

Black > help
Available Commands:
  move [start] [end]  - Move a piece (e.g., 'move e2 e4')
  help                - Show possible moves for a piece
  game start          - Start a new game
  game save           - Save current game
  game end            - End current game
  game restart        - Restart the game
  quit                - Exit the game

Press any key to continue...

Black > history

Move History:
─────────────────────────────────────────
  1. e2-e4          d7-d5
  2. e4xd5
─────────────────────────────────────────

Black > quit
ℹ Thanks for playing Shatranj!
```

## Tips

1. **Algebraic Notation**: Practice reading board coordinates (a-h for columns, 1-8 for rows)
2. **Use Help**: Type `help [location]` to see all legal moves for a piece
3. **Review Moves**: Use `history` to review the game
4. **Plan Ahead**: Think about your opponent's possible responses
5. **Check Status**: Watch for check warnings and protect your King

## Keyboard Shortcuts

Currently no keyboard shortcuts - all input is command-based. Future versions may include:
- Arrow keys for piece selection
- Enter to confirm moves
- ESC to cancel

## Error Messages Reference

| Error Message | Cause | Solution |
|--------------|-------|----------|
| "Empty command" | No input provided | Type a valid command |
| "Unknown command: [cmd]" | Invalid command | Type `help` to see valid commands |
| "No piece at [location]" | Square is empty | Choose a square with your piece |
| "That piece belongs to [color]" | Trying to move opponent's piece | Move your own pieces only |
| "Illegal move for [piece]" | Invalid move for piece type | Use `help [location]` to see legal moves |
| "Invalid location format" | Bad algebraic notation | Use format like `e2`, `d4` |

## Future Features (Coming Soon)

- ✓ Command-line interface with colored output
- ✓ Move validation
- ✓ Capture detection
- ⚠ Check detection (Phase 1)
- ⚠ Checkmate detection (Phase 1)
- ⚠ Special moves: castling, en passant, pawn promotion (Phase 1)
- ⚠ Save/load games (Phase 1)
- ⚠ AI opponent (Phase 2)
- ⚠ Difficulty levels (Phase 3)
- ⚠ Online multiplayer (Phase 4)
- ⚠ Graphical UI (Phase 5)

---

**Last Updated**: 2025-11-05
**Version**: Phase 1 (In Development)
