using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to JSON (array of objects, one per row).
    /// </summary>
    public sealed class GridJsonExporter : IGridExporter, IAsyncGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Json;
        public string Description => "JavaScript Object Notation (JSON)";
        public string FileExtension => ".json";
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
            progress?.Report(new GridExportProgress { RowsProcessed = 0, TotalRows = total, Message = "Starting JSON export" });

            var data = new List<Dictionary<string, object?>>(total);
            for (int i = 0; i < snapshot.Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = snapshot.Rows[i];
                var dict = new Dictionary<string, object?>();
                foreach (var column in snapshot.Columns)
                {
                    row.Values.TryGetValue(column.ColumnName, out var value);
                    dict[column.ColumnName] = GridExportValueFormatter.Normalize(value, column.DataType, options);
                }
                data.Add(dict);

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

            var settings = new JsonSerializerSettings
            {
                Formatting = options.JsonIndented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                DateFormatString = options.DateTimeFormat
            };

            string json = JsonConvert.SerializeObject(data, settings);
            var bytes = Encoding.UTF8.GetBytes(json);
            output.Write(bytes, 0, bytes.Length);
        }
    }
}
