using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Task Card Fonts & Colors
        public Font TaskCardTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TaskCardSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TaskCardUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(34, 45, 34); // Darker forest green
        public Color TaskCardForeColor { get; set; } = Color.LightGreen;
        public Color TaskCardBorderColor { get; set; } = Color.DarkOliveGreen;
        public Color TaskCardTitleForeColor { get; set; } = Color.LimeGreen;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.LimeGreen
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.DarkSeaGreen;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DarkSeaGreen
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.LightGreen;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.Transparent;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.DarkOliveGreen;
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.Lime;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(50, 100, 50);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.LimeGreen;
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            TextColor = Color.LightGreen
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.DarkGreen;
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
