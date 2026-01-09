using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    /// <summary>
    /// Centralized helper for managing chip control theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps chip control states to theme colors
    /// </summary>
    public static class ChipThemeHelpers
    {
        /// <summary>
        /// Gets the background color for a chip based on state and variant
        /// </summary>
        public static Color GetChipBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            ChipVariant variant,
            ChipColor chipColor,
            bool isSelected = false,
            bool isHovered = false,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                // Handle different chip colors
                Color baseColor = GetChipColorForTheme(theme, chipColor);

                if (variant == ChipVariant.Filled)
                {
                    if (isSelected)
                        return baseColor;
                    if (isHovered)
                        return ControlPaint.Light(baseColor, 0.1f);
                    return ControlPaint.Light(baseColor, 0.2f);
                }
                else if (variant == ChipVariant.Outlined)
                {
                    if (isSelected)
                        return Color.FromArgb(20, baseColor);
                    if (isHovered)
                        return Color.FromArgb(10, baseColor);
                    return Color.Transparent;
                }
                else // Text variant
                {
                    if (isSelected)
                        return Color.FromArgb(28, baseColor);
                    if (isHovered)
                        return Color.FromArgb(14, baseColor);
                    return Color.Transparent;
                }
            }

            // Default colors
            if (variant == ChipVariant.Filled)
                return isSelected ? Color.FromArgb(0, 122, 255) : Color.FromArgb(230, 230, 230);
            return Color.Transparent;
        }

        /// <summary>
        /// Gets the foreground/text color for a chip
        /// </summary>
        public static Color GetChipForegroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            ChipVariant variant,
            ChipColor chipColor,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                Color baseColor = GetChipColorForTheme(theme, chipColor);

                if (variant == ChipVariant.Filled)
                {
                    // For filled chips, use white or dark text based on background brightness
                    return GetContrastColor(baseColor);
                }
                else // Outlined or Text
                {
                    if (isSelected)
                        return baseColor;
                    return theme.ForeColor != Color.Empty ? theme.ForeColor : Color.FromArgb(33, 33, 33);
                }
            }

            // Default colors
            if (variant == ChipVariant.Filled)
                return isSelected ? Color.White : Color.FromArgb(33, 33, 33);
            return Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// Gets the border color for a chip
        /// </summary>
        public static Color GetChipBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            ChipVariant variant,
            ChipColor chipColor,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                Color baseColor = GetChipColorForTheme(theme, chipColor);

                if (variant == ChipVariant.Outlined)
                {
                    if (isSelected)
                        return baseColor;
                    if (isHovered)
                        return ControlPaint.Light(baseColor, 0.2f);
                    return theme.BorderColor != Color.Empty ? theme.BorderColor : Color.FromArgb(200, 200, 200);
                }
                else if (variant == ChipVariant.Filled)
                {
                    // Filled chips typically don't show borders, but if they do, use base color
                    return isSelected ? baseColor : Color.Transparent;
                }
                else // Text variant
                {
                    return Color.Transparent;
                }
            }

            // Default colors
            if (variant == ChipVariant.Outlined)
                return isSelected ? Color.FromArgb(0, 122, 255) : Color.FromArgb(200, 200, 200);
            return Color.Transparent;
        }

        /// <summary>
        /// Gets the chip color based on ChipColor enum
        /// </summary>
        private static Color GetChipColorForTheme(IBeepTheme theme, ChipColor chipColor)
        {
            return chipColor switch
            {
                ChipColor.Primary => theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.FromArgb(0, 122, 255),
                ChipColor.Secondary => theme.SecondaryColor != Color.Empty ? theme.SecondaryColor : Color.FromArgb(128, 128, 128),
                ChipColor.Success => theme.SuccessColor != Color.Empty ? theme.SuccessColor : Color.FromArgb(40, 167, 69),
                ChipColor.Warning => theme.WarningColor != Color.Empty ? theme.WarningColor : Color.FromArgb(255, 193, 7),
                ChipColor.Error => theme.ErrorColor != Color.Empty ? theme.ErrorColor : Color.FromArgb(220, 53, 69),
                ChipColor.Info => theme.PrimaryColor != Color.Empty ? theme.PrimaryColor : Color.FromArgb(23, 162, 184),
                ChipColor.Dark => theme.SurfaceColor != Color.Empty ? theme.SurfaceColor : Color.FromArgb(52, 58, 64),
                _ => theme.ButtonBackColor != Color.Empty ? theme.ButtonBackColor : Color.FromArgb(230, 230, 230)
            };
        }

        /// <summary>
        /// Gets a contrasting color (white or black) based on background brightness
        /// </summary>
        private static Color GetContrastColor(Color backgroundColor)
        {
            // Calculate relative luminance
            double luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Gets the title color for chip group
        /// </summary>
        public static Color GetTitleColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.CardTitleForeColor != Color.Empty)
                    return theme.CardTitleForeColor;
                if (theme.LabelForeColor != Color.Empty)
                    return theme.LabelForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.Black;
        }

        /// <summary>
        /// Gets the group background color
        /// </summary>
        public static Color GetGroupBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                if (theme.ButtonBackColor != Color.Empty)
                    return theme.ButtonBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return SystemColors.Window;
        }

        /// <summary>
        /// Gets all theme colors for a chip in one call
        /// </summary>
        public static (Color background, Color foreground, Color border) GetChipColors(
            IBeepTheme theme,
            bool useThemeColors,
            ChipVariant variant,
            ChipColor chipColor,
            bool isSelected = false,
            bool isHovered = false)
        {
            return (
                GetChipBackgroundColor(theme, useThemeColors, variant, chipColor, isSelected, isHovered),
                GetChipForegroundColor(theme, useThemeColors, variant, chipColor, isSelected),
                GetChipBorderColor(theme, useThemeColors, variant, chipColor, isSelected, isHovered)
            );
        }
    }
}
