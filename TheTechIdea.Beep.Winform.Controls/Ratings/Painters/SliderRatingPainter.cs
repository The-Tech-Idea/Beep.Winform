using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Renders a continuous horizontal slider-style rating track.
    /// Filled portion = selected; unfilled = empty.
    /// A circular thumb marks the current position.
    /// </summary>
    public class SliderRatingPainter : RatingPainterBase
    {
        public override string Name => "Slider";

        private const int TrackHeight = 6;
        private const int ThumbRadius = 10;

        public override void Paint(RatingPainterContext ctx)
        {
            SetupGraphics(ctx.Graphics);
            var g      = ctx.Graphics;
            var bounds = ctx.Bounds;

            int paddingH = ThumbRadius + 4;
            int trackL   = bounds.Left  + paddingH;
            int trackR   = bounds.Right - paddingH;
            int centerY  = bounds.Top   + bounds.Height / 2;
            int trackW   = Math.Max(1, trackR - trackL);

            float frac = ctx.StarCount > 0
                ? Math.Max(0f, Math.Min(1f, ctx.PreciseRating / ctx.StarCount))
                : 0f;
            int fillEnd = trackL + (int)(trackW * frac);

            // Track — empty portion
            var emptyRect = new RectangleF(trackL, centerY - TrackHeight / 2f, trackW, TrackHeight);
            using (var emptyBrush = new SolidBrush(ctx.EmptyStarColor))
            using (var path = BuildRoundRect(emptyRect, TrackHeight / 2))
                g.FillPath(emptyBrush, path);

            // Track — filled portion
            if (fillEnd > trackL)
            {
                Color fillColor = ctx.GetGradedColor(ctx.PreciseRating);
                var fillRect = new RectangleF(trackL, centerY - TrackHeight / 2f,
                    fillEnd - trackL, TrackHeight);
                using (var fillBrush = new SolidBrush(fillColor))
                using (var path = BuildRoundRect(fillRect, TrackHeight / 2))
                    g.FillPath(fillBrush, path);
            }

            // Thumb
            if (!ctx.ReadOnly)
            {
                float tx = fillEnd;
                var thumbRect = new RectangleF(tx - ThumbRadius, centerY - ThumbRadius,
                    ThumbRadius * 2, ThumbRadius * 2);

                Color thumbFill = ctx.HoveredStar >= 0 ? ctx.HoverStarColor : ctx.FilledStarColor;
                using (var brush = new SolidBrush(thumbFill))
                    g.FillEllipse(brush, thumbRect);

                if (ctx.StarBorderThickness > 0)
                {
                    using var pen = new Pen(ctx.StarBorderColor, ctx.StarBorderThickness);
                    g.DrawEllipse(pen, thumbRect);
                }
            }

            DrawLabels(ctx);
        }

        public override Rectangle GetHitTestRect(RatingPainterContext ctx, int index)
        {
            // For the slider the whole track is the hit area
            int paddingH = ThumbRadius + 4;
            int h = ctx.Bounds.Height;
            return new Rectangle(ctx.Bounds.Left + paddingH, ctx.Bounds.Top,
                ctx.Bounds.Width - paddingH * 2, h);
        }

        public override Size CalculateSize(RatingPainterContext ctx)
            => new Size(ctx.Bounds.Width > 0 ? ctx.Bounds.Width : 200, ThumbRadius * 2 + 8);

        private static GraphicsPath BuildRoundRect(RectangleF r, int radius)
        {
            var path = new GraphicsPath();
            float d  = radius * 2f;
            if (r.Width < d) d = r.Width;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
