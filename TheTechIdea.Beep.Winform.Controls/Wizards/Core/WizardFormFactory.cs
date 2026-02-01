using System;
using TheTechIdea.Beep.Winform.Controls.Wizards.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Factory for creating wizard forms based on style
    /// Easy to extend with new form styles
    /// </summary>
    public static class WizardFormFactory
    {
        /// <summary>
        /// Create a wizard form for the specified style
        /// </summary>
        public static IWizardFormHost CreateForm(WizardStyle style, WizardInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return style switch
            {
                WizardStyle.HorizontalStepper => new HorizontalStepperWizardForm(instance),
                WizardStyle.VerticalStepper => new VerticalStepperWizardForm(instance),
                WizardStyle.Minimal => new MinimalWizardForm(instance),
                _ => new HorizontalStepperWizardForm(instance)
            };
        }

        /// <summary>
        /// Register a custom form factory for a style
        /// </summary>
        private static Func<WizardInstance, IWizardFormHost>[] _customFactories = new Func<WizardInstance, IWizardFormHost>[16];

        /// <summary>
        /// Register a custom form factory
        /// </summary>
        public static void RegisterFactory(WizardStyle style, Func<WizardInstance, IWizardFormHost> factory)
        {
            _customFactories[(int)style] = factory;
        }
    }
}
