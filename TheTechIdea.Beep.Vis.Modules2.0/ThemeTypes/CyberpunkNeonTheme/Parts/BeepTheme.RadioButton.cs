using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // RadioButton properties

        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(18, 18, 32);         // Cyberpunk Black
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(255, 0, 255);      // Neon Magenta

        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon Magenta (checked BG)
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;                 // White (checked text)
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 255, 128); // Neon Green (checked border)

        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);   // Neon Green (hover BG)
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow (hover text)
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Neon Cyan (hover border)

        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);

        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 0);    // Neon Yellow (selected)
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan (selected BG)
    }
}
