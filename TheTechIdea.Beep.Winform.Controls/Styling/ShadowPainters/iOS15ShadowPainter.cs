using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// iOS 15 shadow painter
    /// Uses subtle shadows consistent with iOS design
    /// </summary>
    public static class iOS15ShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // iOS uses very subtle shadows
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.15f, 3);
        }
    }
}
