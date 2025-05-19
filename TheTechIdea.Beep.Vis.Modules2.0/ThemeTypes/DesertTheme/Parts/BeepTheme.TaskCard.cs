using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(101, 67, 33);    // Dark Brown
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(139, 69, 19);  // Saddle Brown
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(139, 69, 19),
            LineHeight = 1.2f
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(160, 82, 45);  // Sienna
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 82, 45),
            LineHeight = 1.1f
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(255, 235, 205); // Blanched Almond
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(255, 248, 220);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(101, 67, 33),
            LineHeight = 1.1f
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.1f
        };
    }
}
