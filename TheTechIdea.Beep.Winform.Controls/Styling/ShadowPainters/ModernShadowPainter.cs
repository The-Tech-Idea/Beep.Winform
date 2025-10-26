using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Modern shadow painter - medium blur elevation.
    /// </summary>
    public static class ModernShadowPainter
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
                0.28f);
        }
    }
}
