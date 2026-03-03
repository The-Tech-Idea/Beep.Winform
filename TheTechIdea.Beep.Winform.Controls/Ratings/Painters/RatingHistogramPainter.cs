using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Renders a horizontal histogram (bar chart) showing the distribution
    /// of votes per star value, alongside the overall average and count.
    /// Requires <see cref="RatingPainterContext.HistogramData"/> to be populated.
    /// </summary>
    public class RatingHistogramPainter : RatingPainterBase
    {
        public override string Name => "Histogram";

        private const int BarHeight  = 8;
        private const int RowSpacing = 4;
        private const int LabelWidth = 12;   // e.g. "5 ★"
        private const int CountWidth = 36;   // right-side count text

        public override void Paint(RatingPainterContext ctx)
        {
            SetupGraphics(ctx.Graphics);
            var  g      = ctx.Graphics;
            var  bounds = ctx.Bounds;
            var  data   = ctx.HistogramData;

            if (data == null || ctx.StarCount == 0) return;

            int  rows    = ctx.StarCount;
            int  rowH    = BarHeight + RowSpacing;
            int  totalH  = rows * rowH - RowSpacing;
            int  startY  = bounds.Top + (bounds.Height - totalH) / 2;

            int  barLeft  = bounds.Left + LabelWidth + 4;
            int  barRight = bounds.Right - CountWidth - 4;
            int  barW     = Math.Max(1, barRight - barLeft);

            using var labelFont = new Font(FontFamily.GenericSansSerif,
                Math.Max(6f, BarHeight + 1f), GraphicsUnit.Pixel);
            using var sf = new StringFormat
            {
                Alignment     = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };
            using var sfL = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            // Render from highest star to lowest (5 → 1)
            for (int r = 0; r < rows; r++)
            {
                int starVal = rows - r;
                int y       = startY + r * rowH;
                float frac  = data.GetFraction(starVal);
                int  count  = data.GetCount(starVal);

                // Star label on left
                var labelRect = new RectangleF(bounds.Left, y, LabelWidth, BarHeight);
                using (var b = new SolidBrush(ctx.LabelColor != default ? ctx.LabelColor
                                              : Color.FromArgb(80, 80, 80)))
                    g.DrawString(starVal.ToString(), labelFont, b, labelRect, sf);

                // Background track
                var trackRect = new RectangleF(barLeft, y + 1, barW, BarHeight - 2);
                using (var emptyBrush = new SolidBrush(ctx.EmptyStarColor))
                using (var path = RoundRect(trackRect, (BarHeight - 2) / 2))
                    g.FillPath(emptyBrush, path);

                // Filled portion
                float fillW = barW * frac;
                if (fillW > 1)
                {
                    var fillRect = new RectangleF(barLeft, y + 1, fillW, BarHeight - 2);
                    Color fillColor = ctx.GetGradedColor(starVal);
                    using (var brush = new SolidBrush(fillColor))
                    using (var path  = RoundRect(fillRect, (BarHeight - 2) / 2))
                        g.FillPath(brush, path);
                }

                // Count on right
                var countRect = new RectangleF(barRight + 4, y, CountWidth, BarHeight);
                using (var b = new SolidBrush(Color.FromArgb(100, 100, 100)))
                    g.DrawString(count.ToString(), labelFont, b, countRect, sfL);
            }

            // Average + total below histogram
            if (ctx.ShowAverage || ctx.ShowRatingCount)
            {
                float avgY  = startY + totalH + 6;
                Color grey  = Color.FromArgb(90, 90, 90);
                using var smallFont = new Font(FontFamily.GenericSansSerif,
                    Math.Max(6f, BarHeight * 0.9f), GraphicsUnit.Pixel);
                using var b = new SolidBrush(grey);
                using var sfC = new StringFormat { Alignment = StringAlignment.Center };

                string text = "";
                if (ctx.ShowAverage)   text += $"Avg: {data.AverageRating:F1}  ";
                if (ctx.ShowRatingCount) text += $"({data.TotalCount} ratings)";

                g.DrawString(text.Trim(), smallFont, b,
                    new RectangleF(bounds.Left, avgY, bounds.Width, 14), sfC);
            }
        }

        public override Size CalculateSize(RatingPainterContext ctx)
        {
            int rows = ctx.StarCount;
            int h    = rows * (BarHeight + RowSpacing) + (ctx.ShowAverage ? 20 : 0);
            return new Size(180, Math.Max(h, 60));
        }

        private static GraphicsPath RoundRect(RectangleF r, int radius)
        {
            var path = new GraphicsPath();
            float d  = radius * 2f;
            if (r.Width < d) d = Math.Max(1, r.Width);
            path.AddArc(r.Left, r.Top,    d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left,  r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
