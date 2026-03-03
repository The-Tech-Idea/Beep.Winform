using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 3 — Minimal painter: 16 px icon + text, no background, tight spacing.
    /// Uses the theme foreground colour; icon is rendered as a coloured glyph square
    /// when no image loader is available.
    /// </summary>
    public class MinimalMarqueePainter : MarqueePainterBase
    {
        public override string Name => "Minimal";

        private const int IconSize = 16;
        private const int IconGap  = 6;

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            var font   = ctx.ItemFont ?? SystemFonts.DefaultFont;
            bool hasIcon = !string.IsNullOrEmpty(item.ImagePath);
            var sz     = g.MeasureString(item.Text, font);
            int iconW  = hasIcon ? IconSize + IconGap : 0;
            int h      = Math.Max(ctx.ItemHeight, (int)sz.Height + 4);
            return new Size((int)sz.Width + iconW + ctx.Padding * 2, h);
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            var font  = ctx.ItemFont ?? SystemFonts.DefaultFont;
            Color fg  = ResolveForeColor(item, ctx);
            float cy  = dest.Y + dest.Height / 2f;
            float x   = dest.X + ctx.Padding;

            // Icon placeholder (filled square) when ImagePath is set
            // A full image loader integration can replace this stub.
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                float ty = cy - IconSize / 2f;
                using var ib = new SolidBrush(Color.FromArgb(160, fg));
                g.FillRectangle(ib, x, ty, IconSize, IconSize);
                x += IconSize + IconGap;
            }

            // Text
            var textRect = new RectangleF(x, dest.Y, dest.Right - x - ctx.Padding, dest.Height);
            using var fb = new SolidBrush(fg);
            g.DrawString(item.Text, font, fb, textRect, CenteredFormat());
        }
    }
}
