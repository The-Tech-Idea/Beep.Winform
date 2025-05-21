using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Stats Card Fonts & Colors
//<<<<<<< HEAD
        public Font StatsTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StatsSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font StatsUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.White;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.LightGray;

        public Color StatsCardTitleForeColor { get; set; } = Color.Black;
        public Color StatsCardTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.WhiteSmoke;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color StatsCardValueForeColor { get; set; } = Color.Black;
        public Color StatsCardValueBackColor { get; set; } = Color.White;
        public Color StatsCardValueBorderColor { get; set; } = Color.LightGray;

        public Color StatsCardValueHoverForeColor { get; set; } = Color.Black;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.LightBlue;
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.SteelBlue;

        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.DimGray;
        public Color StatsCardInfoBackColor { get; set; } = Color.White;
        public Color StatsCardInfoBorderColor { get; set; } = Color.Gainsboro;

        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Regular,
            TextColor = Color.DimGray,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.SeaGreen;
        public Color StatsCardTrendBackColor { get; set; } = Color.White;
        public Color StatsCardTrendBorderColor { get; set; } = Color.LightGray;

        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.SeaGreen,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };
    }
}
