using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.DataBlocks.Helpers
{
    /// <summary>
    /// Helper class for DataBlock query operations
    /// Provides Oracle Forms-like query building and execution
    /// </summary>
    public static class DataBlockQueryHelper
    {
        /// <summary>
        /// Build query filters from field values
        /// Oracle Forms equivalent: Query-by-Example (QBE)
        /// </summary>
        public static List<AppFilter> BuildQueryFilters(
            Dictionary<string, object> fieldValues,
            Dictionary<string, TheTechIdea.Beep.Winform.Controls.QueryOperator> operators = null)
        {
            var filters = new List<AppFilter>();

            if (fieldValues == null || fieldValues.Count == 0)
                return filters;

            foreach (var kvp in fieldValues)
            {
                if (kvp.Value != null && !string.IsNullOrEmpty(kvp.Value.ToString()))
                {
                    var operatorStr = GetOperatorString(
                        operators?.ContainsKey(kvp.Key) == true 
                            ? operators[kvp.Key] 
                            : TheTechIdea.Beep.Winform.Controls.QueryOperator.Equals);

                    filters.Add(new AppFilter
                    {
                        FieldName = kvp.Key,
                        Operator = operatorStr,
                        FilterValue = kvp.Value.ToString()
                    });
                }
            }

            return filters;
        }

        /// <summary>
        /// Convert QueryOperator enum to string
        /// </summary>
        private static string GetOperatorString(TheTechIdea.Beep.Winform.Controls.QueryOperator op)
        {
            return op switch
            {
                TheTechIdea.Beep.Winform.Controls.QueryOperator.Equals => "=",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.NotEquals => "!=",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.GreaterThan => ">",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.GreaterThanOrEqual => ">=",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.LessThan => "<",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.LessThanOrEqual => "<=",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.Like => "LIKE",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.NotLike => "NOT LIKE",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.In => "IN",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.NotIn => "NOT IN",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.IsNull => "IS NULL",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.IsNotNull => "IS NOT NULL",
                TheTechIdea.Beep.Winform.Controls.QueryOperator.Between => "BETWEEN",
                _ => "="
            };
        }

        /// <summary>
        /// Combine multiple filter lists with AND logic
        /// </summary>
        public static List<AppFilter> CombineFiltersAnd(
            params List<AppFilter>[] filterLists)
        {
            var combined = new List<AppFilter>();

            foreach (var filterList in filterLists)
            {
                if (filterList != null)
                {
                    combined.AddRange(filterList);
                }
            }

            return combined;
        }

        /// <summary>
        /// Build query with WHERE clause
        /// Oracle Forms equivalent: SET_BLOCK_PROPERTY(WHERE_CLAUSE)
        /// </summary>
        public static List<AppFilter> BuildWhereClauseFilters(string whereClause)
        {
            // This is a simplified implementation
            // In a real scenario, you would parse the WHERE clause into AppFilter objects
            // For now, return empty list - actual parsing would require SQL parser
            return new List<AppFilter>();
        }

        /// <summary>
        /// Build query with ORDER BY clause
        /// Oracle Forms equivalent: SET_BLOCK_PROPERTY(ORDER_BY_CLAUSE)
        /// </summary>
        public static string ParseOrderByClause(string orderByClause)
        {
            // Validate and return order by clause
            if (string.IsNullOrWhiteSpace(orderByClause))
                return null;

            // Basic validation - ensure it's a valid ORDER BY clause
            var trimmed = orderByClause.Trim();
            if (trimmed.StartsWith("ORDER BY", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring(8).Trim();
            }

            return trimmed;
        }

        /// <summary>
        /// Clear all query filters (reset query)
        /// Oracle Forms equivalent: CLEAR_BLOCK(ALL_RECORDS)
        /// </summary>
        public static void ClearQueryFilters(Dictionary<string, object> fieldValues)
        {
            if (fieldValues != null)
            {
                var keys = fieldValues.Keys.ToList();
                foreach (var key in keys)
                {
                    fieldValues[key] = null;
                }
            }
        }

        /// <summary>
        /// Validate query filters before execution
        /// </summary>
        public static bool ValidateQueryFilters(List<AppFilter> filters, out string errorMessage)
        {
            errorMessage = null;

            if (filters == null || filters.Count == 0)
            {
                errorMessage = "No query filters specified";
                return false;
            }

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.FieldName))
                {
                    errorMessage = "Query filter has empty field name";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(filter.Operator))
                {
                    errorMessage = $"Query filter for field '{filter.FieldName}' has empty operator";
                    return false;
                }

                // Some operators don't require values (IS NULL, IS NOT NULL)
                if (filter.Operator != "IS NULL" && 
                    filter.Operator != "IS NOT NULL" &&
                    string.IsNullOrWhiteSpace(filter.FilterValue))
                {
                    errorMessage = $"Query filter for field '{filter.FieldName}' has empty value";
                    return false;
                }
            }

            return true;
        }
    }
}
