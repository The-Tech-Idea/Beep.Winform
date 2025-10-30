using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Dracula shadow painter - purple tinted drop shadow.
    /// </summary>
    public static class DraculaShadowPainter
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
                0.35f);
        }
    }
}
