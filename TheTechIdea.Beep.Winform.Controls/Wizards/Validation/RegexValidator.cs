using System;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Validator for regex pattern matching
    /// </summary>
    public class RegexValidator : WizardStepValidator
    {
        /// <summary>
        /// Key of the field to validate
        /// </summary>
        public string FieldKey { get; set; } = string.Empty;

        /// <summary>
        /// Regex pattern to match
        /// </summary>
        public string Pattern { get; set; } = string.Empty;

        /// <summary>
        /// Regex options
        /// </summary>
        public RegexOptions Options { get; set; } = RegexOptions.None;

        public RegexValidator(string fieldKey, string pattern, string errorMessage = "Field format is invalid")
        {
            FieldKey = fieldKey;
            Pattern = pattern;
            ErrorMessage = errorMessage;
           FieldName = fieldKey;
        }

        public override WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            if (string.IsNullOrEmpty(FieldKey))
            {
                return Failure("FieldKey must be specified", "FieldKey", "VALIDATOR_CONFIG_ERROR");
            }

            if (string.IsNullOrEmpty(Pattern))
            {
                return Failure("Pattern must be specified", "Pattern", "VALIDATOR_CONFIG_ERROR");
            }

            if (!context.ContainsKey(FieldKey))
            {
                return Failure(ErrorMessage, FieldName, "FIELD_MISSING");
            }

            var value = context.GetValue<string>(FieldKey);

            if (string.IsNullOrEmpty(value))
            {
                return Failure(ErrorMessage, FieldName, "FIELD_EMPTY");
            }

            try
            {
                var regex = new Regex(Pattern, Options);
                if (!regex.IsMatch(value))
                {
                    return Failure(ErrorMessage, FieldName, "PATTERN_MISMATCH");
                }
            }
            catch (Exception ex)
            {
                return Failure($"Invalid regex pattern: {ex.Message}", "Pattern", "REGEX_ERROR");
            }

            return Success();
        }
    }
}
