using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // RadioButton properties

        // Default state: pastel pink, navy text, mint border
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Checked: mint background, candy pink dot, mint border
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(204, 255, 240);   // Mint
        public Color RadioButtonCheckedForeColor { get; set; } = Color.FromArgb(240, 100, 180);   // Candy Pink
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        // Hover: baby blue, candy pink text, lemon border
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);     // Baby Blue
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);     // Candy Pink
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93);    // Lemon Yellow

        // Fonts: playful, bold for checked
        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Bold);

        // Selected: stronger mint/candy pink for explicit state
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180);  // Candy Pink
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240);  // Mint
    }
}
