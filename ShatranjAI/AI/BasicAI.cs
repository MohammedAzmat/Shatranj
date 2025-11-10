using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ShatranjCore;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;
using ShatranjCore.Interfaces;
using ShatranjCore.Logging;
using ShatranjCore.Pieces;
using ShatranjCore.Validators;

namespace ShatranjAI.AI
{
    /// <summary>
    /// Basic AI implementation using minimax algorithm with alpha-beta pruning
    /// </summary>
    public class BasicAI : IChessAI
    {
        private readonly MoveEvaluator evaluator;
        private readonly CheckDetector checkDetector;
        private readonly ILogger logger;
        private int nodesEvaluated;

        public string Name => "BasicAI";
        public string Version => "1.0";
        public int Depth { get; set; }

        public BasicAI(int depth = 3, ILogger logger = null)
        {
            this.Depth = depth;
            this.evaluator = new MoveEvaluator();
            this.checkDetector = new CheckDetector();
            this.logger = logger;
        }

        /// <summary>
        /// Selects the best move using minimax with alpha-beta pruning
        /// </summary>
        public AIMove SelectMove(IChessBoard board, PieceColor color, Location? enPassantTarget)
        {
            logger?.Info($"{Name} thinking (Depth: {Depth})...");
            Stopwatch sw = Stopwatch.StartNew();
            nodesEvaluated = 0;

            // Get all legal moves
            List<(Location from, Move move)> legalMoves = GetAllLegalMoves(board, color, enPassantTarget);

            if (legalMoves.Count == 0)
            {
                logger?.Warning("No legal moves available!");
                return null;
            }

            // Special case: only one legal move
            if (legalMoves.Count == 1)
            {
                var (from, move) = legalMoves[0];
                sw.Stop();
                logger?.Info($"Only one legal move: {LocationToAlgebraic(from)} -> {LocationToAlgebraic(move.To.Location)}");
                return new AIMove(from, move.To.Location, 0)
                {
                    ThinkingTimeMs = (int)sw.ElapsedMilliseconds,
                    NodesEvaluated = 1,
                    Depth = 1
                };
            }

            // Evaluate each move using minimax
            double bestScore = double.NegativeInfinity;
            (Location from, Move move)? bestMove = null;

            foreach (var (from, move) in legalMoves)
            {
                // Make the move
                Piece piece = board.GetPiece(from);
                Piece captured = board.GetPiece(move.To.Location);
                board.RemovePiece(from);
                board.PlacePiece(piece, move.To.Location);

                // Evaluate with minimax (opponent's turn, so we minimize)
                double score = Minimax(board, Depth - 1, double.NegativeInfinity, double.PositiveInfinity, false, color, enPassantTarget);

                // Undo the move
                board.RemovePiece(move.To.Location);
                board.PlacePiece(piece, from);
                if (captured != null)
                {
                    board.PlacePiece(captured, move.To.Location);
                }

                // Update best move
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (from, move);
                }
            }

            sw.Stop();

            if (bestMove.HasValue)
            {
                var (from, move) = bestMove.Value;
                logger?.Info($"Best move: {LocationToAlgebraic(from)} -> {LocationToAlgebraic(move.To.Location)} " +
                           $"(Score: {bestScore:F2}, Nodes: {nodesEvaluated}, Time: {sw.ElapsedMilliseconds}ms)");

                return new AIMove(from, move.To.Location, bestScore)
                {
                    ThinkingTimeMs = (int)sw.ElapsedMilliseconds,
                    NodesEvaluated = nodesEvaluated,
                    Depth = Depth
                };
            }

            // Fallback: return a random legal move
            logger?.Warning("Minimax failed, selecting random move");
            var random = new Random();
            var fallback = legalMoves[random.Next(legalMoves.Count)];
            return new AIMove(fallback.from, fallback.move.To.Location, 0);
        }

        /// <summary>
        /// Minimax algorithm with alpha-beta pruning
        /// </summary>
        private double Minimax(IChessBoard board, int depth, double alpha, double beta, bool maximizingPlayer, PieceColor aiColor, Location? enPassantTarget)
        {
            nodesEvaluated++;

            // Base case: reached depth limit or game over
            if (depth == 0)
            {
                return evaluator.Evaluate(board, aiColor);
            }

            PieceColor currentColor = maximizingPlayer ? aiColor : GetOpponentColor(aiColor);

            // Check for checkmate or stalemate
            if (checkDetector.IsCheckmate(board, currentColor))
            {
                // Checkmate is very bad for the losing side
                return maximizingPlayer ? -999999 : 999999;
            }

            if (checkDetector.IsStalemate(board, currentColor))
            {
                return 0; // Stalemate is a draw
            }

            List<(Location from, Move move)> legalMoves = GetAllLegalMoves(board, currentColor, enPassantTarget);

            if (legalMoves.Count == 0)
            {
                // No legal moves (shouldn't happen if checkmate/stalemate already checked)
                return 0;
            }

            if (maximizingPlayer)
            {
                double maxEval = double.NegativeInfinity;

                foreach (var (from, move) in legalMoves)
                {
                    // Make move
                    Piece piece = board.GetPiece(from);
                    Piece captured = board.GetPiece(move.To.Location);
                    board.RemovePiece(from);
                    board.PlacePiece(piece, move.To.Location);

                    // Recurse
                    double eval = Minimax(board, depth - 1, alpha, beta, false, aiColor, enPassantTarget);

                    // Undo move
                    board.RemovePiece(move.To.Location);
                    board.PlacePiece(piece, from);
                    if (captured != null)
                    {
                        board.PlacePiece(captured, move.To.Location);
                    }

                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha)
                        break; // Beta cutoff
                }

                return maxEval;
            }
            else
            {
                double minEval = double.PositiveInfinity;

                foreach (var (from, move) in legalMoves)
                {
                    // Make move
                    Piece piece = board.GetPiece(from);
                    Piece captured = board.GetPiece(move.To.Location);
                    board.RemovePiece(from);
                    board.PlacePiece(piece, move.To.Location);

                    // Recurse
                    double eval = Minimax(board, depth - 1, alpha, beta, true, aiColor, enPassantTarget);

                    // Undo move
                    board.RemovePiece(move.To.Location);
                    board.PlacePiece(piece, from);
                    if (captured != null)
                    {
                        board.PlacePiece(captured, move.To.Location);
                    }

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                        break; // Alpha cutoff
                }

                return minEval;
            }
        }

        /// <summary>
        /// Gets all legal moves for a color
        /// </summary>
        private List<(Location from, Move move)> GetAllLegalMoves(IChessBoard board, PieceColor color, Location? enPassantTarget)
        {
            var legalMoves = new List<(Location from, Move move)>();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Location from = new Location(row, col);
                    Piece piece = board.GetPiece(from);

                    if (piece != null && piece.Color == color)
                    {
                        // Get legal moves for this piece (moves that don't leave king in check)
                        List<Move> moves = checkDetector.GetLegalMoves(board, from, color, enPassantTarget);

                        foreach (var move in moves)
                        {
                            legalMoves.Add((from, move));
                        }
                    }
                }
            }

            return legalMoves;
        }

        /// <summary>
        /// Evaluates the current position
        /// </summary>
        public double EvaluatePosition(IChessBoard board, PieceColor color)
        {
            return evaluator.Evaluate(board, color);
        }

        private PieceColor GetOpponentColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
