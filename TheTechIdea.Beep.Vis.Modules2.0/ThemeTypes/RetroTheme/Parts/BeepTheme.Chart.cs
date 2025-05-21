using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
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
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ChartLineColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ChartFillColor { get; set; } = Color.FromArgb(128, 165, 0);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ChartTitleColor { get; set; } = Color.White;
        public Color ChartTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(64, 64, 64);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(255, 165, 0),
            Color.FromArgb(64, 255, 64),
            Color.FromArgb(64, 64, 255),
            Color.FromArgb(255, 64, 64),
            Color.FromArgb(192, 192, 64)
        };
    }
}