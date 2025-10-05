using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material You shadow painter
    /// Uses Material elevation with dynamic color adaptation
    /// </summary>
    public static class MaterialYouShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // MaterialYou uses same elevation system as Material3
            ShadowPainterHelpers.PaintMaterialShadow(g, bounds, radius, elevation);
        }
    }
}
