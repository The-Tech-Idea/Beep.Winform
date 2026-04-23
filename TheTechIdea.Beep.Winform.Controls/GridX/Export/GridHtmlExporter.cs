using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to a self-contained HTML table.
    /// </summary>
    public sealed class GridHtmlExporter : IGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Html;
        public string Description => "HyperText Markup Language (HTML)";
        public string FileExtension => ".html";
        public bool IsAvailable => true;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            options ??= new ExportOptions();
            var columns = ResolveColumns(grid, options);
            var rows = ResolveRows(grid, options);

            using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);

            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"utf-8\"/>");
            writer.WriteLine("<title>Grid Export</title>");
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
                foreach (var col in columns)
                {
                    writer.WriteLine($"<th>{HtmlEncode(col.ColumnCaption ?? col.ColumnName)}</th>");
                }
                writer.WriteLine("</tr>");
                writer.WriteLine("</thead>");
            }

            writer.WriteLine("<tbody>");
            foreach (var row in rows)
            {
                writer.WriteLine("<tr>");
                foreach (var col in columns)
                {
                    var cell = FindCell(row, col.ColumnName);
                    var val = FormatValue(cell?.CellValue, col.DataType, options);
                    writer.WriteLine($"<td>{HtmlEncode(val)}</td>");
                }
                writer.WriteLine("</tr>");
            }
            writer.WriteLine("</tbody>");
            writer.WriteLine("</table>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        private static string FormatValue(object? value, Type? dataType, ExportOptions options)
        {
            if (value == null) return string.Empty;
            if (value is DateTime dt)
                return dt.ToString(options.DateTimeFormat);
            if (value is DateTimeOffset dto)
                return dto.ToString(options.DateTimeFormat);
            return value.ToString() ?? string.Empty;
        }

        private static string HtmlEncode(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            // Use System.Web.HttpUtility if available, otherwise manual fallback
            try
            {
                return HttpUtility.HtmlEncode(text);
            }
            catch
            {
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

        private static List<BeepColumnConfig> ResolveColumns(BeepGridPro grid, ExportOptions options)
        {
            var cols = grid.Data.Columns
                .Where(c => c.Visible || !options.VisibleColumnsOnly)
                .Where(c => !options.ExcludeSystemColumns || !IsSystemColumn(c))
                .Where(c => options.ColumnWhitelist == null || options.ColumnWhitelist.Contains(c.ColumnName))
                .Where(c => options.ColumnBlacklist == null || !options.ColumnBlacklist.Contains(c.ColumnName))
                .ToList();

            if (options.RespectColumnDisplayOrder)
                cols = cols.OrderBy(c => c.DisplayOrder).ToList();

            return cols;
        }

        private static IEnumerable<BeepRowConfig> ResolveRows(BeepGridPro grid, ExportOptions options)
        {
            var rows = grid.Data.Rows.AsEnumerable();
            if (options.VisibleRowsOnly)
                rows = rows.Where(r => r.IsVisible);
            return rows;
        }

        private static BeepCellConfig? FindCell(BeepRowConfig row, string columnName)
        {
            return row.Cells.FirstOrDefault(c => c.ColumnName == columnName);
        }

        private static bool IsSystemColumn(BeepColumnConfig col)
        {
            return col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID;
        }
    }
}
