using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Task Card Fonts & Colors with Material Design defaults
        public TypographyStyle  TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color TaskCardMetricTextBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color TaskCardMetricTextBorderColor { get; set; } =Color.FromArgb(33, 150, 243);

        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TaskCardMetricTextHoverBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } =Color.FromArgb(33, 150, 243);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(33, 150, 243)
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
