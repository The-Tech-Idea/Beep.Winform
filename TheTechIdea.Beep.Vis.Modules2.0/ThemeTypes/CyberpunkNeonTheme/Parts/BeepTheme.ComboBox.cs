using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // ComboBox Colors and Fonts

        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(24, 24, 48);           // Cyberpunk Black
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta

        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan

        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 102, 255);      // Neon Blue
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(0, 255, 128);      // Neon Green
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(255, 255, 0);    // Neon Yellow

        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 40, 80);     // Neon Red
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;

        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);

        // If ComboBox has a checkbox selection mode (like multi-select)
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 255, 128);      // Neon Green
    }
}
