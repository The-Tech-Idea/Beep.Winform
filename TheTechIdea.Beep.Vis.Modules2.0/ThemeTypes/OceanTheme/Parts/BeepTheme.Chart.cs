using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Chart Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for chart background
        public Color ChartLineColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(120, 100, 200, 180); // Semi-transparent teal for fills
        public Color ChartAxisColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for title
        public Color ChartTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for general text
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for legend
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for legend shapes
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(50, 80, 110); // Lighter ocean blue for grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(100, 200, 180), // Bright teal
            Color.FromArgb(150, 180, 200), // Soft aqua
            Color.FromArgb(50, 120, 160),  // Deep sky blue
            Color.FromArgb(255, 90, 90),   // Coral red
            Color.FromArgb(120, 150, 180)  // Muted blue
        };
    }
}