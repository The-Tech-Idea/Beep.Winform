using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; }
        public TypographyStyle StatsSelectedFont { get; set; }
        public TypographyStyle StatsUnSelectedFont { get; set; }
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle StatsCardTitleStyle { get; set; }
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle StatsCardSubStyleStyle { get; set; }
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public TypographyStyle StatsCardValueStyle { get; set; }
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public TypographyStyle StatsCardInfoStyle { get; set; }
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public TypographyStyle StatsCardTrendStyle { get; set; }
    }
}
