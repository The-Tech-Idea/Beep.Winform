using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Centralized helper for managing vertical table theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps table states (header, cell, hover, selected) to theme colors
    /// </summary>
    public static class VerticalTableThemeHelpers
    {
        /// <summary>
        /// Gets the background color for the table
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetTableBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Gets the header background color
        /// Priority: Custom color > Theme Primary/Accent > Default
        /// </summary>
        public static Color GetHeaderBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isFeatured = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isFeatured || isSelected)
                {
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                else
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.1f);
                    if (theme.SecondaryColor != Color.Empty)
                        return ControlPaint.Light(theme.SecondaryColor, 0.2f);
                }
            }

            return isFeatured || isSelected
                ? Color.FromArgb(52, 168, 83) // Green accent
                : Color.FromArgb(245, 247, 250); // Light gray
        }

        /// <summary>
        /// Gets the cell background color
        /// Priority: Custom color > Theme Surface > Default
        /// </summary>
        public static Color GetCellBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false,
            bool isAlternate = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.AccentColor != Color.Empty)
                        return ControlPaint.Light(theme.AccentColor, 0.9f);
                }
                else if (isHovered)
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.05f);
                }
                else if (isAlternate)
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.02f);
                }
                else
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                    if (theme.BackgroundColor != Color.Empty)
                        return theme.BackgroundColor;
                }
            }

            if (isSelected)
                return Color.FromArgb(240, 248, 255); // Light blue
            if (isHovered)
                return Color.FromArgb(250, 250, 250); // Very light gray
            if (isAlternate)
                return Color.FromArgb(248, 249, 250); // Slightly darker
            return Color.White;
        }

        /// <summary>
        /// Gets the border color
        /// Priority: Custom color > Theme Border > Default
        /// </summary>
        public static Color GetBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isFeatured = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isFeatured || isSelected)
                {
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                else
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
            }

            return isFeatured || isSelected
                ? Color.FromArgb(52, 168, 83) // Green accent
                : Color.FromArgb(220, 225, 230); // Light gray
        }

        /// <summary>
        /// Gets the text color for headers
        /// </summary>
        public static Color GetHeaderTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isFeatured = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isFeatured || isSelected)
                {
                    // White text on colored background
                    Color headerBg = GetHeaderBackgroundColor(theme, useThemeColors, isSelected, isFeatured);
                    float luminance = GetLuminance(headerBg);
                    return luminance > 0.5f ? Color.Black : Color.White;
                }
                else
                {
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isFeatured || isSelected
                ? Color.White
                : Color.FromArgb(33, 37, 41); // Dark gray
        }

        /// <summary>
        /// Gets the text color for cells
        /// </summary>
        public static Color GetCellTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return isSelected
                ? Color.FromArgb(33, 37, 41) // Dark gray
                : Color.FromArgb(73, 80, 87); // Medium gray
        }

        /// <summary>
        /// Gets the shadow color for elevation effects
        /// </summary>
        public static Color GetShadowColor(
            IBeepTheme theme,
            bool useThemeColors,
            int elevation = 4)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ShadowColor != Color.Empty)
                    return Color.FromArgb(Math.Min(255, elevation * 10), theme.ShadowColor);
            }

            return Color.FromArgb(Math.Min(255, elevation * 10), Color.Black);
        }

        /// <summary>
        /// Gets all theme colors for a vertical table in one call
        /// </summary>
        public static (Color tableBg, Color headerBg, Color cellBg, Color borderColor, Color headerText, Color cellText, Color shadow) GetThemeColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHeaderSelected = false,
            bool isHeaderFeatured = false,
            bool isCellHovered = false,
            bool isCellSelected = false,
            bool isCellAlternate = false)
        {
            return (
                GetTableBackgroundColor(theme, useThemeColors),
                GetHeaderBackgroundColor(theme, useThemeColors, isHeaderSelected, isHeaderFeatured),
                GetCellBackgroundColor(theme, useThemeColors, isCellHovered, isCellSelected, isCellAlternate),
                GetBorderColor(theme, useThemeColors, isHeaderSelected, isHeaderFeatured),
                GetHeaderTextColor(theme, useThemeColors, isHeaderSelected, isHeaderFeatured),
                GetCellTextColor(theme, useThemeColors, isCellSelected),
                GetShadowColor(theme, useThemeColors, 4)
            );
        }

        #region Helper Methods

        /// <summary>
        /// Calculate relative luminance of a color (for contrast checking)
        /// Returns value between 0 (dark) and 1 (light)
        /// </summary>
        private static float GetLuminance(Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            r = r <= 0.03928f ? r / 12.92f : (float)Math.Pow((r + 0.055f) / 1.055f, 2.4f);
            g = g <= 0.03928f ? g / 12.92f : (float)Math.Pow((g + 0.055f) / 1.055f, 2.4f);
            b = b <= 0.03928f ? b / 12.92f : (float)Math.Pow((b + 0.055f) / 1.055f, 2.4f);

            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }

        #endregion
    }
}
