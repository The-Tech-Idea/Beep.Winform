using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Validation & Business Rules
    /// Provides Oracle Forms-compatible validation system
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        /// <summary>
        /// Dictionary of validation rules
        /// Key: Field name (or "*" for record-level rules), Value: List of rules
        /// </summary>
        private Dictionary<string, List<ValidationRule>> _validationRules = new Dictionary<string, List<ValidationRule>>();
        
        #endregion
        
        #region Validation Rule Registration
        
        /// <summary>
        /// Register a validation rule for a field
        /// </summary>
        public void RegisterValidationRule(string fieldName, ValidationRule rule)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name cannot be null or empty", nameof(fieldName));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
                
            if (!_validationRules.ContainsKey(fieldName))
            {
                _validationRules[fieldName] = new List<ValidationRule>();
            }
            
            _validationRules[fieldName].Add(rule);
            _validationRules[fieldName] = _validationRules[fieldName].OrderBy(r => r.ExecutionOrder).ToList();
        }
        
        /// <summary>
        /// Register a record-level validation rule (applies to entire record)
        /// </summary>
        public void RegisterRecordValidationRule(ValidationRule rule)
        {
            RegisterValidationRule("*", rule);
        }
        
        /// <summary>
        /// Unregister all validation rules for a field
        /// </summary>
        public void UnregisterValidationRules(string fieldName)
        {
            if (_validationRules.ContainsKey(fieldName))
            {
                _validationRules.Remove(fieldName);
            }
        }
        
        /// <summary>
        /// Get all validation rules for a field
        /// </summary>
        public List<ValidationRule> GetValidationRules(string fieldName)
        {
            return _validationRules.ContainsKey(fieldName) 
                ? new List<ValidationRule>(_validationRules[fieldName]) 
                : new List<ValidationRule>();
        }
        
        #endregion
        
        #region Field Validation
        
        /// <summary>
        /// Validate a specific field value
        /// </summary>
        public async Task<IErrorsInfo> ValidateField(string fieldName, object value)
        {
            var errors = new ErrorsInfo { Flag = Errors.Ok };
            
            if (!_validationRules.ContainsKey(fieldName))
                return errors;  // No rules = valid
                
            var context = new ValidationContext
            {
                Block = this,
                FieldName = fieldName,
                NewValue = value,
                RecordValues = GetCurrentRecordValues(),
                Mode = this.BlockMode
            };
            
            foreach (var rule in _validationRules[fieldName].Where(r => r.IsEnabled))
            {
                var result = rule.Validate(value, context);
                
                if (result.Flag != Errors.Ok)
                {
                    errors.Flag = Errors.Failed;
                    errors.Message = result.Message;
                    errors.Ex = result.Ex;
                    
                    // Update item error state
                    if (_items.ContainsKey(fieldName))
                    {
                        _items[fieldName].HasError = true;
                        _items[fieldName].ErrorMessage = result.Message;
                    }
                    
                    return errors;  // Stop on first error
                }
            }
            
            // Clear error state if validation passed
            if (_items.ContainsKey(fieldName))
            {
                _items[fieldName].HasError = false;
                _items[fieldName].ErrorMessage = null;
            }
            
            return errors;
        }
        
        #endregion
        
        #region Record Validation
        
        /// <summary>
        /// Validate the entire current record
        /// </summary>
        public async Task<IErrorsInfo> ValidateCurrentRecord()
        {
            var errors = new ErrorsInfo { Flag = Errors.Ok };
            var allErrors = new List<string>();
            
            // Validate each field
            foreach (var fieldName in _validationRules.Keys.Where(k => k != "*"))
            {
                var fieldValue = GetItemValue(fieldName);
                var fieldErrors = await ValidateField(fieldName, fieldValue);
                
                if (fieldErrors.Flag != Errors.Ok)
                {
                    allErrors.Add(fieldErrors.Message);
                }
            }
            
            // Validate record-level rules
            if (_validationRules.ContainsKey("*"))
            {
                var context = new ValidationContext
                {
                    Block = this,
                    RecordValues = GetCurrentRecordValues(),
                    Mode = this.BlockMode
                };
                
                foreach (var rule in _validationRules["*"].Where(r => r.IsEnabled))
                {
                    var result = rule.Validate(null, context);
                    
                    if (result.Flag != Errors.Ok)
                    {
                        allErrors.Add(result.Message);
                    }
                }
            }
            
            if (allErrors.Count > 0)
            {
                errors.Flag = Errors.Failed;
                errors.Message = string.Join("\n", allErrors);
            }
            
            return errors;
        }
        
        #endregion
        
        #region Validation Helpers
        
        /// <summary>
        /// Clear all validation errors
        /// </summary>
        public void ClearValidationErrors()
        {
            foreach (var item in _items.Values)
            {
                item.HasError = false;
                item.ErrorMessage = null;
            }
        }
        
        /// <summary>
        /// Get all fields with validation errors
        /// </summary>
        public List<string> GetFieldsWithErrors()
        {
            return _items.Values
                .Where(i => i.HasError)
                .Select(i => i.ItemName)
                .ToList();
        }
        
        #endregion
        
        #region Fluent Validation API
        
        /// <summary>
        /// Fluent API for adding validation rules
        /// </summary>
        public ValidationRuleBuilder ForField(string fieldName)
        {
            return new ValidationRuleBuilder(this, fieldName);
        }
        
        #endregion
    }
    
    /// <summary>
    /// Fluent API builder for validation rules
    /// </summary>
    public class ValidationRuleBuilder
    {
        private readonly BeepDataBlock _block;
        private readonly string _fieldName;
        private readonly ValidationRule _rule;
        
        internal ValidationRuleBuilder(BeepDataBlock block, string fieldName)
        {
            _block = block;
            _fieldName = fieldName;
            _rule = new ValidationRule { FieldName = fieldName };
        }
        
        public ValidationRuleBuilder Required(string errorMessage = null)
        {
            _rule.IsRequired = true;
            _rule.ErrorMessage = errorMessage ?? $"{_fieldName} is required";
            _rule.ValidationType = ValidationType.Required;
            return this;
        }
        
        public ValidationRuleBuilder MinLength(int length, string errorMessage = null)
        {
            _rule.MinLength = length;
            _rule.ErrorMessage = errorMessage ?? $"{_fieldName} must be at least {length} characters";
            _rule.ValidationType = ValidationType.Length;
            return this;
        }
        
        public ValidationRuleBuilder MaxLength(int length, string errorMessage = null)
        {
            _rule.MaxLength = length;
            _rule.ErrorMessage = errorMessage ?? $"{_fieldName} must be at most {length} characters";
            _rule.ValidationType = ValidationType.Length;
            return this;
        }
        
        public ValidationRuleBuilder Range(object min, object max, string errorMessage = null)
        {
            _rule.MinValue = min;
            _rule.MaxValue = max;
            _rule.ErrorMessage = errorMessage ?? $"{_fieldName} must be between {min} and {max}";
            _rule.ValidationType = ValidationType.Range;
            return this;
        }
        
        public ValidationRuleBuilder Pattern(string pattern, string errorMessage = null)
        {
            _rule.Pattern = pattern;
            _rule.ErrorMessage = errorMessage ?? $"{_fieldName} format is invalid";
            _rule.ValidationType = ValidationType.Format;
            return this;
        }
        
        public ValidationRuleBuilder MustBe(params object[] validValues)
        {
            _rule.ValidValues = validValues.ToList();
            _rule.ErrorMessage = $"{_fieldName} must be one of: {string.Join(", ", validValues)}";
            _rule.ValidationType = ValidationType.BusinessRule;
            return this;
        }
        
        public ValidationRuleBuilder CannotBe(params object[] invalidValues)
        {
            _rule.InvalidValues = invalidValues.ToList();
            _rule.ErrorMessage = $"{_fieldName} cannot be: {string.Join(", ", invalidValues)}";
            _rule.ValidationType = ValidationType.BusinessRule;
            return this;
        }
        
        public ValidationRuleBuilder Custom(Func<object, ValidationContext, bool> validationFunc, string errorMessage)
        {
            _rule.ValidationFunction = validationFunc;
            _rule.ErrorMessage = errorMessage;
            _rule.ValidationType = ValidationType.BusinessRule;
            return this;
        }
        
        public ValidationRuleBuilder WithMessage(string message)
        {
            _rule.ErrorMessage = message;
            return this;
        }
        
        public ValidationRuleBuilder WithOrder(int order)
        {
            _rule.ExecutionOrder = order;
            return this;
        }
        
        public void Register()
        {
            _block.RegisterValidationRule(_fieldName, _rule);
        }
    }
}

