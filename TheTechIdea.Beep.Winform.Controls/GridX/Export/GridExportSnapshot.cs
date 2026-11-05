using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Immutable, UI-thread-created copy of everything an exporter needs.
    /// It holds no <see cref="System.Windows.Forms.Control"/> references, so an
    /// exporter can run on a background thread without touching the grid.
    /// </summary>
    public sealed class GridExportSnapshot
    {
        public string GridTitle { get; init; } = string.Empty;
        public IReadOnlyList<GridExportColumn> Columns { get; init; } = Array.Empty<GridExportColumn>();
        public IReadOnlyList<GridExportRow> Rows { get; init; } = Array.Empty<GridExportRow>();
        public ExportOptions Options { get; init; } = new();
        public DateTimeOffset CapturedAtUtc { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Captures the grid's current visible view. Must be called on the UI
        /// thread; the result is safe to hand to a background exporter.
        /// </summary>
        public static GridExportSnapshot Capture(BeepGridPro grid, ExportOptions? options = null)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));

            var effectiveOptions = options?.Clone() ?? new ExportOptions();

            var columns = grid.Data.Columns
                .Where(c => c.Visible || !effectiveOptions.VisibleColumnsOnly)
                .Where(c => !effectiveOptions.ExcludeSystemColumns || !IsSystemColumn(c))
                .Where(c => effectiveOptions.ColumnWhitelist == null ||
                            effectiveOptions.ColumnWhitelist.Contains(c.ColumnName))
                .Where(c => effectiveOptions.ColumnBlacklist == null ||
                            !effectiveOptions.ColumnBlacklist.Contains(c.ColumnName))
                .ToList();

            if (effectiveOptions.RespectColumnDisplayOrder)
                columns = columns.OrderBy(c => c.DisplayOrder).ToList();

            var exportColumns = columns.Select(c => new GridExportColumn
            {
                ColumnName = c.ColumnName,
                ColumnCaption = c.ColumnCaption,
                DataType = c.DataType,
                DisplayOrder = c.DisplayOrder
            }).ToList();

            var rows = new List<GridExportRow>();
            var view = grid.DataController.BuildView(includeHidden: !effectiveOptions.VisibleRowsOnly);

            foreach (var vr in view.Rows)
            {
                rows.Add(new GridExportRow
                {
                    RowId = vr.RowId,
                    DisplayIndex = vr.DisplayIndex,
                    Values = vr.Values
                });
            }

            return new GridExportSnapshot
            {
                GridTitle = SafeGridTitle(grid),
                Columns = exportColumns,
                Rows = rows,
                Options = effectiveOptions,
                CapturedAtUtc = DateTimeOffset.UtcNow
            };
        }

        private static string SafeGridTitle(BeepGridPro grid)
        {
            try { return grid.GridTitle ?? string.Empty; }
            catch (ObjectDisposedException ex)
            {
                BeepLog.FallbackOnce("GridExportSnapshot.GridTitle", grid, "read grid title", ex);
                return string.Empty;
            }
        }

        private static bool IsSystemColumn(BeepColumnConfig col)
            => col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID;
    }

    public sealed class GridExportColumn
    {
        public string ColumnName { get; init; } = string.Empty;
        public string? ColumnCaption { get; init; }
        public Type? DataType { get; init; }
        public int DisplayOrder { get; init; }
    }

    public sealed class GridExportRow
    {
        public long RowId { get; init; }
        public int DisplayIndex { get; init; }
        public IReadOnlyDictionary<string, object?> Values { get; init; } =
            new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Progress reported by async exporters.</summary>
    public sealed class GridExportProgress
    {
        public int RowsProcessed { get; init; }
        public int TotalRows { get; init; }
        public double Percent => TotalRows <= 0 ? 0 : (double)RowsProcessed / TotalRows * 100d;
        public string? Message { get; init; }
    }

    /// <summary>
    /// Optional capability for exporters that can write without the grid.
    /// Existing <see cref="IGridExporter"/> implementations remain valid.
    /// </summary>
    public interface IAsyncGridExporter
    {
        GridExportFormat Format { get; }
        bool IsAvailable { get; }
        Task ExportAsync(
            GridExportSnapshot snapshot,
            System.IO.Stream output,
            ExportOptions? options,
            IProgress<GridExportProgress>? progress,
            CancellationToken cancellationToken);
    }

    internal static class GridExportValueFormatter
    {
        public static string ToText(object? value, Type? dataType, ExportOptions options)
        {
            if (value == null) return string.Empty;

            if (dataType == typeof(DateTime) || value is DateTime dt)
            {
                if (value is DateTime dateTime) return dateTime.ToString(options.DateTimeFormat);
                if (value is string dateText && DateTime.TryParse(dateText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedDate))
                    return parsedDate.ToString(options.DateTimeFormat);
            }

            if (dataType == typeof(DateTimeOffset) || value is DateTimeOffset dto)
            {
                if (value is DateTimeOffset offset) return offset.ToString(options.DateTimeFormat);
                if (value is string offsetText && DateTimeOffset.TryParse(offsetText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedOffset))
                    return parsedOffset.ToString(options.DateTimeFormat);
            }

            if (!string.IsNullOrEmpty(options.NumberFormat) &&
                IsNumericType(dataType) &&
                value is IFormattable formattable)
                return formattable.ToString(options.NumberFormat, System.Globalization.CultureInfo.InvariantCulture);

            return value.ToString() ?? string.Empty;
        }

        public static object? Normalize(object? value, Type? dataType, ExportOptions options)
        {
            if (value == null) return null;

            if (dataType == typeof(DateTime) || value is DateTime)
            {
                if (value is DateTime dt) return dt.ToString(options.DateTimeFormat);
                if (value is string dateText && DateTime.TryParse(dateText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedDate))
                    return parsedDate.ToString(options.DateTimeFormat);
            }

            if (dataType == typeof(DateTimeOffset) || value is DateTimeOffset)
            {
                if (value is DateTimeOffset dto) return dto.ToString(options.DateTimeFormat);
                if (value is string offsetText && DateTimeOffset.TryParse(offsetText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedOffset))
                    return parsedOffset.ToString(options.DateTimeFormat);
            }

            return value;
        }

        private static bool IsNumericType(Type? type)
        {
            if (type == null) return false;
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}
