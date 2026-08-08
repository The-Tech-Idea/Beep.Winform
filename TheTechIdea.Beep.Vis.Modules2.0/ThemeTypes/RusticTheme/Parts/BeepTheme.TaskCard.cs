using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; }
        public TypographyStyle TaskCardSelectedFont { get; set; }
        public TypographyStyle TaskCardUnSelectedFont { get; set; }
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TaskCardForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TaskCardTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle TaskCardTitleStyle { get; set; }
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardSubTitleBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public TypographyStyle TaskCardSubStyleStyle { get; set; }
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public TypographyStyle TaskCardMetricTextStyle { get; set; }
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public TypographyStyle TaskCardProgressValueStyle { get; set; }
    }
}
