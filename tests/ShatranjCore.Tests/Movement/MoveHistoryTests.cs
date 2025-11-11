using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Movement;
using ShatranjCore.Pieces;
using ShatranjCore.Board;

namespace ShatranjCore.Tests.Movement
{
    /// <summary>
    /// Test suite for move history
    /// Tests recording, retrieving, and tracking moves with all attributes
    /// </summary>
    public class MoveHistoryTests
    {
        private MoveHistory _history;

        public MoveHistoryTests()
        {
            _history = new MoveHistory();
        }

        public void RunAllTests()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Move History Tests Suite                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            int passed = 0;
            int failed = 0;

            // Test 1: Add Move
            try
            {
                TestAddMove();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 1 PASSED: Add move to history");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 1 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 2: Get All Moves
            try
            {
                TestGetMoves();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 2 PASSED: Get all moves from history");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 2 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 3: Get Last Move
            try
            {
                TestGetLastMove();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 3 PASSED: Get last move");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 3 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 4: Clear History
            try
            {
                TestClear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 4 PASSED: Clear history");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 4 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 5: Multiple Moves
            try
            {
                TestMultipleMoves();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 5 PASSED: Multiple moves in sequence");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 5 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 6: Capture Tracking
            try
            {
                TestCaptureTracking();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 6 PASSED: Capture tracking");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 6 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 7: Check Tracking
            try
            {
                TestCheckTracking();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 7 PASSED: Check tracking");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 7 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Test 8: History State Persistence
            try
            {
                TestHistoryStatePersistence();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Test 8 PASSED: History state persistence");
                Console.ResetColor();
                passed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Test 8 FAILED: {ex.Message}");
                Console.ResetColor();
                failed++;
            }

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Move History Tests Summary: {passed} passed, {failed} failed               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        }

        private void TestAddMove()
        {
            var fromLoc = new Location(6, 4); // e2
            var toLoc = new Location(4, 4);   // e4
            var fromSquare = new Square(fromLoc);
            var toSquare = new Square(toLoc);
            var piece = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            var move = new Move(piece, fromSquare, toSquare);

            _history.Clear();
            _history.AddMove(move, PieceColor.White, wasCapture: false, wasCheck: false, wasCheckmate: false);

            var moves = _history.GetAllMoves();
            if (moves.Count != 1)
            {
                throw new Exception($"Expected 1 move, got {moves.Count}");
            }
        }

        private void TestGetMoves()
        {
            _history.Clear();

            // Add multiple moves
            for (int i = 0; i < 5; i++)
            {
                var fromLoc = new Location(6, i);
                var toLoc = new Location(4, i);
                var fromSquare = new Square(fromLoc);
                var toSquare = new Square(toLoc);
                var piece = new Pawn(6, i, PieceColor.White, PawnMoves.Up);
                var move = new Move(piece, fromSquare, toSquare);
                _history.AddMove(move, PieceColor.White, wasCapture: false, wasCheck: false, wasCheckmate: false);
            }

            var moves = _history.GetAllMoves();
            if (moves.Count != 5)
            {
                throw new Exception($"Expected 5 moves, got {moves.Count}");
            }
        }

        private void TestGetLastMove()
        {
            _history.Clear();

            var fromLoc1 = new Location(6, 4);
            var toLoc1 = new Location(4, 4);
            var fromSquare1 = new Square(fromLoc1);
            var toSquare1 = new Square(toLoc1);
            var piece1 = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            var move1 = new Move(piece1, fromSquare1, toSquare1);

            var fromLoc2 = new Location(1, 4);
            var toLoc2 = new Location(3, 4);
            var fromSquare2 = new Square(fromLoc2);
            var toSquare2 = new Square(toLoc2);
            var piece2 = new Pawn(1, 4, PieceColor.Black, PawnMoves.Down);
            var move2 = new Move(piece2, fromSquare2, toSquare2);

            _history.AddMove(move1, PieceColor.White, wasCapture: false, wasCheck: false, wasCheckmate: false);
            _history.AddMove(move2, PieceColor.Black, wasCapture: false, wasCheck: false, wasCheckmate: false);

            var lastMove = _history.GetLastMove();
            if (lastMove == null)
            {
                throw new Exception("Last move is null");
            }
        }

        private void TestClear()
        {
            _history.Clear();

            // Add some moves
            var fromLoc = new Location(6, 4);
            var toLoc = new Location(4, 4);
            var fromSquare = new Square(fromLoc);
            var toSquare = new Square(toLoc);
            var piece = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            var move = new Move(piece, fromSquare, toSquare);
            _history.AddMove(move, PieceColor.White, wasCapture: false, wasCheck: false, wasCheckmate: false);

            if (_history.GetAllMoves().Count != 1)
            {
                throw new Exception("Move not added");
            }

            // Clear
            _history.Clear();

            if (_history.GetAllMoves().Count != 0)
            {
                throw new Exception("History not cleared");
            }
        }

        private void TestMultipleMoves()
        {
            _history.Clear();

            // Simulate a few moves of an opening
            var positions = new[]
            {
                (new Location(6, 4), new Location(4, 4), PieceColor.White),  // e2-e4
                (new Location(1, 4), new Location(3, 4), PieceColor.Black),  // e7-e5
                (new Location(7, 6), new Location(5, 5), PieceColor.White),  // g1-f3
            };

            foreach (var (fromLoc, toLoc, color) in positions)
            {
                var fromSquare = new Square(fromLoc);
                var toSquare = new Square(toLoc);
                var piece = color == PieceColor.White
                    ? (Piece)new Pawn(fromLoc.Row, fromLoc.Column, color, PawnMoves.Up)
                    : new Pawn(fromLoc.Row, fromLoc.Column, color, PawnMoves.Down);
                var move = new Move(piece, fromSquare, toSquare);
                _history.AddMove(move, color, wasCapture: false, wasCheck: false, wasCheckmate: false);
            }

            var allMoves = _history.GetAllMoves();
            if (allMoves.Count != 3)
            {
                throw new Exception($"Expected 3 moves, got {allMoves.Count}");
            }
        }

        private void TestCaptureTracking()
        {
            _history.Clear();

            // Add a capture move
            var fromLoc = new Location(4, 4); // e4
            var toLoc = new Location(5, 5);   // d5 (captured pawn)
            var fromSquare = new Square(fromLoc);
            var toSquare = new Square(toLoc);
            var piece = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            var move = new Move(piece, fromSquare, toSquare);

            _history.AddMove(move, PieceColor.White, wasCapture: true, wasCheck: false, wasCheckmate: false);

            var moves = _history.GetAllMoves();
            var captureMove = moves[0];

            if (!captureMove.WasCapture)
            {
                throw new Exception("Capture not tracked");
            }
        }

        private void TestCheckTracking()
        {
            _history.Clear();

            // Add a move that causes check
            var fromLoc = new Location(7, 5); // f1
            var toLoc = new Location(2, 0);   // a6 (check)
            var fromSquare = new Square(fromLoc);
            var toSquare = new Square(toLoc);
            var piece = new Bishop(7, 5, PieceColor.White);
            var move = new Move(piece, fromSquare, toSquare);

            _history.AddMove(move, PieceColor.White, wasCapture: false, wasCheck: true, wasCheckmate: false);

            var moves = _history.GetAllMoves();
            var checkMove = moves[0];

            if (!checkMove.WasCheck)
            {
                throw new Exception("Check not tracked");
            }
        }

        private void TestHistoryStatePersistence()
        {
            _history.Clear();

            // Add some moves
            var fromLoc = new Location(6, 4);
            var toLoc = new Location(4, 4);
            var fromSquare = new Square(fromLoc);
            var toSquare = new Square(toLoc);
            var piece = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            var move = new Move(piece, fromSquare, toSquare);

            _history.AddMove(move, PieceColor.White, wasCapture: false, wasCheck: false, wasCheckmate: false);

            // Verify moves persist
            var movesBefore = _history.GetAllMoves();
            var lastMoveBefore = _history.GetLastMove();

            // Get moves again to verify persistence
            var movesAfter = _history.GetAllMoves();
            var lastMoveAfter = _history.GetLastMove();

            if (movesBefore.Count != movesAfter.Count)
            {
                throw new Exception("Move count changed");
            }

            if (lastMoveBefore == null || lastMoveAfter == null)
            {
                throw new Exception("Last move lost");
            }
        }
    }
}
