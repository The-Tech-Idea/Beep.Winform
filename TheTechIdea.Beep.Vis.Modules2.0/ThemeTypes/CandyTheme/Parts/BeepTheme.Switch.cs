using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Switch control Fonts & Colors

        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        // Off/default state: pastel pink, mint border, navy text
        public Color SwitchBackColor { get; set; } = Color.FromArgb(255, 224, 235);         // Pastel Pink
        public Color SwitchBorderColor { get; set; } = Color.FromArgb(127, 255, 212);       // Mint
        public Color SwitchForeColor { get; set; } = Color.FromArgb(44, 62, 80);            // Navy

        // On/selected state: candy pink, lemon border, lemon text
        public Color SwitchSelectedBackColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color SwitchSelectedBorderColor { get; set; } = Color.FromArgb(255, 223, 93);// Lemon Yellow
        public Color SwitchSelectedForeColor { get; set; } = Color.FromArgb(255, 223, 93);  // Lemon Yellow

        // Hover state: mint background, candy pink border, candy pink text
        public Color SwitchHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240);    // Mint
        public Color SwitchHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color SwitchHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
    }
}
