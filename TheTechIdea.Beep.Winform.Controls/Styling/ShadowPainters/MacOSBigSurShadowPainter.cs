using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// macOS Big Sur shadow painter
    /// Uses soft shadows with vibrancy
    /// </summary>
    public static class MacOSBigSurShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // macOS uses subtle, soft shadows
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 3, Color.Black, 0.2f, 4);
        }
    }
}
