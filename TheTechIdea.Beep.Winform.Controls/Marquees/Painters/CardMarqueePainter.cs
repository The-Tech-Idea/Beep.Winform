using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    public class CardMarqueePainter : MarqueePainterBase
    {
        public override string Name => "Card";

        private const float Radius      = 8f;
        private const float Shadow      = 3f;

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            var font   = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var textSz = g.MeasureString(item.Text, font);
            int iconW  = string.IsNullOrEmpty(item.ImagePath) ? 0 : ctx.ImageSize + 6;
            int badgeW = string.IsNullOrEmpty(item.BadgeText) ? 0 : 28;
            int w      = iconW + (int)textSz.Width + badgeW + ctx.Padding * 3;
            int h      = Math.Max(ctx.ItemHeight, (int)textSz.Height + ctx.Padding * 2);
            int shadowOffset = MarqueeThemeHelpers.ShouldShowShadow(ctx.UseThemeColors, ctx.Theme) ? (int)Shadow : 0;
            return new Size(w + shadowOffset, h + shadowOffset);
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool hovered = ctx.CurrentItemIndex == ctx.HoveredItemIndex;
            Color back   = ResolveBackColor(item, ctx);
            Color border = ResolveBorderColor(ctx);
            Color fg     = ResolveForeColor(item, ctx);
            bool showShadow = MarqueeThemeHelpers.ShouldShowShadow(ctx.UseThemeColors, ctx.Theme);

            if (showShadow)
            {
                var shadowRect = new RectangleF(dest.X + Shadow, dest.Y + Shadow, dest.Width - Shadow, dest.Height - Shadow);
                FillRoundedRect(g, shadowRect, Radius, Color.FromArgb(25, ColorUtils.MapSystemColor(SystemColors.ControlText)));
            }

            var cardRect = new RectangleF(dest.X, dest.Y, dest.Width - (showShadow ? Shadow : 0), dest.Height - (showShadow ? Shadow : 0));
            FillRoundedRect(g, cardRect, Radius, hovered ? ColorUtils.LightenColor(back, 20) : back, border, 1f);

            float x    = cardRect.X + ctx.Padding;
            float cy   = cardRect.Y + cardRect.Height / 2f;
            var   font = ctx.ItemFont ?? SystemFonts.DefaultFont;

            float tRight = string.IsNullOrEmpty(item.BadgeText) ? cardRect.Right - ctx.Padding
                                                                 : cardRect.Right - 32;
            var textRect = new RectangleF(x, cardRect.Y, tRight - x, cardRect.Height);
            using var fb = new SolidBrush(fg);
            g.DrawString(item.Text, font, fb, textRect, CenteredFormat());

            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                using var badgeFont = new Font(font.FontFamily, font.Size * 0.75f);
                DrawBadge(g, item.BadgeText,
                    new PointF(cardRect.Right - 32, cy - (font.Height * 0.75f + 2) / 2f),
                    item.BadgeColor, badgeFont);
            }
        }
    }
}
