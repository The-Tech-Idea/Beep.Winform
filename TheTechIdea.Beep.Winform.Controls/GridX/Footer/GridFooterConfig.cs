using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX.Grouping;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Footer
{
    /// <summary>
    /// Configuration and cached aggregate values for the pinned footer row.
    /// Aggregates are computed over the currently visible (filtered) rows.
    /// </summary>
    public sealed class GridFooterConfig
    {
        private bool _visible;
        private int _height = 28;
        private readonly Dictionary<string, object?> _cache = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>Raised when visibility, height, or an aggregate changes.</summary>
        public event EventHandler? Changed;

        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                int next = Math.Max(18, value);
                if (_height == next) return;
                _height = next;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly Dictionary<string, AggregationType> _aggregates =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Read-only view of the configured per-column aggregates. Useful for
        /// designer smart-tags and host-side layout editors.
        /// </summary>
        public IReadOnlyDictionary<string, AggregationType> ColumnAggregates => _aggregates;

        public void SetAggregate(string columnName, AggregationType aggregation)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return;

            if (aggregation == AggregationType.None)
            {
                _aggregates.Remove(columnName);
            }
            else
            {
                _aggregates[columnName] = aggregation;
            }

            Invalidate();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public AggregationType GetAggregate(string columnName)
            => _aggregates.TryGetValue(columnName, out var type) ? type : AggregationType.None;

        public object? GetValue(BeepGridPro grid, string columnName)
        {
            if (grid == null || string.IsNullOrWhiteSpace(columnName)) return null;
            if (!_aggregates.TryGetValue(columnName, out var type) || type == AggregationType.None)
                return null;
            if (_cache.TryGetValue(columnName, out var cached)) return cached;

            var values = new List<object?>();
            foreach (var row in grid.Data.Rows)
            {
                if (!row.IsVisible) continue;
                var cell = row.Cells.FirstOrDefault(c =>
                    string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
                values.Add(cell?.CellValue);
            }

            object? result = GridGroupAggregateEngine.ComputeAggregate(values, type);
            _cache[columnName] = result;
            return result;
        }

        /// <summary>Drops cached aggregate values after a data, filter, or sort change.</summary>
        public void Invalidate() => _cache.Clear();
    }
}
