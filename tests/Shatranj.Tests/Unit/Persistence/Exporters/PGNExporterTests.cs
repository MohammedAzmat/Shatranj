using System;
using System.Collections.Generic;
using Xunit;
using ShatranjCore.Persistence.Exporters;
using ShatranjCore.Movement;
using ShatranjCore.Abstractions;

namespace Shatranj.Tests.Unit.Persistence.Exporters
{
    /// <summary>
    /// Tests for PGNExporter - verifies Portable Game Notation export
    /// </summary>
    public class PGNExporterTests
    {
        private readonly PGNExporter _exporter;

        public PGNExporterTests()
        {
            _exporter = new PGNExporter();
        }

        [Fact]
        public void Export_EmptyMoveList_ReturnsEmptyOrMinimalPGN()
        {
            // Arrange
            var moves = new List<MoveRecord>();
            var metadata = new PGNMetadata { Event = "Test" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.NotNull(pgn);
            Assert.Contains("[Event \"Test\"]", pgn);
        }

        [Fact]
        public void Export_SingleMove_IncludesMoveNumber()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = Abstractions.PieceColor.White }
            };
            var metadata = new PGNMetadata { Event = "Test Game" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("1. e4", pgn);
        }

        [Fact]
        public void Export_TwoMoves_IncludesBothMoves()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = PieceColor.White },
                new MoveRecord { AlgebraicNotation = "c5", Player = PieceColor.Black }
            };
            var metadata = new PGNMetadata { Event = "Sicilian" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("1. e4 c5", pgn);
        }

        [Fact]
        public void Export_CaptureMove_IncludesXNotation()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = PieceColor.White },
                new MoveRecord { AlgebraicNotation = "d5", Player = PieceColor.Black },
                new MoveRecord { AlgebraicNotation = "exd5", Player = PieceColor.White }
            };
            var metadata = new PGNMetadata { Event = "Test" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("exd5", pgn);
        }

        [Fact]
        public void Export_MetadataIncluded_ValidPGNTags()
        {
            // Arrange
            var moves = new List<MoveRecord>();
            var metadata = new PGNMetadata
            {
                Event = "World Championship",
                Site = "New York",
                Date = "2024.01.15",
                White = "World Champion",
                Black = "Challenger",
                Result = "1-0"
            };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("[Event \"World Championship\"]", pgn);
            Assert.Contains("[Site \"New York\"]", pgn);
            Assert.Contains("[Date \"2024.01.15\"]", pgn);
            Assert.Contains("[White \"World Champion\"]", pgn);
            Assert.Contains("[Black \"Challenger\"]", pgn);
            Assert.Contains("[Result \"1-0\"]", pgn);
        }

        [Fact]
        public void Export_OptionalMetadata_IncludedWhenProvided()
        {
            // Arrange
            var moves = new List<MoveRecord>();
            var metadata = new PGNMetadata
            {
                Event = "Test",
                WhiteElo = "2800",
                BlackElo = "2750",
                TimeControl = "60+0",
                ECO = "C20",
                Opening = "King's Pawn Opening"
            };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("[WhiteElo \"2800\"]", pgn);
            Assert.Contains("[BlackElo \"2750\"]", pgn);
            Assert.Contains("[TimeControl \"60+0\"]", pgn);
            Assert.Contains("[ECO \"C20\"]", pgn);
            Assert.Contains("[Opening \"King's Pawn Opening\"]", pgn);
        }

        [Fact]
        public void Export_NoMetadata_StillExportsMovesCorrectly()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = Abstractions.PieceColor.White },
                new MoveRecord { AlgebraicNotation = "c5", Player = Abstractions.PieceColor.Black }
            };

            // Act
            var pgn = _exporter.Export(moves, null);

            // Assert
            Assert.NotNull(pgn);
            Assert.Contains("1. e4 c5", pgn);
        }

        [Fact]
        public void Export_MultipleFullGames_ContainsAllMoves()
        {
            // Arrange
            var moves = new List<MoveRecord>();
            for (int i = 0; i < 40; i++) // 20 moves each side
            {
                var isWhite = (i % 2 == 0);
                moves.Add(new MoveRecord
                {
                    AlgebraicNotation = $"{(char)('a' + (i % 8))}{(1 + (i % 8))}",
                    Player = isWhite ? PieceColor.White : PieceColor.Black
                });
            }
            var metadata = new PGNMetadata { Event = "Long Game" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.NotNull(pgn);
            Assert.Contains("20.", pgn); // Should have move 20
            Assert.Contains("[Event \"Long Game\"]", pgn);
        }

        [Fact]
        public void Export_MoveNumberIncrementsCorrectly()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = PieceColor.White },
                new MoveRecord { AlgebraicNotation = "c5", Player = PieceColor.Black },
                new MoveRecord { AlgebraicNotation = "Nf3", Player = PieceColor.White },
                new MoveRecord { AlgebraicNotation = "d6", Player = PieceColor.Black }
            };
            var metadata = new PGNMetadata { Event = "Sicilian Defense" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("1.", pgn); // Move 1
            Assert.Contains("2.", pgn); // Move 2
        }

        [Fact]
        public void Export_WithResult_IncludesResultAtEnd()
        {
            // Arrange
            var moves = new List<MoveRecord>
            {
                new MoveRecord { AlgebraicNotation = "e4", Player = Abstractions.PieceColor.White }
            };
            var metadata = new PGNMetadata { Event = "Test", Result = "1-0" };

            // Act
            var pgn = _exporter.Export(moves, metadata);

            // Assert
            Assert.Contains("1-0", pgn);
        }
    }
}
