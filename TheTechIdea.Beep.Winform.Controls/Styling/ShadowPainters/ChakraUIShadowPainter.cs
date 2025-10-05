using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Chakra UI shadow painter
    /// Uses Chakra UI's shadow tokens
    /// </summary>
    public static class ChakraUIShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Chakra UI uses subtle shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.18f, 4);
        }
    }
}
