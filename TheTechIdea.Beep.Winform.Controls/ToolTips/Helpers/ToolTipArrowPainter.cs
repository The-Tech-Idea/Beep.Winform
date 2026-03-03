using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Sprint 1 — Dedicated arrow / caret painter for tooltips.
    /// Supports Sharp, Rounded and Hidden styles.
    /// All sizes are DPI-scaled automatically from the target control.
    /// </summary>
    public static class ToolTipArrowPainter
    {
        // ──────────────────────────────────────────────────────────────────────
        // Public entry point
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Draw an arrow caret on the tooltip.
        /// The arrow is painted at the midpoint of the chosen edge, shifted by <paramref name="arrowOffset"/>.
        /// </summary>
        /// <param name="g">Active Graphics context (SmoothingMode should be AntiAlias).</param>
        /// <param name="tooltipBounds">Full tooltip bounds (including padding – arrow is on the edge).</param>
        /// <param name="placement">Resolved placement (not Auto).</param>
        /// <param name="arrowStyle">Sharp, Rounded or Hidden.</param>
        /// <param name="arrowSize">Base size in logical pixels (DPI-scaling caller's responsibility).</param>
        /// <param name="arrowOffset">Signed pixel offset from center of the edge (+ve = toward End).</param>
        /// <param name="fillColor">Arrow fill color.</param>
        /// <param name="borderColor">Arrow border/outline color (may be transparent).</param>
        /// <param name="borderWidth">Border pen width in pixels.</param>
        public static void DrawArrow(
            Graphics g,
            Rectangle tooltipBounds,
            ToolTipPlacement placement,
            ToolTipArrowStyle arrowStyle,
            int arrowSize,
            int arrowOffset,
            Color fillColor,
            Color borderColor,
            float borderWidth = 1f)
        {
            if (arrowStyle == ToolTipArrowStyle.Hidden || arrowSize <= 0)
                return;

            var tip = CalculateTipPoint(tooltipBounds, placement, arrowSize, arrowOffset);
            var (left, right) = CalculateBasePoints(tip, placement, arrowSize);

            using (var path = BuildArrowPath(tip, left, right, arrowStyle, arrowSize))
            {
                // Fill
                using (var brush = new SolidBrush(fillColor))
                    g.FillPath(brush, path);

                // Border
                if (borderColor != Color.Transparent && borderWidth > 0)
                {
                    using (var pen = new Pen(borderColor, borderWidth) { LineJoin = LineJoin.Round })
                        g.DrawPath(pen, path);
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // DPI helper
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a DPI-scaled arrow size for the given control.
        /// Falls back to <paramref name="logicalSize"/> on non-DPI-aware hosts.
        /// </summary>
        public static int GetDpiScaledSize(Control ownerControl, int logicalSize = 8)
        {
            try
            {
                if (ownerControl == null) return logicalSize;
                using var g = ownerControl.CreateGraphics();
                float dpiScale = g.DpiX / 96f;
                return Math.Max(4, (int)Math.Round(logicalSize * dpiScale));
            }
            catch
            {
                return logicalSize;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Geometry helpers
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The sharp point (tip) of the arrow – outside the tooltip bounds.
        /// </summary>
        private static PointF CalculateTipPoint(
            Rectangle bounds, ToolTipPlacement placement, int arrowSize, int arrowOffset)
        {
            int halfBase = arrowSize;   // base half-width equals arrowSize

            switch (placement)
            {
                // Arrow points downward  (tooltip is above the target)
                case ToolTipPlacement.Top:
                    return new PointF(bounds.Left + bounds.Width / 2f + arrowOffset,
                                      bounds.Bottom + arrowSize);

                case ToolTipPlacement.TopStart:
                    return new PointF(bounds.Left + halfBase * 2 + arrowOffset,
                                      bounds.Bottom + arrowSize);

                case ToolTipPlacement.TopEnd:
                    return new PointF(bounds.Right - halfBase * 2 + arrowOffset,
                                      bounds.Bottom + arrowSize);

                // Arrow points upward  (tooltip is below the target)
                case ToolTipPlacement.Bottom:
                    return new PointF(bounds.Left + bounds.Width / 2f + arrowOffset,
                                      bounds.Top - arrowSize);

                case ToolTipPlacement.BottomStart:
                    return new PointF(bounds.Left + halfBase * 2 + arrowOffset,
                                      bounds.Top - arrowSize);

                case ToolTipPlacement.BottomEnd:
                    return new PointF(bounds.Right - halfBase * 2 + arrowOffset,
                                      bounds.Top - arrowSize);

                // Arrow points rightward  (tooltip is to the left of the target)
                case ToolTipPlacement.Left:
                    return new PointF(bounds.Right + arrowSize,
                                      bounds.Top + bounds.Height / 2f + arrowOffset);

                case ToolTipPlacement.LeftStart:
                    return new PointF(bounds.Right + arrowSize,
                                      bounds.Top + halfBase * 2 + arrowOffset);

                case ToolTipPlacement.LeftEnd:
                    return new PointF(bounds.Right + arrowSize,
                                      bounds.Bottom - halfBase * 2 + arrowOffset);

                // Arrow points leftward  (tooltip is to the right of the target)
                case ToolTipPlacement.Right:
                    return new PointF(bounds.Left - arrowSize,
                                      bounds.Top + bounds.Height / 2f + arrowOffset);

                case ToolTipPlacement.RightStart:
                    return new PointF(bounds.Left - arrowSize,
                                      bounds.Top + halfBase * 2 + arrowOffset);

                case ToolTipPlacement.RightEnd:
                    return new PointF(bounds.Left - arrowSize,
                                      bounds.Bottom - halfBase * 2 + arrowOffset);

                default:
                    return new PointF(bounds.Left + bounds.Width / 2f,
                                      bounds.Bottom + arrowSize);
            }
        }

        /// <summary>
        /// Compute the two base corners of the arrow triangle.
        /// </summary>
        private static (PointF left, PointF right) CalculateBasePoints(
            PointF tip, ToolTipPlacement placement, int arrowSize)
        {
            switch (placement)
            {
                case ToolTipPlacement.Top:
                case ToolTipPlacement.TopStart:
                case ToolTipPlacement.TopEnd:
                    // Base is on tooltip bottom edge — horizontal spread
                    return (new PointF(tip.X - arrowSize, tip.Y - arrowSize),
                            new PointF(tip.X + arrowSize, tip.Y - arrowSize));

                case ToolTipPlacement.Bottom:
                case ToolTipPlacement.BottomStart:
                case ToolTipPlacement.BottomEnd:
                    // Base is on tooltip top edge
                    return (new PointF(tip.X - arrowSize, tip.Y + arrowSize),
                            new PointF(tip.X + arrowSize, tip.Y + arrowSize));

                case ToolTipPlacement.Left:
                case ToolTipPlacement.LeftStart:
                case ToolTipPlacement.LeftEnd:
                    // Base is on tooltip right edge — vertical spread
                    return (new PointF(tip.X - arrowSize, tip.Y - arrowSize),
                            new PointF(tip.X - arrowSize, tip.Y + arrowSize));

                case ToolTipPlacement.Right:
                case ToolTipPlacement.RightStart:
                case ToolTipPlacement.RightEnd:
                    // Base is on tooltip left edge
                    return (new PointF(tip.X + arrowSize, tip.Y - arrowSize),
                            new PointF(tip.X + arrowSize, tip.Y + arrowSize));

                default:
                    return (new PointF(tip.X - arrowSize, tip.Y - arrowSize),
                            new PointF(tip.X + arrowSize, tip.Y - arrowSize));
            }
        }

        /// <summary>
        /// Build a <see cref="GraphicsPath"/> for the arrow in the requested style.
        /// </summary>
        private static GraphicsPath BuildArrowPath(
            PointF tip, PointF baseLeft, PointF baseRight,
            ToolTipArrowStyle style, int arrowSize)
        {
            var path = new GraphicsPath();

            if (style == ToolTipArrowStyle.Rounded)
            {
                // Approximate rounded triangle via Bezier curves
                // Control point toward the tip at 60 % of the height
                float cpFraction = 0.45f;
                PointF mid = new PointF(
                    (baseLeft.X + baseRight.X) / 2f,
                    (baseLeft.Y + baseRight.Y) / 2f);

                PointF cp1 = Lerp(baseLeft, tip, cpFraction);
                PointF cp2 = Lerp(tip, baseRight, 1f - cpFraction);

                path.AddBezier(baseLeft, cp1, tip, tip);         // left leg
                path.AddBezier(tip, tip, cp2, baseRight);        // right leg
                path.CloseFigure();
            }
            else
            {
                // Sharp flat triangle
                path.AddPolygon(new[] { tip, baseLeft, baseRight });
            }

            return path;
        }

        private static PointF Lerp(PointF a, PointF b, float t)
            => new PointF(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }
}
