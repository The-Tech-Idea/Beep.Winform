using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Ant Design shadow painter
    /// Uses Ant Design's elevation shadow system
    /// </summary>
    public static class AntDesignShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Ant Design uses subtle shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.16f, 4);
        }
    }
}
