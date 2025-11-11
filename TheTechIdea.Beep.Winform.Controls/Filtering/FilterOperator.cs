using System;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Defines the comparison operators available for filtering grid data
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Equal to (exact match)
        /// </summary>
        Equals,

        /// <summary>
        /// Not equal to
        /// </summary>
        NotEquals,

        /// <summary>
        /// Contains substring (case-insensitive)
        /// </summary>
        Contains,

        /// <summary>
        /// Does not contain substring (case-insensitive)
        /// </summary>
        NotContains,

        /// <summary>
        /// Starts with substring (case-insensitive)
        /// </summary>
        StartsWith,

        /// <summary>
        /// Ends with substring (case-insensitive)
        /// </summary>
        EndsWith,

        /// <summary>
        /// Greater than (numeric/date comparison)
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal to (numeric/date comparison)
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Less than (numeric/date comparison)
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal to (numeric/date comparison)
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Between two values (inclusive, for numeric/date)
        /// </summary>
        Between,

        /// <summary>
        /// Not between two values (for numeric/date)
        /// </summary>
        NotBetween,

        /// <summary>
        /// Is null or empty
        /// </summary>
        IsNull,

        /// <summary>
        /// Is not null or empty
        /// </summary>
        IsNotNull,

        /// <summary>
        /// Matches regular expression pattern
        /// </summary>
        Regex,

        /// <summary>
        /// In a list of values
        /// </summary>
        In,

        /// <summary>
        /// Not in a list of values
        /// </summary>
        NotIn
    }

    /// <summary>
    /// Logical operator for combining multiple filter criteria
    /// </summary>
    public enum FilterLogic
    {
        /// <summary>
        /// All criteria must match (AND)
        /// </summary>
        And,

        /// <summary>
        /// At least one criterion must match (OR)
        /// </summary>
        Or
    }

    /// <summary>
    /// Helper class for filter operator display names and descriptions
    /// </summary>
    public static class FilterOperatorExtensions
    {
        public static string GetDisplayName(this FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Equals => "Equals",
                FilterOperator.NotEquals => "Not Equals",
                FilterOperator.Contains => "Contains",
                FilterOperator.NotContains => "Does Not Contain",
                FilterOperator.StartsWith => "Starts With",
                FilterOperator.EndsWith => "Ends With",
                FilterOperator.GreaterThan => "Greater Than",
                FilterOperator.GreaterThanOrEqual => "Greater Than or Equal",
                FilterOperator.LessThan => "Less Than",
                FilterOperator.LessThanOrEqual => "Less Than or Equal",
                FilterOperator.Between => "Between",
                FilterOperator.NotBetween => "Not Between",
                FilterOperator.IsNull => "Is Empty",
                FilterOperator.IsNotNull => "Is Not Empty",
                FilterOperator.Regex => "Matches Pattern",
                FilterOperator.In => "In List",
                FilterOperator.NotIn => "Not In List",
                _ => op.ToString()
            };
        }

        public static string GetSymbol(this FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Equals => "=",
                FilterOperator.NotEquals => "≠",
                FilterOperator.Contains => "⊃",
                FilterOperator.NotContains => "⊅",
                FilterOperator.StartsWith => "^",
                FilterOperator.EndsWith => "$",
                FilterOperator.GreaterThan => ">",
                FilterOperator.GreaterThanOrEqual => "≥",
                FilterOperator.LessThan => "<",
                FilterOperator.LessThanOrEqual => "≤",
                FilterOperator.Between => "↔",
                FilterOperator.NotBetween => "↮",
                FilterOperator.IsNull => "∅",
                FilterOperator.IsNotNull => "∄",
                FilterOperator.Regex => ".*",
                FilterOperator.In => "∈",
                FilterOperator.NotIn => "∉",
                _ => "?"
            };
        }

        /// <summary>
        /// Gets operators appropriate for the data type
        /// </summary>
        public static FilterOperator[] GetOperatorsForType(Type dataType)
        {
            if (dataType == null)
                return GetTextOperators();

            if (dataType == typeof(string))
                return GetTextOperators();

            if (IsNumericType(dataType))
                return GetNumericOperators();

            if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
                return GetDateOperators();

            if (dataType == typeof(bool) || dataType == typeof(bool?))
                return GetBooleanOperators();

            return GetTextOperators();
        }

        private static FilterOperator[] GetTextOperators()
        {
            return new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.Contains,
                FilterOperator.NotContains,
                FilterOperator.StartsWith,
                FilterOperator.EndsWith,
                FilterOperator.IsNull,
                FilterOperator.IsNotNull,
                FilterOperator.Regex,
                FilterOperator.In,
                FilterOperator.NotIn
            };
        }

        private static FilterOperator[] GetNumericOperators()
        {
            return new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual,
                FilterOperator.Between,
                FilterOperator.NotBetween,
                FilterOperator.IsNull,
                FilterOperator.IsNotNull,
                FilterOperator.In,
                FilterOperator.NotIn
            };
        }

        private static FilterOperator[] GetDateOperators()
        {
            return new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual,
                FilterOperator.Between,
                FilterOperator.NotBetween,
                FilterOperator.IsNull,
                FilterOperator.IsNotNull
            };
        }

        private static FilterOperator[] GetBooleanOperators()
        {
            return new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.IsNull,
                FilterOperator.IsNotNull
            };
        }

        private static bool IsNumericType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(decimal) || type == typeof(double) || type == typeof(float) ||
                   type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort);
        }
    }
}
