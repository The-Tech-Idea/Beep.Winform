using System;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Helpers
{
    /// <summary>
    /// Pre-built validation rules for common scenarios
    /// </summary>
    public static class ValidationRuleHelpers
    {
        #region Email Validation
        
        public static ValidationRule EmailRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Email",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                ErrorMessage = $"{fieldName} must be a valid email address"
            };
        }
        
        #endregion
        
        #region Phone Validation
        
        public static ValidationRule PhoneRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Phone",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                Pattern = @"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$",
                ErrorMessage = $"{fieldName} must be a valid phone number (e.g., (555) 123-4567)"
            };
        }
        
        #endregion
        
        #region URL Validation
        
        public static ValidationRule URLRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_URL",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                Pattern = @"^https?://[^\s]+$",
                ErrorMessage = $"{fieldName} must be a valid URL"
            };
        }
        
        #endregion
        
        #region Numeric Validation
        
        public static ValidationRule PositiveNumberRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Positive",
                FieldName = fieldName,
                ValidationType = ValidationType.Range,
                MinValue = 0,
                ErrorMessage = $"{fieldName} must be a positive number"
            };
        }
        
        public static ValidationRule PercentageRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Percentage",
                FieldName = fieldName,
                ValidationType = ValidationType.Range,
                MinValue = 0,
                MaxValue = 100,
                ErrorMessage = $"{fieldName} must be between 0 and 100"
            };
        }
        
        #endregion
        
        #region Date Validation
        
        public static ValidationRule FutureDateRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Future",
                FieldName = fieldName,
                ValidationType = ValidationType.Range,
                MinValue = DateTime.Today,
                ErrorMessage = $"{fieldName} must be today or in the future"
            };
        }
        
        public static ValidationRule PastDateRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Past",
                FieldName = fieldName,
                ValidationType = ValidationType.Range,
                MaxValue = DateTime.Today,
                ErrorMessage = $"{fieldName} must be today or in the past"
            };
        }
        
        #endregion
        
        #region Credit Card Validation
        
        public static ValidationRule CreditCardRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_CreditCard",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var cardNumber = value.ToString().Replace(" ", "").Replace("-", "");
                    
                    // Luhn algorithm
                    if (cardNumber.Length < 13 || cardNumber.Length > 19)
                        return false;
                        
                    int sum = 0;
                    bool alternate = false;
                    
                    for (int i = cardNumber.Length - 1; i >= 0; i--)
                    {
                        if (!char.IsDigit(cardNumber[i]))
                            return false;
                            
                        int digit = cardNumber[i] - '0';
                        
                        if (alternate)
                        {
                            digit *= 2;
                            if (digit > 9)
                                digit -= 9;
                        }
                        
                        sum += digit;
                        alternate = !alternate;
                    }
                    
                    return sum % 10 == 0;
                },
                ErrorMessage = $"{fieldName} must be a valid credit card number"
            };
        }
        
        #endregion
        
        #region ZIP Code Validation
        
        public static ValidationRule USZipCodeRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_USZip",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                Pattern = @"^\d{5}(-\d{4})?$",
                ErrorMessage = $"{fieldName} must be a valid US ZIP code (e.g., 12345 or 12345-6789)"
            };
        }
        
        #endregion
        
        #region SSN Validation
        
        public static ValidationRule SSNRule(string fieldName)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_SSN",
                FieldName = fieldName,
                ValidationType = ValidationType.Format,
                Pattern = @"^\d{3}-\d{2}-\d{4}$",
                ErrorMessage = $"{fieldName} must be a valid SSN (e.g., 123-45-6789)"
            };
        }
        
        #endregion
        
        #region Business Rules
        
        public static ValidationRule UniqueValueRule(string fieldName, Func<object, bool> checkUnique)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_Unique",
                FieldName = fieldName,
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) => checkUnique(value),
                ErrorMessage = $"{fieldName} must be unique"
            };
        }
        
        public static ValidationRule ConditionalRequiredRule(string fieldName, string condition, Func<ValidationContext, bool> conditionFunc)
        {
            return new ValidationRule
            {
                RuleName = $"{fieldName}_ConditionalRequired",
                FieldName = fieldName,
                ValidationType = ValidationType.BusinessRule,
                ConditionalExpression = condition,
                ValidationFunction = (value, context) =>
                {
                    // Only validate if condition is true
                    if (!conditionFunc(context))
                        return true;
                        
                    // If condition is true, value is required
                    return value != null && !string.IsNullOrEmpty(value.ToString());
                },
                ErrorMessage = $"{fieldName} is required when {condition}"
            };
        }
        
        #endregion
    }
}

