using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Holographic shadow painter - prismatic glow.
    /// </summary>
    public static class HolographicShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            Color glow = StyleShadows.GetShadowColor(style);
            return ShadowPainterHelpers.PaintNeonGlow(g, path, radius, glow, 0.9f, StyleShadows.GetShadowBlur(style));
        }
    }
}
