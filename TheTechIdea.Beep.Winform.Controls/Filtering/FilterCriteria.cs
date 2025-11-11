using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Represents a single filter criterion for a grid column
    /// </summary>
    public class FilterCriteria
    {
        /// <summary>
        /// Name of the column to filter
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Filter operator to apply
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// Primary filter value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Secondary filter value (used for Between/NotBetween operators)
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        /// Case-sensitive comparison (for text operators)
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Whether this filter is currently enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public FilterCriteria()
        {
        }

        public FilterCriteria(string columnName, FilterOperator op, object value, bool caseSensitive = false)
        {
            ColumnName = columnName;
            Operator = op;
            Value = value;
            CaseSensitive = caseSensitive;
        }

        public FilterCriteria(string columnName, FilterOperator op, object value, object value2, bool caseSensitive = false)
        {
            ColumnName = columnName;
            Operator = op;
            Value = value;
            Value2 = value2;
            CaseSensitive = caseSensitive;
        }

        /// <summary>
        /// Creates a clone of this filter criteria
        /// </summary>
        public FilterCriteria Clone()
        {
            return new FilterCriteria
            {
                ColumnName = this.ColumnName,
                Operator = this.Operator,
                Value = this.Value,
                Value2 = this.Value2,
                CaseSensitive = this.CaseSensitive,
                IsEnabled = this.IsEnabled
            };
        }

        public override string ToString()
        {
            if (!IsEnabled)
                return $"[Disabled] {ColumnName}";

            var displayValue = Value?.ToString() ?? "null";
            if (Operator == FilterOperator.Between || Operator == FilterOperator.NotBetween)
            {
                var displayValue2 = Value2?.ToString() ?? "null";
                return $"{ColumnName} {Operator.GetDisplayName()} {displayValue} and {displayValue2}";
            }

            return $"{ColumnName} {Operator.GetDisplayName()} {displayValue}";
        }
    }

    /// <summary>
    /// Represents a collection of filter criteria with logical combination
    /// </summary>
    public class FilterConfiguration
    {
        /// <summary>
        /// Name of this filter configuration (for saving/loading)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of this filter configuration
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of filter criteria
        /// </summary>
        public List<FilterCriteria> Criteria { get; set; } = new List<FilterCriteria>();

        /// <summary>
        /// Logical operator for combining criteria (AND/OR)
        /// </summary>
        public FilterLogic Logic { get; set; } = FilterLogic.And;

        /// <summary>
        /// Whether this configuration is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Timestamp when this configuration was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Timestamp when this configuration was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public FilterConfiguration()
        {
        }

        public FilterConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds a filter criterion to this configuration
        /// </summary>
        public void AddCriteria(FilterCriteria criteria)
        {
            if (criteria != null)
            {
                Criteria.Add(criteria);
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Removes a filter criterion by column name
        /// </summary>
        public bool RemoveCriteria(string columnName)
        {
            var removed = Criteria.RemoveAll(c => c.ColumnName == columnName) > 0;
            if (removed)
                ModifiedDate = DateTime.Now;
            return removed;
        }

        /// <summary>
        /// Clears all filter criteria
        /// </summary>
        public void Clear()
        {
            Criteria.Clear();
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Gets a criterion by column name
        /// </summary>
        public FilterCriteria GetCriterion(string columnName)
        {
            return Criteria.FirstOrDefault(c => c.ColumnName == columnName);
        }

        /// <summary>
        /// Checks if a filter exists for the specified column
        /// </summary>
        public bool HasFilter(string columnName)
        {
            return Criteria.Any(c => c.ColumnName == columnName && c.IsEnabled);
        }

        /// <summary>
        /// Gets count of enabled filters
        /// </summary>
        public int EnabledFilterCount => Criteria.Count(c => c.IsEnabled);

        /// <summary>
        /// Creates a clone of this filter configuration
        /// </summary>
        public FilterConfiguration Clone()
        {
            return new FilterConfiguration
            {
                Name = this.Name,
                Description = this.Description,
                Logic = this.Logic,
                IsActive = this.IsActive,
                CreatedDate = this.CreatedDate,
                ModifiedDate = DateTime.Now,
                Criteria = this.Criteria.Select(c => c.Clone()).ToList()
            };
        }

        public override string ToString()
        {
            var count = EnabledFilterCount;
            var logic = Logic == FilterLogic.And ? "AND" : "OR";
            return $"{Name} ({count} filter{(count != 1 ? "s" : "")} with {logic})";
        }
    }
}
