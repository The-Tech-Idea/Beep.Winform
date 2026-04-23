using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Exports grid data to JSON (array of objects, one per row).
    /// </summary>
    public sealed class GridJsonExporter : IGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Json;
        public string Description => "JavaScript Object Notation (JSON)";
        public string FileExtension => ".json";
        public bool IsAvailable => true;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            options ??= new ExportOptions();
            var columns = ResolveColumns(grid, options);
            var rows = ResolveRows(grid, options);

            var data = new List<Dictionary<string, object?>>();
            foreach (var row in rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var col in columns)
                {
                    var cell = FindCell(row, col.ColumnName);
                    dict[col.ColumnName] = NormalizeValue(cell?.CellValue, col.DataType, options);
                }
                data.Add(dict);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = options.JsonIndented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                DateFormatString = options.DateTimeFormat
            };

            string json = JsonConvert.SerializeObject(data, settings);
            var bytes = Encoding.UTF8.GetBytes(json);
            output.Write(bytes, 0, bytes.Length);
        }

        private static object? NormalizeValue(object? value, Type? dataType, ExportOptions options)
        {
            if (value == null) return null;
            if (value is DateTime dt)
                return dt.ToString(options.DateTimeFormat);
            if (value is DateTimeOffset dto)
                return dto.ToString(options.DateTimeFormat);
            return value;
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
