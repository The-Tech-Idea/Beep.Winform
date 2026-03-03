using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 3 — Default painter: plain text rendering.
    /// Wraps the original <see cref="BeepMarquee"/> draw logic so it participates
    /// in the new painter pipeline without losing backwards compatibility.
    /// </summary>
    public class DefaultMarqueePainter : MarqueePainterBase
    {
        public override string Name => "Default";

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            var font = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var sz = g.MeasureString(item.Text, font);
            int h = Math.Max(ctx.ItemHeight, (int)sz.Height + ctx.Padding * 2);
            int w = (int)sz.Width + ctx.Padding * 2;
            return new Size(w, h);
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            var font   = ctx.ItemFont ?? SystemFonts.DefaultFont;
            var fg     = ResolveForeColor(item, ctx);
            using var brush = new SolidBrush(fg);
            g.DrawString(item.Text, font, brush, dest, CenteredFormat());
        }
    }
}
