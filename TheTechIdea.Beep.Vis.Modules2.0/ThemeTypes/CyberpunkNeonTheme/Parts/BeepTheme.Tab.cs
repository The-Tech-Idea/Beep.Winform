using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Tab Fonts & Colors

        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold | FontStyle.Italic);

        public Color TabBackColor { get; set; } = Color.FromArgb(18, 18, 32);              // Dark background
        public Color TabForeColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon cyan text

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon magenta active bg
        public Color ActiveTabForeColor { get; set; } = Color.White;                       // Active text

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(34, 34, 68);      // Darker panel inactive
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon green inactive text

        public Color TabBorderColor { get; set; } = Color.FromArgb(0, 255, 255);           // Neon cyan border

        public Color TabHoverBackColor { get; set; } = Color.FromArgb(255, 255, 0);        // Neon yellow hover bg
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon cyan hover text

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon magenta selected bg
        public Color TabSelectedForeColor { get; set; } = Color.White;                     // Selected text

        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);   // Neon green selected border
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon yellow hover border
    }
}
