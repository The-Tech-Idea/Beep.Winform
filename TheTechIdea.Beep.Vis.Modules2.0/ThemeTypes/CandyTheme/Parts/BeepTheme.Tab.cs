using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Tab Fonts & Colors

        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Italic);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(255, 224, 235);          // Pastel Pink
        public Color TabForeColor { get; set; } = Color.FromArgb(44, 62, 80);             // Navy

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(255, 223, 93);     // Lemon Yellow

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(255, 253, 194);  // Lemon Yellow
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(44, 62, 80);     // Navy

        public Color TabBorderColor { get; set; } = Color.FromArgb(127, 255, 212);        // Mint

        public Color TabHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);     // Baby Blue
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);     // Candy Pink

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240);  // Mint
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(240, 100, 180);// Candy Pink

        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93);    // Lemon Yellow
    }
}
