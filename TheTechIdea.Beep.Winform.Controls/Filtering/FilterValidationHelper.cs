using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Validates filter criteria and provides helpful error messages
    /// Ensures filters are logically correct and type-compatible
    /// </summary>
    public class FilterValidationHelper
    {
        /// <summary>
        /// Validates a single filter criterion
        /// </summary>
        public FilterValidationResult Validate(FilterCriteria criteria, DbFieldCategory columnType)
        {
            if (criteria == null)
                return FilterValidationResult.Error("Filter criterion is null");
            
            // Check required fields
            if (string.IsNullOrWhiteSpace(criteria.ColumnName))
                return FilterValidationResult.Error("Column name is required");
            
            // Validate operator compatibility with column type
            var operatorCheck = ValidateOperatorCompatibility(criteria.Operator, columnType);
            if (!operatorCheck.IsValid)
                return operatorCheck;
            
            // Validate value requirements
            var valueCheck = ValidateValueRequirements(criteria.Operator, criteria.Value, criteria.Value2);
            if (!valueCheck.IsValid)
                return valueCheck;
            
            // Validate value type compatibility
            var typeCheck = ValidateValueType(criteria.Value, columnType);
            if (!typeCheck.IsValid)
                return typeCheck;
            
            // All checks passed
            return FilterValidationResult.Success();
        }
        
        /// <summary>
        /// Validates operator compatibility with column data type
        /// </summary>
        private FilterValidationResult ValidateOperatorCompatibility(FilterOperator op, DbFieldCategory columnType)
        {
            // Text operators
            var textOperators = new[] 
            { 
                FilterOperator.Contains, 
                FilterOperator.NotContains, 
                FilterOperator.StartsWith, 
                FilterOperator.EndsWith,
                FilterOperator.Regex 
            };
            
            if (textOperators.Contains(op))
            {
                if (columnType != DbFieldCategory.String && columnType != DbFieldCategory.Text)
                {
                    return FilterValidationResult.Error(
                        $"'{op.GetDisplayName()}' operator can only be used with text columns",
                        $"Try using '=' or '!=' for {columnType} columns");
                }
            }
            
            // Numeric comparison operators
            var numericOperators = new[]
            {
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual
            };
            
            if (numericOperators.Contains(op))
            {
                if (columnType != DbFieldCategory.Numeric && 
                    columnType != DbFieldCategory.Decimal &&
                    columnType != DbFieldCategory.Integer &&
                    columnType != DbFieldCategory.Float &&
                    columnType != DbFieldCategory.Currency)
                {
                    return FilterValidationResult.Warning(
                        $"'{op.GetDisplayName()}' is typically used with numeric columns",
                        $"This may not work as expected with {columnType} columns");
                }
            }
            
            return FilterValidationResult.Success();
        }
        
        /// <summary>
        /// Validates that required values are provided for the operator
        /// </summary>
        private FilterValidationResult ValidateValueRequirements(FilterOperator op, object value, object value2)
        {
            // Null/Not Null operators don't need values
            if (op == FilterOperator.IsNull || op == FilterOperator.IsNotNull)
            {
                return FilterValidationResult.Success();
            }
            
            // Between operators need two values
            if (op == FilterOperator.Between || op == FilterOperator.NotBetween)
            {
                if (value == null)
                {
                    return FilterValidationResult.Error(
                        "First value is required for 'Between' operator",
                        "Enter the minimum value");
                }
                
                if (value2 == null)
                {
                    return FilterValidationResult.Error(
                        "Second value is required for 'Between' operator",
                        "Enter the maximum value");
                }
                
                // Validate range order
                if (value is IComparable comparable && value2 is IComparable comparable2)
                {
                    if (comparable.CompareTo(value2) > 0)
                    {
                        return FilterValidationResult.Warning(
                            "First value is greater than second value",
                            "The range might be backwards");
                    }
                }
                
                return FilterValidationResult.Success();
            }
            
            // In/NotIn operators need value
            if (op == FilterOperator.In || op == FilterOperator.NotIn)
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return FilterValidationResult.Error(
                        "Value list is required for 'In' operator",
                        "Enter comma-separated values");
                }
                
                return FilterValidationResult.Success();
            }
            
            // All other operators need at least one value
            if (value == null)
            {
                return FilterValidationResult.Error(
                    $"Value is required for '{op.GetDisplayName()}' operator",
                    "Enter a value to filter by");
            }
            
            // Empty string check for string operators
            if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            {
                if (op != FilterOperator.Equals && op != FilterOperator.NotEquals)
                {
                    return FilterValidationResult.Warning(
                        "Empty value might not produce expected results",
                        "Consider using 'Is Null' or 'Is Not Null' operators");
                }
            }
            
            return FilterValidationResult.Success();
        }
        
        /// <summary>
        /// Validates value type compatibility with column type
        /// </summary>
        private FilterValidationResult ValidateValueType(object value, DbFieldCategory columnType)
        {
            if (value == null)
                return FilterValidationResult.Success();
            
            // For string representation, try to parse
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (columnType)
                {
                    case DbFieldCategory.Numeric:
                    case DbFieldCategory.Integer:
                    case DbFieldCategory.Float:
                    case DbFieldCategory.Decimal:
                    case DbFieldCategory.Currency:
                        if (!IsNumeric(strValue))
                        {
                            return FilterValidationResult.Error(
                                $"'{strValue}' is not a valid number",
                                "Enter a numeric value (e.g., 100, 99.95)");
                        }
                        break;
                    
                    case DbFieldCategory.Date:
                    case DbFieldCategory.DateTime:
                        if (!DateTime.TryParse(strValue, out _))
                        {
                            return FilterValidationResult.Error(
                                $"'{strValue}' is not a valid date",
                                "Enter a date (e.g., 2024-01-15, Jan 15 2024)");
                        }
                        break;
                    
                    case DbFieldCategory.Boolean:
                        if (!bool.TryParse(strValue, out _) && 
                            strValue != "0" && strValue != "1" &&
                            !strValue.Equals("yes", StringComparison.OrdinalIgnoreCase) &&
                            !strValue.Equals("no", StringComparison.OrdinalIgnoreCase))
                        {
                            return FilterValidationResult.Error(
                                $"'{strValue}' is not a valid boolean",
                                "Enter true/false, yes/no, or 1/0");
                        }
                        break;
                }
            }
            
            return FilterValidationResult.Success();
        }
        
        /// <summary>
        /// Checks if a string represents a numeric value
        /// </summary>
        private bool IsNumeric(string value)
        {
            return double.TryParse(value, out _) || 
                   decimal.TryParse(value, out _) ||
                   int.TryParse(value, out _);
        }
        
        /// <summary>
        /// Gets compatible operators for a column type
        /// </summary>
        public List<FilterOperator> GetCompatibleOperators(DbFieldCategory columnType)
        {
            var operators = new List<FilterOperator>();
            
            // Common operators for all types
            operators.Add(FilterOperator.Equals);
            operators.Add(FilterOperator.NotEquals);
            operators.Add(FilterOperator.IsNull);
            operators.Add(FilterOperator.IsNotNull);
            
            switch (columnType)
            {
                case DbFieldCategory.String:
                case DbFieldCategory.Text:
                    operators.Add(FilterOperator.Contains);
                    operators.Add(FilterOperator.NotContains);
                    operators.Add(FilterOperator.StartsWith);
                    operators.Add(FilterOperator.EndsWith);
                    operators.Add(FilterOperator.Regex);
                    operators.Add(FilterOperator.In);
                    operators.Add(FilterOperator.NotIn);
                    break;
                
                case DbFieldCategory.Numeric:
                case DbFieldCategory.Integer:
                case DbFieldCategory.Float:
                case DbFieldCategory.Decimal:
                case DbFieldCategory.Currency:
                    operators.Add(FilterOperator.GreaterThan);
                    operators.Add(FilterOperator.GreaterThanOrEqual);
                    operators.Add(FilterOperator.LessThan);
                    operators.Add(FilterOperator.LessThanOrEqual);
                    operators.Add(FilterOperator.Between);
                    operators.Add(FilterOperator.NotBetween);
                    operators.Add(FilterOperator.In);
                    operators.Add(FilterOperator.NotIn);
                    break;
                
                case DbFieldCategory.Date:
                case DbFieldCategory.DateTime:
                    operators.Add(FilterOperator.GreaterThan);
                    operators.Add(FilterOperator.GreaterThanOrEqual);
                    operators.Add(FilterOperator.LessThan);
                    operators.Add(FilterOperator.LessThanOrEqual);
                    operators.Add(FilterOperator.Between);
                    operators.Add(FilterOperator.NotBetween);
                    break;
                
                case DbFieldCategory.Boolean:
                    // Only equals/not equals for boolean
                    break;
            }
            
            return operators;
        }
    }
    
    /// <summary>
    /// Result of filter validation
    /// </summary>
    public class FilterValidationResult
    {
        public bool IsValid { get; set; }
        public FilterValidationLevel Level { get; set; }
        public string Message { get; set; }
        public string Suggestion { get; set; }
        
        public static FilterValidationResult Success()
        {
            return new FilterValidationResult { IsValid = true, Level = FilterValidationLevel.Success };
        }
        
        public static FilterValidationResult Error(string message, string suggestion = null)
        {
            return new FilterValidationResult 
            { 
                IsValid = false, 
                Level = FilterValidationLevel.Error, 
                Message = message,
                Suggestion = suggestion
            };
        }
        
        public static FilterValidationResult Warning(string message, string suggestion = null)
        {
            return new FilterValidationResult 
            { 
                IsValid = true, 
                Level = FilterValidationLevel.Warning, 
                Message = message,
                Suggestion = suggestion
            };
        }
        
        public static FilterValidationResult Info(string message)
        {
            return new FilterValidationResult 
            { 
                IsValid = true, 
                Level = FilterValidationLevel.Info, 
                Message = message
            };
        }
    }
    
    /// <summary>
    /// Validation level
    /// </summary>
    public enum FilterValidationLevel
    {
        Success,
        Info,
        Warning,
        Error
    }
}

