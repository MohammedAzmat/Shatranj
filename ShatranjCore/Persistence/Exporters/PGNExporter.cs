using System;
using System.Collections.Generic;
using System.Text;
using ShatranjCore.Movement;

namespace ShatranjCore.Persistence.Exporters
{
    /// <summary>
    /// Exports games to PGN (Portable Game Notation) format.
    /// Single Responsibility: Only converts MoveRecords to PGN string.
    /// </summary>
    public class PGNExporter : IPGNExporter
    {
        public string Export(List<MoveRecord> moves, PGNMetadata metadata = null)
        {
            var sb = new StringBuilder();

            // Add metadata if provided
            if (metadata != null)
            {
                AppendTag(sb, "Event", metadata.Event ?? "?");
                AppendTag(sb, "Site", metadata.Site ?? "?");
                AppendTag(sb, "Date", metadata.Date ?? DateTime.Now.ToString("yyyy.MM.dd"));
                AppendTag(sb, "White", metadata.White ?? "White");
                AppendTag(sb, "Black", metadata.Black ?? "Black");
                AppendTag(sb, "Result", metadata.Result ?? "*");

                if (!string.IsNullOrEmpty(metadata.WhiteElo))
                    AppendTag(sb, "WhiteElo", metadata.WhiteElo);
                if (!string.IsNullOrEmpty(metadata.BlackElo))
                    AppendTag(sb, "BlackElo", metadata.BlackElo);
                if (!string.IsNullOrEmpty(metadata.TimeControl))
                    AppendTag(sb, "TimeControl", metadata.TimeControl);
                if (!string.IsNullOrEmpty(metadata.ECO))
                    AppendTag(sb, "ECO", metadata.ECO);
                if (!string.IsNullOrEmpty(metadata.Opening))
                    AppendTag(sb, "Opening", metadata.Opening);

                sb.AppendLine();
            }

            // Add moves in PGN format
            for (int i = 0; i < moves.Count; i++)
            {
                var record = moves[i];

                // Add move number before white's move
                if (i % 2 == 0)
                {
                    int moveNumber = (i / 2) + 1;
                    sb.Append($"{moveNumber}. ");
                }

                sb.Append(record.AlgebraicNotation);
                sb.Append(" ");

                // Add line break every 2 moves (after black's move)
                if ((i + 1) % 2 == 0)
                {
                    sb.AppendLine();
                }
            }

            // Add final result if provided
            if (metadata?.Result != null && metadata.Result != "*")
            {
                sb.Append(metadata.Result);
            }

            return sb.ToString();
        }

        private void AppendTag(StringBuilder sb, string tagName, string tagValue)
        {
            sb.AppendLine($"[{tagName} \"{tagValue}\"]");
        }
    }
}
