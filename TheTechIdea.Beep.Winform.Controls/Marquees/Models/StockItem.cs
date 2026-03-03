using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Sprint 6 — Specialised marquee item for financial stock-ticker display.
    /// Renders as: Symbol (bold)  Price  ▲/▼ Δ%  (green/red colour coding).
    /// </summary>
    public class StockItem : MarqueeItem
    {
        /// <summary>Ticker symbol (e.g. "AAPL", "MSFT").</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Current price.</summary>
        public decimal Price { get; set; }

        /// <summary>Absolute price change since last close. Positive = up, negative = down.</summary>
        public decimal Change { get; set; }

        /// <summary>Percentage change (0.032 = 3.2%). Positive = up, negative = down.</summary>
        public decimal ChangePercent { get; set; }

        /// <summary>Number of decimal places used when formatting the price.</summary>
        public int PriceDecimals { get; set; } = 2;

        /// <summary>Colour applied when Change > 0. Default Material green.</summary>
        public Color GainColor { get; set; } = Color.FromArgb(76, 175, 80);

        /// <summary>Colour applied when Change &lt; 0. Default Material red.</summary>
        public Color LossColor { get; set; } = Color.FromArgb(244, 67, 54);

        /// <summary>Colour applied when Change == 0.</summary>
        public Color NeutralColor { get; set; } = Color.DimGray;

        /// <summary>Returns the colour that matches the sign of <see cref="Change"/>.</summary>
        public Color EffectiveChangeColor =>
            Change > 0 ? GainColor : Change < 0 ? LossColor : NeutralColor;

        /// <summary>Arrow prefix matching the sign of Change ("▲", "▼", "—").</summary>
        public string ChangeIndicator =>
            Change > 0 ? "▲" : Change < 0 ? "▼" : "—";
    }
}
