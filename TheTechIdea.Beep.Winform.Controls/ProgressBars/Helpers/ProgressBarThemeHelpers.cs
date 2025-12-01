using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// Centralized helper for managing progress bar theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps progress bar elements to theme colors with fallbacks
    /// </summary>
    public static class ProgressBarThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the progress bar
        /// Priority: Custom color > Theme ProgressBarBackColor > Theme SurfaceColor > Default light gray
        /// </summary>
        public static Color GetProgressBarBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color (highest priority)
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarBackColor != Color.Empty)
                    return theme.ProgressBarBackColor;

                // Fallback to SurfaceColor
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;

                // Fallback to PanelBackColor
                if (theme.PanelBackColor != Color.Empty)
                    return theme.PanelBackColor;

                // Fallback to BackColor
                if (theme.BackColor != Color.Empty)
                    return theme.BackColor;
            }

            // Priority 3: Default light gray
            return Color.FromArgb(240, 240, 240);
        }

        /// <summary>
        /// Gets the foreground (progress fill) color for the progress bar
        /// Priority: Custom color > Theme ProgressBarForeColor > Theme PrimaryColor > Default blue
        /// </summary>
        public static Color GetProgressBarForeColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarForeColor != Color.Empty)
                    return theme.ProgressBarForeColor;

                // Fallback to PrimaryColor
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;

                // Fallback to AccentColor
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            // Default blue color
            return Color.FromArgb(52, 152, 219);
        }

        /// <summary>
        /// Gets the text color for text displayed inside the progress bar
        /// Priority: Custom color > Theme ProgressBarInsideTextColor > Theme PrimaryTextColor > Default white
        /// </summary>
        public static Color GetProgressBarTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarInsideTextColor != Color.Empty)
                    return theme.ProgressBarInsideTextColor;

                // Fallback to PrimaryTextColor
                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                // Fallback to ForeColor
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;

                // Fallback to ButtonForeColor
                if (theme.ButtonForeColor != Color.Empty)
                    return theme.ButtonForeColor;
            }

            // Default white for text on colored background
            return Color.White;
        }

        /// <summary>
        /// Gets the border color for the progress bar
        /// Priority: Custom color > Theme ProgressBarBorderColor > Theme BorderColor > Default gray
        /// </summary>
        public static Color GetProgressBarBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarBorderColor != Color.Empty)
                    return theme.ProgressBarBorderColor;

                // Fallback to BorderColor
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            // Default gray with low opacity
            return Color.FromArgb(30, 0, 0, 0);
        }

        /// <summary>
        /// Gets the success color for progress bar (used in auto-color mode)
        /// Priority: Custom color > Theme ProgressBarSuccessColor > Theme SuccessColor > Default green
        /// </summary>
        public static Color GetProgressBarSuccessColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarSuccessColor != Color.Empty)
                    return theme.ProgressBarSuccessColor;

                // Fallback to SuccessColor
                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;
            }

            // Default green
            return Color.FromArgb(34, 197, 94);
        }

        /// <summary>
        /// Gets the warning color for progress bar (used in auto-color mode)
        /// Priority: Custom color > Theme ProgressBarWarningColor > Theme WarningColor > Default orange
        /// </summary>
        public static Color GetProgressBarWarningColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Check for ProgressBarWarningColor (if it exists in theme)
                var warningProp = theme.GetType().GetProperty("ProgressBarWarningColor");
                if (warningProp != null)
                {
                    var warningValue = warningProp.GetValue(theme);
                    if (warningValue is Color warningColor && warningColor != Color.Empty)
                        return warningColor;
                }

                // Fallback to WarningColor
                if (theme.WarningColor != Color.Empty)
                    return theme.WarningColor;
            }

            // Default orange
            return Color.FromArgb(245, 158, 11);
        }

        /// <summary>
        /// Gets the error color for progress bar (used in auto-color mode)
        /// Priority: Custom color > Theme ProgressBarErrorColor > Theme ErrorColor > Default red
        /// </summary>
        public static Color GetProgressBarErrorColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarErrorColor != Color.Empty)
                    return theme.ProgressBarErrorColor;

                // Fallback to ErrorColor
                if (theme.ErrorColor != Color.Empty)
                    return theme.ErrorColor;
            }

            // Default red
            return Color.FromArgb(239, 68, 68);
        }

        /// <summary>
        /// Gets the secondary progress color (for overlay/secondary progress)
        /// Priority: Custom color > Theme SecondaryColor with opacity > Default gray with opacity
        /// </summary>
        public static Color GetProgressBarSecondaryColor(
            IBeepTheme theme,
            bool useThemeColors,
            int opacity = 50,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return Color.FromArgb(opacity, customColor.Value);

            if (useThemeColors && theme != null)
            {
                Color baseColor = theme.SecondaryColor != Color.Empty 
                    ? theme.SecondaryColor 
                    : Color.Gray;
                return Color.FromArgb(opacity, baseColor.R, baseColor.G, baseColor.B);
            }

            // Default gray with opacity
            return Color.FromArgb(opacity, 100, 100, 100);
        }

        /// <summary>
        /// Gets the hover background color for progress bar interactive elements
        /// Priority: Custom color > Theme ProgressBarHoverBackColor > Default light overlay
        /// </summary>
        public static Color GetProgressBarHoverBackColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarHoverBackColor != Color.Empty)
                    return Color.FromArgb(60, theme.ProgressBarHoverBackColor);

                // Fallback to PrimaryColor with opacity
                if (theme.PrimaryColor != Color.Empty)
                    return Color.FromArgb(40, theme.PrimaryColor);
            }

            // Default light overlay
            return Color.FromArgb(40, Color.Black);
        }

        /// <summary>
        /// Gets the hover foreground color for progress bar interactive elements
        /// Priority: Custom color > Theme ProgressBarHoverForeColor > Default dark overlay
        /// </summary>
        public static Color GetProgressBarHoverForeColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarHoverForeColor != Color.Empty)
                    return Color.FromArgb(80, theme.ProgressBarHoverForeColor);

                // Fallback to PrimaryColor with higher opacity
                if (theme.PrimaryColor != Color.Empty)
                    return Color.FromArgb(60, theme.PrimaryColor);
            }

            // Default dark overlay
            return Color.FromArgb(60, Color.Black);
        }

        /// <summary>
        /// Gets the hover border color for progress bar interactive elements
        /// Priority: Custom color > Theme ProgressBarHoverBorderColor > Default border
        /// </summary>
        public static Color GetProgressBarHoverBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.ProgressBarHoverBorderColor != Color.Empty)
                    return theme.ProgressBarHoverBorderColor;

                // Fallback to BorderColor
                if (theme.BorderColor != Color.Empty)
                    return Color.FromArgb(120, theme.BorderColor);
            }

            // Default border with opacity
            return Color.FromArgb(120, Color.Black);
        }

        /// <summary>
        /// Gets all relevant theme colors for a progress bar in one go
        /// </summary>
        public static (
            Color backColor,
            Color foreColor,
            Color textColor,
            Color borderColor,
            Color successColor,
            Color warningColor,
            Color errorColor,
            Color secondaryColor,
            Color hoverBackColor,
            Color hoverForeColor,
            Color hoverBorderColor
        ) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customForeColor = null,
            Color? customTextColor = null,
            Color? customBorderColor = null,
            Color? customSuccessColor = null,
            Color? customWarningColor = null,
            Color? customErrorColor = null,
            int secondaryOpacity = 50)
        {
            Color back = GetProgressBarBackColor(theme, useThemeColors, customBackColor);
            Color fore = GetProgressBarForeColor(theme, useThemeColors, customForeColor);
            Color text = GetProgressBarTextColor(theme, useThemeColors, customTextColor);
            Color border = GetProgressBarBorderColor(theme, useThemeColors, customBorderColor);
            Color success = GetProgressBarSuccessColor(theme, useThemeColors, customSuccessColor);
            Color warning = GetProgressBarWarningColor(theme, useThemeColors, customWarningColor);
            Color error = GetProgressBarErrorColor(theme, useThemeColors, customErrorColor);
            Color secondary = GetProgressBarSecondaryColor(theme, useThemeColors, secondaryOpacity);
            Color hoverBack = GetProgressBarHoverBackColor(theme, useThemeColors);
            Color hoverFore = GetProgressBarHoverForeColor(theme, useThemeColors);
            Color hoverBorder = GetProgressBarHoverBorderColor(theme, useThemeColors);

            return (back, fore, text, border, success, warning, error, secondary, hoverBack, hoverFore, hoverBorder);
        }

        /// <summary>
        /// Applies theme colors to the BeepProgressBar control properties
        /// </summary>
        public static void ApplyThemeColors(
            BeepProgressBar progressBar,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (progressBar == null || theme == null || !useThemeColors) return;

            // Apply background color
            progressBar.BackColor = GetProgressBarBackColor(theme, useThemeColors, progressBar.BackColor);

            // Apply text color
            progressBar.TextColor = GetProgressBarTextColor(theme, useThemeColors, progressBar.TextColor);

            // Apply progress color
            progressBar.ProgressColor = GetProgressBarForeColor(theme, useThemeColors, progressBar.ProgressColor);

            // Apply border color (via border pen - handled in ApplyTheme)
            // Note: Border color is applied through the _borderPen in BeepProgressBar.ApplyTheme()

            // Apply secondary progress color
            progressBar.SecondaryProgressColor = GetProgressBarSecondaryColor(theme, useThemeColors, 50);

            // Apply success/warning/error colors for auto-color mode
            progressBar.SuccessColor = GetProgressBarSuccessColor(theme, useThemeColors, progressBar.SuccessColor);
            progressBar.WarningColor = GetProgressBarWarningColor(theme, useThemeColors, progressBar.WarningColor);
            progressBar.ErrorColor = GetProgressBarErrorColor(theme, useThemeColors, progressBar.ErrorColor);
        }
    }
}

