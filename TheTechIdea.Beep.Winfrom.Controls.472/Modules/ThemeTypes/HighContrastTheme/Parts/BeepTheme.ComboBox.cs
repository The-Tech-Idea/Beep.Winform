using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.Black;
        public Color ComboBoxForeColor { get; set; } = Color.White;
        public Color ComboBoxBorderColor { get; set; } = Color.White;
        public Color ComboBoxHoverBackColor { get; set; } = Color.DarkGray;
        public Color ComboBoxHoverForeColor { get; set; } = Color.Yellow;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.Yellow;
        public Color ComboBoxSelectedBackColor { get; set; } = Color.Yellow;
        public Color ComboBoxSelectedForeColor { get; set; } = Color.Black;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.White;
        public Color ComboBoxErrorBackColor { get; set; } = Color.Red;
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle  ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.Black;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.Yellow;
    }
}
