using System;
using System.Collections.Generic;
using System.Linq;
using ShatranjCore;
using ShatranjCore.Pieces;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Movement;

namespace ShatranjCore.Tests.PieceTests
{
    /// <summary>
    /// Unit tests for Pawn piece movement.
    /// Tests forward movement, captures, double move, and en passant.
    /// </summary>
    public static class PawnTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("│              Pawn Movement Tests                         │");
            Console.WriteLine("└─────────────────────────────────────────────────────────┘");

            Test_Pawn_StartPosition_CanMoveOneOrTwoSquares();
            Test_Pawn_NonStartPosition_CanMoveOneSquare();
            Test_Pawn_CannotMoveForwardWhenBlocked();
            Test_Pawn_CanCaptureDiagonally();
            Test_Pawn_CannotCaptureForward();
            Test_Pawn_CannotMoveBackward();
            Test_Pawn_White_MovesUp();
            Test_Pawn_Black_MovesDown();
            Test_Pawn_EnPassant_Available();
            Test_Pawn_EnPassant_NotAvailable();
        }

        /// <summary>
        /// Test that a pawn at starting position can move 1 or 2 squares forward.
        /// </summary>
        public static void Test_Pawn_StartPosition_CanMoveOneOrTwoSquares()
        {
            Console.Write("  Testing pawn start position (1 or 2 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e2 (row 6, col 4) - starting position
            Pawn pawn = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(6, 4));

            List<Move> moves = pawn.GetMoves(new Location(6, 4), board);

            // Should be able to move to e3 (row 5) or e4 (row 4)
            bool canMoveOneSquare = moves.Any(m => m.To.Location.Row == 5 && m.To.Location.Column == 4);
            bool canMoveTwoSquares = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column == 4);

            if (canMoveOneSquare && canMoveTwoSquares && moves.Count == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 2 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn not at starting position can only move 1 square forward.
        /// </summary>
        public static void Test_Pawn_NonStartPosition_CanMoveOneSquare()
        {
            Console.Write("  Testing pawn non-start position (1 move)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e4 (row 4, col 4) - non-starting position
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            pawn.isMoved = true; // Mark as moved
            board.PlacePiece(pawn, new Location(4, 4));

            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Should only be able to move to e5 (row 3)
            bool canMoveOneSquare = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 4);
            bool cannotMoveTwoSquares = !moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 4);

            if (canMoveOneSquare && cannotMoveTwoSquares && moves.Count == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 1 move, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn cannot move forward when blocked.
        /// </summary>
        public static void Test_Pawn_CannotMoveForwardWhenBlocked()
        {
            Console.Write("  Testing pawn cannot move when blocked... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e2 (row 6, col 4)
            Pawn pawn = new Pawn(6, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(6, 4));

            // Place black pawn at e3 (row 5, col 4) - blocking
            Pawn blockingPawn = new Pawn(5, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blockingPawn, new Location(5, 4));

            List<Move> moves = pawn.GetMoves(new Location(6, 4), board);

            if (moves.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 0 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn can capture diagonally.
        /// </summary>
        public static void Test_Pawn_CanCaptureDiagonally()
        {
            Console.Write("  Testing pawn can capture diagonally... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e4 (row 4, col 4)
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Place black pawns diagonally at d5 (row 3, col 3) and f5 (row 3, col 5)
            Pawn enemyPawn1 = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            Pawn enemyPawn2 = new Pawn(3, 5, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn1, new Location(3, 3));
            board.PlacePiece(enemyPawn2, new Location(3, 5));

            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Should be able to capture both pawns and move forward (3 moves total)
            bool canCaptureLeft = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 3 && m.CapturedPiece != null);
            bool canCaptureRight = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 5 && m.CapturedPiece != null);
            bool canMoveForward = moves.Any(m => m.To.Location.Row == 3 && m.To.Location.Column == 4 && m.CapturedPiece == null);

            if (canCaptureLeft && canCaptureRight && canMoveForward && moves.Count == 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 3 moves with 2 captures, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn cannot capture forward.
        /// </summary>
        public static void Test_Pawn_CannotCaptureForward()
        {
            Console.Write("  Testing pawn cannot capture forward... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e4 (row 4, col 4)
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            // Place black pawn directly in front at e5 (row 3, col 4)
            Pawn enemyPawn = new Pawn(3, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(3, 4));

            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Pawn should not be able to move (blocked and cannot capture forward)
            if (moves.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 0 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn cannot move backward.
        /// </summary>
        public static void Test_Pawn_CannotMoveBackward()
        {
            Console.Write("  Testing pawn cannot move backward... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e4 (row 4, col 4)
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // Check that no move goes backward (higher row number for white)
            bool hasBackwardMove = moves.Any(m => m.To.Location.Row > 4);

            if (!hasBackwardMove)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Pawn should not move backward)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that white pawns move up (decreasing row number).
        /// </summary>
        public static void Test_Pawn_White_MovesUp()
        {
            Console.Write("  Testing white pawn moves up... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e4 (row 4, col 4)
            Pawn pawn = new Pawn(4, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(pawn, new Location(4, 4));

            List<Move> moves = pawn.GetMoves(new Location(4, 4), board);

            // All moves should be to row 3 (up)
            bool allMovesUp = moves.All(m => m.To.Location.Row < 4);

            if (allMovesUp && moves.Count == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (White pawn should move up)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that black pawns move down (increasing row number).
        /// </summary>
        public static void Test_Pawn_Black_MovesDown()
        {
            Console.Write("  Testing black pawn moves down... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place black pawn at e5 (row 3, col 4)
            Pawn pawn = new Pawn(3, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(pawn, new Location(3, 4));

            List<Move> moves = pawn.GetMoves(new Location(3, 4), board);

            // All moves should be to row 4 (down)
            bool allMovesDown = moves.All(m => m.To.Location.Row > 3);

            if (allMovesDown && moves.Count == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Black pawn should move down)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn can capture en passant when available.
        /// </summary>
        public static void Test_Pawn_EnPassant_Available()
        {
            Console.Write("  Testing pawn en passant capture... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e5 (row 3, col 4)
            Pawn whitePawn = new Pawn(3, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(whitePawn, new Location(3, 4));

            // Place black pawn at d5 (row 3, col 3) - just moved 2 squares
            Pawn blackPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blackPawn, new Location(3, 3));

            // En passant target is d6 (row 2, col 3)
            Location enPassantTarget = new Location(2, 3);

            List<Move> moves = whitePawn.GetMovesWithEnPassant(new Location(3, 4), board, enPassantTarget);

            // Should have: forward move (e6), and en passant capture (d6)
            bool hasForwardMove = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 4);
            bool hasEnPassant = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3 && m.CapturedPiece != null);

            if (hasForwardMove && hasEnPassant && moves.Count == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 2 moves including en passant, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a pawn cannot capture en passant when not available.
        /// </summary>
        public static void Test_Pawn_EnPassant_NotAvailable()
        {
            Console.Write("  Testing pawn without en passant... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white pawn at e5 (row 3, col 4)
            Pawn whitePawn = new Pawn(3, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(whitePawn, new Location(3, 4));

            // Place black pawn at d5 (row 3, col 3) - but no en passant available
            Pawn blackPawn = new Pawn(3, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blackPawn, new Location(3, 3));

            // No en passant target
            List<Move> moves = whitePawn.GetMovesWithEnPassant(new Location(3, 4), board, null);

            // Should only have forward move (e6)
            bool hasOnlyForwardMove = moves.Count == 1 && moves.All(m => m.To.Location.Row == 2 && m.To.Location.Column == 4);

            if (hasOnlyForwardMove)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 1 forward move only, got {moves.Count})");
                Console.ResetColor();
            }
        }
    }
}
