using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(90, 45, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ChartLineColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ChartFillColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color ChartTitleColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ChartTextColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(200, 180, 160);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(160, 82, 45),
            Color.FromArgb(178, 34, 34),
            Color.FromArgb(205, 133, 63),
            Color.FromArgb(188, 143, 143),
            Color.FromArgb(139, 69, 19)
        };
    }
}