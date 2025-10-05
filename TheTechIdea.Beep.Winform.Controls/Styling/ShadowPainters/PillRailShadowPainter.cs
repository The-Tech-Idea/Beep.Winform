using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Pill rail shadow painter - no shadow (placeholder for consistency)
    /// Pill controls typically don't use shadows for a clean look
    /// </summary>
    public static class PillRailShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level0)
        {
            // Pill rail style has no shadows - intentionally empty
        }
    }
}
