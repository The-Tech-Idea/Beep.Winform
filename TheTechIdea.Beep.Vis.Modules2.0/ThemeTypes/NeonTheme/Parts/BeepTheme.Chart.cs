using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Chart Fonts & Colors
        public TypographyStyle ChartTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156),
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ChartSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(46, 204, 113),
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ChartBackColor { get; set; } = Color.FromArgb(30, 30, 50);
        public Color ChartLineColor { get; set; } = Color.FromArgb(26, 188, 156);
        public Color ChartFillColor { get; set; } = Color.FromArgb(50, 26, 188, 156);
        public Color ChartAxisColor { get; set; } = Color.FromArgb(155, 89, 182);
        public Color ChartTitleColor { get; set; } = Color.FromArgb(26, 188, 156);
        public Color ChartTextColor { get; set; } = Color.FromArgb(236, 240, 241);
        public Color ChartLegendBackColor { get; set; } = Color.FromArgb(40, 40, 60);
        public Color ChartLegendTextColor { get; set; } = Color.FromArgb(236, 240, 241);
        public Color ChartLegendShapeColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color ChartGridLineColor { get; set; } = Color.FromArgb(50, 155, 89, 182);
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.FromArgb(26, 188, 156),  // Neon turquoise
            Color.FromArgb(46, 204, 113),  // Neon green
            Color.FromArgb(155, 89, 182),  // Neon purple
            Color.FromArgb(241, 196, 15),  // Neon yellow
            Color.FromArgb(231, 76, 60)    // Neon red
        };
    }
}