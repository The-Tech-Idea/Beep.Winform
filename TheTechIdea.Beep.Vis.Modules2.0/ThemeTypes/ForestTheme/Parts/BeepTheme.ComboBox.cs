using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(224, 255, 224); // LightGreen
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(34, 139, 34);   // ForestGreen
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(85, 107, 47); // DarkOliveGreen
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(204, 255, 204); // PaleGreen
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(34, 139, 34);   // ForestGreen
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(107, 142, 35); // OliveDrab
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113); // MediumSeaGreen
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // FireBrick
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
    }
}
