using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Vercel clean shadow painter - no shadow (placeholder for consistency)
    /// Vercel style uses stark, flat design without shadows
    /// </summary>
    public static class VercelCleanShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Vercel Clean UX: Flat design with very subtle state shadows
            if (!StyleShadows.HasShadow(style)) return;

            // Only show shadow on interaction states (Vercel's minimal depth)
            if (state == ControlState.Normal) return;

            float shadowOpacity = 0.05f; // Very subtle
            if (state == ControlState.Hovered)
                shadowOpacity = 0.08f; // Slightly more on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.06f; // Moderate on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.03f; // Minimal on press

            // Use StyleShadows for consistent Vercel shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 8);
        }
    }
}
