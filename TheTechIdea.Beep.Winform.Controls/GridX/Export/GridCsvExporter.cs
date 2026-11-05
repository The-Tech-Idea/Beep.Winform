using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to CSV (RFC 4180 compliant).
    /// </summary>
    public sealed class GridCsvExporter : IGridExporter, IAsyncGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Csv;
        public string Description => "Comma-separated values (CSV)";
        public string FileExtension => ".csv";
        public bool IsAvailable => true;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            var snapshot = GridExportSnapshot.Capture(grid, options);
            Write(snapshot, output, null, CancellationToken.None);
        }

        public Task ExportAsync(GridExportSnapshot snapshot, Stream output, ExportOptions? options,
            IProgress<GridExportProgress>? progress, CancellationToken cancellationToken)
        {
            return Task.Run(() => Write(snapshot, output, progress, cancellationToken), cancellationToken);
        }

        private static void Write(GridExportSnapshot snapshot, Stream output,
            IProgress<GridExportProgress>? progress, CancellationToken cancellationToken)
        {
            var options = snapshot.Options;
            int total = snapshot.Rows.Count;
            progress?.Report(new GridExportProgress { RowsProcessed = 0, TotalRows = total, Message = "Starting CSV export" });

            using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);

            if (options.IncludeHeaders)
            {
                WriteLine(writer, snapshot.Columns.Select(c => c.ColumnCaption ?? c.ColumnName), options);
            }

            for (int i = 0; i < snapshot.Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = snapshot.Rows[i];
                var values = new List<string>(snapshot.Columns.Count);
                foreach (var column in snapshot.Columns)
                {
                    row.Values.TryGetValue(column.ColumnName, out var value);
                    values.Add(GridExportValueFormatter.ToText(value, column.DataType, options));
                }
                WriteLine(writer, values, options);

                if ((i + 1) % 200 == 0 || i + 1 == total)
                {
                    progress?.Report(new GridExportProgress
                    {
                        RowsProcessed = i + 1,
                        TotalRows = total,
                        Message = $"Exported {i + 1} of {total} rows"
                    });
                }
            }

            writer.Flush();
        }

        private static void WriteLine(StreamWriter writer, IEnumerable<string> values, ExportOptions options)
        {
            bool first = true;
            foreach (var v in values)
            {
                if (!first) writer.Write(options.CsvDelimiter);
                first = false;
                writer.Write(Escape(v, options));
            }
            writer.WriteLine();
        }

        private static string Escape(string? value, ExportOptions options)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            bool needsQuotes = value.Contains(options.CsvDelimiter) ||
                               value.Contains(options.CsvQuote) ||
                               value.Contains('\n') ||
                               value.Contains('\r');
            if (!needsQuotes) return value;

            string escaped = value.Replace(options.CsvQuote.ToString(), new string(options.CsvQuote, 2));
            return options.CsvQuote + escaped + options.CsvQuote;
        }
    }
}
