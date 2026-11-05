using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to a self-contained HTML table.
    /// </summary>
    public sealed class GridHtmlExporter : IGridExporter, IAsyncGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Html;
        public string Description => "HyperText Markup Language (HTML)";
        public string FileExtension => ".html";
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
            progress?.Report(new GridExportProgress { RowsProcessed = 0, TotalRows = total, Message = "Starting HTML export" });

            using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);

            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"utf-8\"/>");
            writer.WriteLine($"<title>{HtmlEncode(snapshot.GridTitle)}</title>");
            if (!string.IsNullOrEmpty(options.HtmlInlineStyles))
            {
                writer.WriteLine("<style>");
                writer.WriteLine(options.HtmlInlineStyles);
                writer.WriteLine("</style>");
            }
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");

            writer.WriteLine($"<table class=\"{HtmlEncode(options.HtmlTableClass)}\">");

            if (options.IncludeHeaders)
            {
                writer.WriteLine("<thead>");
                writer.WriteLine("<tr>");
                foreach (var col in snapshot.Columns)
                {
                    writer.WriteLine($"<th>{HtmlEncode(col.ColumnCaption ?? col.ColumnName)}</th>");
                }
                writer.WriteLine("</tr>");
                writer.WriteLine("</thead>");
            }

            writer.WriteLine("<tbody>");
            for (int i = 0; i < snapshot.Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = snapshot.Rows[i];
                writer.WriteLine("<tr>");
                foreach (var col in snapshot.Columns)
                {
                    row.Values.TryGetValue(col.ColumnName, out var value);
                    writer.WriteLine($"<td>{HtmlEncode(GridExportValueFormatter.ToText(value, col.DataType, options))}</td>");
                }
                writer.WriteLine("</tr>");

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
            writer.WriteLine("</tbody>");
            writer.WriteLine("</table>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
            writer.Flush();
        }

        private static string HtmlEncode(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            try
            {
                return HttpUtility.HtmlEncode(text);
            }
            catch (Exception ex)
            {
                BeepLog.FallbackOnce("GridHtmlExporter.HtmlEncode", null, "encode HTML text", ex);
                return ManualHtmlEncode(text);
            }
        }

        private static string ManualHtmlEncode(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                switch (c)
                {
                    case '&': sb.Append("&amp;"); break;
                    case '<': sb.Append("&lt;"); break;
                    case '>': sb.Append("&gt;"); break;
                    case '"': sb.Append("&quot;"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
    }
}
