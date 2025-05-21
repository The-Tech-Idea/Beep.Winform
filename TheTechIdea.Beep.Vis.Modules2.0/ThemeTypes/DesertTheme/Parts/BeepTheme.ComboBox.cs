using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // ComboBox Colors and Fonts - Desert Theme
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(255, 250, 240); // Soft Cream (Surface)
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(56, 44, 21);    // Dark Brown (OnPrimary)
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan (Primary)
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(244, 214, 162); // Light Sand (Secondary)
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(56, 44, 21);    // Dark Brown
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(201, 144, 66); // Warm Clay (Accent)
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(201, 144, 66); // Warm Clay (Accent)
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(201, 144, 66); // Accent
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 228, 225); // Light red for error background
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(178, 34, 34);   // Firebrick Red error text

        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);

        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(201, 144, 66); // Accent color for checkbox selected background
    }
}
