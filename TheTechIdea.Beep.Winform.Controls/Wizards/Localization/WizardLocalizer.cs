using System;
using System.Globalization;
using System.Resources;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Localization
{
    /// <summary>
    /// Localization helper for wizard strings
    /// </summary>
    public static class WizardLocalizer
    {
        private static ResourceManager? _resourceManager;
        private static CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// Set the resource manager for localization
        /// </summary>
        public static void SetResourceManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Set the current culture
        /// </summary>
        public static void SetCulture(CultureInfo culture)
        {
            _currentCulture = culture ?? CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// Get localized string
        /// </summary>
        public static string GetString(string key, string defaultValue = "")
        {
            if (_resourceManager == null)
                return defaultValue;

            try
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                return value ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        // Common wizard strings
        public static string NextButton => GetString("Wizard_NextButton", "Next");
        public static string BackButton => GetString("Wizard_BackButton", "Back");
        public static string FinishButton => GetString("Wizard_FinishButton", "Finish");
        public static string CancelButton => GetString("Wizard_CancelButton", "Cancel");
        public static string SkipButton => GetString("Wizard_SkipButton", "Skip");
        public static string HelpButton => GetString("Wizard_HelpButton", "Help");
        public static string CancelConfirmation => GetString("Wizard_CancelConfirmation", "Are you sure you want to cancel?");
        public static string CancelTitle => GetString("Wizard_CancelTitle", "Cancel Wizard");
        public static string ValidationError => GetString("Wizard_ValidationError", "Please correct the errors before continuing.");
        public static string StepComplete => GetString("Wizard_StepComplete", "Step completed");
        public static string StepIncomplete => GetString("Wizard_StepIncomplete", "Step incomplete");
    }
}
