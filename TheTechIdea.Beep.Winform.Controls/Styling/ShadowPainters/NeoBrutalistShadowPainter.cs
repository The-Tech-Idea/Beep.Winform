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

            // Disable anti-aliasing for sharp edges
            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            try
            {
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

                    // Corner shadow (fills the gap)
                    g.FillRectangle(brush,
                        bounds.Right,
                        bounds.Bottom,
                        offsetX,
                        offsetY);
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
