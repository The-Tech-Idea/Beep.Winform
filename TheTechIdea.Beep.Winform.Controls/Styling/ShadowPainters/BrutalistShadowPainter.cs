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

            // Shadow offset - the "depth" of the 3D stacked effect
            int offsetX = StyleShadows.GetShadowOffsetX(style); // Default: 6
            int offsetY = StyleShadows.GetShadowOffsetY(style); // Default: 6

            if (offsetX < 4) offsetX = 4;
            if (offsetY < 4) offsetY = 4;

            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Save full graphics state - ExcludeClip must be scoped or it leaks
            // into every subsequent draw call (background, text, icons, etc.)
            var savedState = g.Save();
            try
            {
                // Turn off anti-aliasing for crisp Brutalist hard edges
                g.SmoothingMode = SmoothingMode.None;

                // Exclude the control's own area so the shadow only shows
                // where it sticks out from behind the control (the offset strip).
                using (var interior = new Region(path))
                    g.ExcludeClip(interior);

                // Draw single solid shadow rectangle offset behind the control
                using (var brush = new SolidBrush(shadowColor))
                {
                    g.FillRectangle(brush,
                        bounds.X + offsetX,
                        bounds.Y + offsetY,
                        bounds.Width,
                        bounds.Height);
                }
            }
            finally
            {
                // Restores SmoothingMode AND the clip region in one call
                g.Restore(savedState);
            }

            return path;
        }
    }
}