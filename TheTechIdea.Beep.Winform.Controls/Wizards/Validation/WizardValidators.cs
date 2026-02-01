using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Validates that required context keys have values
    /// </summary>
    public class RequiredFieldValidator : IWizardValidator
    {
        public List<string> RequiredKeys { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = "This field is required";

        public RequiredFieldValidator() { }

        public RequiredFieldValidator(params string[] keys)
        {
            RequiredKeys.AddRange(keys);
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            foreach (var key in RequiredKeys)
            {
                if (!context.ContainsKey(key))
                {
                    return WizardValidationResult.Error($"{key} is required", key);
                }

                var value = context.GetValue<object>(key, null);
                if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                {
                    return WizardValidationResult.Error(ErrorMessage, key);
                }
            }

            return WizardValidationResult.Success();
        }
    }

    /// <summary>
    /// Validates using a custom function
    /// </summary>
    public class CustomValidator : IWizardValidator
    {
        private readonly Func<WizardContext, WizardStep, WizardValidationResult> _validationFunc;

        public CustomValidator(Func<WizardContext, WizardStep, WizardValidationResult> validationFunc)
        {
            _validationFunc = validationFunc ?? throw new ArgumentNullException(nameof(validationFunc));
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            return _validationFunc(context, step);
        }
    }

    /// <summary>
    /// Validates using a simple predicate
    /// </summary>
    public class PredicateValidator : IWizardValidator
    {
        private readonly Func<WizardContext, bool> _predicate;
        public string ErrorMessage { get; set; } = "Validation failed";

        public PredicateValidator(Func<WizardContext, bool> predicate, string errorMessage = null)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            if (!string.IsNullOrEmpty(errorMessage))
                ErrorMessage = errorMessage;
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            return _predicate(context) 
                ? WizardValidationResult.Success() 
                : WizardValidationResult.Error(ErrorMessage);
        }
    }

    /// <summary>
    /// Validates using a regex pattern
    /// </summary>
    public class RegexValidator : IWizardValidator
    {
        public string ContextKey { get; set; }
        public string Pattern { get; set; }
        public string ErrorMessage { get; set; } = "Invalid format";

        public RegexValidator() { }

        public RegexValidator(string contextKey, string pattern, string errorMessage = null)
        {
            ContextKey = contextKey;
            Pattern = pattern;
            if (!string.IsNullOrEmpty(errorMessage))
                ErrorMessage = errorMessage;
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            var value = context.GetValue<string>(ContextKey, "");
            
            if (string.IsNullOrEmpty(value))
                return WizardValidationResult.Success(); // Let RequiredFieldValidator handle empty

            if (!Regex.IsMatch(value, Pattern))
            {
                return WizardValidationResult.Error(ErrorMessage, ContextKey);
            }

            return WizardValidationResult.Success();
        }
    }

    /// <summary>
    /// Common regex patterns
    /// </summary>
    public static class ValidationPatterns
    {
        public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const string Phone = @"^\+?[\d\s\-()]{10,}$";
        public const string Url = @"^https?://[\w\-]+(\.[\w\-]+)+[/#?]?.*$";
        public const string ZipCode = @"^\d{5}(-\d{4})?$";
        public const string Integer = @"^\-?\d+$";
        public const string Decimal = @"^\-?\d+(\.\d+)?$";
    }

    /// <summary>
    /// Email validator
    /// </summary>
    public class EmailValidator : RegexValidator
    {
        public EmailValidator(string contextKey)
            : base(contextKey, ValidationPatterns.Email, "Invalid email address")
        {
        }
    }

    /// <summary>
    /// Range validator for numeric values
    /// </summary>
    public class RangeValidator : IWizardValidator
    {
        public string ContextKey { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string ErrorMessage { get; set; }

        public RangeValidator() { }

        public RangeValidator(string contextKey, double? min = null, double? max = null)
        {
            ContextKey = contextKey;
            Min = min;
            Max = max;
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            var value = context.GetValue<double?>(ContextKey, null);
            
            if (!value.HasValue)
                return WizardValidationResult.Success();

            if (Min.HasValue && value < Min)
            {
                var msg = ErrorMessage ?? $"Value must be at least {Min}";
                return WizardValidationResult.Error(msg, ContextKey);
            }

            if (Max.HasValue && value > Max)
            {
                var msg = ErrorMessage ?? $"Value must be at most {Max}";
                return WizardValidationResult.Error(msg, ContextKey);
            }

            return WizardValidationResult.Success();
        }
    }

    /// <summary>
    /// String length validator
    /// </summary>
    public class StringLengthValidator : IWizardValidator
    {
        public string ContextKey { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string ErrorMessage { get; set; }

        public StringLengthValidator() { }

        public StringLengthValidator(string contextKey, int? minLength = null, int? maxLength = null)
        {
            ContextKey = contextKey;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            var value = context.GetValue<string>(ContextKey, "");
            
            if (string.IsNullOrEmpty(value))
                return WizardValidationResult.Success();

            if (MinLength.HasValue && value.Length < MinLength)
            {
                var msg = ErrorMessage ?? $"Must be at least {MinLength} characters";
                return WizardValidationResult.Error(msg, ContextKey);
            }

            if (MaxLength.HasValue && value.Length > MaxLength)
            {
                var msg = ErrorMessage ?? $"Must be at most {MaxLength} characters";
                return WizardValidationResult.Error(msg, ContextKey);
            }

            return WizardValidationResult.Success();
        }
    }

    /// <summary>
    /// Composite validator that runs multiple validators
    /// </summary>
    public class CompositeValidator : IWizardValidator
    {
        public List<IWizardValidator> Validators { get; set; } = new List<IWizardValidator>();

        public CompositeValidator() { }

        public CompositeValidator(params IWizardValidator[] validators)
        {
            Validators.AddRange(validators);
        }

        public WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            foreach (var validator in Validators)
            {
                var result = validator.Validate(context, step);
                if (!result.IsValid)
                    return result;
            }
            return WizardValidationResult.Success();
        }
    }
}
