using System;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Validator for required fields
    /// </summary>
    public class RequiredFieldValidator : WizardStepValidator
    {
        /// <summary>
        /// Key of the field to validate
        /// </summary>
        public string FieldKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to allow empty strings
        /// </summary>
        public bool AllowEmptyString { get; set; } = false;

        public RequiredFieldValidator(string fieldKey, string errorMessage = "This field is required")
        {
            FieldKey = fieldKey;
            ErrorMessage = errorMessage;
           FieldName = fieldKey;
        }

        public override WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            if (string.IsNullOrEmpty(FieldKey))
            {
                return Failure("FieldKey must be specified", "FieldKey", "VALIDATOR_CONFIG_ERROR");
            }

            if (!context.ContainsKey(FieldKey))
            {
                return Failure(ErrorMessage, FieldName, "FIELD_MISSING");
            }

            var value = context.GetValue<object>(FieldKey);

            if (value == null)
            {
                return Failure(ErrorMessage, FieldName, "FIELD_NULL");
            }

            if (value is string strValue && !AllowEmptyString && string.IsNullOrWhiteSpace(strValue))
            {
                return Failure(ErrorMessage, FieldName, "FIELD_EMPTY");
            }

            return Success();
        }
    }
}
