using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Task Card Fonts & Colors

        public TypographyStyle TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 16f, FontStyle.Bold);
        public TypographyStyle TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.FromArgb(18, 18, 32);                // Dark background
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(0, 255, 255);               // Neon cyan text
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(255, 0, 255);             // Neon magenta border

        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(0, 255, 128);          // Neon green title
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 255, 128)
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(255, 255, 0);       // Neon yellow subtitle
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(255, 255, 0)
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(34, 34, 68);      // Dark panel
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon cyan border

        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(0, 255, 128); // Neon green hover text
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon magenta hover bg
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);// Neon yellow hover border
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.White
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(18, 18, 32);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(0, 255, 255);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
