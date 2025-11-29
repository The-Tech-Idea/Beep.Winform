using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Solarized shadow painter - Warm subtle ambient shadow
    /// Matches Solarized theme's warm color science
    /// Very subtle, almost ambient elevation
    /// </summary>
    public static class SolarizedShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Solarized uses warm-tinted subtle shadow
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Solarized: Very subtle shadow
            int alpha = state switch
            {
                ControlState.Hovered => 35,
                ControlState.Pressed => 18,
                ControlState.Focused => 32,
                ControlState.Disabled => 10,
                _ => 22
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 1);
        }
    }
}
