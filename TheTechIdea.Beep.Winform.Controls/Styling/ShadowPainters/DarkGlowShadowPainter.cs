using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Dark glow shadow painter
    /// Uses colored glow effect instead of traditional shadow
    /// </summary>
    public static class DarkGlowShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level0)
        {
            // DarkGlow uses cyan glow instead of shadow
            Color glowColor = Color.FromArgb(0, 255, 255); // Cyan glow
            
            // Get glow color from theme if available
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor("Accent");
                if (themeColor != Color.Empty)
                    glowColor = themeColor;
            }

            ShadowPainterHelpers.PaintGlow(g, bounds, radius, glowColor, 0.8f);
        }
    }
}
