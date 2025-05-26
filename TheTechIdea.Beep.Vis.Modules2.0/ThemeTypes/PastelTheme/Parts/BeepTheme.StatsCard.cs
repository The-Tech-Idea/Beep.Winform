using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color StatsCardTrendBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(245, 183, 203) };
    }
}