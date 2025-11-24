using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle  TaskCardTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  TaskCardSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  TaskCardUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.Black;
        public Color TaskCardBorderColor { get; set; } = Color.LightGray;
        public Color TaskCardTitleForeColor { get; set; } = Color.Black;
        public Color TaskCardTitleBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public TypographyStyle TaskCardTitleStyle { get; set; }
        public Color TaskCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color TaskCardSubTitleBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public TypographyStyle TaskCardSubStyleStyle { get; set; }
        public Color TaskCardMetricTextForeColor { get; set; } = Color.DarkBlue;
        public Color TaskCardMetricTextBackColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color TaskCardMetricTextBorderColor { get; set; } =Color.FromArgb(255, 255, 204);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.LightBlue;
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.LightBlue;
        public TypographyStyle TaskCardMetricTextStyle { get; set; }
        public Color TaskCardProgressValueForeColor { get; set; } = Color.White;
        public Color TaskCardProgressValueBackColor { get; set; } = Color.DodgerBlue;
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.DodgerBlue;
        public TypographyStyle TaskCardProgressValueStyle { get; set; }
    }
}
