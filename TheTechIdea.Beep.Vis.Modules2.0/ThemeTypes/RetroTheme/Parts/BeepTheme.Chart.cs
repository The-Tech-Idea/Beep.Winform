using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Chart Fonts & Colors
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 215, 0), // Retro yellow
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(192, 192, 192), // Light gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for chart background
        public Color ChartLineColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(120, 0, 255, 255); // Semi-transparent cyan for fills
        public Color ChartAxisColor { get; set; } = Color.FromArgb(192, 192, 192); // Light gray for axes
        public Color ChartTitleColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for title
        public Color ChartTextColor { get; set; } = Color.FromArgb(255, 255, 255); // White for general text
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for legend
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(255, 255, 255); // White for legend text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for legend shapes
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(0, 255, 255), // Bright cyan
            Color.FromArgb(255, 215, 0), // Retro yellow
            Color.FromArgb(255, 85, 85), // Retro red
            Color.FromArgb(0, 128, 128), // Darker teal
            Color.FromArgb(192, 192, 192) // Light gray
        };
    }
}