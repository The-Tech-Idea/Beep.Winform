using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor.Forms.Helpers;
using TheTechIdea.Beep.Editor.UOWManager.Models;
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
            
            // Delegate to FormsManager when coordinated
            if (IsCoordinated)
            {
                try
                {
                    var dmRule = ValidationBridge.ToBeepDMRule(
                        blockName: this.Name,
                        fieldName: fieldName,
                        ruleName: rule.RuleName,
                        validationType: (int)rule.ValidationType,
                        errorMessage: rule.ErrorMessage,
                        isRequired: rule.IsRequired,
                        minValue: rule.MinValue,
                        maxValue: rule.MaxValue,
                        pattern: rule.Pattern,
                        executionOrder: rule.ExecutionOrder);
                    _formsManager.Validation.RegisterRule(dmRule);
                }
                catch { /* local rules still active as fallback */ }
            }
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
            
            if (IsCoordinated)
            {
                try { _formsManager.Validation.UnregisterItemRules(this.Name, fieldName); }
                catch { }
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
            
            // Delegate to FormsManager when coordinated
            if (IsCoordinated)
            {
                try
                {
                    var result = _formsManager.Validation.ValidateItem(this.Name, fieldName, value);
                    if (!result.IsValid)
                    {
                        errors.Flag = Errors.Failed;
                        errors.Message = result.ErrorMessage;
                        if (TryResolveItem(fieldName, out var errItem, out _))
                        {
                            errItem.SetError(result.ErrorMessage);
                        }
                        return errors;
                    }
                }
                catch { /* fall through to local validation */ }
            }
            
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
                    if (TryResolveItem(fieldName, out var resolvedItem, out _))
                    {
                        resolvedItem.SetError(result.Message);
                    }
                    
                    return errors;  // Stop on first error
                }
            }
            
            // Clear error state if validation passed
            if (TryResolveItem(fieldName, out var clearedItem, out _))
            {
                clearedItem.ClearError();
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
            
            // Delegate to FormsManager when coordinated
            if (IsCoordinated)
            {
                try
                {
                    var recordValues = GetCurrentRecordValues();
                    var result = _formsManager.Validation.ValidateRecord(this.Name, recordValues);
                    if (!result.IsValid)
                    {
                        errors.Flag = Errors.Failed;
                        errors.Message = string.Join("\n", result.ItemResults
                            .Where(r => !r.IsValid)
                            .Select(r => r.ErrorMessage));
                        return errors;
                    }
                }
                catch { /* fall through to local validation */ }
            }
            
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
                item.ClearError();
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
            _rule = new ValidationRule {FieldName = fieldName };
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

