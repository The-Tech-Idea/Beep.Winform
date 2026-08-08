using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; }
        public TypographyStyle TabHoverFont { get; set; }
        public TypographyStyle TabSelectedFont { get; set; }
        public Color TabBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TabForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TabBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}
