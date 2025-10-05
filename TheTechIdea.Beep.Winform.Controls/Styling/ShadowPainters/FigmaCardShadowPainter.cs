using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Figma card shadow painter
    /// Uses Figma's design system shadow
    /// </summary>
    public static class FigmaCardShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Figma uses subtle card shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 3, Color.Black, 0.16f, 4);
        }
    }
}
