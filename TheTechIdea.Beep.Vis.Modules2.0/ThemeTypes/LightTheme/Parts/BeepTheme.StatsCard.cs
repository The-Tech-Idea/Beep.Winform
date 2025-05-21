using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Stats Card Fonts & Colors
//<<<<<<< HEAD
        public Font StatsTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.LightGray;

        public Color StatsCardTitleForeColor { get; set; } = Color.Black;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black,
            FontStyle = FontStyle.Regular
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            FontStyle = FontStyle.Italic
        };

        public Color StatsCardValueForeColor { get; set; } = Color.DarkBlue;
        public Color StatsCardValueBackColor { get; set; } = Color.Transparent;
        public Color StatsCardValueBorderColor { get; set; } = Color.LightGray;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.Navy;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.LightYellow;
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.Gold;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            TextColor = Color.DarkBlue,
            FontStyle = FontStyle.Regular
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.DimGray;
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DimGray,
            FontStyle = FontStyle.Italic
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.Green;
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.LightGreen;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Green,
            FontStyle = FontStyle.Regular
        };
    }
}
