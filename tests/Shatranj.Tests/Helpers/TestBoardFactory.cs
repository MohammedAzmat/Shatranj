using ShatranjCore.Board;
using ShatranjCore.Pieces;
using ShatranjCore.Abstractions;
using ShatranjCore.Interfaces;

namespace Shatranj.Tests.Helpers
{
    /// <summary>
    /// Factory for creating pre-configured chess boards for testing.
    /// </summary>
    public static class TestBoardFactory
    {
        /// <summary>
        /// Creates a standard starting chess board.
        /// </summary>
        public static ChessBoard CreateStandardBoard()
        {
            var board = new ChessBoard(PieceColor.White);
            return board;
        }

        /// <summary>
        /// Creates an empty chess board.
        /// </summary>
        public static ChessBoard CreateEmptyBoard()
        {
            var board = new ChessBoard(PieceColor.White);

            // Clear all pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var location = new Location(row, col);
                    var piece = board.GetPiece(location);
                    if (piece != null)
                    {
                        board.RemovePiece(location);
                    }
                }
            }

            return board;
        }

        /// <summary>
        /// Creates a board with only kings (useful for endgame testing).
        /// </summary>
        public static ChessBoard CreateKingsOnlyBoard()
        {
            var board = CreateEmptyBoard();

            // White king at e1
            var whiteKing = new King(7, 4, PieceColor.White);
            board.PlacePiece(whiteKing, new Location(7, 4));

            // Black king at e8
            var blackKing = new King(0, 4, PieceColor.Black);
            board.PlacePiece(blackKing, new Location(0, 4));

            return board;
        }

        /// <summary>
        /// Creates a board for testing castling scenarios.
        /// </summary>
        public static ChessBoard CreateCastlingTestBoard()
        {
            var board = CreateEmptyBoard();

            // White pieces
            var whiteKing = new King(7, 4, PieceColor.White);
            var whiteRookKingside = new Rook(7, 7, PieceColor.White);
            var whiteRookQueenside = new Rook(7, 0, PieceColor.White);

            board.PlacePiece(whiteKing, new Location(7, 4));
            board.PlacePiece(whiteRookKingside, new Location(7, 7));
            board.PlacePiece(whiteRookQueenside, new Location(7, 0));

            // Black pieces
            var blackKing = new King(0, 4, PieceColor.Black);
            var blackRookKingside = new Rook(0, 7, PieceColor.Black);
            var blackRookQueenside = new Rook(0, 0, PieceColor.Black);

            board.PlacePiece(blackKing, new Location(0, 4));
            board.PlacePiece(blackRookKingside, new Location(0, 7));
            board.PlacePiece(blackRookQueenside, new Location(0, 0));

            return board;
        }

        /// <summary>
        /// Creates a board for testing pawn promotion scenarios.
        /// </summary>
        public static ChessBoard CreatePromotionTestBoard()
        {
            var board = CreateEmptyBoard();

            // White king
            var whiteKing = new King(7, 4, PieceColor.White);
            board.PlacePiece(whiteKing, new Location(7, 4));

            // White pawn ready for promotion (on rank 7)
            var whitePawn = new Pawn(1, 4, PieceColor.White, PawnMoves.Up);
            board.PlacePiece(whitePawn, new Location(1, 4));

            // Black king
            var blackKing = new King(0, 4, PieceColor.Black);
            board.PlacePiece(blackKing, new Location(0, 4));

            // Black pawn ready for promotion (on rank 2)
            var blackPawn = new Pawn(6, 4, PieceColor.Black, PawnMoves.Down);
            board.PlacePiece(blackPawn, new Location(6, 4));

            return board;
        }

        /// <summary>
        /// Creates a board with all pieces for testing move generation.
        /// </summary>
        public static ChessBoard CreateFullPieceBoard()
        {
            return CreateStandardBoard();
        }
    }
}
