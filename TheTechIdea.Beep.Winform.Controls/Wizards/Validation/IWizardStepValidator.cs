using System;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Models;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Validation
{
    /// <summary>
    /// Interface for wizard step validators
    /// </summary>
    public interface IWizardStepValidator
    {
        /// <summary>
        /// Validate the wizard step
        /// </summary>
        /// <param name="context">Wizard context</param>
        /// <param name="step">Wizard step to validate</param>
        /// <returns>Validation result</returns>
        WizardValidationResult Validate(WizardContext context, WizardStep step);
    }
}
