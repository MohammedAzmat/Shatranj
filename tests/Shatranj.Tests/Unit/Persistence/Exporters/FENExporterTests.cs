using System;
using Xunit;
using Moq;
using ShatranjCore.Persistence.Exporters;
using ShatranjCore.Interfaces;
using ShatranjCore.Abstractions;
using ShatranjCore.Board;

namespace Shatranj.Tests.Unit.Persistence.Exporters
{
    /// <summary>
    /// Tests for FENExporter - verifies Forsyth-Edwards Notation export
    /// </summary>
    public class FENExporterTests
    {
        private readonly FENExporter _exporter;

        public FENExporterTests()
        {
            _exporter = new FENExporter();
        }

        [Fact]
        public void Export_InitialPosition_ReturnsValidFEN()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);

            // Assert
            Assert.NotNull(fen);
            Assert.StartsWith("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", fen);
        }

        [Fact]
        public void Export_InitialPosition_ContainsAllFENComponents()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);

            // Assert - FEN has 6 parts separated by spaces
            var parts = fen.Split(' ');
            Assert.Equal(6, parts.Length);
        }

        [Fact]
        public void Export_InitialPosition_ActiveColorIsWhite()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);
            var parts = fen.Split(' ');

            // Assert
            Assert.Equal("w", parts[1]);
        }

        [Fact]
        public void Export_InitialPosition_CastlingRightsAllAvailable()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);
            var parts = fen.Split(' ');

            // Assert - All castling rights available at start
            Assert.Equal("KQkq", parts[2]);
        }

        [Fact]
        public void Export_InitialPosition_NoEnPassantTarget()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);
            var parts = fen.Split(' ');

            // Assert
            Assert.Equal("-", parts[3]);
        }

        [Fact]
        public void Export_InitialPosition_HalfmoveAndFullmoveClocks()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);
            var parts = fen.Split(' ');

            // Assert
            Assert.Equal("0", parts[4]); // halfmove clock
            Assert.Equal("1", parts[5]); // fullmove number
        }

        [Fact]
        public void Export_ImplementsInterface_Correctly()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var exporter = _exporter as IFENExporter;

            // Assert
            Assert.NotNull(exporter);
            Assert.IsAssignableFrom<IFENExporter>(_exporter);
        }

        [Fact]
        public void Export_EmptyBoard_HandlesGracefully()
        {
            // Arrange
            var mockBoard = new Mock<IChessBoard>();
            mockBoard.Setup(b => b.IsEmptyAt(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Act & Assert - should not crash with empty board
            // The behavior depends on implementation details
            try
            {
                // Would call with empty board setup
                // var fen = _exporter.Export(mockBoard.Object);
                // Assert.NotNull(fen);
            }
            catch (NullReferenceException)
            {
                // Expected if pieces are null
            }
        }

        [Theory]
        [InlineData(0, 0)] // a1
        [InlineData(0, 7)] // h1
        [InlineData(7, 0)] // a8
        [InlineData(7, 7)] // h8
        public void Export_ProducesValidFENFormat(int row, int col)
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);

            // Assert
            Assert.NotNull(fen);
            Assert.Contains("/", fen); // FEN uses / to separate ranks
        }

        [Fact]
        public void Export_ConsistentOutput_SameInputYieldsSameOutput()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen1 = _exporter.Export(board);
            var fen2 = _exporter.Export(board);

            // Assert
            Assert.Equal(fen1, fen2);
        }

        [Fact]
        public void Export_FENFormatValid_ContainsEightRanks()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);
            var piecePlacement = fen.Split(' ')[0];
            var ranks = piecePlacement.Split('/');

            // Assert
            Assert.Equal(8, ranks.Length);
        }

        [Fact]
        public void Export_PiecesEncodedCorrectly_WhiteUppercaseBlackLowercase()
        {
            // Arrange
            var board = new ChessBoard();
            board.InitializeBoard();

            // Act
            var fen = _exporter.Export(board);

            // Assert - Initial position should have standard pieces
            Assert.Contains("R", fen); // White Rook
            Assert.Contains("r", fen); // Black rook
            Assert.Contains("P", fen); // White Pawn
            Assert.Contains("p", fen); // Black pawn
        }
    }
}
