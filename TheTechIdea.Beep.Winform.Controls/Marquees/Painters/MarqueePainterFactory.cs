using TheTechIdea.Beep.Winform.Controls.Marquees;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    /// <summary>
    /// Sprint 3 — Routes <see cref="MarqueeStyle"/> to the correct painter instance.
    /// </summary>
    public static class MarqueePainterFactory
    {
        /// <summary>Returns a new painter instance for the given style.</summary>
        public static IMarqueeItemRenderer Create(MarqueeStyle style) => style switch
        {
            MarqueeStyle.Card        => new CardMarqueePainter(),
            MarqueeStyle.Pill        => new PillMarqueePainter(),
            MarqueeStyle.StockTicker => new StockTickerPainter(),
            MarqueeStyle.NewsBanner  => new NewsBannerPainter(),
            MarqueeStyle.Minimal     => new MinimalMarqueePainter(),
            _                        => new DefaultMarqueePainter()
        };
    }
}
