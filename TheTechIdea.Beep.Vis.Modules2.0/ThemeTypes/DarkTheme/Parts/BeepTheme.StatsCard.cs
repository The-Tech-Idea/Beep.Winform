using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StatsCardForeColor { get; set; } = Color.LightGray;
        public Color StatsCardBorderColor { get; set; } = Color.DimGray;
        public Color StatsCardTitleForeColor { get; set; } = Color.WhiteSmoke;
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle { FontWeight = FontWeight.Bold, TextColor = Color.WhiteSmoke };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle { FontWeight = FontWeight.Normal, TextColor = Color.Gray };

        public Color StatsCardValueForeColor { get; set; } = Color.Gold;
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StatsCardValueBorderColor { get; set; } = Color.DarkGoldenrod;
        public Color StatsCardValueHoverForeColor { get; set; } = Color.Yellow;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.YellowGreen;
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle { FontWeight = FontWeight.Bold, TextColor = Color.Gold };

        public Color StatsCardInfoForeColor { get; set; } = Color.LightGray;
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StatsCardInfoBorderColor { get; set; } = Color.DimGray;
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle { FontWeight = FontWeight.Normal, TextColor = Color.LightGray };

        public Color StatsCardTrendForeColor { get; set; } = Color.LimeGreen;
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StatsCardTrendBorderColor { get; set; } = Color.Green;
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle { FontWeight = FontWeight.SemiBold, TextColor = Color.LimeGreen };
    }
}
