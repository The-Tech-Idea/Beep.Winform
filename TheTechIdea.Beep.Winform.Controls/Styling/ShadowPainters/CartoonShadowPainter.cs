using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Cartoon shadow painter - Bold, playful offset shadow
    /// Larger offset than Retro, creates comic book/cartoon effect
    /// Fun and whimsical with hard edges
    /// </summary>
    public static class CartoonShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // Cartoon: Larger, bolder offset (comic book feel)
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            
            // Cartoon uses larger offset for playful effect
            if (offsetX < 4) offsetX = 5;
            if (offsetY < 4) offsetY = 6;

            Color shadowColor = StyleShadows.GetShadowColor(style);

            // State-based adjustments
            if (state == ControlState.Hovered)
            {
                // Hover: Larger shadow (bouncy effect)
                offsetX += 2;
                offsetY += 2;
            }
            else if (state == ControlState.Pressed)
            {
                // Pressed: Minimal shadow (pressed down)
                offsetX = 2;
                offsetY = 2;
            }

            // Disable anti-aliasing for sharp cartoon edges
            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            try
            {
                // Use hard offset shadow helper
                return ShadowPainterHelpers.PaintHardOffsetShadow(
                    g, path,
                    offsetX, offsetY,
                    shadowColor);
            }
            finally
            {
                g.SmoothingMode = prevMode;
            }
        }
    }
}
