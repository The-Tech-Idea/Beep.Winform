using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle  StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color StatsCardForeColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardBorderColor { get; set; } = Color.DarkSlateGray;

        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public Color StatsCardValueForeColor { get; set; } = Color.LightGreen;
        public Color StatsCardValueBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public Color StatsCardValueBorderColor { get; set; } = Color.DarkGreen;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.LimeGreen;
        public Color StatsCardValueHoverBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.LimeGreen;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.LightGreen
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.LightGray;
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public Color StatsCardInfoBorderColor { get; set; } = Color.Gray;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.Lime;
        public Color StatsCardTrendBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public Color StatsCardTrendBorderColor { get; set; } = Color.LimeGreen;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Lime
        };
    }
}
