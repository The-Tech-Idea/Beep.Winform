using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(30, 30, 30);                // Dark background
        public Color ComboBoxForeColor { get; set; } = Color.White;                              // White text
        public Color ComboBoxBorderColor { get; set; } = Color.Gray;                             // Gray border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);          // Slightly lighter on hover
        public Color ComboBoxHoverForeColor { get; set; } = Color.White;                         // Text stays white
        public Color ComboBoxHoverBorderColor { get; set; } = Color.Cyan;                        // Cyan border on hover
        public Color ComboBoxSelectedBackColor { get; set; } = Color.Cyan;                       // Cyan background when selected
        public Color ComboBoxSelectedForeColor { get; set; } = Color.Black;                      // Black text on selected
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.Cyan;                     // Cyan border selected
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(60, 0, 0);            // Dark red background on error
        public Color ComboBoxErrorForeColor { get; set; } = Color.Red;                           // Red text on error
        public TypographyStyle ComboBoxItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ComboBoxListFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.Cyan;
    }
}
