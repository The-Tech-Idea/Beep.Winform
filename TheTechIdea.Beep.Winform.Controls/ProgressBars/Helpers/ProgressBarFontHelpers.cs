using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in progress bar controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class ProgressBarFontHelpers
    {
        /// <summary>
        /// Gets the font for progress bar text (percentage, progress, custom text)
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetProgressBarTextFont(
            BeepProgressBar progressBar,
            ProgressBarDisplayMode displayMode,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            // Base size from ControlStyle
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Adjust size based on display mode
            float textSize = displayMode switch
            {
                ProgressBarDisplayMode.Percentage => baseSize - 1f,        // Slightly smaller for percentage
                ProgressBarDisplayMode.CurrProgress => baseSize - 1f,     // Smaller for "value/max"
                ProgressBarDisplayMode.CustomText => baseSize,            // Standard size for custom text
                ProgressBarDisplayMode.TextAndPercentage => baseSize - 1f, // Smaller for combined text
                ProgressBarDisplayMode.TextAndCurrProgress => baseSize - 1f,
                ProgressBarDisplayMode.TaskProgress => baseSize - 1f,     // Smaller for task count
                ProgressBarDisplayMode.CenterPercentage => baseSize,       // Standard for center text
                ProgressBarDisplayMode.LoadingText => baseSize,            // Standard for loading text
                ProgressBarDisplayMode.StepLabels => baseSize - 1.5f,      // Smaller for step labels
                ProgressBarDisplayMode.ValueOverMax => baseSize - 1f,      // Smaller for "value/max"
                _ => baseSize - 1f                                         // Default slightly smaller
            };

            // Adjust size based on ProgressBarSize
            textSize = AdjustSizeForBarSize(textSize, progressBar.BarSize);

            // Ensure minimum readable size
            textSize = Math.Max(7f, textSize);

            // Font style: Regular for most cases, can be customized
            FontStyle fontStyle = FontStyle.Regular;

            // Get font family from ControlStyle
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            
            // Parse font family string (may contain fallbacks like "Roboto, Segoe UI")
            string primaryFont = fontFamily.Split(',')[0].Trim();

            // Use BeepFontManager to get the font
            return BeepFontManager.GetFont(primaryFont, textSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for progress bar percentage text (when displayed)
        /// Typically slightly smaller than regular text
        /// </summary>
        public static Font GetProgressBarPercentageFont(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float percentageSize = Math.Max(8f, baseSize - 1f);

            // Adjust for bar size
            percentageSize = AdjustSizeForBarSize(percentageSize, progressBar.BarSize);

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, percentageSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for progress bar label text (task text, custom labels)
        /// </summary>
        public static Font GetProgressBarLabelFont(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float labelSize = Math.Max(8f, baseSize - 0.5f);

            // Adjust for bar size
            labelSize = AdjustSizeForBarSize(labelSize, progressBar.BarSize);

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, labelSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the default font for the progress bar control
        /// Uses ControlStyle to determine appropriate font
        /// </summary>
        public static Font GetProgressBarFont(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            // Use ControlStyle-based font
            float fontSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = StyleTypography.GetFontStyle(controlStyle);
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            // Adjust for bar size
            fontSize = AdjustSizeForBarSize(fontSize, progressBar.BarSize);

            return BeepFontManager.GetFont(primaryFont, fontSize, fontStyle);
        }

        /// <summary>
        /// Gets a compact font for small progress bar controls
        /// Used when progress bar size is constrained
        /// </summary>
        public static Font GetCompactFont(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float compactSize = Math.Max(7f, baseSize - 2f); // Smaller for compact

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, compactSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets a bold font for emphasized progress bar text
        /// </summary>
        public static Font GetBoldFont(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Adjust for bar size
            baseSize = AdjustSizeForBarSize(baseSize, progressBar.BarSize);

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, FontStyle.Bold);
        }

        /// <summary>
        /// Adjusts font size based on ProgressBarSize
        /// Smaller bars need smaller fonts
        /// </summary>
        private static float AdjustSizeForBarSize(float baseSize, ProgressBarSize barSize)
        {
            return barSize switch
            {
                ProgressBarSize.Thin => Math.Max(6f, baseSize - 2f),      // Thin bars need smaller fonts
                ProgressBarSize.Small => Math.Max(7f, baseSize - 1.5f),   // Small bars need slightly smaller
                ProgressBarSize.Medium => baseSize,                       // Medium bars use standard size
                ProgressBarSize.Large => Math.Min(baseSize + 1f, 16f),    // Large bars can use slightly larger
                ProgressBarSize.ExtraLarge => Math.Min(baseSize + 1.5f, 18f), // Extra large can use larger
                _ => baseSize
            };
        }

        /// <summary>
        /// Gets font size for a specific progress bar element
        /// Returns size in points
        /// </summary>
        public static float GetFontSizeForElement(
            ProgressBarSize barSize,
            ProgressBarDisplayMode displayMode,
            BeepControlStyle controlStyle)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            // Adjust for display mode
            float elementSize = displayMode switch
            {
                ProgressBarDisplayMode.Percentage => baseSize - 1f,
                ProgressBarDisplayMode.CurrProgress => baseSize - 1f,
                ProgressBarDisplayMode.CustomText => baseSize,
                ProgressBarDisplayMode.TextAndPercentage => baseSize - 1f,
                ProgressBarDisplayMode.TextAndCurrProgress => baseSize - 1f,
                ProgressBarDisplayMode.TaskProgress => baseSize - 1f,
                ProgressBarDisplayMode.CenterPercentage => baseSize,
                ProgressBarDisplayMode.LoadingText => baseSize,
                ProgressBarDisplayMode.StepLabels => baseSize - 1.5f,
                ProgressBarDisplayMode.ValueOverMax => baseSize - 1f,
                _ => baseSize - 1f
            };

            // Adjust for bar size
            return AdjustSizeForBarSize(elementSize, barSize);
        }

        /// <summary>
        /// Gets font style for a specific progress bar element
        /// </summary>
        public static FontStyle GetFontStyleForElement(
            ProgressBarDisplayMode displayMode)
        {
            return displayMode switch
            {
                ProgressBarDisplayMode.CustomText => FontStyle.Regular,
                ProgressBarDisplayMode.LoadingText => FontStyle.Regular,
                ProgressBarDisplayMode.Percentage => FontStyle.Regular,
                ProgressBarDisplayMode.CenterPercentage => FontStyle.Bold, // Center percentage is often bold
                ProgressBarDisplayMode.TaskProgress => FontStyle.Regular,
                _ => FontStyle.Regular
            };
        }

        /// <summary>
        /// Applies font theme to progress bar control
        /// Updates the control's TextFont property based on ControlStyle and DisplayMode
        /// </summary>
        public static void ApplyFontTheme(
            BeepProgressBar progressBar,
            BeepControlStyle controlStyle)
        {
            if (progressBar == null)
                return;

            // Get appropriate font for the control based on visual mode
            Font newFont = GetProgressBarTextFont(
                progressBar, 
                progressBar.VisualMode, 
                controlStyle);

            // Update control font if different
            if (progressBar.TextFont != newFont && newFont != null)
            {
                // Dispose old font if it was created by us
                // Note: We should be careful not to dispose system fonts
                // For now, just assign the new font
                progressBar.TextFont = newFont;
            }
        }
    }
}

