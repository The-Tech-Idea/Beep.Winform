using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color ChartLineColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ChartFillColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color ChartAxisColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ChartTitleColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ChartTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(25, 25, 112), // Deep midnight blue
            Color.FromArgb(65, 65, 145), // Royal blue
            Color.FromArgb(255, 215, 0), // Gold
            Color.FromArgb(178, 34, 34), // Crimson
            Color.FromArgb(0, 128, 0) // Emerald
        };
    }
}