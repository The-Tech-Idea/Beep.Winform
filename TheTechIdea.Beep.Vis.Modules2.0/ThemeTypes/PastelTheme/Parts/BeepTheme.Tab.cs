using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color TabBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color TabForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TabBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
    }
}