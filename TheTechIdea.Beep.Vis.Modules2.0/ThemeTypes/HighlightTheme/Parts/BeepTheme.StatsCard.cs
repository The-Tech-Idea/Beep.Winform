using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Stats Card Fonts & Colors
        public Font StatsTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Color StatsCardBackColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.LightGray;
        public Color StatsCardTitleForeColor { get; set; } = Color.DimGray;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; }
        public Color StatsCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; }
        public Color StatsCardValueForeColor { get; set; } = Color.Black;
        public Color StatsCardValueBackColor { get; set; } = Color.Transparent;
        public Color StatsCardValueBorderColor { get; set; } = Color.LightGray;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.DarkBlue;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.LightBlue;
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.SteelBlue;
        public TypographyStyle StatsCardValueStyle { get; set; }
        public Color StatsCardInfoForeColor { get; set; } = Color.Gray;
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle StatsCardInfoStyle { get; set; }
        public Color StatsCardTrendForeColor { get; set; } = Color.ForestGreen;
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.DarkGreen;
        public TypographyStyle StatsCardTrendStyle { get; set; }
    }
}
