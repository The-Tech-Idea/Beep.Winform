using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    public class PillMarqueePainter : MarqueePainterBase
    {
        private static readonly StringFormat _pillFormat = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming      = StringTrimming.EllipsisCharacter
        };

        public override string Name => "Pill";

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            var font   = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var sz     = g.MeasureString(item.Text, font);
            int h      = Math.Max(ctx.ItemHeight / 2, (int)sz.Height + 6);
            int w      = (int)sz.Width + h;
            return new Size(w + 4, h + 4);
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = ctx.ItemFont ?? SystemFonts.DefaultFont;

            var sz   = g.MeasureString(item.Text, font);
            int h    = Math.Max(ctx.ItemHeight / 2, (int)sz.Height + 6);
            int w    = (int)sz.Width + h;
            float cy = dest.Y + dest.Height / 2f;
            var pillRect = new RectangleF(dest.X + 2, cy - h / 2f, w, h);

            Color bg = item.BackgroundColor != Color.Transparent
                ? item.BackgroundColor
                : (ctx.UseThemeColors && ctx.Theme != null ? ctx.Theme.PrimaryColor : ColorUtils.MapSystemColor(SystemColors.Highlight));

            FillRoundedRect(g, pillRect, h / 2f, bg);

            using var fb = new SolidBrush(ColorUtils.GetContrastColor(bg));
            g.DrawString(item.Text, font, fb, pillRect, _pillFormat);
        }
    }
}
