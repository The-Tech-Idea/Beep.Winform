using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Centralized helper for managing tooltip theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Uses IBeepTheme properties: ToolTipBackColor, ToolTipForeColor, ToolTipBorderColor
    /// </summary>
    public static class ToolTipThemeHelpers
    {
        /// <summary>
        /// Gets the background color for a tooltip based on theme and type
        /// Priority: Custom color > Theme color > Semantic color > Default
        /// </summary>
        public static Color GetToolTipBackColor(
            IBeepTheme theme,
            ToolTipType type,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color (highest priority)
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors (from ApplyTheme)
            if (useThemeColors && theme != null)
            {
                // Use theme's tooltip-specific colors
                if (theme.ToolTipBackColor != Color.Empty)
                {
                    // For semantic types, blend with semantic colors
                    return type switch
                    {
                        ToolTipType.Success => BlendColors(theme.ToolTipBackColor, theme.SuccessColor, 0.3f),
                        ToolTipType.Warning => BlendColors(theme.ToolTipBackColor, theme.WarningColor, 0.3f),
                        ToolTipType.Error => BlendColors(theme.ToolTipBackColor, theme.ErrorColor, 0.3f),
                        ToolTipType.Info => BlendColors(theme.ToolTipBackColor, theme.AccentColor, 0.3f),
                        ToolTipType.Primary => BlendColors(theme.ToolTipBackColor, theme.PrimaryColor, 0.3f),
                        ToolTipType.Secondary => BlendColors(theme.ToolTipBackColor, theme.SecondaryColor, 0.3f),
                        ToolTipType.Accent => BlendColors(theme.ToolTipBackColor, theme.AccentColor, 0.3f),
                        _ => theme.ToolTipBackColor
                    };
                }
            }

            // Priority 3: Semantic colors from theme
            if (useThemeColors && theme != null)
            {
                return type switch
                {
                    ToolTipType.Success => theme.SuccessColor,
                    ToolTipType.Warning => theme.WarningColor,
                    ToolTipType.Error => theme.ErrorColor,
                    ToolTipType.Info => theme.AccentColor,
                    ToolTipType.Primary => theme.PrimaryColor,
                    ToolTipType.Secondary => theme.SecondaryColor,
                    ToolTipType.Accent => theme.AccentColor,
                    _ => theme.SurfaceColor
                };
            }

            // Priority 4: Default colors
            return type switch
            {
                ToolTipType.Success => Color.FromArgb(76, 175, 80),
                ToolTipType.Warning => Color.FromArgb(255, 152, 0),
                ToolTipType.Error => Color.FromArgb(244, 67, 54),
                ToolTipType.Info => Color.FromArgb(33, 150, 243),
                ToolTipType.Primary => Color.FromArgb(25, 118, 210),
                ToolTipType.Secondary => Color.FromArgb(156, 39, 176),
                ToolTipType.Accent => Color.FromArgb(255, 87, 34),
                _ => Color.FromArgb(45, 45, 48) // Default dark tooltip
            };
        }

        /// <summary>
        /// Gets the foreground/text color for a tooltip based on theme and type
        /// </summary>
        public static Color GetToolTipForeColor(
            IBeepTheme theme,
            ToolTipType type,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (theme.ToolTipForeColor != Color.Empty)
                {
                    return theme.ToolTipForeColor;
                }

                // For semantic types with colored backgrounds, use white text
                if (type == ToolTipType.Success || type == ToolTipType.Warning || 
                    type == ToolTipType.Error || type == ToolTipType.Info ||
                    type == ToolTipType.Primary || type == ToolTipType.Secondary ||
                    type == ToolTipType.Accent)
                {
                    return Color.White;
                }

                return theme.ForeColor;
            }

            // Priority 3: Default colors
            if (type == ToolTipType.Success || type == ToolTipType.Warning || 
                type == ToolTipType.Error || type == ToolTipType.Info ||
                type == ToolTipType.Primary || type == ToolTipType.Secondary ||
                type == ToolTipType.Accent)
            {
                return Color.White;
            }

            return Color.White; // Default light text
        }

        /// <summary>
        /// Gets the border color for a tooltip based on theme and type
        /// </summary>
        public static Color GetToolTipBorderColor(
            IBeepTheme theme,
            ToolTipType type,
            bool useThemeColors,
            Color? customColor = null)
        {
            // Priority 1: Custom color
            if (customColor.HasValue)
                return customColor.Value;

            // Priority 2: Theme colors
            if (useThemeColors && theme != null)
            {
                if (theme.ToolTipBorderColor != Color.Empty)
                {
                    // For semantic types, darken the semantic color
                    return type switch
                    {
                        ToolTipType.Success => DarkenColor(theme.SuccessColor, 0.2f),
                        ToolTipType.Warning => DarkenColor(theme.WarningColor, 0.2f),
                        ToolTipType.Error => DarkenColor(theme.ErrorColor, 0.2f),
                        ToolTipType.Info => DarkenColor(theme.AccentColor, 0.2f),
                        ToolTipType.Primary => DarkenColor(theme.PrimaryColor, 0.2f),
                        ToolTipType.Secondary => DarkenColor(theme.SecondaryColor, 0.2f),
                        ToolTipType.Accent => DarkenColor(theme.AccentColor, 0.2f),
                        _ => theme.ToolTipBorderColor
                    };
                }

                return theme.BorderColor;
            }

            // Priority 3: Default colors
            return Color.FromArgb(60, 60, 60); // Default dark border
        }

        /// <summary>
        /// Gets all theme colors for a tooltip in one call
        /// </summary>
        public static (Color backColor, Color foreColor, Color borderColor) GetThemeColors(
            IBeepTheme theme,
            ToolTipType type,
            bool useThemeColors,
            Color? customBackColor = null,
            Color? customForeColor = null,
            Color? customBorderColor = null)
        {
            return (
                GetToolTipBackColor(theme, type, useThemeColors, customBackColor),
                GetToolTipForeColor(theme, type, useThemeColors, customForeColor),
                GetToolTipBorderColor(theme, type, useThemeColors, customBorderColor)
            );
        }

        /// <summary>
        /// Applies theme colors to a ToolTipConfig
        /// Used by ApplyTheme() method
        /// </summary>
        public static void ApplyThemeColors(
            ToolTipConfig config,
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (config == null || theme == null) return;

            // Only apply theme colors if custom colors are not set
            if (!config.BackColor.HasValue)
            {
                config.BackColor = GetToolTipBackColor(theme, config.Type, useThemeColors);
            }

            if (!config.ForeColor.HasValue)
            {
                config.ForeColor = GetToolTipForeColor(theme, config.Type, useThemeColors);
            }

            if (!config.BorderColor.HasValue)
            {
                config.BorderColor = GetToolTipBorderColor(theme, config.Type, useThemeColors);
            }
        }

        /// <summary>
        /// Gets link color for tooltips
        /// </summary>
        public static Color GetToolTipLinkColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null && theme.ToolTipLinkColor != Color.Empty)
                return theme.ToolTipLinkColor;
            
            return Color.FromArgb(0, 120, 215); // Default link blue
        }

        /// <summary>
        /// Gets link hover color for tooltips
        /// </summary>
        public static Color GetToolTipLinkHoverColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null && theme.ToolTipLinkHoverColor != Color.Empty)
                return theme.ToolTipLinkHoverColor;
            
            return Color.FromArgb(0, 102, 204); // Darker blue for hover
        }

        /// <summary>
        /// Gets shadow color for tooltips
        /// </summary>
        public static Color GetToolTipShadowColor(IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null && theme.ToolTipShadowColor != Color.Empty)
                return theme.ToolTipShadowColor;
            
            return Color.FromArgb(96, 0, 0, 0); // Default semi-transparent black
        }

        #region Helper Methods

        /// <summary>
        /// Blends two colors together
        /// </summary>
        private static Color BlendColors(Color color1, Color color2, float ratio)
        {
            float r = color1.R * (1 - ratio) + color2.R * ratio;
            float g = color1.G * (1 - ratio) + color2.G * ratio;
            float b = color1.B * (1 - ratio) + color2.B * ratio;
            return Color.FromArgb(color1.A, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Darkens a color by a factor
        /// </summary>
        private static Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor)
            );
        }

        /// <summary>
        /// Lightens a color by a factor
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
    }
}

