using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(139, 69, 19), // SaddleBrown
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ChartLineColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color ChartFillColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color ChartAxisColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color ChartTitleColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color ChartTextColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 200, 200); // Light Gray
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(160, 82, 45), // Sienna
            Color.FromArgb(205, 133, 63), // Peru
            Color.FromArgb(139, 69, 19), // SaddleBrown
            Color.FromArgb(184, 134, 11), // DarkGoldenrod
            Color.FromArgb(107, 142, 35) // OliveDrab
        };
    }
}