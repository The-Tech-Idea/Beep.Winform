using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Stats Card Fonts & Colors with Material Design defaults
        public TypographyStyle  StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 16f, FontStyle.Bold);
        public TypographyStyle  StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.White;
        public Color StatsCardForeColor { get; set; } = Color.Black;
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color StatsCardValueBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400

        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(55, 71, 79); // Blue Grey 800
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(76, 175, 80); // Green 500 (positive trend)
        public Color StatsCardTrendBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(56, 142, 60); // Green 700
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(76, 175, 80)
        };
    }
}
