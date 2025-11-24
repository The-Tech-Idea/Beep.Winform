using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 800
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Grey 100
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 800
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary Blue 500
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Blue 50
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary Blue 500
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary Blue 500
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 238); // Red 50
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(211, 47, 47); // Red 700
        public TypographyStyle  ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10,FontStyle.Regular);
        public TypographyStyle  ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary Blue 500
    }
}
