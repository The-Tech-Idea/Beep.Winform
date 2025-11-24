using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle  TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.Black;
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(250, 250, 250);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(30, 30, 30)
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(250, 250, 250);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.Gray
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.White;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(0, 90, 180);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Black
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(0, 153, 0);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(0, 102, 0);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };
    }
}
