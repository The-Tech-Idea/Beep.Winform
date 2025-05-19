using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Task Card Fonts & Colors
<<<<<<< HEAD
        public Font TaskCardTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TaskCardSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TaskCardUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color TaskCardBackColor { get; set; } = Color.White;
        public Color TaskCardForeColor { get; set; } = Color.Black;
        public Color TaskCardBorderColor { get; set; } = Color.LightGray;
        public Color TaskCardTitleForeColor { get; set; } = Color.Black;
        public Color TaskCardTitleBackColor { get; set; } = Color.Transparent;
=======
        public TypographyStyle TaskCardTitleFont { get; set; }
        public TypographyStyle TaskCardSelectedFont { get; set; }
        public TypographyStyle TaskCardUnSelectedFont { get; set; }
        public Color TaskCardBackColor { get; set; }
        public Color TaskCardForeColor { get; set; }
        public Color TaskCardBorderColor { get; set; }
        public Color TaskCardTitleForeColor { get; set; }
        public Color TaskCardTitleBackColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
        public TypographyStyle TaskCardTitleStyle { get; set; }
        public Color TaskCardSubTitleForeColor { get; set; } = Color.Gray;
        public Color TaskCardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle TaskCardSubStyleStyle { get; set; }
        public Color TaskCardMetricTextForeColor { get; set; } = Color.DarkBlue;
        public Color TaskCardMetricTextBackColor { get; set; } = Color.Transparent;
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.Transparent;
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
