using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

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

        #region High Contrast Support

        /// <summary>
        /// Get accessible colors for high contrast mode
        /// Returns (completedColor, activeColor, pendingColor, errorColor, warningColor, textColor, borderColor)
        /// </summary>
        public static (Color completedColor, Color activeColor, Color pendingColor, Color errorColor, Color warningColor, Color textColor, Color borderColor) GetHighContrastColors()
        {
            if (!IsHighContrastMode())
            {
                // Return default colors if not in high contrast
                return (
                    Color.FromArgb(34, 197, 94),   // Green for completed
                    Color.FromArgb(59, 130, 246),  // Blue for active
                    Color.FromArgb(156, 163, 175), // Gray for pending
                    Color.FromArgb(239, 68, 68),   // Red for error
                    Color.FromArgb(245, 158, 11), // Orange for warning
                    SystemColors.WindowText,      // System text color
                    SystemColors.WindowFrame      // System border color
                );
            }

            // Use system colors in high contrast mode
            return (
                SystemColors.Highlight,          // Completed (system highlight)
                SystemColors.HotTrack,           // Active (system hot track)
                SystemColors.ControlDark,        // Pending (system control dark)
                SystemColors.MenuHighlight,      // Error (system menu highlight)
                SystemColors.Info,               // Warning (system info)
                SystemColors.WindowText,         // Text (system window text)
                SystemColors.WindowFrame         // Border (system window frame)
            );
        }

        /// <summary>
        /// Adjust colors for high contrast mode
        /// </summary>
        public static (Color completedColor, Color activeColor, Color pendingColor, Color errorColor, Color warningColor, Color textColor, Color borderColor) AdjustColorsForHighContrast(
            Color preferredCompletedColor,
            Color preferredActiveColor,
            Color preferredPendingColor,
            Color preferredErrorColor,
            Color preferredWarningColor,
            Color preferredTextColor,
            Color preferredBorderColor)
        {
            if (!IsHighContrastMode())
            {
                // Return preferred colors if not in high contrast
                return (preferredCompletedColor, preferredActiveColor, preferredPendingColor, 
                    preferredErrorColor, preferredWarningColor, preferredTextColor, preferredBorderColor);
            }

            // Use high contrast system colors
            return GetHighContrastColors();
        }

        /// <summary>
        /// Apply high contrast adjustments to stepper colors
        /// </summary>
        public static void ApplyHighContrastAdjustments(
            dynamic stepper,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (stepper == null || !IsHighContrastMode())
                return;

            // Get high contrast colors
            var (completedColor, activeColor, pendingColor, errorColor, warningColor, textColor, borderColor) = GetHighContrastColors();

            // Apply to stepper if properties exist
            try
            {
                if (HasProperty(stepper, "CompletedStepColor"))
                    stepper.CompletedStepColor = completedColor;
                if (HasProperty(stepper, "ActiveStepColor"))
                    stepper.ActiveStepColor = activeColor;
                if (HasProperty(stepper, "PendingStepColor"))
                    stepper.PendingStepColor = pendingColor;
                if (HasProperty(stepper, "ErrorStepColor"))
                    stepper.ErrorStepColor = errorColor;
                if (HasProperty(stepper, "WarningStepColor"))
                    stepper.WarningStepColor = warningColor;
            }
            catch
            {
                // Properties may not exist, ignore
            }
        }

        #endregion

        #region WCAG Compliance

        /// <summary>
        /// Calculate contrast ratio between two colors (WCAG formula)
        /// </summary>
        public static double CalculateContrastRatio(Color color1, Color color2)
        {
            var l1 = GetRelativeLuminance(color1);
            var l2 = GetRelativeLuminance(color2);

            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);

            if (darker == 0)
                return lighter > 0 ? double.MaxValue : 1.0;

            return (lighter + 0.05) / (darker + 0.05);
        }

        /// <summary>
        /// Get relative luminance of a color (WCAG formula)
        /// </summary>
        public static double GetRelativeLuminance(Color color)
        {
            double r = GetColorComponent(color.R);
            double g = GetColorComponent(color.G);
            double b = GetColorComponent(color.B);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Convert color component to linear RGB space
        /// </summary>
        private static double GetColorComponent(byte component)
        {
            double c = component / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Ensure contrast ratio meets WCAG standards
        /// WCAG AA: 4.5:1 for normal text, 3:1 for large text
        /// </summary>
        public static bool EnsureContrastRatio(Color foreColor, Color backColor, double minRatio = 4.5)
        {
            var ratio = CalculateContrastRatio(foreColor, backColor);
            return ratio >= minRatio;
        }

        /// <summary>
        /// Adjust color to meet minimum contrast ratio
        /// </summary>
        public static Color AdjustForContrast(Color foreColor, Color backColor, double minRatio = 4.5)
        {
            if (EnsureContrastRatio(foreColor, backColor, minRatio))
            {
                return foreColor; // Already meets requirement
            }

            // Try to darken or lighten foreground to meet contrast
            var backLuminance = GetRelativeLuminance(backColor);
            var foreLuminance = GetRelativeLuminance(foreColor);

            // Determine if we need to lighten or darken
            bool needLighter = backLuminance > 0.5; // Light background needs dark text
            bool needDarker = backLuminance <= 0.5; // Dark background needs light text

            // Adjust color
            if (needLighter && foreLuminance < backLuminance)
            {
                // Need darker text for light background
                return DarkenColor(foreColor, 0.7f);
            }
            else if (needDarker && foreLuminance > backLuminance)
            {
                // Need lighter text for dark background
                return LightenColor(foreColor, 1.3f);
            }

            // Fallback: use black or white
            return backLuminance > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Darken a color by a factor
        /// </summary>
        private static Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * factor)),
                Math.Max(0, (int)(color.G * factor)),
                Math.Max(0, (int)(color.B * factor))
            );
        }

        /// <summary>
        /// Lighten a color by a factor
        /// </summary>
        private static Color LightenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor))
            );
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
                catch
                {
                    // Ignore
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
        private static bool HasProperty(dynamic obj, string propertyName)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName);
                return prop != null;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
