using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        /// Now uses ToolTipThemeHelpers for consistent theme color management
        /// </summary>
        public static (Color background, Color foreground, Color border) GetColors(
            ToolTipConfig config, 
            IBeepTheme theme)
        {
            // Use ToolTipThemeHelpers for centralized theme color management
            var useThemeColors = config.UseBeepThemeColors && theme != null;
            return ToolTipThemeHelpers.GetThemeColors(
                theme,
                config.Type,
                useThemeColors,
                config.BackColor,
                config.ForeColor,
                config.BorderColor);
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

    