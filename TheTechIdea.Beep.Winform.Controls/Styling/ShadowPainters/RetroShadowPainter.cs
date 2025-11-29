using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Retro shadow painter - Classic 90s UI offset shadow
    /// Hard-edged shadow like Windows 95/classic Mac
    /// Smaller offset than Brutalist, gives "raised button" feel
    /// </summary>
    public static class RetroShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // Retro: Smaller hard offset (classic UI feel)
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            
            // Retro uses smaller offset
            if (offsetX < 1) offsetX = 2;
            if (offsetY < 1) offsetY = 2;

            RectangleF bounds = path.GetBounds();
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // State-based (pressed inverts the shadow direction)
            if (state == ControlState.Pressed)
            {
                // Pressed: no shadow (appears pressed down)
                return path;
            }

            // Disable anti-aliasing for sharp retro edges
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
