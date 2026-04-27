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

            // Check for registered custom factory first
            int styleIndex = (int)style;
            if (styleIndex >= 0 && styleIndex < _customFactories.Length && _customFactories[styleIndex] != null)
            {
                return _customFactories[styleIndex](instance);
            }

            return style switch
            {
                WizardStyle.HorizontalStepper => new HorizontalStepperWizardForm(instance),
                WizardStyle.VerticalStepper => new VerticalStepperWizardForm(instance),
                WizardStyle.Minimal => new MinimalWizardForm(instance),
                WizardStyle.Cards => new CardsWizardForm(instance),
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
            if ((int)style < 0 || (int)style >= _customFactories.Length)
                throw new ArgumentOutOfRangeException(nameof(style), $"Style value {(int)style} is out of range (0-{_customFactories.Length - 1}).");
            _customFactories[(int)style] = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Unregister a custom form factory
        /// </summary>
        public static void UnregisterFactory(WizardStyle style)
        {
            if ((int)style >= 0 && (int)style < _customFactories.Length)
                _customFactories[(int)style] = null;
        }
    }
}
