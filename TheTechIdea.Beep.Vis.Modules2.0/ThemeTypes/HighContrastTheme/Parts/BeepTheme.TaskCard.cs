using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle  TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.Black;
        public Color TaskCardForeColor { get; set; } = Color.White;
        public Color TaskCardBorderColor { get; set; } = Color.White;

        public Color TaskCardTitleForeColor { get; set; } = Color.Yellow;
        public Color TaskCardTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Yellow
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Black;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.LightGray
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.Lime;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.Lime;
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.Lime;
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.White;
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Lime
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.Yellow;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.Black;
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.Yellow;
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.Yellow
        };
    }
}
