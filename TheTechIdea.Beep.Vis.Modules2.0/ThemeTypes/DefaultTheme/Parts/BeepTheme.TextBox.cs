using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.White;
        public Color TextBoxForeColor { get; set; } = Color.Black;
        public Color TextBoxBorderColor { get; set; } = Color.Gray;
        public Color TextBoxHoverBorderColor { get; set; } = Color.DodgerBlue;
        public Color TextBoxHoverBackColor { get; set; } = Color.WhiteSmoke;
        public Color TextBoxHoverForeColor { get; set; } = Color.Black;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.Gray;
        public Color TextBoxErrorBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorForeColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.LightCoral;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.IndianRed;
        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
    }
}
