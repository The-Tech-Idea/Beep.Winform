using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Sprint 6 — Specialised marquee item for news/headline ticker.
    /// Renders as: [Category pill]  Headline text.
    /// </summary>
    public class NewsItem : MarqueeItem
    {
        /// <summary>Category label shown in a colour pill (e.g. "BREAKING", "TECH").</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Fill colour of the category pill.</summary>
        public Color CategoryColor { get; set; } = Color.SteelBlue;

        /// <summary>
        /// Optional URL or route string that the host application can use when the
        /// user clicks the item.  Empty means no navigation.
        /// </summary>
        public string Link { get; set; } = string.Empty;
    }
}
