using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Chart Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for chart background
        public Color ChartLineColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(150, 170, 210, 170); // Semi-transparent pastel green for fills
        public Color ChartAxisColor { get; set; } = Color.FromArgb(140, 140, 140); // Slightly lighter gray for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for title
        public Color ChartTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for general text
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for legend
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for legend shapes
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(170, 210, 170), // Pastel green
            Color.FromArgb(200, 200, 240), // Pastel lavender
            Color.FromArgb(255, 220, 200), // Soft peach
            Color.FromArgb(235, 203, 217), // Soft pastel pink
            Color.FromArgb(210, 230, 220)  // Pastel mint
        };
    }
}