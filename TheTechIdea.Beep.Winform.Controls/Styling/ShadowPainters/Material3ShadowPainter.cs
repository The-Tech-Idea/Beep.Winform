using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material Design 3 shadow painter
    /// Uses Material elevation levels for consistent shadows
    /// </summary>
    public static class Material3ShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level2)
        {
            // Material3 uses elevation-based shadows
            ShadowPainterHelpers.PaintMaterialShadow(g, bounds, radius, elevation);
        }
    }
}
