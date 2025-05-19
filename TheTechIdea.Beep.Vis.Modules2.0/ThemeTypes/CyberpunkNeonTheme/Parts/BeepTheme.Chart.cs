using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Chart Fonts & Colors

        public TypographyStyle ChartTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 13f, FontStyle.Bold);
        public TypographyStyle ChartSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Italic);

        public Color ChartBackColor { get; set; } = Color.FromArgb(20, 20, 40);             // Cyberpunk Deep Black
        public Color ChartLineColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan
        public Color ChartFillColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon Magenta
        public Color ChartAxisColor { get; set; } = Color.FromArgb(0, 255, 128);            // Neon Green
        public Color ChartTitleColor { get; set; } = Color.FromArgb(255, 0, 255);           // Neon Magenta
        public Color ChartTextColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan

        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(34, 34, 68);       // Cyberpunk Panel
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan

        public Color ChartGridLineColor { get; set; } = Color.FromArgb(54, 162, 235);       // Neon Soft Blue

        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(255, 0, 255),      // Neon Magenta
            Color.FromArgb(0, 255, 255),      // Neon Cyan
            Color.FromArgb(255, 255, 0),      // Neon Yellow
            Color.FromArgb(0, 255, 128),      // Neon Green
            Color.FromArgb(0, 102, 255),      // Neon Blue
            Color.FromArgb(255, 64, 129),     // Neon Pink
            Color.FromArgb(255, 128, 0),      // Neon Orange
            Color.FromArgb(255, 0, 128)       // Neon Fuchsia
        };
    }
}
