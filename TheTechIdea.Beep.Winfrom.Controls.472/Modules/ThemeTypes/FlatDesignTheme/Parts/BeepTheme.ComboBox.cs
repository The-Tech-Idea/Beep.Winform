using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Gray border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240); // Light gray on hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary blue border on hover
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Primary blue selected background
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White; // White text on selection
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue border on selection
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 238); // Light red background on error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(211, 47, 47); // Red text on error
        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243);
    }
}
