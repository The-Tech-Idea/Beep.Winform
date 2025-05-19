using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Task Card Fonts & Colors

        public TypographyStyle TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        // Card backgrounds/borders
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(255, 224, 235);         // Pastel Pink
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(44, 62, 80);            // Navy
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(127, 255, 212);       // Mint

        // Card title
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color TaskCardTitleBackColor { get; set; } = Color.FromArgb(255, 253, 194);    // Lemon Yellow

        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180),
            LetterSpacing = 0.09f,
            LineHeight = 1.13f
        };

        // Subtitle
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color TaskCardSubTitleBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(127, 255, 212),
            LetterSpacing = 0.05f,
            LineHeight = 1.1f
        };

        // Metric text (e.g., tags or numbers)
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(54, 162, 235);    // Soft Blue
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(255, 253, 194);   // Lemon Yellow
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon

        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(54, 162, 235),
            LetterSpacing = 0.06f,
            LineHeight = 1.09f
        };

        // Progress value (e.g., percent complete)
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(240, 100, 180);   // Candy Pink
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(204, 255, 240);   // Mint
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(54, 162, 235);  // Soft Blue

        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180),
            LetterSpacing = 0.07f,
            LineHeight = 1.08f
        };
    }
}
