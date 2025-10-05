using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Stripe dashboard shadow painter
    /// Uses Stripe's professional shadow system
    /// </summary>
    public static class StripeDashboardShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // Stripe uses medium shadow for card elevation
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 4, Color.Black, 0.22f, 5);
        }
    }
}
