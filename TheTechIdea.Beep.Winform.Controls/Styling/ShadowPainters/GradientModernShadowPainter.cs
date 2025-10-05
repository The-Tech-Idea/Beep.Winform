using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Gradient modern shadow painter
    /// Uses modern soft shadow for gradient backgrounds
    /// </summary>
    public static class GradientModernShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // Gradient modern uses soft, modern shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 5, Color.Black, 0.3f, 6);
        }
    }
}
