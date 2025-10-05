using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Microsoft Fluent 2 shadow painter
    /// Uses modern soft shadows
    /// </summary>
    public static class Fluent2ShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // Fluent uses soft, modern shadows
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 4, Color.Black, 0.25f, 5);
        }
    }
}
