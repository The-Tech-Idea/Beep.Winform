using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Discord style shadow painter
    /// Uses Discord's card shadow system
    /// </summary>
    public static class DiscordStyleShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Discord uses subtle shadow for depth
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.2f, 4);
        }
    }
}
