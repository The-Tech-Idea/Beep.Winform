using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Light gray border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light blue hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue border on hover
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue selection background
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Darker blue border selected
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 238); // Light red background for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(183, 28, 28); // Dark red text for error
        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue background when selected
    }
}
