using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Grouping
{
    /// <summary>
    /// Represents a summary row displayed after a group, showing aggregate values
    /// for columns that have <see cref="BeepColumnConfig.AggregationType"/> set.
    /// </summary>
    public sealed class GridGroupSummaryRow
    {
        /// <summary>
        /// The group this summary belongs to.
        /// </summary>
        public GridGroup Group { get; set; } = null!;

        /// <summary>
        /// Aggregate values keyed by column name.
        /// </summary>
        public Dictionary<string, object?> Values { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Height of the summary row in pixels.
        /// </summary>
        public int Height { get; set; } = 22;

        /// <summary>
        /// Whether the summary row is visible (only visible when the group is expanded).
        /// </summary>
        public bool IsVisible => Group?.IsCollapsed == false;
    }

    /// <summary>
    /// Computes aggregate values for group summary rows.
    /// </summary>
    public static class GridGroupAggregateEngine
    {
        /// <summary>
        /// Computes aggregates for all columns that have an aggregation type set,
        /// and attaches a <see cref="GridGroupSummaryRow"/> to the group.
        /// </summary>
        public static void ComputeForGroup(BeepGridPro grid, GridGroup group)
        {
            var summary = new GridGroupSummaryRow { Group = group };
            summary.Height = Math.Max(22, grid.RowHeight);

            var columnsWithAggregate = grid.Data.Columns
                .Where(c => c.AggregationType != AggregationType.None && !c.IsRowNumColumn && !c.IsSelectionCheckBox)
                .ToList();

            foreach (var col in columnsWithAggregate)
            {
                var values = group.RowIndices
                    .Where(ri => ri >= 0 && ri < grid.Data.Rows.Count)
                    .Select(ri => GetCellValue(grid, ri, col.ColumnName))
                    .ToList();

                if (values.Count == 0)
                {
                    summary.Values[col.ColumnName] = null;
                    continue;
                }

                summary.Values[col.ColumnName] = ComputeAggregate(values, col.AggregationType);
            }

            group.SummaryRow = summary;
        }

        private static object? GetCellValue(BeepGridPro grid, int rowIndex, string columnName)
        {
            var row = grid.Data.Rows[rowIndex];
            var cell = row.Cells.FirstOrDefault(c =>
                string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
            return cell?.CellValue;
        }

        private static object? ComputeAggregate(List<object?> values, AggregationType type)
        {
            var nonNullNumeric = values
                .Where(v => v != null && v != DBNull.Value)
                .Select(v => Convert.ToDecimal(v, System.Globalization.CultureInfo.InvariantCulture))
                .ToList();

            var nonNullAll = values.Where(v => v != null && v != DBNull.Value).ToList();

            switch (type)
            {
                case AggregationType.Sum:
                    return nonNullNumeric.Count > 0 ? nonNullNumeric.Sum() : (decimal?)null;

                case AggregationType.Average:
                    return nonNullNumeric.Count > 0 ? nonNullNumeric.Average() : (decimal?)null;

                case AggregationType.Count:
                    return values.Count;

                case AggregationType.Min:
                    return nonNullNumeric.Count > 0 ? nonNullNumeric.Min() : (decimal?)null;

                case AggregationType.Max:
                    return nonNullNumeric.Count > 0 ? nonNullNumeric.Max() : (decimal?)null;

                case AggregationType.First:
                    return nonNullAll.FirstOrDefault();

                case AggregationType.Last:
                    return nonNullAll.LastOrDefault();

                case AggregationType.DistinctCount:
                    return nonNullAll.Distinct().Count();

                case AggregationType.Custom:
                default:
                    return null;
            }
        }
    }
}
