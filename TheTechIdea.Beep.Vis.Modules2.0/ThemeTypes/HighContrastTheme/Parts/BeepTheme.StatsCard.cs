using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Stats Card Fonts & Colors
//<<<<<<< HEAD
        public Font StatsTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.Black;
        public Color StatsCardForeColor { get; set; } = Color.White;
        public Color StatsCardBorderColor { get; set; } = Color.White;

        public Color StatsCardTitleForeColor { get; set; } = Color.Yellow;
        public Color StatsCardTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.Yellow
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray
        };

        public Color StatsCardValueForeColor { get; set; } = Color.Lime;
        public Color StatsCardValueBackColor { get; set; } = Color.Black;
        public Color StatsCardValueBorderColor { get; set; } = Color.Lime;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.Black;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.Lime;
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.White;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 13,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Bold,
            TextColor = Color.Lime
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.White;
        public Color StatsCardInfoBackColor { get; set; } = Color.DarkSlateGray;
        public Color StatsCardInfoBorderColor { get; set; } = Color.White;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.White
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.Orange;
        public Color StatsCardTrendBackColor { get; set; } = Color.Black;
        public Color StatsCardTrendBorderColor { get; set; } = Color.Orange;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Orange
        };
    }
}
