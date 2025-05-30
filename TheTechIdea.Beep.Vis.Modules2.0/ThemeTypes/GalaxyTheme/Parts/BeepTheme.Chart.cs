using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {

        public TypographyStyle  ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Regular);
        public Color ChartBackColor { get; set; } = Color.FromArgb(15, 15, 35); // Deep space background
        public Color ChartLineColor { get; set; } = Color.FromArgb(100, 180, 255); // Bright blue for lines
        public Color ChartFillColor { get; set; } = Color.FromArgb(50, 80, 150, 100); // Semi-transparent blue
        public Color ChartAxisColor { get; set; } = Color.FromArgb(120, 120, 180); // Muted purple-blue
        public Color ChartTitleColor { get; set; } = Color.FromArgb(200, 200, 255); // Light lavender
        public Color ChartTextColor { get; set; } = Color.FromArgb(180, 180, 230); // Soft lavender
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(25, 25, 50); // Slightly lighter than background
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(200, 200, 240); // Light text
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(150, 150, 210); // Medium lavender
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(50, 50, 90, 120); // Semi-transparent grid lines
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(106, 90, 205),    // SlateBlue
            Color.FromArgb(100, 180, 255),   // Bright blue
            Color.FromArgb(180, 90, 220),    // Purple
            Color.FromArgb(70, 180, 180),    // Teal
            Color.FromArgb(238, 130, 238),   // Violet
            Color.FromArgb(65, 105, 225),    // RoyalBlue
            Color.FromArgb(30, 144, 255),    // DodgerBlue
            Color.FromArgb(138, 43, 226)     // BlueViolet
        };

    }
}