using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Stats Card Fonts & Colors

        public TypographyStyle StatsTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle StatsSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle StatsUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        // Card backgrounds/borders
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(255, 224, 235);       // Pastel Pink
        public Color StatsCardForeColor { get; set; } = Color.FromArgb(44, 62, 80);          // Navy
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(127, 255, 212);     // Mint

        // Card title
        public Color StatsCardTitleForeColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(255, 253, 194);  // Lemon Yellow

        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180),
            LetterSpacing = 0.1f,
            LineHeight = 1.15f
        };

        // Card subtitle
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(127, 255, 212),
            LetterSpacing = 0.06f,
            LineHeight = 1.1f
        };

        // Value (main stat number)
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(54, 162, 235);   // Soft Blue
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(255, 253, 194);  // Lemon Yellow
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(240, 100, 180);// Candy Pink

        public Color StatsCardValueHoverForeColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon

        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(54, 162, 235),
            LetterSpacing = 0.12f,
            LineHeight = 1.15f
        };

        // Info (footnote, small details)
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(206, 183, 255);   // Pastel Lavender
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(255, 253, 194);   // Lemon Yellow
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 9.5f,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(206, 183, 255),
            LetterSpacing = 0.03f,
            LineHeight = 1.05f
        };

        // Trend (up/down arrows, sparkline, etc.)
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(255, 99, 132);   // Candy Red (down) or Soft Blue (up)
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(255, 253, 194);  // Lemon Yellow
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(127, 255, 212);// Mint

        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 99, 132), // Candy Red
            LetterSpacing = 0.08f,
            LineHeight = 1.1f
        };
    }
}
