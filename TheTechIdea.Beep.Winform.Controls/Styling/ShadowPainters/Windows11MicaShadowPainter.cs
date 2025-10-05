using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Windows 11 Mica shadow painter
    /// Uses subtle mica-style shadows
    /// </summary>
    public static class Windows11MicaShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Mica uses very subtle shadows
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.12f, 3);
        }
    }
}
