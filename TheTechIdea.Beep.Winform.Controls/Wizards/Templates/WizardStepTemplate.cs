using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Base template class for wizard steps
    /// Provides common functionality for step templates
    /// </summary>
    public abstract class WizardStepTemplate
    {
        /// <summary>
        /// Create a UserControl for this step template
        /// </summary>
        public abstract UserControl CreateStepControl();

        /// <summary>
        /// Get the step configuration
        /// </summary>
        public abstract WizardStep GetStepConfig();

        /// <summary>
        /// Apply data from context to the step control
        /// </summary>
        public virtual void ApplyData(UserControl control, WizardContext context)
        {
            // Override in derived classes to populate control with context data
        }

        /// <summary>
        /// Extract data from step control to context
        /// </summary>
        public virtual void ExtractData(UserControl control, WizardContext context)
        {
            // Override in derived classes to extract data from control to context
        }
    }
}
