using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Notion minimal shadow painter - no shadow (placeholder for consistency)
    /// Notion style emphasizes clean, flat design without shadows
    /// </summary>
    public static class NotionMinimalShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level0)
        {
            // Notion minimal style has no shadows - intentionally empty
        }
    }
}
