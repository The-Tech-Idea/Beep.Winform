using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Tokyo shadow painter - cyan night-city glow.
    /// </summary>
    public static class TokyoShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                StyleShadows.GetShadowColor(style),
                0.9f,
                StyleShadows.GetShadowBlur(style));
        }
    }
}
