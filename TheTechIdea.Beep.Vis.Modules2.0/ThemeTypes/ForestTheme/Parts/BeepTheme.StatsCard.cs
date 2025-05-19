using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color StatsCardForeColor { get; set; } = Color.White;
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(0, 100, 0); // DarkGreen
        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.LightGreen;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGreen
        };

        public Color StatsCardValueForeColor { get; set; } = Color.White;
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(46, 139, 87); // MediumSeaGreen
        public Color StatsCardValueBorderColor { get; set; } = Color.DarkGreen;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(60, 179, 113); // MediumSpringGreen
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.ForestGreen;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            TextColor = Color.White
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.LightGreen;
        public Color StatsCardInfoBackColor { get; set; } = Color.Transparent;
        public Color StatsCardInfoBorderColor { get; set; } = Color.ForestGreen;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGreen
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.LimeGreen;
        public Color StatsCardTrendBackColor { get; set; } = Color.Transparent;
        public Color StatsCardTrendBorderColor { get; set; } = Color.DarkGreen;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            TextColor = Color.LimeGreen
        };
    }
}
