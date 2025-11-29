using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Arc Linux shadow painter - Arc theme design
    /// Minimal, clean shadows for a modern flat appearance
    /// Subtle diffuse shadow with state awareness
    /// </summary>
    public static class ArcLinuxShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Arc uses minimal neutral shadows
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // State-based alpha - Arc has minimal interactive feedback
            int alpha = state switch
            {
                ControlState.Hovered => 45,    // Slightly more visible
                ControlState.Pressed => 25,    // Very subtle when pressed
                ControlState.Focused => 40,    // Moderate
                ControlState.Disabled => 12,   // Almost invisible
                _ => 30                        // Default - minimal
            };

            // Use clean single-layer drop shadow (Arc flat style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                2); // Minimal spread
        }
    }
}
