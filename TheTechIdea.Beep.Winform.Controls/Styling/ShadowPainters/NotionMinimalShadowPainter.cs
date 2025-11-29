using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Notion minimal shadow painter - Notion app design
    /// Clean, flat design with very subtle interaction shadows
    /// Focus on content, minimal chrome
    /// </summary>
    public static class NotionMinimalShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // Notion: No shadow in normal state (clean, flat)
            if (state == ControlState.Normal || state == ControlState.Disabled)
                return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Notion very subtle interaction shadows
            int alpha = state switch
            {
                ControlState.Hovered => 20,    // Very subtle hover
                ControlState.Pressed => 10,    // Minimal press
                ControlState.Focused => 18,    // Subtle focus
                ControlState.Selected => 25,   // Slightly more for selection
                _ => 0
            };

            if (alpha == 0) return path;

            // Use subtle shadow (Notion minimalism)
            return ShadowPainterHelpers.PaintSubtleShadow(
                g, path, radius,
                2, alpha);
        }
    }
}
