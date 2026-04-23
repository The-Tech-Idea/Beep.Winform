using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Virtualization
{
    /// <summary>
    /// Adapter that makes common in-memory .NET collections virtualizable
    /// by wrapping them as an <see cref="IVirtualDataSource"/>.
    /// </summary>
    public sealed class GridVirtualDataSource : IVirtualDataSource
    {
        private readonly Func<long, int, IEnumerable<VirtualRowData>> _getRows;
        private readonly Action<long, int, int> _preload;
        private long _totalRowCount;

        public long TotalRowCount => _totalRowCount;

        private GridVirtualDataSource(long count, Func<long, int, IEnumerable<VirtualRowData>> getRows, Action<long, int, int> preload)
        {
            _totalRowCount = count;
            _getRows = getRows;
            _preload = preload;
        }

        public IEnumerable<VirtualRowData> GetRows(long startIndex, int count)
            => _getRows(startIndex, count);

        public void PreloadWindow(long startIndex, int visibleCount, int overscan)
            => _preload(startIndex, visibleCount, overscan);

        public event EventHandler? TotalRowCountChanged;

        /// <summary>
        /// Update the total row count and raise <see cref="TotalRowCountChanged"/>.
        /// </summary>
        public void SetTotalRowCount(long count)
        {
            if (_totalRowCount != count)
            {
                _totalRowCount = count;
                TotalRowCountChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Create a virtual data source from any <see cref="IList"/>.
        /// </summary>
        public static GridVirtualDataSource FromList(IList list, IEnumerable<string> columnNames)
        {
            var names = columnNames.ToArray();
            return new GridVirtualDataSource(
                list.Count,
                (start, count) => GetRowsFromList(list, names, start, count),
                (s, v, o) => { }
            );
        }

        /// <summary>
        /// Create a virtual data source from a <see cref="DataTable"/>.
        /// </summary>
        public static GridVirtualDataSource FromDataTable(DataTable table, IEnumerable<string> columnNames)
        {
            var names = columnNames.ToArray();
            return new GridVirtualDataSource(
                table.Rows.Count,
                (start, count) => GetRowsFromDataTable(table, names, start, count),
                (s, v, o) => { }
            );
        }

        /// <summary>
        /// Create a virtual data source from a <see cref="System.Data.DataView"/>.
        /// </summary>
        public static GridVirtualDataSource FromDataView(System.Data.DataView view, IEnumerable<string> columnNames)
        {
            var names = columnNames.ToArray();
            var table = view.Table;
            return new GridVirtualDataSource(
                view.Count,
                (start, count) => GetRowsFromDataView(view, table, names, start, count),
                (s, v, o) => { }
            );
        }

        private static IEnumerable<VirtualRowData> GetRowsFromList(IList list, string[] names, long start, int count)
        {
            int end = Math.Min((int)start + count, list.Count);
            for (int i = (int)start; i < end; i++)
            {
                var item = list[i];
                var values = new Dictionary<string, object?>();
                foreach (var name in names)
                {
                    var prop = item?.GetType().GetProperty(name);
                    values[name] = prop?.GetValue(item);
                }
                yield return new VirtualRowData { Index = i, Values = values, OriginalItem = item };
            }
        }

        private static IEnumerable<VirtualRowData> GetRowsFromDataTable(DataTable table, string[] names, long start, int count)
        {
            int end = Math.Min((int)start + count, table.Rows.Count);
            for (int i = (int)start; i < end; i++)
            {
                var row = table.Rows[i];
                var values = new Dictionary<string, object?>();
                foreach (var name in names)
                {
                    if (table.Columns.Contains(name))
                        values[name] = row[name];
                }
                yield return new VirtualRowData { Index = i, Values = values, OriginalItem = row };
            }
        }

        private static IEnumerable<VirtualRowData> GetRowsFromDataView(System.Data.DataView view, System.Data.DataTable? table, string[] names, long start, int count)
        {
            int end = Math.Min((int)start + count, view.Count);
            for (int i = (int)start; i < end; i++)
            {
                var drv = view[i];
                var values = new Dictionary<string, object?>();
                foreach (var name in names)
                {
                    if (table?.Columns.Contains(name) == true)
                        values[name] = drv[name];
                }
                yield return new VirtualRowData { Index = i, Values = values, OriginalItem = drv };
            }
        }
    }
}
