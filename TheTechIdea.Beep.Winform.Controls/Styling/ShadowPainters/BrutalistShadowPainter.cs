// File: BrutalistShadowPainter.cs
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Classic Brutalist shadow - solid black rectangle offset to bottom-right
    /// Creates the iconic "stacked paper" 3D effect with hard edges
    /// No blur, no gradients, just a clean solid shadow block
    /// </summary>
    public static class BrutalistShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            var bounds = path.GetBounds();

            // Turn off anti-aliasing → mandatory for crisp Brutalist hard edges
            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            try
            {
                // Shadow offset - the "depth" of the 3D stacked effect
                int offsetX = StyleShadows.GetShadowOffsetX(style); // Default: 6
                int offsetY = StyleShadows.GetShadowOffsetY(style); // Default: 6
                
                // Ensure minimum offset for visible effect
                if (offsetX < 4) offsetX = 4;
                if (offsetY < 4) offsetY = 4;

                // Shadow color from StyleShadows (solid black for Brutalist)
                Color shadowColor = StyleShadows.GetShadowColor(style);

                // Draw single solid shadow rectangle offset behind the control
                using (var brush = new SolidBrush(shadowColor))
                {
                    // The shadow is a simple rectangle, same size as control, offset down-right
                    g.FillRectangle(brush,
                        bounds.X + offsetX,
                        bounds.Y + offsetY,
                        bounds.Width,
                        bounds.Height);
                }
            }
            finally
            {
                g.SmoothingMode = prevMode;
            }

            return path;
        }
    }
}