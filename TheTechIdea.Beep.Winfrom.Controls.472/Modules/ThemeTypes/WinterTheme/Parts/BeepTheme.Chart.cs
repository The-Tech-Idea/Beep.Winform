using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color ChartLineColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ChartFillColor { get; set; } = Color.FromArgb(100, 149, 237, 128);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(80, 120, 160);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(100, 149, 237),
            Color.FromArgb(255, 193, 7),
            Color.FromArgb(156, 39, 176),
            Color.FromArgb(77, 182, 172),
            Color.FromArgb(239, 83, 80)
        };
    }
}