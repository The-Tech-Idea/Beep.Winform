using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Provides smart suggestions for filter values
    /// Analyzes data to suggest recent, frequent, and relevant values
    /// </summary>
    public class FilterSuggestionProvider
    {
        private object _dataSource;
        private Dictionary<string, List<object>> _recentValuesCache = new Dictionary<string, List<object>>();
        private Dictionary<string, List<ValueCount>> _frequentValuesCache = new Dictionary<string, List<ValueCount>>();
        private DateTime _lastCacheUpdate = DateTime.MinValue;
        private const int CacheValidityMinutes = 5;
        
        /// <summary>
        /// Sets the data source for analyzing values
        /// </summary>
        public void SetDataSource(object dataSource)
        {
            if (_dataSource != dataSource)
            {
                _dataSource = dataSource;
                InvalidateCache();
            }
        }
        
        /// <summary>
        /// Gets smart suggestions for a filter value
        /// </summary>
        public List<FilterSuggestion> GetSuggestions(
            string columnName, 
            string partialValue = null, 
            int maxResults = 10)
        {
            var suggestions = new List<FilterSuggestion>();
            
            // 1. Add recent values (from history)
            var recent = GetRecentValues(columnName, Math.Min(3, maxResults / 3));
            suggestions.AddRange(recent.Select(v => new FilterSuggestion
            {
                Value = v?.ToString() ?? "",
                DisplayText = v?.ToString() ?? "(empty)",
                Type = FilterSuggestionType.Recent,
                Icon = "⭐",
                Relevance = 1.0f
            }));
            
            // 2. Add frequent values (from data analysis)
            var frequent = GetFrequentValues(columnName, partialValue, Math.Min(5, maxResults / 2));
            suggestions.AddRange(frequent.Select(vc => new FilterSuggestion
            {
                Value = vc.Value?.ToString() ?? "",
                DisplayText = vc.Value?.ToString() ?? "(empty)",
                MatchCount = vc.Count,
                Type = FilterSuggestionType.Frequent,
                Icon = "📊",
                Relevance = 0.8f
            }));
            
            // 3. Add fuzzy matches (if partial value provided)
            if (!string.IsNullOrEmpty(partialValue))
            {
                var fuzzy = GetFuzzyMatches(columnName, partialValue, Math.Min(5, maxResults / 2));
                suggestions.AddRange(fuzzy.Select(v => new FilterSuggestion
                {
                    Value = v?.ToString() ?? "",
                    DisplayText = v?.ToString() ?? "",
                    Type = FilterSuggestionType.FuzzyMatch,
                    Icon = "🔍",
                    Relevance = CalculateFuzzyRelevance(v?.ToString() ?? "", partialValue)
                }));
            }
            
            // 4. Sort and deduplicate
            return suggestions
                .GroupBy(s => s.Value)
                .Select(g => g.OrderByDescending(s => s.Type == FilterSuggestionType.Recent ? 3 : 
                                                     s.Type == FilterSuggestionType.Frequent ? 2 : 1)
                             .ThenByDescending(s => s.MatchCount)
                             .ThenByDescending(s => s.Relevance)
                             .First())
                .OrderByDescending(s => s.Type == FilterSuggestionType.Recent ? 3 : 
                                       s.Type == FilterSuggestionType.Frequent ? 2 : 1)
                .ThenByDescending(s => s.MatchCount)
                .ThenByDescending(s => s.Relevance)
                .Take(maxResults)
                .ToList();
        }
        
        /// <summary>
        /// Gets recently used values for a column
        /// </summary>
        private List<object> GetRecentValues(string columnName, int count)
        {
            if (_recentValuesCache.TryGetValue(columnName, out var cached))
            {
                return cached.Take(count).ToList();
            }
            
            return new List<object>();
        }
        
        /// <summary>
        /// Gets most frequent values from data source
        /// </summary>
        private List<ValueCount> GetFrequentValues(string columnName, string partialValue, int count)
        {
            // Check cache
            string cacheKey = $"{columnName}_{partialValue}";
            if (_frequentValuesCache.TryGetValue(cacheKey, out var cached) && 
                (DateTime.Now - _lastCacheUpdate).TotalMinutes < CacheValidityMinutes)
            {
                return cached.Take(count).ToList();
            }
            
            // Analyze data source
            var values = ExtractColumnValues(columnName);
            if (values == null || values.Count == 0)
                return new List<ValueCount>();
            
            // Filter by partial value if provided
            if (!string.IsNullOrEmpty(partialValue))
            {
                values = values
                    .Where(v => v?.ToString()?.IndexOf(partialValue, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            
            // Group and count
            var grouped = values
                .Where(v => v != null)
                .GroupBy(v => v)
                .Select(g => new ValueCount 
                { 
                    Value = g.Key, 
                    Count = g.Count() 
                })
                .OrderByDescending(vc => vc.Count)
                .Take(count)
                .ToList();
            
            // Cache results
            _frequentValuesCache[cacheKey] = grouped;
            _lastCacheUpdate = DateTime.Now;
            
            return grouped;
        }
        
        /// <summary>
        /// Gets fuzzy matches for partial value
        /// </summary>
        private List<object> GetFuzzyMatches(string columnName, string partialValue, int count)
        {
            if (string.IsNullOrEmpty(partialValue))
                return new List<object>();
            
            var values = ExtractColumnValues(columnName);
            if (values == null || values.Count == 0)
                return new List<object>();
            
            // Simple fuzzy matching - can be enhanced with Levenshtein distance
            return values
                .Where(v => v != null)
                .Distinct()
                .Where(v => v.ToString().IndexOf(partialValue, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(v => v.ToString().Length) // Prefer shorter matches
                .Take(count)
                .ToList();
        }
        
        /// <summary>
        /// Extracts all values for a column from the data source
        /// </summary>
        private List<object> ExtractColumnValues(string columnName)
        {
            var values = new List<object>();
            
            if (_dataSource == null)
                return values;
            
            try
            {
                // Handle IEnumerable<T>
                if (_dataSource is System.Collections.IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;
                        
                        // Try to get property value
                        var value = GetPropertyValue(item, columnName);
                        if (value != null)
                        {
                            values.Add(value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail - return empty list
            }
            
            return values;
        }
        
        /// <summary>
        /// Gets property value from an object using reflection
        /// </summary>
        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return null;
            
            try
            {
                // Try direct property access
                var type = obj.GetType();
                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    return property.GetValue(obj);
                }
                
                // Try dictionary access (for ExpandoObject, Dictionary, etc.)
                if (obj is System.Collections.IDictionary dict && dict.Contains(propertyName))
                {
                    return dict[propertyName];
                }
            }
            catch
            {
                // Silently fail
            }
            
            return null;
        }
        
        /// <summary>
        /// Calculates fuzzy match relevance score (0-1)
        /// </summary>
        private float CalculateFuzzyRelevance(string value, string query)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(query))
                return 0f;
            
            // Exact match
            if (value.Equals(query, StringComparison.OrdinalIgnoreCase))
                return 1.0f;
            
            // Starts with
            if (value.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                return 0.9f;
            
            // Contains
            if (value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Higher score for earlier position
                int index = value.IndexOf(query, StringComparison.OrdinalIgnoreCase);
                float positionScore = 1.0f - ((float)index / value.Length);
                return 0.5f + (positionScore * 0.3f);
            }
            
            // No match
            return 0f;
        }
        
        /// <summary>
        /// Adds a value to recent values cache
        /// </summary>
        public void AddToRecentValues(string columnName, object value)
        {
            if (value == null) return;
            
            if (!_recentValuesCache.ContainsKey(columnName))
            {
                _recentValuesCache[columnName] = new List<object>();
            }
            
            var recent = _recentValuesCache[columnName];
            
            // Remove if already exists (to move to front)
            recent.Remove(value);
            
            // Add to front
            recent.Insert(0, value);
            
            // Keep only last 20
            if (recent.Count > 20)
            {
                recent.RemoveAt(recent.Count - 1);
            }
        }
        
        /// <summary>
        /// Invalidates all caches
        /// </summary>
        public void InvalidateCache()
        {
            _frequentValuesCache.Clear();
            _lastCacheUpdate = DateTime.MinValue;
        }
        
        /// <summary>
        /// Clears recent values for a column
        /// </summary>
        public void ClearRecentValues(string columnName)
        {
            _recentValuesCache.Remove(columnName);
        }
        
        /// <summary>
        /// Clears all recent values
        /// </summary>
        public void ClearAllRecentValues()
        {
            _recentValuesCache.Clear();
        }
    }
    
    /// <summary>
    /// Represents a filter suggestion
    /// </summary>
    public class FilterSuggestion
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public int MatchCount { get; set; }
        public float Relevance { get; set; }
        public FilterSuggestionType Type { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
    }
    
    /// <summary>
    /// Type of filter suggestion
    /// </summary>
    public enum FilterSuggestionType
    {
        Recent,          // Recently used value
        Frequent,        // Most common in data
        FuzzyMatch,      // Fuzzy text match
        Template,        // Predefined template
        Smart            // AI-suggested
    }
    
    /// <summary>
    /// Value with occurrence count
    /// </summary>
    public class ValueCount
    {
        public object Value { get; set; }
        public int Count { get; set; }
    }
}

