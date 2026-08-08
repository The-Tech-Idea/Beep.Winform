using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in stepper controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class StepperAccessibilityHelpers
    {
        #region Windows API for System Settings

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int fuWinIni);

        private const int SPI_GETCLIENTAREAANIMATION = 0x1042;
        private const int SPI_GETANIMATION = 0x0048;
        private const int SPI_GETHIGHCONTRAST = 0x0042;

        #endregion

        #region System Detection

        /// <summary>
        /// Check if Windows high contrast mode is enabled
        /// </summary>
        public static bool IsHighContrastMode()
        {
            try
            {
                // Use SystemInformation for reliable detection
                return SystemInformation.HighContrast;
            }
            catch
            {
                // Fallback: check via API
                try
                {
                    bool highContrast = false;
                    SystemParametersInfo(SPI_GETHIGHCONTRAST, 0, ref highContrast, 0);
                    return highContrast;
                }
                catch
                {
                    // Final fallback: check SystemColors
                    return SystemColors.Control != SystemColors.Window;
                }
            }
        }

        /// <summary>
        /// Check if reduced motion is enabled (Windows animation settings)
        /// </summary>
        public static bool IsReducedMotionEnabled()
        {
            try
            {
                bool animationsEnabled = true;
                SystemParametersInfo(SPI_GETCLIENTAREAANIMATION, 0, ref animationsEnabled, 0);
                return !animationsEnabled; // Inverted: if animations disabled, reduced motion is enabled
            }
            catch
            {
                // Fallback: check environment variable
                var prefersReducedMotion = Environment.GetEnvironmentVariable("PREFERS_REDUCED_MOTION");
                return prefersReducedMotion == "1" || prefersReducedMotion?.ToLowerInvariant() == "true";
            }
        }

        #endregion

        #region ARIA Attributes

        /// <summary>
        /// Generate accessible name for the main stepper control
        /// </summary>
        public static string GenerateControlAccessibleName(
            dynamic stepper,
            string customName = null)
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;

            if (stepper == null)
                return "Stepper";

            int currentStep = GetCurrentStep(stepper);
            int stepCount = GetStepCount(stepper);

            return $"Stepper, step {currentStep + 1} of {stepCount}";
        }

        /// <summary>
        /// Generate accessible description for the main stepper control
        /// </summary>
        public static string GenerateControlAccessibleDescription(
            dynamic stepper,
            string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            if (stepper == null)
                return "Interactive step-by-step progress indicator";

            int currentStep = GetCurrentStep(stepper);
            int stepCount = GetStepCount(stepper);
            string currentStepLabel = GetStepLabel(stepper, currentStep);

            string description = $"Currently on step {currentStep + 1} of {stepCount}";
            if (!string.IsNullOrEmpty(currentStepLabel))
            {
                description += $": {currentStepLabel}";
            }

            return description;
        }

        /// <summary>
        /// Generate accessible name for an individual step
        /// </summary>
        public static string GenerateStepAccessibleName(
            dynamic stepper,
            int stepIndex,
            StepState state,
            string stepLabel = null)
        {
            if (stepper == null)
                return $"Step {stepIndex + 1}";

            int stepCount = GetStepCount(stepper);
            int currentStep = GetCurrentStep(stepper);

            string stateText = state switch
            {
                StepState.Completed => "completed",
                StepState.Active => "active",
                StepState.Error => "error",
                StepState.Warning => "warning",
                _ => "pending"
            };

            string positionText = $"Step {stepIndex + 1} of {stepCount}";
            string labelText = !string.IsNullOrEmpty(stepLabel) ? $": {stepLabel}" : "";

            return $"{positionText}{labelText}, {stateText}";
        }

        /// <summary>
        /// Generate accessible description for an individual step
        /// </summary>
        public static string GenerateStepAccessibleDescription(
            dynamic stepper,
            int stepIndex,
            StepState state,
            string stepLabel = null)
        {
            if (stepper == null)
                return $"Step {stepIndex + 1}";

            int currentStep = GetCurrentStep(stepper);
            bool isClickable = IsStepClickable(stepper);

            string description = $"Step {stepIndex + 1}";
            if (!string.IsNullOrEmpty(stepLabel))
            {
                description += $": {stepLabel}";
            }

            description += $", {state.ToString().ToLower()}";

            if (stepIndex == currentStep)
            {
                description += ", currently active";
            }

            if (isClickable)
            {
                description += ". Click to navigate to this step";
            }

            return description;
        }

        /// <summary>
        /// Apply accessibility settings to a stepper control
        /// Sets ARIA attributes for screen readers
        /// </summary>
        public static void ApplyAccessibilitySettings(
            Control stepper,
            string accessibleName = null,
            string accessibleDescription = null)
        {
            if (stepper == null)
                return;

            // Set ARIA attributes for screen readers
            stepper.AccessibleName = GenerateControlAccessibleName(stepper, accessibleName);
            stepper.AccessibleDescription = GenerateControlAccessibleDescription(stepper, accessibleDescription);
            stepper.AccessibleRole = AccessibleRole.List;
        }

        #endregion

        #region Accessible Sizing

        /// <summary>
        /// Get accessible minimum size (larger touch targets for accessibility)
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size normalMinimumSize)
        {
            // Ensure minimum 44x44 pixels for touch targets (WCAG recommendation)
            return new Size(
                Math.Max(normalMinimumSize.Width, 44),
                Math.Max(normalMinimumSize.Height, 44)
            );
        }

        /// <summary>
        /// Get accessible step button size (minimum size for step circles/buttons)
        /// </summary>
        public static Size GetAccessibleStepButtonSize(Size normalButtonSize)
        {
            // Ensure minimum 32x32 pixels for step buttons (WCAG recommendation for interactive elements)
            return new Size(
                Math.Max(normalButtonSize.Width, 32),
                Math.Max(normalButtonSize.Height, 32)
            );
        }

        /// <summary>
        /// Get accessible border width (thicker borders for visibility)
        /// </summary>
        public static int GetAccessibleBorderWidth(int normalBorderWidth)
        {
            if (IsHighContrastMode())
            {
                return Math.Max(normalBorderWidth, 2); // Minimum 2px in high contrast
            }
            return normalBorderWidth;
        }

        /// <summary>
        /// Get accessible font size (minimum readable size)
        /// </summary>
        public static float GetAccessibleFontSize(float preferredSize)
        {
            // Minimum 12pt for accessibility (WCAG recommendation)
            return Math.Max(preferredSize, 12f);
        }

        /// <summary>
        /// Get accessible connector line width (thicker lines for visibility)
        /// </summary>
        public static int GetAccessibleConnectorLineWidth(int normalLineWidth)
        {
            if (IsHighContrastMode())
            {
                return Math.Max(normalLineWidth, 2); // Minimum 2px in high contrast
            }
            return normalLineWidth;
        }

        #endregion

        #region Reduced Motion

        /// <summary>
        /// Should animations be disabled based on user preferences
        /// </summary>
        public static bool ShouldDisableAnimations(bool currentAnimationEnabled)
        {
            if (!currentAnimationEnabled)
            {
                return true; // Already disabled
            }

            return IsReducedMotionEnabled();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get current step index from stepper (dynamic access)
        /// </summary>
        private static int GetCurrentStep(dynamic stepper)
        {
            try
            {
                return stepper.CurrentStep ?? 0;
            }
            catch
            {
                try
                {
                    return stepper.SelectedIndex >= 0 ? stepper.SelectedIndex : 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get step count from stepper (dynamic access)
        /// </summary>
        private static int GetStepCount(dynamic stepper)
        {
            try
            {
                return stepper.StepCount ?? 1;
            }
            catch
            {
                try
                {
                    return stepper.ListItems?.Count ?? 1;
                }
                catch
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Get step label from stepper (dynamic access)
        /// </summary>
        private static string GetStepLabel(dynamic stepper, int stepIndex)
        {
            try
            {
                return stepper.GetStepLabel(stepIndex) ?? "";
            }
            catch
            {
                try
                {
                    if (stepper.ListItems != null && stepIndex >= 0 && stepIndex < stepper.ListItems.Count)
                    {
                        return stepper.ListItems[stepIndex].Name ?? "";
                    }
                }
                catch (Exception ex)
                {
                    BeepLog.WarnOnce("Stepper.a11yName", null, "read step name for accessibility", ex.Message);
                }
                return "";
            }
        }

        /// <summary>
        /// Check if step is clickable (dynamic access)
        /// </summary>
        private static bool IsStepClickable(dynamic stepper)
        {
            try
            {
                return stepper.AllowStepNavigation ?? true;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Check if object has a property (dynamic access)
        /// </summary>

        #endregion
    }
}
