using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Stats Card Fonts & Colors

        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);

        public Color StatsCardBackColor { get; set; } = Color.FromArgb(18, 18, 32);               // Cyberpunk Black
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon Cyan
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon Magenta

        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon Cyan
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(18, 18, 32);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.FromArgb(0, 255, 255)
        };

        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(0, 255, 128);      // Neon Green
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(18, 18, 32);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0, 255, 128)
        };

        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(255, 255, 0);         // Neon Yellow
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(34, 34, 68);          // Deep panel
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon Magenta
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 128);  // Neon Green

        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.FromArgb(255, 255, 0)
        };

        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(255, 0, 255);          // Neon Magenta
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(18, 18, 32);           // Cyberpunk Black
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan

        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 0, 255)
        };

        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon Cyan (trend up)
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(18, 18, 32);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(255, 255, 0);       // Neon Yellow

        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0, 255, 255)
        };
    }
}
