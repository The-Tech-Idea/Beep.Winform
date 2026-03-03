using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 3/6 — Stock-ticker painter.
    /// Layout: [Symbol (bold)]  [Price]  [▲/▼ Δ%] in green or red.
    /// Falls back gracefully to <see cref="DefaultMarqueePainter"/> for plain
    /// <see cref="MarqueeItem"/> entries that are not <see cref="StockItem"/>s.
    /// </summary>
    public class StockTickerPainter : MarqueePainterBase
    {
        public override string Name => "Stock Ticker";

        private readonly DefaultMarqueePainter _fallback = new DefaultMarqueePainter();

        // Font size multipliers relative to base font
        private const float SymbolScale  = 1.1f;
        private const float PriceScale   = 1.0f;
        private const float ChangeScale  = 0.9f;
        private const int   ColGap       = 8;

        public override Size Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx)
        {
            if (item is not StockItem stock)
                return _fallback.Measure(g, item, ctx);

            var (sf, pf, cf) = GetFonts(ctx);
            try
            {
                float sw = g.MeasureString(stock.Symbol,                             sf).Width;
                float pw = g.MeasureString(FormatPrice(stock),                       pf).Width;
                float cw = g.MeasureString(FormatChange(stock),                      cf).Width;
                int   w  = (int)(sw + pw + cw + ColGap * 3 + ctx.Padding * 2);
                int   h  = Math.Max(ctx.ItemHeight, (int)sf.GetHeight(g) + ctx.Padding * 2);
                return new Size(w, h);
            }
            finally { DisposeAll(sf, pf, cf); }
        }

        public override void Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx)
        {
            if (!item.IsVisible) return;
            if (item is not StockItem stock) { _fallback.Draw(g, item, dest, ctx); return; }

            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var (sf, pf, cf) = GetFonts(ctx);
            try
            {
                float x  = dest.X + ctx.Padding;
                float cy = dest.Y + dest.Height / 2f;
                Color fg = ResolveForeColor(item, ctx);

                // Symbol
                using (var sb = new SolidBrush(fg))
                    g.DrawString(stock.Symbol, sf, sb, x, cy - sf.GetHeight(g) / 2f);
                x += g.MeasureString(stock.Symbol, sf).Width + ColGap;

                // Price
                using (var pb = new SolidBrush(fg))
                    g.DrawString(FormatPrice(stock), pf, pb, x, cy - pf.GetHeight(g) / 2f);
                x += g.MeasureString(FormatPrice(stock), pf).Width + ColGap;

                // Change
                using (var cb = new SolidBrush(stock.EffectiveChangeColor))
                    g.DrawString(FormatChange(stock), cf, cb, x, cy - cf.GetHeight(g) / 2f);
            }
            finally { DisposeAll(sf, pf, cf); }
        }

        // ── Helpers ────────────────────────────────────────────────────

        private static (Font sym, Font price, Font chg) GetFonts(MarqueeRenderContext ctx)
        {
            var baseFont = ctx.ItemFont ?? SystemFonts.DefaultFont;
            return (
                new Font(baseFont, FontStyle.Bold),
                new Font(baseFont, FontStyle.Regular),
                new Font(baseFont.FontFamily, baseFont.Size * 0.9f, FontStyle.Regular)
            );
        }

        private static string FormatPrice(StockItem s) =>
            s.Price.ToString("F" + s.PriceDecimals);

        private static string FormatChange(StockItem s) =>
            $"{s.ChangeIndicator}{Math.Abs(s.ChangePercent):F2}%";

        private static void DisposeAll(params Font[] fonts)
        {
            foreach (var f in fonts) f?.Dispose();
        }
    }
}
