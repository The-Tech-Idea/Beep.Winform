using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Generic filter engine for applying filter configurations to any data collection
    /// Supports all filter operators and multi-column filtering
    /// </summary>
    /// <typeparam name="T">Type of data items to filter</typeparam>
    public class FilterEngine<T>
    {
        private readonly IEnumerable<T> _dataSource;
        private readonly Dictionary<string, PropertyInfo> _propertyCache = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// Creates a filter engine for the specified data source
        /// </summary>
        /// <param name="dataSource">Data source to filter</param>
        public FilterEngine(IEnumerable<T> dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            CacheProperties();
        }

        /// <summary>
        /// Caches property info for faster access
        /// </summary>
        private void CacheProperties()
        {
            _propertyCache.Clear();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                _propertyCache[prop.Name] = prop;
            }
        }

        /// <summary>
        /// Applies a filter configuration to the data
        /// </summary>
        /// <param name="config">Filter configuration to apply</param>
        /// <returns>Filtered collection of items</returns>
        public IEnumerable<T> ApplyFilter(FilterConfiguration config)
        {
            if (config == null || config.Criteria.Count == 0)
            {
                return _dataSource;
            }

            var enabledCriteria = config.Criteria.Where(c => c.IsEnabled).ToList();

            if (enabledCriteria.Count == 0)
            {
                return _dataSource;
            }

            return _dataSource.Where(item =>
            {
                bool matches = config.Logic == FilterLogic.And
                    ? MatchesAllCriteria(item, enabledCriteria)
                    : MatchesAnyCriteria(item, enabledCriteria);
                return matches;
            });
        }

        /// <summary>
        /// Quick filter: searches for text across all or specific property(ies)
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="propertyName">Property to search, or null for all properties</param>
        /// <returns>Filtered collection of items</returns>
        public IEnumerable<T> ApplyQuickFilter(string searchText, string? propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return _dataSource;
            }

            var searchLower = searchText.ToLowerInvariant();

            return _dataSource.Where(item =>
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    // Search all properties
                    foreach (var prop in _propertyCache.Values)
                    {
                        var value = GetPropertyValue(item, prop.Name);
                        if (value != null && value.ToString()!.ToLowerInvariant().Contains(searchLower))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    // Search specific property
                    var value = GetPropertyValue(item, propertyName);
                    return value != null && value.ToString()!.ToLowerInvariant().Contains(searchLower);
                }
            });
        }

        /// <summary>
        /// Applies filter and returns indices of matching items
        /// </summary>
        /// <param name="config">Filter configuration to apply</param>
        /// <returns>List of indices that match the filter</returns>
        public List<int> ApplyFilterGetIndices(FilterConfiguration config)
        {
            if (config == null || config.Criteria.Count == 0)
            {
                return Enumerable.Range(0, _dataSource.Count()).ToList();
            }

            var enabledCriteria = config.Criteria.Where(c => c.IsEnabled).ToList();

            if (enabledCriteria.Count == 0)
            {
                return Enumerable.Range(0, _dataSource.Count()).ToList();
            }

            var matchingIndices = new List<int>();
            int index = 0;

            foreach (var item in _dataSource)
            {
                bool matches = config.Logic == FilterLogic.And
                    ? MatchesAllCriteria(item, enabledCriteria)
                    : MatchesAnyCriteria(item, enabledCriteria);

                if (matches)
                {
                    matchingIndices.Add(index);
                }
                index++;
            }

            return matchingIndices;
        }

        private bool MatchesAllCriteria(T item, List<FilterCriteria> criteria)
        {
            return criteria.All(c => MatchesCriterion(item, c));
        }

        private bool MatchesAnyCriteria(T item, List<FilterCriteria> criteria)
        {
            return criteria.Any(c => MatchesCriterion(item, c));
        }

        private bool MatchesCriterion(T item, FilterCriteria criterion)
        {
            var propertyValue = GetPropertyValue(item, criterion.ColumnName);

            return criterion.Operator switch
            {
                FilterOperator.Equals => CompareEquals(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.NotEquals => !CompareEquals(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.Contains => CompareContains(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.NotContains => !CompareContains(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.StartsWith => CompareStartsWith(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.EndsWith => CompareEndsWith(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.GreaterThan => CompareGreaterThan(propertyValue, criterion.Value),
                FilterOperator.GreaterThanOrEqual => CompareGreaterThanOrEqual(propertyValue, criterion.Value),
                FilterOperator.LessThan => CompareLessThan(propertyValue, criterion.Value),
                FilterOperator.LessThanOrEqual => CompareLessThanOrEqual(propertyValue, criterion.Value),
                FilterOperator.Between => CompareBetween(propertyValue, criterion.Value, criterion.Value2),
                FilterOperator.NotBetween => !CompareBetween(propertyValue, criterion.Value, criterion.Value2),
                FilterOperator.IsNull => IsNullOrEmpty(propertyValue),
                FilterOperator.IsNotNull => !IsNullOrEmpty(propertyValue),
                FilterOperator.Regex => MatchesRegex(propertyValue, criterion.Value),
                FilterOperator.In => CompareIn(propertyValue, criterion.Value, criterion.CaseSensitive),
                FilterOperator.NotIn => !CompareIn(propertyValue, criterion.Value, criterion.CaseSensitive),
                _ => false
            };
        }

        /// <summary>
        /// Gets property value from an item using reflection
        /// </summary>
        private object? GetPropertyValue(T item, string propertyName)
        {
            if (item == null || string.IsNullOrEmpty(propertyName))
                return null;

            if (_propertyCache.TryGetValue(propertyName, out PropertyInfo? prop))
            {
                try
                {
                    return prop.GetValue(item);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        #region Comparison Methods

        private bool CompareEquals(object? cellValue, object? filterValue, bool caseSensitive)
        {
            if (cellValue == null && filterValue == null) return true;
            if (cellValue == null || filterValue == null) return false;

            if (cellValue is string str1 && filterValue is string str2)
            {
                var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return string.Equals(str1, str2, comparison);
            }

            return cellValue.ToString() == filterValue.ToString();
        }

        private bool CompareContains(object? cellValue, object? filterValue, bool caseSensitive)
        {
            if (cellValue == null || filterValue == null) return false;

            var str1 = cellValue.ToString()!;
            var str2 = filterValue.ToString()!;

            if (!caseSensitive)
            {
                str1 = str1.ToLowerInvariant();
                str2 = str2.ToLowerInvariant();
            }

            return str1.Contains(str2);
        }

        private bool CompareStartsWith(object? cellValue, object? filterValue, bool caseSensitive)
        {
            if (cellValue == null || filterValue == null) return false;

            var str1 = cellValue.ToString()!;
            var str2 = filterValue.ToString()!;

            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return str1.StartsWith(str2, comparison);
        }

        private bool CompareEndsWith(object? cellValue, object? filterValue, bool caseSensitive)
        {
            if (cellValue == null || filterValue == null) return false;

            var str1 = cellValue.ToString()!;
            var str2 = filterValue.ToString()!;

            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return str1.EndsWith(str2, comparison);
        }

        private bool CompareGreaterThan(object? cellValue, object? filterValue)
        {
            try
            {
                if (cellValue is IComparable comparable && TryConvert(filterValue, cellValue.GetType(), out object? converted))
                {
                    return comparable.CompareTo(converted) > 0;
                }
            }
            catch { }
            return false;
        }

        private bool CompareGreaterThanOrEqual(object? cellValue, object? filterValue)
        {
            try
            {
                if (cellValue is IComparable comparable && TryConvert(filterValue, cellValue.GetType(), out object? converted))
                {
                    return comparable.CompareTo(converted) >= 0;
                }
            }
            catch { }
            return false;
        }

        private bool CompareLessThan(object? cellValue, object? filterValue)
        {
            try
            {
                if (cellValue is IComparable comparable && TryConvert(filterValue, cellValue.GetType(), out object? converted))
                {
                    return comparable.CompareTo(converted) < 0;
                }
            }
            catch { }
            return false;
        }

        private bool CompareLessThanOrEqual(object? cellValue, object? filterValue)
        {
            try
            {
                if (cellValue is IComparable comparable && TryConvert(filterValue, cellValue.GetType(), out object? converted))
                {
                    return comparable.CompareTo(converted) <= 0;
                }
            }
            catch { }
            return false;
        }

        private bool CompareBetween(object? cellValue, object? filterValue1, object? filterValue2)
        {
            try
            {
                if (cellValue is IComparable comparable &&
                    TryConvert(filterValue1, cellValue.GetType(), out object? converted1) &&
                    TryConvert(filterValue2, cellValue.GetType(), out object? converted2))
                {
                    return comparable.CompareTo(converted1) >= 0 && comparable.CompareTo(converted2) <= 0;
                }
            }
            catch { }
            return false;
        }

        private bool IsNullOrEmpty(object? cellValue)
        {
            if (cellValue == null) return true;
            if (cellValue is string str) return string.IsNullOrWhiteSpace(str);
            return false;
        }

        private bool MatchesRegex(object? cellValue, object? pattern)
        {
            if (cellValue == null || pattern == null) return false;

            try
            {
                var regex = new Regex(pattern.ToString()!, RegexOptions.IgnoreCase);
                return regex.IsMatch(cellValue.ToString()!);
            }
            catch
            {
                return false;
            }
        }

        private bool CompareIn(object? cellValue, object? filterValue, bool caseSensitive)
        {
            if (cellValue == null || filterValue == null) return false;

            // Parse comma-separated list
            var values = filterValue.ToString()!.Split(',').Select(v => v.Trim());
            var cellStr = cellValue.ToString()!;

            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return values.Any(v => string.Equals(cellStr, v, comparison));
        }

        private bool TryConvert(object? value, Type targetType, out object? result)
        {
            result = null;
            if (value == null) return false;

            try
            {
                // Handle nullable types
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType)!;
                }

                // Handle DateTime
                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime dt))
                    {
                        result = dt;
                        return true;
                    }
                    return false;
                }

                // Handle numeric types
                if (targetType.IsPrimitive || targetType == typeof(decimal))
                {
                    result = Convert.ChangeType(value, targetType);
                    return true;
                }

                result = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
