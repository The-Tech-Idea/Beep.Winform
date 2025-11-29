using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Nordic shadow painter - Similar to Nord with blue undertone
    /// Scandinavian design aesthetic with cool colors
    /// Subtle, clean elevation
    /// </summary>
    public static class NordicShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color shadowColor = StyleShadows.GetShadowColor(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            int alpha = state switch
            {
                ControlState.Hovered => 42,
                ControlState.Pressed => 20,
                ControlState.Focused => 38,
                ControlState.Disabled => 12,
                _ => 28
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 2);
        }
    }
}
