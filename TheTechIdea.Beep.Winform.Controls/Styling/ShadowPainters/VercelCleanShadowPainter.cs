using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Vercel clean shadow painter - Vercel/Next.js design aesthetic
    /// Stark, minimal design with extremely subtle shadows
    /// Focus on typography and whitespace
    /// </summary>
    public static class VercelCleanShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // Vercel: No shadow in normal state (stark flat design)
            if (state == ControlState.Normal || state == ControlState.Disabled)
                return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Vercel extremely subtle interaction shadows
            int alpha = state switch
            {
                ControlState.Hovered => 18,    // Very subtle hover
                ControlState.Pressed => 8,     // Minimal press
                ControlState.Focused => 15,    // Subtle focus
                ControlState.Selected => 22,   // Slightly more for selection
                _ => 0
            };

            if (alpha == 0) return path;

            // Use subtle shadow (Vercel stark minimalism)
            return ShadowPainterHelpers.PaintSubtleShadow(
                g, path, radius,
                1, alpha);
        }
    }
}
