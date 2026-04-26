using TheTechIdea.Beep.Winform.Controls.Marquees;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    public static class MarqueePainterFactory
    {
        private static readonly IMarqueeItemRenderer _default = new DefaultMarqueePainter();
        private static readonly IMarqueeItemRenderer _card = new CardMarqueePainter();
        private static readonly IMarqueeItemRenderer _pill = new PillMarqueePainter();
        private static readonly IMarqueeItemRenderer _stock = new StockTickerPainter();
        private static readonly IMarqueeItemRenderer _news = new NewsBannerPainter();
        private static readonly IMarqueeItemRenderer _minimal = new MinimalMarqueePainter();

        public static IMarqueeItemRenderer Create(MarqueeStyle style) => style switch
        {
            MarqueeStyle.Card        => _card,
            MarqueeStyle.Pill        => _pill,
            MarqueeStyle.StockTicker => _stock,
            MarqueeStyle.NewsBanner  => _news,
            MarqueeStyle.Minimal     => _minimal,
            _                        => _default
        };
    }
}
