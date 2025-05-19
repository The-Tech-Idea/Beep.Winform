using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
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
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color ChartLineColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ChartFillColor { get; set; } = Color.FromArgb(100, 255, 193, 7);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(52, 58, 64);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(255, 193, 7),
            Color.FromArgb(54, 162, 235),
            Color.FromArgb(255, 99, 132),
            Color.FromArgb(75, 192, 192),
            Color.FromArgb(153, 102, 255),
            Color.FromArgb(255, 159, 64)
        };
    }
}