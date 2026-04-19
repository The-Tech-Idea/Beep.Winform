using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color TaskCardBackColor { get; set; } = Color.Black;
        public Color TaskCardForeColor { get; set; } = Color.WhiteSmoke;
        public Color TaskCardBorderColor { get; set; } = Color.Gray;
        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.DimGray;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.DimGray;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.Silver;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.DimGray;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.DimGray;
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.DimGray;
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Regular,
            TextColor = Color.Silver
        };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.DimGray;
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.Gray;
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
