using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StatsCardValueForeColor { get; set; } = Color.White;
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(0, 140, 190);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 180, 230) };
    }
}