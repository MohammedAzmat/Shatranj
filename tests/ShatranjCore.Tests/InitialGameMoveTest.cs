using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore.Abstractions;
using ShatranjCore;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Tests
{
    /// <summary>
    /// Test to diagnose why e2-e4 move doesn't work in initial game state
    /// </summary>
    public static class InitialGameMoveTest
    {
        public static void RunTest()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     Initial Game State - e2 to e4 Move Test            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            TestE2ToE4Move();
        }

        private static void TestE2ToE4Move()
        {
            Console.WriteLine("Step 1: Create initial board state");
            IChessBoard board = new ChessBoard(PieceColor.White);
            Console.WriteLine("✓ Board created\n");

            // e2 in algebraic notation = row 6, column 4
            Location e2 = new Location(6, 4);
            Location e4 = new Location(4, 4);

            Console.WriteLine($"Step 2: Validate coordinates");
            Console.WriteLine($"  e2 (algebraic) → Location(row=6, col=4)");
            Console.WriteLine($"  e4 (algebraic) → Location(row=4, col=4)");
            Console.WriteLine($"  Expected pawn color: White\n");

            Console.WriteLine("Step 3: Check if piece exists at e2");
            Piece piece = board.GetPiece(e2);
            if (piece == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED: No piece at e2");
                Console.ResetColor();
                return;
            }
            Console.WriteLine($"✓ Piece found at e2: {piece.GetType().Name}");
            Console.WriteLine($"  Color: {piece.Color}");
            Console.WriteLine($"  Notation: {piece.Notation}");
            Console.WriteLine($"  Piece.location: ({piece.location.Row}, {piece.location.Column})\n");

            if (piece is not Pawn)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED: Expected Pawn, got {piece.GetType().Name}");
                Console.ResetColor();
                return;
            }

            Pawn pawn = (Pawn)piece;
            Console.WriteLine("Step 4: Check pawn properties");
            Console.WriteLine($"  Direction: {pawn.Direction}");
            Console.WriteLine($"  isMoved: {pawn.isMoved}");
            Console.WriteLine($"  Color: {pawn.Color}");

            Console.WriteLine("\nStep 4b: Debug GetMoves validation");
            Piece pieceAtSource = board.GetPiece(e2);
            Console.WriteLine($"  Piece at source == null: {pieceAtSource == null}");
            Console.WriteLine($"  Piece at source type: {pieceAtSource?.GetType().Name}");
            Console.WriteLine($"  Pawn instance type: {pawn.GetType().Name}");
            Console.WriteLine($"  Types match: {pieceAtSource?.GetType() == pawn.GetType()}");
            Console.WriteLine($"  Same instance: {pieceAtSource == pawn}\n");

            Console.WriteLine("Step 5: Detailed move generation analysis");
            // Manually trace through the pawn move logic
            int dirMult = pawn.Direction == PawnMoves.Up ? 1 : -1;
            Console.WriteLine($"  Direction multiplier: {dirMult}");

            int oneForward = e2.Row + (1 * dirMult);
            Console.WriteLine($"  One forward target: row={oneForward}, col={e2.Column}");
            Console.WriteLine($"  Is in bounds: {board.IsInBounds(oneForward, e2.Column)}");
            Console.WriteLine($"  Is empty: {board.IsEmptyAt(oneForward, e2.Column)}");

            int twoForward = e2.Row + (2 * dirMult);
            Console.WriteLine($"  Two forward target: row={twoForward}, col={e2.Column}");
            Console.WriteLine($"  Is in bounds: {board.IsInBounds(twoForward, e2.Column)}");
            Console.WriteLine($"  Is empty: {board.IsEmptyAt(twoForward, e2.Column)}");
            Console.WriteLine();

            Console.WriteLine("Step 6: Get valid moves for pawn at e2");
            List<Move> validMoves = pawn.GetMoves(e2, board);
            Console.WriteLine($"  Total valid moves: {validMoves.Count}");

            if (validMoves.Count > 0)
            {
                Console.WriteLine("  Valid move destinations:");
                foreach (var move in validMoves)
                {
                    Console.WriteLine($"    → ({move.To.Location.Row}, {move.To.Location.Column})");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ✗ No valid moves found!");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.WriteLine("Step 7: Check if e2→e4 is in valid moves");
            bool e4IsValid = validMoves.Any(m =>
                m.To.Location.Row == e4.Row &&
                m.To.Location.Column == e4.Column);

            if (e4IsValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ e2→e4 is a VALID move");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ e2→e4 is NOT a valid move");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.WriteLine("Step 8: Check intermediate square (e3)");
            Location e3 = new Location(5, 4);
            bool isEmpty3 = board.IsEmptyAt(e3.Row, e3.Column);
            Console.WriteLine($"  e3 empty: {isEmpty3}");

            bool isEmpty4 = board.IsEmptyAt(e4.Row, e4.Column);
            Console.WriteLine($"  e4 empty: {isEmpty4}");
            Console.WriteLine();

            Console.WriteLine("Step 9: Check CanMove method");
            bool canMove = pawn.CanMove(e2, e4, board);
            Console.WriteLine($"  pawn.CanMove(e2, e4, board): {canMove}");
            Console.WriteLine();

            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            if (e4IsValid && canMove)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("║                    ✓ TEST PASSED                       ║");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("║                    ✗ TEST FAILED                       ║");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Debugging info:");
                Console.WriteLine($"  - Valid moves count: {validMoves.Count}");
                Console.WriteLine($"  - e4 in valid moves: {e4IsValid}");
                Console.WriteLine($"  - CanMove result: {canMove}");
                Console.WriteLine($"  - Pawn location: ({pawn.location.Row}, {pawn.location.Column})");
                Console.WriteLine($"  - Pawn direction: {pawn.Direction}");
            }
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }
    }
}
