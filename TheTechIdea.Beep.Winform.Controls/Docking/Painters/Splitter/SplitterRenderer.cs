using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter
{
    /// <summary>
    /// Paints the docking splitter bar and its centred grip, reflecting hover/drag state from the
    /// supplied <see cref="DockingPainterContext"/>. Geometry/interaction (drag math, hit-test) stay
    /// in the splitter control / layout engine; this renderer only paints the resolved state.
    /// </summary>
    internal sealed class SplitterRenderer
    {
        private const int GripDotCount = 3;
        private const int GripDotSize = 2;
        private const int GripDotSpacing = 4;

        /// <summary>
        /// Paints the splitter into <c>ctx.Bounds</c>.
        /// </summary>
        /// <param name="g">Target graphics.</param>
        /// <param name="ctx">Render context (palette + hover/drag flags + bounds).</param>
        /// <param name="orientation">Vertical = bar between left/right panels; Horizontal = top/bottom.</param>
        public void Paint(Graphics g, DockingPainterContext ctx, SplitterOrientation orientation)
        {
            if (g == null || ctx == null)
                return;

            var colors = ctx.Colors;
            Rectangle bounds = ctx.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color barColor = (ctx.IsDragging || ctx.IsHover)
                ? colors.HoverBackColor
                : colors.SplitterBackColor;

            using (var brush = new SolidBrush(barColor))
                g.FillRectangle(brush, bounds);

            PaintGrip(g, bounds, orientation, colors.TabBorderColor);
        }

        private static void PaintGrip(Graphics g, Rectangle bounds, SplitterOrientation orientation, Color gripColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int span = (GripDotCount - 1) * GripDotSpacing;

            using var brush = new SolidBrush(gripColor);

            for (int i = 0; i < GripDotCount; i++)
            {
                int offset = i * GripDotSpacing - span / 2;
                int dx, dy;

                if (orientation == SplitterOrientation.Vertical)
                {
                    // Tall thin bar → stack dots vertically.
                    dx = cx - GripDotSize / 2;
                    dy = cy + offset - GripDotSize / 2;
                }
                else
                {
                    // Wide thin bar → lay dots horizontally.
                    dx = cx + offset - GripDotSize / 2;
                    dy = cy - GripDotSize / 2;
                }

                g.FillEllipse(brush, dx, dy, GripDotSize, GripDotSize);
            }
        }
    }
}
