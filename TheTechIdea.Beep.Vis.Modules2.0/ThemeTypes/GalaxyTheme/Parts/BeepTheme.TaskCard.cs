using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Task Card Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TaskCardForeColor { get; set; } = Color.White;
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33);

        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Italic,
            TextColor = Color.LightGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.Black;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.White;
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } =Color.FromArgb(10, 10, 30);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.White;
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };
    }
}
