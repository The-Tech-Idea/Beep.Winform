using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Task Card Fonts & Colors
        public Font TaskCardTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font TaskCardSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font TaskCardUnSelectedFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);

        public Color TaskCardBackColor { get; set; } = Color.FromArgb(40, 40, 50);
        public Color TaskCardForeColor { get; set; } = Color.LightGray;
        public Color TaskCardBorderColor { get; set; } = Color.DimGray;

        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };

        public Color TaskCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray
        };

        public Color TaskCardMetricTextForeColor { get; set; } = Color.LightGray;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.Transparent;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.DimGray;
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(60, 60, 70);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.SteelBlue;
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.LightGray
        };

        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(70, 70, 90);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.SteelBlue;
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
    }
}
