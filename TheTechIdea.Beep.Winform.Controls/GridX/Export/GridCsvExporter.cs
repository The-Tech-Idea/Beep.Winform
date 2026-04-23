using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to CSV (RFC 4180 compliant).
    /// </summary>
    public sealed class GridCsvExporter : IGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Csv;
        public string Description => "Comma-separated values (CSV)";
        public string FileExtension => ".csv";
        public bool IsAvailable => true;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            options ??= new ExportOptions();
            var columns = ResolveColumns(grid, options);
            var rows = ResolveRows(grid, options);

            using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);

            if (options.IncludeHeaders)
            {
                WriteLine(writer, columns.Select(c => c.ColumnCaption ?? c.ColumnName), options);
            }

            foreach (var row in rows)
            {
                var values = new List<string>(columns.Count);
                foreach (var col in columns)
                {
                    var cell = FindCell(row, col.ColumnName);
                    values.Add(FormatValue(cell?.CellValue, col.DataType, options));
                }
                WriteLine(writer, values, options);
            }
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

            // Double-up quotes inside the value
            string escaped = value.Replace(options.CsvQuote.ToString(), new string(options.CsvQuote, 2));
            return options.CsvQuote + escaped + options.CsvQuote;
        }

        private static string FormatValue(object? value, Type? dataType, ExportOptions options)
        {
            if (value == null) return string.Empty;
            if (value is DateTime dt)
                return dt.ToString(options.DateTimeFormat);
            if (value is DateTimeOffset dto)
                return dto.ToString(options.DateTimeFormat);
            if (value is IFormattable fmt && !string.IsNullOrEmpty(options.NumberFormat))
                return fmt.ToString(options.NumberFormat, System.Globalization.CultureInfo.InvariantCulture);
            return value.ToString() ?? string.Empty;
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
