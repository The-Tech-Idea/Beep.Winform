using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Solarized shadow painter - very subtle ambient shadow.
    /// </summary>
    public static class SolarizedShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            return ShadowPainterHelpers.PaintDropShadow(
                g, path, radius,
                StyleShadows.GetShadowOffsetX(style),
                StyleShadows.GetShadowOffsetY(style),
                StyleShadows.GetShadowBlur(style),
                StyleShadows.GetShadowColor(style),
                0.18f);
        }
    }
}
