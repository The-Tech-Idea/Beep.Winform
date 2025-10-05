using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Glass acrylic shadow painter
    /// Uses soft, diffused shadow for frosted glass effect
    /// </summary>
    public static class GlassAcrylicShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // Glass acrylic uses soft, diffused shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 6, Color.Black, 0.2f, 8);
        }
    }
}
