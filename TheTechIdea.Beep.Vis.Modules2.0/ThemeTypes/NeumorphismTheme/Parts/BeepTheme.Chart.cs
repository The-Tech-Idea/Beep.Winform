using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Chart Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for chart background
        public Color ChartLineColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(150, 90, 180, 90); // Semi-transparent soft green for fills
        public Color ChartAxisColor { get; set; } = Color.FromArgb(150, 150, 160); // Light gray for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for title
        public Color ChartTextColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for general text
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for legend
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for legend shapes
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(90, 180, 90),   // Soft green
            Color.FromArgb(80, 150, 200),  // Soft blue
            Color.FromArgb(255, 180, 90),  // Soft orange
            Color.FromArgb(200, 90, 90),   // Soft red
            Color.FromArgb(150, 90, 180)   // Soft purple
        };
    }
}