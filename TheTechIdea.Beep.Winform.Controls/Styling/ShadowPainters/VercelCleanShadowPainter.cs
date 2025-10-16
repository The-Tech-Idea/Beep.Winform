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
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Vercel Clean UX: Flat design with very subtle state shadows
            if (!StyleShadows.HasShadow(style)) return path;

            // Only show shadow on interaction states (Vercel's minimal depth)
            if (state == ControlState.Normal) return path;

            // Calculate shadow properties based on state
            float shadowOpacity = 0.05f; // Very subtle
            if (state == ControlState.Hovered)
                shadowOpacity = 0.08f; // Slightly more on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.06f; // Moderate on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.03f; // Minimal on press

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, 0, 0, StyleShadows.GetShadowColor(style), shadowOpacity, StyleShadows.GetShadowBlur(style) / 8);
            return remainingPath;
        }
    }
}
