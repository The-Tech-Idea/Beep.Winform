using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color ChartLineColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ChartFillColor { get; set; } = Color.FromArgb(76, 175, 80, 128);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ChartLegendTextColor { get; set; } = Color.White;
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(50, 50, 50);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(76, 175, 80),
            Color.FromArgb(255, 193, 7),
            Color.FromArgb(156, 39, 176),
            Color.FromArgb(33, 150, 243),
            Color.FromArgb(244, 67, 54)
        };
    }
}