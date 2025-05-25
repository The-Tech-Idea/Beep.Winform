using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color ChartLineColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ChartFillColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color ChartTitleColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color ChartTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 200, 200);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(60, 179, 113),
            Color.FromArgb(255, 99, 71),
            Color.FromArgb(135, 206, 250),
            Color.FromArgb(255, 215, 0),
            Color.FromArgb(144, 238, 144)
        };
    }
}