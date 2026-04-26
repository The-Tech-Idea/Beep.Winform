using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    public class NewsBannerPainter : MarqueePainterBase
    {
        private static readonly StringFormat _pillFormat = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public override string Name => "News Banner";

        private const float PillRadius  = 4f;
        private const int   PillPad     = 5;

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            var font   = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var sm     = GetSmallFont(font);
            try
            {
                float headlineW = g.MeasureString(item.Text, font).Width;
                float catW      = 0;
                if (item is NewsItem ni && !string.IsNullOrEmpty(ni.Category))
                    catW = g.MeasureString(ni.Category, sm).Width + PillPad * 2 + 8;
                int h = Math.Max(ctx.ItemHeight, (int)font.GetHeight(g) + ctx.Padding * 2);
                return new Size((int)(catW + headlineW + ctx.Padding * 2), h);
            }
            finally { sm.Dispose(); }
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int alpha = Math.Max(0, Math.Min(255, (int)(ctx.NewsAlpha * 255)));
            var font  = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var sm    = GetSmallFont(font);
            float x   = dest.X + ctx.Padding;
            float cy  = dest.Y + dest.Height / 2f;

            try
            {
                if (item is NewsItem ni && !string.IsNullOrEmpty(ni.Category))
                {
                    var catSz   = g.MeasureString(ni.Category, sm);
                    float pw    = catSz.Width + PillPad * 2;
                    float ph    = catSz.Height + 2;
                    var pillR   = new RectangleF(x, cy - ph / 2f, pw, ph);
                    var catBg   = Color.FromArgb(alpha, ni.CategoryColor);
                    FillRoundedRect(g, pillR, PillRadius, catBg);
                    using var tb = new SolidBrush(Color.FromArgb(alpha, ColorUtils.GetContrastColor(ni.CategoryColor)));
                    g.DrawString(ni.Category, sm, tb, pillR, _pillFormat);
                    x += pw + 8;
                }

                Color fg = ResolveForeColor(item, ctx);
                using var fb = new SolidBrush(Color.FromArgb(alpha, fg));
                g.DrawString(item.Text, font, fb,
                    new RectangleF(x, dest.Y, dest.Right - x - ctx.Padding, dest.Height),
                    CenteredFormat());
            }
            finally { sm.Dispose(); }
        }

        private static Font GetSmallFont(Font f) =>
            new Font(f.FontFamily, f.Size * 0.8f, FontStyle.Bold);
    }
}
