using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Textbox colors and Fonts
//<<<<<<< HEAD
        public Color TextBoxBackColor { get; set; } = Color.Black;
        public Color TextBoxForeColor { get; set; } = Color.White;
        public Color TextBoxBorderColor { get; set; } = Color.White;
        public Color TextBoxHoverBorderColor { get; set; } = Color.Yellow;
        public Color TextBoxHoverBackColor { get; set; } = Color.DarkSlateGray;
        public Color TextBoxHoverForeColor { get; set; } = Color.White;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.Lime;
        public Color TextBoxSelectedBackColor { get; set; } = Color.Yellow;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.Gray;
        public Color TextBoxErrorBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorBackColor { get; set; } = Color.Black;
        public Color TextBoxErrorForeColor { get; set; } = Color.White;
        public Color TextBoxErrorTextColor { get; set; } = Color.Red;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.Black;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.DarkRed;

        public TypographyStyle  TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle  TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
