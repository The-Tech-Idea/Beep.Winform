using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Metro2 shadow painter - Evolution of Metro with subtle depth
    /// Maintains flat aesthetic but adds minimal shadow for elevation hints
    /// Very subtle state-aware shadows
    /// </summary>
    public static class Metro2ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Metro2 - minimal shadows to maintain flat aesthetic
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Metro2 state-based - very subtle
            int alpha = state switch
            {
                ControlState.Hovered => 35,    // Slight increase
                ControlState.Pressed => 15,    // Very subtle
                ControlState.Focused => 30,    // Moderate
                ControlState.Selected => 40,   // Slightly more
                ControlState.Disabled => 8,    // Minimal
                _ => 25                        // Default subtle
            };

            // Use clean drop shadow (Metro2 refined flat)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                1); // Minimal spread
        }
    }
}
