using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.DarkGray;

        public Color StatsCardTitleForeColor { get; set; } = Color.Black;
        public Color StatsCardTitleBackColor { get; set; } = Color.DimGray;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.DimGray;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public Color StatsCardValueForeColor { get; set; } = Color.Black;
        public Color StatsCardValueBackColor { get; set; } = Color.DimGray;
        public Color StatsCardValueBorderColor { get; set; } = Color.DimGray;

        public Color StatsCardValueHoverForeColor { get; set; } = Color.DimGray;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.LightGray;
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.Gray;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Black
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.DimGray;
        public Color StatsCardInfoBackColor { get; set; } = Color.DimGray;
        public Color StatsCardInfoBorderColor { get; set; } = Color.DimGray;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DimGray
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.DarkSlateGray;
        public Color StatsCardTrendBackColor { get; set; } = Color.DimGray;
        public Color StatsCardTrendBorderColor { get; set; } = Color.DimGray;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DarkSlateGray
        };
    }
}
