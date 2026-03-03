using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Renders rating using rotated square (diamond) shapes.
    /// Suitable for luxury / premium aesthetics.
    /// </summary>
    public class DiamondRatingPainter : RatingPainterBase
    {
        public override string Name => "Diamond";

        public override void Paint(RatingPainterContext ctx)
        {
            SetupGraphics(ctx.Graphics);
            var g = ctx.Graphics;

            int count    = ctx.StarCount;
            int size     = ctx.StarSize;
            int spacing  = ctx.Spacing;
            var bounds   = ctx.Bounds;

            int totalW   = count * size + (count - 1) * spacing;
            int startX   = bounds.Left + (bounds.Width  - totalW) / 2;
            int centerY  = bounds.Top  + bounds.Height / 2;

            for (int i = 0; i < count; i++)
            {
                float cx = startX + i * (size + spacing) + size / 2f;
                float cy = centerY;

                bool filled  = ctx.UseColorGrade
                    ? (i + 1) <= (int)Math.Round(ctx.PreciseRating)
                    : (i < ctx.SelectedRating);
                bool hovered = (!ctx.ReadOnly && ctx.HoveredStar >= 0 && i <= ctx.HoveredStar);

                Color fill   = hovered ? ctx.HoverStarColor
                             : filled  ? ctx.GetGradedColor(i + 1)
                             : ctx.EmptyStarColor;

                float half = size * 0.5f;
                PointF[] pts = {
                    new PointF(cx,        cy - half),
                    new PointF(cx + half, cy),
                    new PointF(cx,        cy + half),
                    new PointF(cx - half, cy)
                };

                using var path = new GraphicsPath();
                path.AddPolygon(pts);

                using (var brush = new SolidBrush(fill))
                    g.FillPath(brush, path);

                if (ctx.StarBorderThickness > 0)
                {
                    using var pen = new Pen(ctx.StarBorderColor, ctx.StarBorderThickness);
                    g.DrawPath(pen, path);
                }
            }

            DrawLabels(ctx);
        }

        public override Size CalculateSize(RatingPainterContext ctx)
        {
            int w = ctx.StarCount * ctx.StarSize + (ctx.StarCount - 1) * ctx.Spacing;
            return new Size(w + 4, ctx.StarSize + 4);
        }
    }
}
