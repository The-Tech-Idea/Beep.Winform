using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Cartoon background painter - playful, animated style
    /// Warm cream background with cel-shading inspired highlight
    /// Fun, friendly aesthetic with subtle outline hint
    /// </summary>
    public static class CartoonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Cartoon: warm cream/peach background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xFF, 0xF5, 0xE6);
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0xFF, 0x99, 0x33);

            // Solid background with strong state changes (cartoon-style feedback)
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Cel-shading inspired top highlight
                float highlightHeight = bounds.Height / 4f;
                if (highlightHeight > 2)
                {
                    var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, highlightHeight);
                    var topGrad = PaintersFactory.GetLinearGradientBrush(
                        topRect,
                        Color.FromArgb(35, Color.White),
                        Color.Transparent,
                        LinearGradientMode.Vertical);
                    g.FillRectangle(topGrad, topRect);
                }

                if (BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style))
                {
                    // Cartoon outline hint (characteristic of cel-shading)
                    var pen = PaintersFactory.GetPen(Color.FromArgb(25, accent), 1f);
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}
