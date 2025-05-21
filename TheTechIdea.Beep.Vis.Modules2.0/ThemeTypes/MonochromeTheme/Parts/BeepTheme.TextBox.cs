using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.WhiteSmoke;
        public Color TextBoxForeColor { get; set; } = Color.Black;
        public Color TextBoxBorderColor { get; set; } = Color.Gray;
        public Color TextBoxHoverBorderColor { get; set; } = Color.DimGray;
        public Color TextBoxHoverBackColor { get; set; } = Color.White;
        public Color TextBoxHoverForeColor { get; set; } = Color.Black;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.Black;
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.DarkGray;

        public Color TextBoxErrorBorderColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorForeColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.IndianRed;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.LightCoral;

        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
