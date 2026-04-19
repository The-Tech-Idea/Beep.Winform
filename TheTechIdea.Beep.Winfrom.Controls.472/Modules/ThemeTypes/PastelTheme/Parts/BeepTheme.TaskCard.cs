using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(245, 183, 203);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle() { FontSize = 12f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}
