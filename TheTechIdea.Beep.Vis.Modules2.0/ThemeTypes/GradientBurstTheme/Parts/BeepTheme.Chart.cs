using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle  ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.FromArgb(25, 118, 210);  // Blue
        public Color ChartFillColor { get; set; } = Color.FromArgb(187, 222, 251); // Light Blue
        public Color ChartAxisColor { get; set; } = Color.FromArgb(84, 110, 122);  // Blue Gray
        public Color ChartTitleColor { get; set; } = Color.FromArgb(33, 33, 33);    // Dark Gray
        public Color ChartTextColor { get; set; } = Color.FromArgb(66, 66, 66);    // Medium Gray

        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light Gray
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(25, 118, 210);

        public Color ChartGridLineColor { get; set; } = Color.FromArgb(224, 224, 224);

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(33, 150, 243), // Blue
            Color.FromArgb(244, 67, 54),  // Red
            Color.FromArgb(76, 175, 80),  // Green
            Color.FromArgb(255, 193, 7),  // Amber
            Color.FromArgb(156, 39, 176)  // Purple
        };
    }
}
