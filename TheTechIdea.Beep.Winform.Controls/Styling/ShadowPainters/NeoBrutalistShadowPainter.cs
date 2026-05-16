using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// NeoBrutalist shadow painter - Bold 3D layered effect
    /// Hard black shadow on right and bottom edges (L-shaped)
    /// More aggressive than Brutalist - creates "stacked layers" effect
    /// </summary>
    public static class NeoBrutalistShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // NeoBrutalist: L-shaped shadow on right + bottom edges
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            
            // Ensure minimum offset for visible effect
            if (offsetX < 3) offsetX = 4;
            if (offsetY < 3) offsetY = 4;

            RectangleF bounds = path.GetBounds();
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Save full graphics state so ExcludeClip and SmoothingMode changes
            // don't leak into subsequent drawing (background, text, icons, etc.)
            var savedState = g.Save();
            try
            {
                // Disable anti-aliasing for sharp Neo-Brutalist hard edges
                g.SmoothingMode = SmoothingMode.None;

                // Exclude the control's own area so the shadow only appears
                // in the L-shaped strip that sticks out from behind the control.
                using (var interior = new Region(path))
                    g.ExcludeClip(interior);

                using (var brush = new SolidBrush(shadowColor))
                {
                    // Right edge shadow
                    g.FillRectangle(brush,
                        bounds.Right,
                        bounds.Y + offsetY,
                        offsetX,
                        bounds.Height);

                    // Bottom edge shadow
                    g.FillRectangle(brush,
                        bounds.X + offsetX,
                        bounds.Bottom,
                        bounds.Width,
                        offsetY);

                    // Corner shadow (fills the gap at bottom-right)
                    g.FillRectangle(brush,
                        bounds.Right,
                        bounds.Bottom,
                        offsetX,
                        offsetY);
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
