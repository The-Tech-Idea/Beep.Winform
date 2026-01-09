using System;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Base validator class for wizard steps
    /// </summary>
    public abstract class WizardStepValidator : IWizardStepValidator
    {
        /// <summary>
        /// Error message to display when validation fails
        /// </summary>
        public string ErrorMessage { get; set; } = "Validation failed";

        /// <summary>
        /// Error code for programmatic handling
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Field name that failed validation
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Validate the wizard step
        /// </summary>
        public abstract WizardValidationResult Validate(WizardContext context, WizardStep step);

        /// <summary>
        /// Create a success validation result
        /// </summary>
        protected WizardValidationResult Success()
        {
            return WizardValidationResult.Success();
        }

        /// <summary>
        /// Create a failure validation result
        /// </summary>
        protected WizardValidationResult Failure(string errorMessage, string fieldName = "", string errorCode = "")
        {
            return WizardValidationResult.Failure(
                errorMessage ?? ErrorMessage,
                fieldName ?? FieldName,
                errorCode ?? ErrorCode);
        }
    }
}
