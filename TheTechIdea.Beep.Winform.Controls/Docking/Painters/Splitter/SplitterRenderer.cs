using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter
{
    /// <summary>
    /// Paints the docking splitter bar and its centred grip, reflecting hover/drag state from the
    /// supplied <see cref="DockingPainterContext"/>. Geometry/interaction (drag math, hit-test) stay
    /// in the splitter control / layout engine; this renderer only paints the resolved state.
    /// </summary>
    internal sealed class SplitterRenderer : System.IDisposable
    {
        private const int GripDotCount = 3;
        private readonly PaintResourceCache _cache = new PaintResourceCache();

        public void Dispose() => _cache.Dispose();

        /// <summary>
        /// Paints the splitter into <c>ctx.Bounds</c>. The <see cref="DockingStyleFlavor"/>
        /// from <paramref name="ctx"/> controls the grip dot size, spacing, and whether the
        /// bar gets a translucent overlay (macOS).
        /// </summary>
        /// <param name="g">Target graphics.</param>
        /// <param name="ctx">Render context (palette + hover/drag flags + bounds).</param>
        /// <param name="orientation">Vertical = bar between left/right panels; Horizontal = top/bottom.</param>
        public void Paint(Graphics g, DockingPainterContext ctx, SplitterOrientation orientation)
        {
            if (g == null || ctx == null)
                return;

            var colors = ctx.Colors;
            var flavor = ctx.Flavor;
            Rectangle bounds = ctx.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color barColor = (ctx.IsDragging || ctx.IsHover)
                ? colors.HoverBackColor
                : colors.SplitterBackColor;

            using (var brush = _cache.GetBrush(barColor))
                g.FillRectangle(brush, bounds);

            if (flavor.UseTranslucentSplitter)
            {
                using var overlay = _cache.GetBrush(Color.FromArgb(48, ControlPaint.Light(barColor, 0.10f)));
                g.FillRectangle(overlay, bounds);
            }

            PaintGrip(g, bounds, orientation, colors.TabBorderColor, flavor.SplitterGripSize, flavor.SplitterGripSpacing);
        }

        private void PaintGrip(Graphics g, Rectangle bounds, SplitterOrientation orientation,
            Color gripColor, int gripSize, int gripSpacing)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int span = (GripDotCount - 1) * gripSpacing;

            using var brush = _cache.GetBrush(gripColor);

            for (int i = 0; i < GripDotCount; i++)
            {
                int offset = i * gripSpacing - span / 2;
                int dx, dy;

                if (orientation == SplitterOrientation.Vertical)
                {
                    // Tall thin bar → stack dots vertically.
                    dx = cx - gripSize / 2;
                    dy = cy + offset - gripSize / 2;
                }
                else
                {
                    // Wide thin bar → lay dots horizontally.
                    dx = cx + offset - gripSize / 2;
                    dy = cy - gripSize / 2;
                }

                g.FillEllipse(brush, dx, dy, gripSize, gripSize);
            }
        }
    }
}
