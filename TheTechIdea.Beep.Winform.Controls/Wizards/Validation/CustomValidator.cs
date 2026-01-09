using System;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Validator for custom validation logic
    /// </summary>
    public class CustomValidator : WizardStepValidator
    {
        /// <summary>
        /// Custom validation function
        /// </summary>
        public Func<WizardContext, WizardStep, WizardValidationResult> ValidationFunction { get; set; }

        public CustomValidator(Func<WizardContext, WizardStep, WizardValidationResult> validationFunction, string errorMessage = "Validation failed")
        {
            ValidationFunction = validationFunction ?? throw new ArgumentNullException(nameof(validationFunction));
            ErrorMessage = errorMessage;
        }

        public override WizardValidationResult Validate(WizardContext context, WizardStep step)
        {
            if (ValidationFunction == null)
            {
                return Failure("ValidationFunction must be specified", "ValidationFunction", "VALIDATOR_CONFIG_ERROR");
            }

            try
            {
                return ValidationFunction(context, step);
            }
            catch (Exception ex)
            {
                return Failure($"Validation error: {ex.Message}", FieldName, "VALIDATION_EXCEPTION");
            }
        }
    }
}
