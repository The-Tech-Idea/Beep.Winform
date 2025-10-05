using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Vercel clean shadow painter - no shadow (placeholder for consistency)
    /// Vercel style uses stark, flat design without shadows
    /// </summary>
    public static class VercelCleanShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level0)
        {
            // Vercel clean style has no shadows - intentionally empty
        }
    }
}
