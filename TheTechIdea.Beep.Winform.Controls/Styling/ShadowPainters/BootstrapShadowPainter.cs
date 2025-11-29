using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Bootstrap shadow painter - Bootstrap 5 shadow utilities
    /// Clean card shadows with elevation levels
    /// shadow, shadow-sm, shadow-lg, shadow-none
    /// </summary>
    public static class BootstrapShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Bootstrap shadow color - neutral
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Bootstrap elevation levels (shadow-sm, shadow, shadow-lg)
            int baseAlpha = (int)elevation switch
            {
                0 => 0,     // shadow-none
                1 => 25,    // shadow-sm
                2 => 35,    // shadow (default)
                3 => 45,    // shadow-lg
                _ => 55     // shadow-xl (custom)
            };

            // State-based adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.3f),
                ControlState.Pressed => (int)(baseAlpha * 0.7f),
                ControlState.Focused => (int)(baseAlpha * 1.2f),
                ControlState.Disabled => (int)(baseAlpha * 0.4f),
                _ => baseAlpha
            };

            if (alpha == 0) return path;

            int spread = (int)elevation + 1;

            // Use clean drop shadow (Bootstrap card style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
