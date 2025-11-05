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
    /// Unit tests for Queen piece movement.
    /// Tests horizontal, vertical, and diagonal movement (combination of Rook and Bishop).
    /// </summary>
    public static class QueenTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine("│              Queen Movement Tests                        │");
            Console.WriteLine("└─────────────────────────────────────────────────────────┘");

            Test_Queen_Center_EmptyBoard_Has27Moves();
            Test_Queen_Corner_EmptyBoard_Has21Moves();
            Test_Queen_CanCaptureEnemyPiece();
            Test_Queen_CannotMovePastFriendlyPiece();
            Test_Queen_CannotMovePastEnemyPiece();
            Test_Queen_MovesInAllDirections();
        }

        /// <summary>
        /// Test that a queen in the center of an empty board has 27 possible moves.
        /// </summary>
        public static void Test_Queen_Center_EmptyBoard_Has27Moves()
        {
            Console.Write("  Testing queen center position (27 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at d4 (row 4, col 3)
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            if (moves.Count == 27)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 27 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a queen in a corner has 21 possible moves.
        /// </summary>
        public static void Test_Queen_Corner_EmptyBoard_Has21Moves()
        {
            Console.Write("  Testing queen corner position (21 moves)... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at a1 (row 7, col 0)
            Queen queen = new Queen(7, 0, PieceColor.White);
            board.PlacePiece(queen, new Location(7, 0));

            List<Move> moves = queen.GetMoves(new Location(7, 0), board);

            if (moves.Count == 21)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ FAILED (Expected 21 moves, got {moves.Count})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a queen can capture enemy pieces in all directions.
        /// </summary>
        public static void Test_Queen_CanCaptureEnemyPiece()
        {
            Console.Write("  Testing queen can capture enemy pieces... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at d4 (row 4, col 3)
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place black pawns in different directions
            Pawn enemyPawn1 = new Pawn(2, 3, PieceColor.Black, PawnMoves.Down); // Vertical (d6)
            Pawn enemyPawn2 = new Pawn(4, 6, PieceColor.Black, PawnMoves.Down); // Horizontal (g4)
            Pawn enemyPawn3 = new Pawn(2, 5, PieceColor.Black, PawnMoves.Down); // Diagonal (f6)

            board.PlacePiece(enemyPawn1, new Location(2, 3));
            board.PlacePiece(enemyPawn2, new Location(4, 6));
            board.PlacePiece(enemyPawn3, new Location(2, 5));

            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Check if queen can capture all three pawns
            bool canCaptureVertical = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3 && m.CapturedPiece != null);
            bool canCaptureHorizontal = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column == 6 && m.CapturedPiece != null);
            bool canCaptureDiagonal = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 5 && m.CapturedPiece != null);

            if (canCaptureVertical && canCaptureHorizontal && canCaptureDiagonal)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Queen should capture in all directions)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a queen cannot move past friendly pieces.
        /// </summary>
        public static void Test_Queen_CannotMovePastFriendlyPiece()
        {
            Console.Write("  Testing queen cannot move past friendly pieces... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at d4 (row 4, col 3)
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place white pawn at d6 (row 2, col 3) - friendly piece blocking vertical
            Pawn friendlyPawn = new Pawn(2, 3, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(friendlyPawn, new Location(2, 3));

            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Queen should not be able to reach d6 or beyond (d7, d8)
            bool canReachD6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3);
            bool canReachD7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 3);
            bool canReachD8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 3);

            if (!canReachD6 && !canReachD7 && !canReachD8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Queen should not move past friendly piece)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a queen cannot move past an enemy piece.
        /// </summary>
        public static void Test_Queen_CannotMovePastEnemyPiece()
        {
            Console.Write("  Testing queen cannot move past enemy piece... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at d4 (row 4, col 3)
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            // Place black pawn at d6 (row 2, col 3) - enemy piece
            Pawn enemyPawn = new Pawn(2, 3, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(enemyPawn, new Location(2, 3));

            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Queen should be able to capture d6 but not reach beyond (d7, d8)
            bool canReachD6 = moves.Any(m => m.To.Location.Row == 2 && m.To.Location.Column == 3);
            bool canReachD7 = moves.Any(m => m.To.Location.Row == 1 && m.To.Location.Column == 3);
            bool canReachD8 = moves.Any(m => m.To.Location.Row == 0 && m.To.Location.Column == 3);

            if (canReachD6 && !canReachD7 && !canReachD8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Queen should capture but not move past enemy)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Test that a queen moves in all 8 directions (horizontal, vertical, and diagonal).
        /// </summary>
        public static void Test_Queen_MovesInAllDirections()
        {
            Console.Write("  Testing queen moves in all 8 directions... ");

            IChessBoard board = new ChessBoard(PieceColor.White);

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board.RemovePiece(new Location(row, col));
                }
            }

            // Place white queen at d4 (row 4, col 3)
            Queen queen = new Queen(4, 3, PieceColor.White);
            board.PlacePiece(queen, new Location(4, 3));

            List<Move> moves = queen.GetMoves(new Location(4, 3), board);

            // Check that queen can move in all 8 directions
            bool hasUp = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column == 3);
            bool hasDown = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column == 3);
            bool hasLeft = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column < 3);
            bool hasRight = moves.Any(m => m.To.Location.Row == 4 && m.To.Location.Column > 3);
            bool hasUpLeft = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column < 3);
            bool hasUpRight = moves.Any(m => m.To.Location.Row < 4 && m.To.Location.Column > 3);
            bool hasDownLeft = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column < 3);
            bool hasDownRight = moves.Any(m => m.To.Location.Row > 4 && m.To.Location.Column > 3);

            if (hasUp && hasDown && hasLeft && hasRight && hasUpLeft && hasUpRight && hasDownLeft && hasDownRight)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ PASSED");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ FAILED (Queen should move in all 8 directions)");
                Console.ResetColor();
            }
        }
    }
}
