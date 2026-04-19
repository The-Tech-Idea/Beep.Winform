using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle TabHoverFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle TabSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color TabBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color TabForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color TabBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color TabHoverForeColor { get; set; } = Color.White;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
    }
}
