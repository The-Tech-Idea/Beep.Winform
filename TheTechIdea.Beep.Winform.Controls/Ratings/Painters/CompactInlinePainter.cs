using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Compact read-only inline star rating — very small, suitable for list rows.
    /// Renders filled and half-filled stars at reduced size (≤ 16 px).
    /// </summary>
    public class CompactInlinePainter : RatingPainterBase
    {
        public override string Name => "CompactInline";

        public override void Paint(RatingPainterContext ctx)
        {
            SetupGraphics(ctx.Graphics);
            var g = ctx.Graphics;

            int count   = ctx.StarCount;
            int size    = Math.Min(ctx.StarSize, 16);   // cap at 16 px
            int spacing = Math.Max(1, ctx.Spacing / 3);
            var bounds  = ctx.Bounds;

            int totalW  = count * size + (count - 1) * spacing;
            int startX  = bounds.Left;
            int centerY = bounds.Top + bounds.Height / 2;
            int topY    = centerY - size / 2;

            float precise = ctx.PreciseRating;

            for (int i = 0; i < count; i++)
            {
                float x  = startX + i * (size + spacing);
                float cx = x + size / 2f;
                float cy = topY + size / 2f;

                float filled  = Math.Max(0f, Math.Min(1f, precise - i));  // 0, 0.5, or 1

                // Draw empty star outline
                using var fullPath = StarPath(cx, cy, size / 2f, size / 4f);
                using (var emptyBrush = new SolidBrush(ctx.EmptyStarColor))
                    g.FillPath(emptyBrush, fullPath);

                // Clip-fill the star proportionally (left-fill)
                if (filled > 0f)
                {
                    float clipW = size * filled;
                    var  clip   = new RectangleF(x, topY, clipW, size);
                    var  oldClip = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);

                    Color fill = ctx.GetGradedColor(i + 1);
                    using var fillBrush = new SolidBrush(fill);
                    using var filledPath = StarPath(cx, cy, size / 2f, size / 4f);
                    g.FillPath(fillBrush, filledPath);
                    g.Clip = oldClip;
                }
            }
        }

        public override Size CalculateSize(RatingPainterContext ctx)
        {
            int size    = Math.Min(ctx.StarSize, 16);
            int spacing = Math.Max(1, ctx.Spacing / 3);
            int w = ctx.StarCount * size + (ctx.StarCount - 1) * spacing;
            return new Size(w + 2, size + 2);
        }

        public override Rectangle GetHitTestRect(RatingPainterContext ctx, int index)
            => Rectangle.Empty;   // read-only — no hit testing

        // ── Helpers ──────────────────────────────────────────────────────────────
        private static GraphicsPath StarPath(float cx, float cy, float outer, float inner)
        {
            const int points = 5;
            double step = Math.PI / points;
            var path = new GraphicsPath();
            PointF[] pts = new PointF[points * 2];

            for (int i = 0; i < points * 2; i++)
            {
                double angle = -Math.PI / 2 + i * step;
                float  r     = (i % 2 == 0) ? outer : inner;
                pts[i] = new PointF(
                    cx + (float)(r * Math.Cos(angle)),
                    cy + (float)(r * Math.Sin(angle)));
            }
            path.AddPolygon(pts);
            return path;
        }
    }
}
