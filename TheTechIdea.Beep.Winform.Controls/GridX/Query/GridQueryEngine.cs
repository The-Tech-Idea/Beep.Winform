using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Query
{
    /// <summary>
    /// UI-free query pipeline: filter, group, sort, page. The same engine is
    /// used by in-memory grids, the virtual source, and export snapshots.
    /// </summary>
    public static class GridQueryEngine
    {
        public static GridQueryResult Execute(IEnumerable<GridViewRow> rows, GridQuery query)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            query ??= new GridQuery();

            var filtered = rows.Where(row => MatchesAllGroups(row, query.Filters)).ToList();
            int total = filtered.Count;

            List<GridViewRow> ordered;
            if (query.Groups.Count > 0)
            {
                var withKeys = filtered
                    .Select(row =>
                    {
                        var (rawKeys, path) = BuildGroupPath(row, query);
                        return new GroupedRow(row, rawKeys, path);
                    })
                    .ToList();

                IOrderedEnumerable<GroupedRow>? grouped = null;
                for (int i = 0; i < query.Groups.Count; i++)
                {
                    int keyIndex = i;
                    var group = query.Groups[i];
                    bool descending = group.SortDirection == GroupSortDirection.Descending;

                    grouped = grouped == null
                        ? (descending
                            ? withKeys.OrderByDescending(x => x.Keys[keyIndex], SafeValueComparer.Instance)
                            : withKeys.OrderBy(x => x.Keys[keyIndex], SafeValueComparer.Instance))
                        : (descending
                            ? grouped.ThenByDescending(x => x.Keys[keyIndex], SafeValueComparer.Instance)
                            : grouped.ThenBy(x => x.Keys[keyIndex], SafeValueComparer.Instance));
                }

                IOrderedEnumerable<GroupedRow> groupedRows = grouped!;
                groupedRows = ApplySorts(groupedRows, query.Sorts);

                ordered = groupedRows
                    .Select(x => x.Row.WithGroupPath(x.Path))
                    .ToList();
            }
            else
            {
                ordered = ApplySorts(filtered, query.Sorts).ToList();
            }

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].DisplayIndex = i;

            IEnumerable<GridViewRow> page = ordered;
            if (query.PageSize > 0)
                page = ordered.Skip(Math.Max(0, query.Offset)).Take(query.PageSize);

            return new GridQueryResult
            {
                Rows = page.ToList(),
                TotalCount = total,
                Revision = query.Revision
            };
        }

        public static bool MatchesAllGroups(GridViewRow row, IEnumerable<GridFilterGroup> groups)
        {
            foreach (var group in groups)
            {
                if (group == null || group.Conditions.Count == 0) continue;

                bool groupMatched;
                if (group.Logic == FilterLogic.Or)
                    groupMatched = group.Conditions.Any(c => Matches(row, c));
                else
                    groupMatched = group.Conditions.All(c => Matches(row, c));

                if (!groupMatched) return false;
            }
            return true;
        }

        public static bool Matches(GridViewRow row, GridFilterCondition condition)
        {
            if (condition == null || string.IsNullOrWhiteSpace(condition.ColumnName))
                return true;

            row.Values.TryGetValue(condition.ColumnName, out var raw);
            string text = Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;
            string expected = condition.Value ?? string.Empty;
            string expected2 = condition.Value2 ?? string.Empty;

            switch (condition.Operator)
            {
                case FilterOperator.IsNull:
                    return raw == null || string.IsNullOrWhiteSpace(text);
                case FilterOperator.IsNotNull:
                    return raw != null && !string.IsNullOrWhiteSpace(text);
                case FilterOperator.Equals:
                    return Compare(raw, expected) == 0;
                case FilterOperator.NotEquals:
                    return Compare(raw, expected) != 0;
                case FilterOperator.Contains:
                    return text.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
                case FilterOperator.NotContains:
                    return text.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0;
                case FilterOperator.StartsWith:
                    return text.StartsWith(expected, StringComparison.OrdinalIgnoreCase);
                case FilterOperator.EndsWith:
                    return text.EndsWith(expected, StringComparison.OrdinalIgnoreCase);
                case FilterOperator.GreaterThan:
                    return Compare(raw, expected) > 0;
                case FilterOperator.GreaterThanOrEqual:
                    return Compare(raw, expected) >= 0;
                case FilterOperator.LessThan:
                    return Compare(raw, expected) < 0;
                case FilterOperator.LessThanOrEqual:
                    return Compare(raw, expected) <= 0;
                case FilterOperator.Between:
                    return Compare(raw, expected) >= 0 && Compare(raw, expected2) <= 0;
                case FilterOperator.NotBetween:
                    return Compare(raw, expected) < 0 || Compare(raw, expected2) > 0;
                case FilterOperator.Regex:
                    if (string.IsNullOrEmpty(expected)) return true;
                    try { return Regex.IsMatch(text, expected, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant); }
                    catch (ArgumentException ex)
                    {
                        GridErrorReporter.FallbackOnce("GridQueryEngine.Regex", null, "evaluate regex filter", ex);
                        return false;
                    }
                case FilterOperator.In:
                    return SplitValues(expected).Any(v => Compare(raw, v) == 0);
                case FilterOperator.NotIn:
                    return !SplitValues(expected).Any(v => Compare(raw, v) == 0);
                default:
                    return true;
            }
        }

        private static IOrderedEnumerable<GridViewRow> ApplySorts(
            IEnumerable<GridViewRow> rows,
            IReadOnlyList<GridSortDescriptor> sorts)
        {
            IOrderedEnumerable<GridViewRow>? ordered = null;
            foreach (var sort in sorts)
            {
                if (sort == null || string.IsNullOrWhiteSpace(sort.ColumnName)) continue;
                Func<GridViewRow, object?> key = row =>
                {
                    row.Values.TryGetValue(sort.ColumnName, out var value);
                    return value;
                };

                ordered = ordered == null
                    ? (sort.Direction == SortDirection.Descending
                        ? rows.OrderByDescending(key, SafeValueComparer.Instance)
                        : rows.OrderBy(key, SafeValueComparer.Instance))
                    : (sort.Direction == SortDirection.Descending
                        ? ordered.ThenByDescending(key, SafeValueComparer.Instance)
                        : ordered.ThenBy(key, SafeValueComparer.Instance));
            }

            return ordered ?? rows.OrderBy(_ => 0, SafeValueComparer.Instance);
        }

        private static IOrderedEnumerable<GroupedRow> ApplySorts(
            IOrderedEnumerable<GroupedRow> rows,
            IReadOnlyList<GridSortDescriptor> sorts)
        {
            IOrderedEnumerable<GroupedRow> ordered = rows;
            foreach (var sort in sorts)
            {
                if (sort == null || string.IsNullOrWhiteSpace(sort.ColumnName)) continue;
                Func<GroupedRow, object?> key = x =>
                {
                    x.Row.Values.TryGetValue(sort.ColumnName, out var value);
                    return value;
                };

                ordered = sort.Direction == SortDirection.Descending
                    ? ordered.ThenByDescending(key, SafeValueComparer.Instance)
                    : ordered.ThenBy(key, SafeValueComparer.Instance);
            }
            return ordered;
        }

        private static (List<object?> Raw, List<string> Path) BuildGroupPath(GridViewRow row, GridQuery query)
        {
            var raw = new List<object?>(query.Groups.Count);
            var path = new List<string>(query.Groups.Count);
            foreach (var group in query.Groups)
            {
                row.Values.TryGetValue(group.ColumnName, out var value);
                raw.Add(value);
                path.Add(group.ValueFormat != null && value is IFormattable formattable
                    ? formattable.ToString(group.ValueFormat, CultureInfo.CurrentCulture)
                    : Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty);
            }
            return (raw, path);
        }

        private static IEnumerable<string> SplitValues(string value)
            => value.Split(new[] { ',', ';', '|', '\u001F' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static int Compare(object? value, string expected)
        {
            if (value == null)
                return string.IsNullOrEmpty(expected) ? 0 : -1;

            if (TryConvert(value, out decimal leftNumber) && TryConvert(expected, out decimal rightNumber))
                return leftNumber.CompareTo(rightNumber);

            if (value is DateTime leftDate && TryParseDate(expected, out var rightDate))
                return leftDate.CompareTo(rightDate);

            string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            return string.Compare(text, expected, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryConvert(object value, out decimal result)
        {
            try
            {
                result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                GridErrorReporter.FallbackOnce("GridQueryEngine.ToDecimal", null, "convert filter value", ex);
                result = 0;
                return false;
            }
        }

        private static bool TryConvert(string value, out decimal result)
            => decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

        private static bool TryParseDate(string value, out DateTime result)
            => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

        private sealed record GroupedRow(GridViewRow Row, List<object?> Keys, List<string> Path);

        private sealed class SafeValueComparer : IComparer<object?>
        {
            public static readonly SafeValueComparer Instance = new();

            public int Compare(object? x, object? y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                if (x is string xs && y is string ys)
                    return string.Compare(xs, ys, StringComparison.OrdinalIgnoreCase);

                if (TryConvert(x, out var xn) && TryConvert(y, out var yn))
                    return xn.CompareTo(yn);

                if (x is IComparable comparable)
                {
                    try
                    {
                        if (x.GetType().IsInstanceOfType(y))
                            return comparable.CompareTo(y);
                    }
                    catch (ArgumentException ex)
                    {
                        GridErrorReporter.FallbackOnce("GridQueryEngine.Comparer", null, "compare sort values", ex);
                    }
                }

                return string.Compare(
                    Convert.ToString(x, CultureInfo.InvariantCulture),
                    Convert.ToString(y, CultureInfo.InvariantCulture),
                    StringComparison.OrdinalIgnoreCase);
            }

            private static bool TryConvert(object value, out decimal result)
            {
                try
                {
                    result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return true;
                }
                catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
                {
                    GridErrorReporter.FallbackOnce("GridQueryEngine.ToDecimal", null, "convert sort value", ex);
                    result = 0;
                    return false;
                }
            }
        }
    }
}
