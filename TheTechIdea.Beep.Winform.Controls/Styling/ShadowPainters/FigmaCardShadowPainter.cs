using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Figma card shadow painter
    /// Uses Figma's design system shadow
    /// </summary>
    public static class FigmaCardShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            return ShadowPainterHelpers.PaintCardShadow(g, path, radius, ShadowPainterHelpers.CardShadowStyle.Medium);
        }
    }
}
