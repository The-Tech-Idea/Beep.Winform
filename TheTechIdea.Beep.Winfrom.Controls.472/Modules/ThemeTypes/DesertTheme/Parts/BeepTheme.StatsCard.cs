using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.FromArgb(250, 240, 230); // NavajoWhite
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark Brown
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color StatsCardTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public TypographyStyle StatsCardTitleStyle { get; set; }

        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color StatsCardSubTitleBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public TypographyStyle StatsCardSubStyleStyle { get; set; }

        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(255, 140, 0); // DarkOrange
        public Color StatsCardValueBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(255, 165, 0); // Orange

        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(255, 239, 213); // PapayaWhip
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(255, 165, 0); // Orange
        public TypographyStyle StatsCardValueStyle { get; set; }

        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color StatsCardInfoBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public TypographyStyle StatsCardInfoStyle { get; set; }

        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color StatsCardTrendBackColor { get; set; } =Color.FromArgb(210, 180, 140);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(50, 205, 50); // LimeGreen
        public TypographyStyle StatsCardTrendStyle { get; set; }
    }
}
