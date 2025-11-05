using System;
using System.Collections.Generic;
using System.Linq;

namespace ShatranjCore
{
    /// <summary>
    /// Enhanced Chess Game with improved terminal UI and command system.
    /// Follows SOLID principles with dependency injection and separation of concerns.
    /// </summary>
    public class EnhancedChessGame
    {
        private readonly IChessBoard board;
        private readonly ConsoleBoardRenderer renderer;
        private readonly CommandParser commandParser;
        private readonly MoveHistory moveHistory;
        private readonly List<Piece> capturedPieces;

        private Player[] players;
        private PieceColor currentPlayer;
        private GameResult gameResult;
        private bool isRunning;

        public EnhancedChessGame()
        {
            board = new ChessBoard(PieceColor.White);
            renderer = new ConsoleBoardRenderer();
            commandParser = new CommandParser();
            moveHistory = new MoveHistory();
            capturedPieces = new List<Piece>();
            gameResult = GameResult.InProgress;
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void Start()
        {
            InitializeGame();
            GameLoop();
        }

        /// <summary>
        /// Initializes a new game.
        /// </summary>
        private void InitializeGame()
        {
            currentPlayer = PieceColor.White;
            isRunning = true;
            gameResult = GameResult.InProgress;
            capturedPieces.Clear();
            moveHistory.Clear();

            // Initialize players
            players = new Player[2];
            players[0] = new Player(PieceColor.White, PlayerType.Human) { HasTurn = true };
            players[1] = new Player(PieceColor.Black, PlayerType.Human) { HasTurn = false };

            renderer.DisplayInfo("New game started! White moves first.");
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        private void GameLoop()
        {
            while (isRunning)
            {
                // Render the board
                var lastMove = moveHistory.GetLastMove();
                Location? lastFrom = lastMove != null ? (Location?)lastMove.Move.From.Location : null;
                Location? lastTo = lastMove != null ? (Location?)lastMove.Move.To.Location : null;

                renderer.RenderBoard(board, lastFrom, lastTo);

                // Display game status
                var status = new GameStatus
                {
                    CurrentPlayer = currentPlayer,
                    IsCheck = false, // TODO: Implement check detection
                    LastMove = lastMove?.AlgebraicNotation,
                    CapturedPieces = capturedPieces
                };
                renderer.DisplayGameStatus(status);

                // Get and process command
                Console.Write($"{currentPlayer} > ");
                string input = Console.ReadLine();

                ProcessCommand(input);
            }
        }

        /// <summary>
        /// Processes a user command.
        /// </summary>
        private void ProcessCommand(string input)
        {
            var command = commandParser.Parse(input);

            switch (command.Type)
            {
                case CommandType.Move:
                    HandleMoveCommand(command);
                    break;

                case CommandType.ShowMoves:
                    HandleShowMovesCommand(command);
                    break;

                case CommandType.ShowHelp:
                    renderer.DisplayCommands();
                    WaitForKey();
                    break;

                case CommandType.ShowHistory:
                    moveHistory.DisplayHistory();
                    WaitForKey();
                    break;

                case CommandType.StartGame:
                    renderer.DisplayInfo("Starting new game...");
                    InitializeGame();
                    break;

                case CommandType.RestartGame:
                    renderer.DisplayInfo("Restarting game...");
                    InitializeGame();
                    break;

                case CommandType.EndGame:
                    renderer.DisplayInfo("Ending game...");
                    isRunning = false;
                    break;

                case CommandType.SaveGame:
                    renderer.DisplayInfo("Game saving not yet implemented.");
                    WaitForKey();
                    break;

                case CommandType.Quit:
                    renderer.DisplayInfo("Thanks for playing Shatranj!");
                    isRunning = false;
                    break;

                case CommandType.Invalid:
                    renderer.DisplayError(command.ErrorMessage);
                    WaitForKey();
                    break;
            }
        }

        /// <summary>
        /// Handles a move command.
        /// </summary>
        private void HandleMoveCommand(GameCommand command)
        {
            try
            {
                Piece piece = board.GetPiece(command.From);

                // Validate piece exists
                if (piece == null)
                {
                    renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                    WaitForKey();
                    return;
                }

                // Validate piece belongs to current player
                if (piece.Color != currentPlayer)
                {
                    renderer.DisplayError($"That piece belongs to {piece.Color}, not {currentPlayer}!");
                    WaitForKey();
                    return;
                }

                // Check if move is valid
                if (!piece.CanMove(command.From, command.To, board))
                {
                    renderer.DisplayError($"Illegal move for {piece.GetType().Name}");
                    WaitForKey();
                    return;
                }

                // Execute the move
                ExecuteMove(command.From, command.To, piece);

                // Switch turns
                SwitchTurns();
            }
            catch (Exception ex)
            {
                renderer.DisplayError($"Error processing move: {ex.Message}");
                WaitForKey();
            }
        }

        /// <summary>
        /// Executes a move on the board.
        /// </summary>
        private void ExecuteMove(Location from, Location to, Piece piece)
        {
            Piece capturedPiece = board.GetPiece(to);
            bool wasCapture = capturedPiece != null;

            if (wasCapture)
            {
                capturedPieces.Add(capturedPiece);
                renderer.DisplayInfo($"{piece.GetType().Name} captures {capturedPiece.GetType().Name}!");
            }

            // Move the piece
            board.RemovePiece(from);
            board.PlacePiece(piece, to);
            piece.isMoved = true;

            // Record the move
            Move move = new Move(
                piece,
                new Square(from.Row, from.Column, piece),
                new Square(to.Row, to.Column, capturedPiece),
                capturedPiece
            );

            moveHistory.AddMove(move, currentPlayer, wasCapture);

            // TODO: Check for checkmate, stalemate, etc.
        }

        /// <summary>
        /// Handles showing available moves for a piece.
        /// </summary>
        private void HandleShowMovesCommand(GameCommand command)
        {
            Piece piece = board.GetPiece(command.From);

            if (piece == null)
            {
                renderer.DisplayError($"No piece at {LocationToAlgebraic(command.From)}");
                WaitForKey();
                return;
            }

            if (piece.Color != currentPlayer)
            {
                renderer.DisplayError($"That piece belongs to {piece.Color}, not {currentPlayer}!");
                WaitForKey();
                return;
            }

            List<Move> moves = piece.GetMoves(command.From, board);
            renderer.DisplayPossibleMoves(command.From, moves);
            WaitForKey();
        }

        /// <summary>
        /// Switches to the next player's turn.
        /// </summary>
        private void SwitchTurns()
        {
            currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            players[0].HasTurn = !players[0].HasTurn;
            players[1].HasTurn = !players[1].HasTurn;
        }

        /// <summary>
        /// Waits for user to press a key.
        /// </summary>
        private void WaitForKey()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Converts a Location to algebraic notation.
        /// </summary>
        private string LocationToAlgebraic(Location location)
        {
            char file = (char)('a' + location.Column);
            int rank = 8 - location.Row;
            return $"{file}{rank}";
        }
    }
}
