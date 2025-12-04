using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Represents a validation rule for a field or record
    /// Oracle Forms-compatible validation with modern C# enhancements
    /// </summary>
    public class ValidationRule
    {
        #region Properties
        
        /// <summary>
        /// Unique rule name
        /// </summary>
        public string RuleName { get; set; }
        
        /// <summary>
        /// Rule description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Field name this rule applies to (null = record-level rule)
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// Validation type
        /// </summary>
        public ValidationType ValidationType { get; set; }
        
        /// <summary>
        /// Error message to display when validation fails
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Warning message (doesn't block save, just informs)
        /// </summary>
        public string WarningMessage { get; set; }
        
        /// <summary>
        /// Execution order (lower = earlier)
        /// </summary>
        public int ExecutionOrder { get; set; }
        
        /// <summary>
        /// Whether this rule is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        #endregion
        
        #region Validation Logic
        
        /// <summary>
        /// Validation function - returns true if valid
        /// </summary>
        public Func<object, ValidationContext, bool> ValidationFunction { get; set; }
        
        /// <summary>
        /// Alternative: Expression-based validation (for serialization)
        /// </summary>
        public string ValidationExpression { get; set; }
        
        #endregion
        
        #region Rule Conditions
        
        /// <summary>
        /// Required: Field must have a value
        /// </summary>
        public bool IsRequired { get; set; }
        
        /// <summary>
        /// Min length (for strings)
        /// </summary>
        public int? MinLength { get; set; }
        
        /// <summary>
        /// Max length (for strings)
        /// </summary>
        public int? MaxLength { get; set; }
        
        /// <summary>
        /// Min value (for numbers/dates)
        /// </summary>
        public object MinValue { get; set; }
        
        /// <summary>
        /// Max value (for numbers/dates)
        /// </summary>
        public object MaxValue { get; set; }
        
        /// <summary>
        /// Regex pattern (for string validation)
        /// </summary>
        public string Pattern { get; set; }
        
        /// <summary>
        /// Valid values (for enum-like validation)
        /// </summary>
        public List<object> ValidValues { get; set; }
        
        /// <summary>
        /// Invalid values (blacklist)
        /// </summary>
        public List<object> InvalidValues { get; set; }
        
        #endregion
        
        #region Business Rules
        
        /// <summary>
        /// Cross-field validation: Other fields that must be validated together
        /// </summary>
        public List<string> DependentFields { get; set; } = new List<string>();
        
        /// <summary>
        /// Conditional validation: Only validate if condition is true
        /// Example: "CustomerType = 'Corporate'" → Only validate TaxID for corporate customers
        /// </summary>
        public string ConditionalExpression { get; set; }
        
        /// <summary>
        /// Computed validation: Validate against a computed value
        /// Example: LineTotal = Quantity * UnitPrice
        /// </summary>
        public string ComputationExpression { get; set; }
        
        #endregion
        
        #region Statistics
        
        /// <summary>
        /// Number of times this rule has been executed
        /// </summary>
        public int ExecutionCount { get; set; }
        
        /// <summary>
        /// Number of times this rule failed
        /// </summary>
        public int FailureCount { get; set; }
        
        /// <summary>
        /// Last execution time
        /// </summary>
        public DateTime? LastExecutionTime { get; set; }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Validate a value against this rule
        /// </summary>
        public ErrorsInfo Validate(object value, ValidationContext context)
        {
            ExecutionCount++;
            LastExecutionTime = DateTime.Now;
            
            var errors = new ErrorsInfo { Flag = Errors.Ok };
            
            if (!IsEnabled)
                return errors;
                
            try
            {
                // Custom validation function
                if (ValidationFunction != null)
                {
                    if (!ValidationFunction(value, context))
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"Validation failed for {FieldName}";
                        return errors;
                    }
                }
                
                // Built-in validations
                if (IsRequired && (value == null || string.IsNullOrEmpty(value.ToString())))
                {
                    FailureCount++;
                    errors.Flag = Errors.Failed;
                    errors.Message = ErrorMessage ?? $"{FieldName} is required";
                    return errors;
                }
                
                if (value is string strValue)
                {
                    if (MinLength.HasValue && strValue.Length < MinLength.Value)
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} must be at least {MinLength} characters";
                        return errors;
                    }
                    
                    if (MaxLength.HasValue && strValue.Length > MaxLength.Value)
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} must be at most {MaxLength} characters";
                        return errors;
                    }
                    
                    if (!string.IsNullOrEmpty(Pattern))
                    {
                        var regex = new System.Text.RegularExpressions.Regex(Pattern);
                        if (!regex.IsMatch(strValue))
                        {
                            FailureCount++;
                            errors.Flag = Errors.Failed;
                            errors.Message = ErrorMessage ?? $"{FieldName} format is invalid";
                            return errors;
                        }
                    }
                }
                
                // Range validation
                if (MinValue != null && value is IComparable minComp)
                {
                    if (minComp.CompareTo(Convert.ChangeType(MinValue, value.GetType())) < 0)
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} must be at least {MinValue}";
                        return errors;
                    }
                }
                
                if (MaxValue != null && value is IComparable maxComp)
                {
                    if (maxComp.CompareTo(Convert.ChangeType(MaxValue, value.GetType())) > 0)
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} must be at most {MaxValue}";
                        return errors;
                    }
                }
                
                // Valid values check
                if (ValidValues != null && ValidValues.Count > 0)
                {
                    var valueStr = value?.ToString();
                    if (!ValidValues.Any(v => v?.ToString() == valueStr))
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} must be one of: {string.Join(", ", ValidValues)}";
                        return errors;
                    }
                }
                
                // Invalid values check
                if (InvalidValues != null && InvalidValues.Count > 0)
                {
                    var valueStr = value?.ToString();
                    if (InvalidValues.Any(v => v?.ToString() == valueStr))
                    {
                        FailureCount++;
                        errors.Flag = Errors.Failed;
                        errors.Message = ErrorMessage ?? $"{FieldName} cannot be: {value}";
                        return errors;
                    }
                }
            }
            catch (Exception ex)
            {
                FailureCount++;
                errors.Flag = Errors.Failed;
                errors.Message = $"Validation error: {ex.Message}";
                errors.Ex = ex;
            }
            
            return errors;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Validation type enumeration
    /// </summary>
    public enum ValidationType
    {
        /// <summary>Required field validation</summary>
        Required,
        
        /// <summary>Format/pattern validation</summary>
        Format,
        
        /// <summary>Range validation (min/max)</summary>
        Range,
        
        /// <summary>Length validation (strings)</summary>
        Length,
        
        /// <summary>Cross-field validation</summary>
        CrossField,
        
        /// <summary>Custom business rule</summary>
        BusinessRule,
        
        /// <summary>Database lookup validation</summary>
        Lookup,
        
        /// <summary>Expression-based validation</summary>
        Expression,
        
        /// <summary>Computed field validation</summary>
        Computed
    }
    
    /// <summary>
    /// Validation context - provides info for validation logic
    /// </summary>
    public class ValidationContext
    {
        /// <summary>
        /// The block being validated
        /// </summary>
        public BeepDataBlock Block { get; set; }
        
        /// <summary>
        /// Current record values
        /// </summary>
        public Dictionary<string, object> RecordValues { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Field being validated
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// Old value (before change)
        /// </summary>
        public object OldValue { get; set; }
        
        /// <summary>
        /// New value (being validated)
        /// </summary>
        public object NewValue { get; set; }
        
        /// <summary>
        /// Whether this is a new record
        /// </summary>
        public bool IsNewRecord { get; set; }
        
        /// <summary>
        /// Block mode
        /// </summary>
        public DataBlockMode Mode { get; set; }
        
        /// <summary>
        /// Custom data for validation logic
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
    }
}

