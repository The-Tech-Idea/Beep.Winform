using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 3 — Card painter: each item rendered inside a rounded Material-3 card.
    /// Optionally shows a leading icon (via ImagePath) and a trailing badge.
    /// </summary>
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
            return new Size(w + (int)Shadow, h + (int)Shadow);
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            bool hovered = ctx.CurrentItemIndex == ctx.HoveredItemIndex;
            Color back   = ResolveBackColor(item, ctx);
            Color border = ResolveBorderColor(ctx);
            Color fg     = ResolveForeColor(item, ctx);

            // Shadow
            var shadowRect = new RectangleF(dest.X + Shadow, dest.Y + Shadow, dest.Width - Shadow, dest.Height - Shadow);
            FillRoundedRect(g, shadowRect, Radius, Color.FromArgb(30, Color.Black));

            // Card background
            var cardRect = new RectangleF(dest.X, dest.Y, dest.Width - Shadow, dest.Height - Shadow);
            FillRoundedRect(g, cardRect, Radius, hovered ? Lighten(back, 20) : back, border, 1f);

            // Content area
            float x    = cardRect.X + ctx.Padding;
            float cy   = cardRect.Y + cardRect.Height / 2f;
            var   font = ctx.ItemFont ?? SystemFonts.DefaultFont;

            // Text
            float tRight = string.IsNullOrEmpty(item.BadgeText) ? cardRect.Right - ctx.Padding
                                                                 : cardRect.Right - 32;
            var textRect = new RectangleF(x, cardRect.Y, tRight - x, cardRect.Height);
            using var fb = new SolidBrush(fg);
            g.DrawString(item.Text, font, fb, textRect, CenteredFormat());

            // Badge
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                using var badgeFont = new Font(font.FontFamily, font.Size * 0.75f);
                DrawBadge(g, item.BadgeText,
                    new PointF(cardRect.Right - 32, cy - (font.Height * 0.75f + 2) / 2f),
                    item.BadgeColor, badgeFont);
            }
        }

        private static Color Lighten(Color c, int amount) =>
            Color.FromArgb(c.A,
                Math.Min(255, c.R + amount),
                Math.Min(255, c.G + amount),
                Math.Min(255, c.B + amount));
    }
}
