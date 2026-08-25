using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// Helper class for accessibility features in progress bar controls
    /// Supports screen readers, high contrast mode, reduced motion, and WCAG compliance
    /// </summary>
    public static class ProgressBarAccessibilityHelpers
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
            // SystemInformation.HighContrast cannot throw - the previous three-level
            // try/catch tower (P/Invoke fallback, SystemColors heuristic) was fiction.
            return SystemInformation.HighContrast;
        }

        /// <summary>
        /// Check if reduced motion is enabled (Windows animation settings)
        /// </summary>
        public static bool IsReducedMotionEnabled()
        {
            bool animationsEnabled = true;
            SystemParametersInfo(SPI_GETCLIENTAREAANIMATION, 0, ref animationsEnabled, 0);
            return !animationsEnabled;
        }

        #endregion

        #region ARIA Attributes

        /// <summary>
        /// Generate accessible name for screen readers
        /// </summary>
        public static string GenerateAccessibleName(
            BeepProgressBar progressBar,
            string customName = null)
        {
            if (!string.IsNullOrEmpty(customName))
                return customName;

            if (progressBar == null)
                return "Progress bar";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            return $"Progress bar, {percentage}% complete";
        }

        /// <summary>
        /// Generate accessible description for screen readers
        /// </summary>
        public static string GenerateAccessibleDescription(
            BeepProgressBar progressBar,
            string customDescription = null)
        {
            if (!string.IsNullOrEmpty(customDescription))
                return customDescription;

            if (progressBar == null)
                return "Shows the current progress";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            string description = $"Progress is {percentage}% complete";
            
            if (progressBar.ShowTaskCount && progressBar.TotalTasks > 0)
            {
                description += $". {progressBar.CompletedTasks} of {progressBar.TotalTasks} tasks completed";
            }
            else
            {
                description += $". Value is {progressBar.Value} out of {progressBar.Maximum}";
            }

            return description;
        }

        /// <summary>
        /// Generate accessible value for screen readers
        /// </summary>
        public static string GenerateAccessibleValue(BeepProgressBar progressBar)
        {
            if (progressBar == null)
                return "0%";

            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) / 
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            return $"{percentage}%";
        }

        /// <summary>
        /// Apply accessibility settings to a progress bar control
        /// Sets ARIA attributes for screen readers
        /// </summary>
        public static void ApplyAccessibilitySettings(
            BeepProgressBar progressBar,
            string accessibleName = null,
            string accessibleDescription = null,
            ProgressPainterKind painterKind = ProgressPainterKind.Linear,
            System.Collections.Generic.IReadOnlyDictionary<string, object> parameters = null)
        {
            if (progressBar == null)
                return;

            // Set ARIA attributes for screen readers
            progressBar.AccessibleName = GenerateAccessibleName(progressBar, accessibleName);
            progressBar.AccessibleDescription = GenerateAccessibleDescription(progressBar, accessibleDescription);
            progressBar.AccessibleRole = AccessibleRole.ProgressBar;
            progressBar.AccessibleDefaultActionDescription = GeneratePainterSemanticSummary(progressBar, painterKind, parameters);
        }

        public static string GeneratePainterSemanticSummary(
            BeepProgressBar progressBar,
            ProgressPainterKind painterKind,
            System.Collections.Generic.IReadOnlyDictionary<string, object> parameters)
        {
            int percentage = (int)((float)(progressBar.Value - progressBar.Minimum) /
                Math.Max(1, progressBar.Maximum - progressBar.Minimum) * 100);

            if (painterKind == ProgressPainterKind.StepperCircles || painterKind == ProgressPainterKind.ChevronSteps)
            {
                int steps = parameters != null && parameters.TryGetValue("Steps", out var stepsObj) && stepsObj is IConvertible
                    ? Convert.ToInt32(stepsObj)
                    : 4;
                int current = parameters != null && parameters.TryGetValue("Current", out var currentObj) && currentObj is IConvertible
                    ? Convert.ToInt32(currentObj)
                    : Math.Max(0, (int)Math.Round((steps - 1) * percentage / 100f));
                return $"Step {Math.Min(steps, current + 1)} of {steps}, {percentage}% complete";
            }

            if (painterKind == ProgressPainterKind.DotsLoader || painterKind == ProgressPainterKind.ArrowHeadAnimated)
            {
                return $"Loading indicator, {percentage}% complete";
            }

            return $"{percentage}% complete";
        }

        #endregion

        #region High Contrast Support




        #endregion

        #region WCAG Compliance

        /// <summary>
        /// Calculate contrast ratio between two colors (WCAG formula).
        /// Delegates to <see cref="ColorUtils"/> — the single owner of colour math.
        /// </summary>
        public static double CalculateContrastRatio(Color color1, Color color2)
            => ColorUtils.ContrastRatio(color1, color2);

        /// <summary>
        /// Get relative luminance of a color (WCAG formula).
        /// Delegates to <see cref="ColorUtils"/>.
        /// </summary>
        public static double GetRelativeLuminance(Color color)
            => ColorUtils.RelativeLuminance(color);

        /// <summary>
        /// Ensure contrast ratio meets WCAG standards
        /// WCAG AA: 4.5:1 for normal text, 3:1 for large text
        /// </summary>
        public static bool EnsureContrastRatio(Color foreColor, Color backColor, double minRatio = 4.5)
            => ColorUtils.ContrastRatio(foreColor, backColor) >= minRatio;

        /// <summary>
        /// Adjust a color so it is legible on <paramref name="backColor"/>.
        /// Delegates to <see cref="ColorUtils.EnsureReadable"/> — the single owner.
        /// </summary>
        public static Color AdjustForContrast(Color foreColor, Color backColor, double minRatio = 4.5)
            => ColorUtils.EnsureReadable(foreColor, backColor, minRatio);

        #endregion

        #region Accessible Sizing

        /// <summary>
        /// Get accessible minimum size (larger touch targets for accessibility)
        /// </summary>
        public static Size GetAccessibleMinimumSize(Size normalMinimumSize)
        {
            // Ensure minimum 44x44 pixels for touch targets (WCAG recommendation)
            // But for progress bars, height can be smaller (minimum 8px)
            return new Size(
                Math.Max(normalMinimumSize.Width, 44),  // Minimum width for touch
                Math.Max(normalMinimumSize.Height, 8)    // Minimum height (progress bars can be thin)
            );
        }

        /// <summary>
        /// Get accessible bar height based on ProgressBarSize
        /// Adjusts height for better accessibility in high contrast mode
        /// </summary>
        public static int GetAccessibleBarHeight(ProgressBarSize barSize)
        {
            int baseHeight = barSize switch
            {
                ProgressBarSize.Thin => 4,
                ProgressBarSize.Small => 8,
                ProgressBarSize.Medium => 12,
                ProgressBarSize.Large => 20,
                ProgressBarSize.ExtraLarge => 30,
                _ => 12
            };

            // Increase height slightly in high contrast mode for better visibility
            if (IsHighContrastMode())
            {
                baseHeight = Math.Max(baseHeight + 2, 8); // Minimum 8px in high contrast
            }

            return baseHeight;
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
            // Minimum 10pt for progress bar text (can be smaller than regular text)
            return Math.Max(preferredSize, 10f);
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
    }
}

