using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Renders an NPS-style numeric scale:
    /// a row of numbered buttons (1–StarCount) where the selected
    /// and hovered ones are highlighted.
    /// Buttons 1–6 → red/orange, 7–8 → yellow, 9–10 → green (when UseColorGrade).
    /// </summary>
    public class NumericScalePainter : RatingPainterBase
    {
        public override string Name => "NumericScale";

        // Radius of the rounded rectangle button corners
        private const int CornerR = 4;

        public override void Paint(RatingPainterContext ctx)
        {
            SetupGraphics(ctx.Graphics);
            var g = ctx.Graphics;

            int count   = ctx.StarCount;
            int size    = ctx.StarSize;
            int spacing = Math.Max(2, ctx.Spacing / 2);
            var bounds  = ctx.Bounds;

            int btnW    = size;
            int totalW  = count * btnW + (count - 1) * spacing;
            int startX  = bounds.Left + (bounds.Width - totalW) / 2;
            int centerY = bounds.Top  + bounds.Height / 2;
            int btnH    = Math.Min(size, bounds.Height - 4);
            int btnTop  = centerY - btnH / 2;

            using var labelFont = new Font(FontFamily.GenericSansSerif,
                Math.Max(6f, size * 0.45f), FontStyle.Bold, GraphicsUnit.Pixel);
            using var sf  = new StringFormat { Alignment = StringAlignment.Center,
                                               LineAlignment = StringAlignment.Center };

            for (int i = 0; i < count; i++)
            {
                int val  = i + 1;
                float bx = startX + i * (btnW + spacing);
                var   br = new RectangleF(bx, btnTop, btnW, btnH);

                bool selected = (val <= ctx.SelectedRating);
                bool hovered  = (!ctx.ReadOnly && ctx.HoveredStar == i);

                Color fill;
                Color fore;
                if (hovered)
                {
                    fill = ctx.HoverStarColor;
                    fore = Color.White;
                }
                else if (selected)
                {
                    fill = ctx.GetGradedColor(val);
                    fore = Color.White;
                }
                else
                {
                    fill = ctx.EmptyStarColor;
                    fore = Color.FromArgb(100, 100, 100);
                }

                // Draw rounded-rect button
                using var path = BuildRoundRect(br, CornerR);
                using (var brush = new SolidBrush(fill))
                    g.FillPath(brush, path);

                if (ctx.StarBorderThickness > 0 || !selected)
                {
                    Color borderCol = selected || hovered
                        ? fill
                        : ctx.StarBorderColor != Color.Empty ? ctx.StarBorderColor
                          : Color.FromArgb(180, 180, 180);
                    float borderW = ctx.StarBorderThickness > 0 ? ctx.StarBorderThickness : 1f;
                    using var pen = new Pen(borderCol, borderW);
                    g.DrawPath(pen, path);
                }

                // Number label
                using var foreBrush = new SolidBrush(fore);
                g.DrawString(val.ToString(), labelFont, foreBrush, br, sf);
            }

            DrawLabels(ctx);
        }

        public override Rectangle GetHitTestRect(RatingPainterContext ctx, int index)
        {
            if (index < 0 || index >= ctx.StarCount) return Rectangle.Empty;
            int size    = ctx.StarSize;
            int spacing = Math.Max(2, ctx.Spacing / 2);
            int btnH    = Math.Min(size, ctx.Bounds.Height - 4);
            int totalW  = ctx.StarCount * size + (ctx.StarCount - 1) * spacing;
            int startX  = ctx.Bounds.Left + (ctx.Bounds.Width - totalW) / 2;
            int topY    = ctx.Bounds.Top  + (ctx.Bounds.Height - btnH) / 2;
            return new Rectangle(startX + index * (size + spacing), topY, size, btnH);
        }

        public override Size CalculateSize(RatingPainterContext ctx)
        {
            int spacing = Math.Max(2, ctx.Spacing / 2);
            int totalW  = ctx.StarCount * ctx.StarSize + (ctx.StarCount - 1) * spacing;
            return new Size(totalW + 4, ctx.StarSize + 4);
        }

        private static GraphicsPath BuildRoundRect(RectangleF r, int radius)
        {
            var path = new GraphicsPath();
            float d  = radius * 2f;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
