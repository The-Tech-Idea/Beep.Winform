using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(0, 140, 190);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}