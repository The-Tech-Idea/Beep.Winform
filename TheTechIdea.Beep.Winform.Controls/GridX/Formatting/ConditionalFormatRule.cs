using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX.Query;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Formatting
{
    public enum GridFormatTarget
    {
        Cell,
        Row
    }

    /// <summary>Visual overrides applied by a conditional formatting rule.</summary>
    public sealed class GridCellStyle
    {
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public FontStyle? FontStyle { get; set; }

        public bool IsEmpty => BackColor == null && ForeColor == null && FontStyle == null;

        public GridCellStyle Clone() => new()
        {
            BackColor = BackColor,
            ForeColor = ForeColor,
            FontStyle = FontStyle
        };
    }

    /// <summary>
    /// A rule evaluated against the current view. <see cref="ColumnName"/> is
    /// the column whose value is tested; for <see cref="GridFormatTarget.Row"/>
    /// the resulting style is applied to every cell in the row.
    /// </summary>
    public sealed class ConditionalFormatRule
    {
        public string ColumnName { get; set; } = string.Empty;
        public GridFormatTarget Target { get; set; } = GridFormatTarget.Cell;
        public FilterOperator Operator { get; set; } = FilterOperator.Equals;
        public string? Value { get; set; }
        public string? Value2 { get; set; }
        public GridCellStyle Style { get; set; } = new();
        public int Priority { get; set; }
        public bool StopIfTrue { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Evaluates conditional rules for the current frame and caches the result
    /// by row index + column name. Call <see cref="BeginFrame"/> at the start
    /// of each paint so the cache never survives a data or scroll change.
    /// </summary>
    public sealed class GridConditionalFormatEngine
    {
        private readonly BeepGridPro _grid;
        private readonly List<ConditionalFormatRule> _rules = new();
        private readonly Dictionary<(int Row, string Column), GridCellStyle?> _cache = new();
        private readonly Dictionary<int, GridViewRowAdapter> _rowCache = new();

        public GridConditionalFormatEngine(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public IReadOnlyList<ConditionalFormatRule> Rules => _rules;

        public void Add(ConditionalFormatRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
            Invalidate();
        }

        public bool Remove(ConditionalFormatRule rule)
        {
            bool removed = _rules.Remove(rule);
            if (removed) Invalidate();
            return removed;
        }

        public void Clear()
        {
            _rules.Clear();
            Invalidate();
        }

        public void Invalidate()
        {
            _cache.Clear();
            _rowCache.Clear();
        }

        public void BeginFrame()
        {
            _cache.Clear();
            _rowCache.Clear();
        }

        public GridCellStyle? Resolve(BeepRowConfig row, int rowIndex, BeepColumnConfig column)
        {
            if (row == null || column == null || _rules.Count == 0) return null;

            var key = (rowIndex, column.ColumnName);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            GridCellStyle? result = null;
            foreach (var rule in _rules
                .Where(r => r != null)
                .OrderByDescending(r => r.Priority))
            {
                if (!rule.IsEnabled || rule.Style == null || rule.Style.IsEmpty) continue;
                if (rule.Target == GridFormatTarget.Cell &&
                    !string.IsNullOrWhiteSpace(rule.ColumnName) &&
                    !string.Equals(rule.ColumnName, column.ColumnName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!Matches(row, rowIndex, rule)) continue;

                result = rule.Style;
                if (rule.StopIfTrue) break;
            }

            _cache[key] = result;
            return result;
        }

        private bool Matches(BeepRowConfig row, int rowIndex, ConditionalFormatRule rule)
        {
            if (string.IsNullOrWhiteSpace(rule.ColumnName)) return true;

            if (!_rowCache.TryGetValue(rowIndex, out var adapter) || !ReferenceEquals(adapter.Row, row))
            {
                adapter = new GridViewRowAdapter(row);
                _rowCache[rowIndex] = adapter;
            }

            var condition = new GridFilterCondition
            {
                ColumnName = rule.ColumnName,
                Operator = rule.Operator,
                Value = rule.Value,
                Value2 = rule.Value2
            };

            return GridQueryEngine.Matches(adapter.View, condition);
        }

        private sealed class GridViewRowAdapter
        {
            public GridViewRowAdapter(BeepRowConfig row)
            {
                Row = row;
                var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in row.Cells)
                    values[cell.ColumnName] = cell.CellValue;
                View = new GridViewRow { Values = values };
            }

            public BeepRowConfig Row { get; }
            public GridViewRow View { get; }
        }
    }
}
