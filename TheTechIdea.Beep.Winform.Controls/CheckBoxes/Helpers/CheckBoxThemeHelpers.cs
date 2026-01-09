using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers
{
    /// <summary>
    /// Centralized helper for managing checkbox theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps checkbox states (checked, unchecked, indeterminate) to theme colors
    /// </summary>
    public static class CheckBoxThemeHelpers
    {
        /// <summary>
        /// Gets the background color for checkbox when checked
        /// Priority: Custom color > Theme CheckBoxCheckedBackColor > PrimaryColor > Default
        /// </summary>
        public static Color GetCheckedBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxCheckedBackColor != Color.Empty)
                    return theme.CheckBoxCheckedBackColor;
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.SuccessColor != Color.Empty)
                    return theme.SuccessColor;
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            return Color.FromArgb(33, 150, 243); // Material Blue
        }

        /// <summary>
        /// Gets the background color for checkbox when unchecked
        /// </summary>
        public static Color GetUncheckedBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxBackColor != Color.Empty)
                    return theme.CheckBoxBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return Color.Transparent;
        }

        /// <summary>
        /// Gets the background color for checkbox when indeterminate
        /// </summary>
        public static Color GetIndeterminateBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxBackColor != Color.Empty)
                    return theme.CheckBoxBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.Transparent;
        }

        /// <summary>
        /// Gets the border color for checkbox
        /// </summary>
        public static Color GetBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isChecked = false,
            bool isIndeterminate = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isChecked)
                {
                    if (theme.CheckBoxCheckedBackColor != Color.Empty)
                        return theme.CheckBoxCheckedBackColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isIndeterminate)
                {
                    if (theme.CheckBoxBorderColor != Color.Empty)
                        return theme.CheckBoxBorderColor;
                }
                if (theme.CheckBoxBorderColor != Color.Empty)
                    return theme.CheckBoxBorderColor;
                if (theme.BorderColor != Color.Empty)
                    return theme.BorderColor;
            }

            return isChecked ? Color.FromArgb(33, 150, 243) : Color.FromArgb(128, 128, 128);
        }

        /// <summary>
        /// Gets the foreground/text color for checkbox
        /// </summary>
        public static Color GetForegroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxForeColor != Color.Empty)
                    return theme.CheckBoxForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
                if (theme.LabelForeColor != Color.Empty)
                    return theme.LabelForeColor;
            }

            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the check mark color when checked
        /// </summary>
        public static Color GetCheckMarkColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxCheckedForeColor != Color.Empty)
                    return theme.CheckBoxCheckedForeColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Gets the indeterminate mark color
        /// </summary>
        public static Color GetIndeterminateMarkColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.CheckBoxForeColor != Color.Empty)
                    return theme.CheckBoxForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(128, 128, 128);
        }

        /// <summary>
        /// Gets all checkbox colors for a state in one call
        /// </summary>
        public static (Color background, Color border, Color checkMark, Color foreground) GetCheckBoxColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isChecked = false,
            bool isIndeterminate = false)
        {
            Color bg = isChecked
                ? GetCheckedBackgroundColor(theme, useThemeColors)
                : (isIndeterminate
                    ? GetIndeterminateBackgroundColor(theme, useThemeColors)
                    : GetUncheckedBackgroundColor(theme, useThemeColors));

            Color border = GetBorderColor(theme, useThemeColors, isChecked, isIndeterminate);
            Color checkMark = isChecked
                ? GetCheckMarkColor(theme, useThemeColors)
                : (isIndeterminate
                    ? GetIndeterminateMarkColor(theme, useThemeColors)
                    : Color.Transparent);
            Color fg = GetForegroundColor(theme, useThemeColors);

            return (bg, border, checkMark, fg);
        }
    }
}
