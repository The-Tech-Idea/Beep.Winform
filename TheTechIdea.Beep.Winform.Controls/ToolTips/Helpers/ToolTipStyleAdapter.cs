using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper class to integrate ToolTips with BeepStyling system
    /// Provides Style-aware rendering using BeepControlStyle directly
    /// </summary>
    public static class ToolTipStyleAdapter
    {
        /// <summary>
        /// Get BeepControlStyle from config (it's already BeepControlStyle)
        /// </summary>
        public static BeepControlStyle GetBeepControlStyle(ToolTipConfig config)
        {
            return config.Style;
        }

        /// <summary>
        /// Get colors for tooltip based on ToolTipType and BeepControlStyle
        /// </summary>
        public static (Color background, Color foreground, Color border) GetColors(
            ToolTipConfig config, 
            IBeepTheme theme)
        {
            var beepStyle = config.Style;

            // If custom colors specified, use them
            if (config.BackColor.HasValue && config.ForeColor.HasValue && config.BorderColor.HasValue)
            {
                return (config.BackColor.Value, config.ForeColor.Value, config.BorderColor.Value);
            }

            // Use BeepStyling color system
            Color background, foreground, border;

            if (config.UseBeepThemeColors && theme != null)
            {
                // Get semantic colors based on ToolTipType
                background = GetSemanticBackgroundColor(config.Type, theme, beepStyle);
                foreground = GetSemanticForegroundColor(config.Type, theme);
                border = GetSemanticBorderColor(config.Type, theme, beepStyle);
            }
            else
            {
                // Use Style-specific colors
                background = config.BackColor ?? StyleColors.GetBackground(beepStyle);
                foreground = config.ForeColor ?? StyleColors.GetForeground(beepStyle);
                border = config.BorderColor ?? StyleColors.GetBorder(beepStyle);
            }

            return (background, foreground, border);
        }

        /// <summary>
        /// Get background color based on ToolTipType
        /// </summary>
        private static Color GetSemanticBackgroundColor(ToolTipType type, IBeepTheme theme, BeepControlStyle style)
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
                _ => StyleColors.GetBackground(style)
            };
        }

        /// <summary>
        /// Get foreground color based on ToolTipType
        /// </summary>
        private static Color GetSemanticForegroundColor(ToolTipType type, IBeepTheme theme)
        {
            return type switch
            {
                ToolTipType.Success or ToolTipType.Warning or ToolTipType.Error or 
                ToolTipType.Info or ToolTipType.Primary => Color.White,
                _ => theme.ForeColor
            };
        }

        /// <summary>
        /// Get border color based on ToolTipType
        /// </summary>
        private static Color GetSemanticBorderColor(ToolTipType type, IBeepTheme theme, BeepControlStyle style)
        {
            return type switch
            {
                ToolTipType.Success => ControlPaint.Dark(theme.SuccessColor, 0.2f),
                ToolTipType.Warning => ControlPaint.Dark(theme.WarningColor, 0.2f),
                ToolTipType.Error => ControlPaint.Dark(theme.ErrorColor, 0.2f),
                ToolTipType.Info => ControlPaint.Dark(theme.AccentColor, 0.2f),
                ToolTipType.Primary => ControlPaint.Dark(theme.PrimaryColor, 0.2f),
                ToolTipType.Secondary => ControlPaint.Dark(theme.SecondaryColor, 0.2f),
                ToolTipType.Accent => ControlPaint.Dark(theme.AccentColor, 0.2f),
                _ => StyleColors.GetBorder(style)
            };
        }

        /// <summary>
        /// Get corner radius for tooltip Style
        /// </summary>
        public static int GetCornerRadius(BeepControlStyle style)
        {
            return StyleBorders.GetRadius(style);
        }

        /// <summary>
        /// Check if Style uses shadows
        /// </summary>
        public static bool HasShadow(BeepControlStyle style)
        {
            return style != BeepControlStyle.Minimal && 
                   style != BeepControlStyle.NotionMinimal &&
                   style != BeepControlStyle.VercelClean;
        }

        /// <summary>
        /// Get padding for tooltip Style
        /// </summary>
        public static int GetPadding(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 or BeepControlStyle.MaterialYou => 16,
                BeepControlStyle.iOS15 or BeepControlStyle.MacOSBigSur => 12,
                BeepControlStyle.Fluent2 or BeepControlStyle.Windows11Mica => 14,
                BeepControlStyle.Minimal or BeepControlStyle.NotionMinimal => 10,
                BeepControlStyle.GradientModern or BeepControlStyle.GlassAcrylic => 16,
                _ => 12
            };
        }

        /// <summary>
        /// Apply BeepStyling background to tooltip bounds
        /// </summary>
        public static void PaintStyledBackground(Graphics g, Rectangle bounds, ToolTipConfig config)
        {
            var beepStyle = GetBeepControlStyle(config);
            
            // Temporarily set BeepStyling settings
            var originalStyle = BeepStyling.CurrentControlStyle;
            var originalTheme = BeepStyling.CurrentTheme;
            var originalUseTheme = BeepStyling.UseThemeColors;

            try
            {
                BeepStyling.CurrentControlStyle = beepStyle;
                BeepStyling.UseThemeColors = config.UseBeepThemeColors;

                // Use BeepStyling to paint background
                BeepStyling.PaintStyleBackground(g, bounds, beepStyle);
            }
            finally
            {
                // Restore original settings
                BeepStyling.CurrentControlStyle = originalStyle;
                BeepStyling.CurrentTheme = originalTheme;
                BeepStyling.UseThemeColors = originalUseTheme;
            }
        }
    }
}

    