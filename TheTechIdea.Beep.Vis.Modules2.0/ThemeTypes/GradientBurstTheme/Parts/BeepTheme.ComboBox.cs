using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(120, 144, 156); // Blue Gray

        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(232, 240, 253); // Light Blue
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210);  // Blue
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);  // Darker Blue

        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue Selected
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(13, 71, 161);   // Deep Blue
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(13, 71, 161);

        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 238); // Light Red
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(183, 28, 28);   // Dark Red

        public TypographyStyle  ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);

        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(63, 81, 181); // Indigo
    }
}
