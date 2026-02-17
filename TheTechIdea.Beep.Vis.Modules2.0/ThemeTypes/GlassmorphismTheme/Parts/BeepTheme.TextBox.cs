using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.White;
        public Color TextBoxForeColor { get; set; } = Color.Black;
        public Color TextBoxBorderColor { get; set; } = Color.LightGray;

        public Color TextBoxHoverBorderColor { get; set; } = Color.SkyBlue;
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color TextBoxHoverForeColor { get; set; } = Color.Black;

        public Color TextBoxSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;

        public Color TextBoxPlaceholderColor { get; set; } = Color.Gray;

        public Color TextBoxErrorBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorForeColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.IndianRed;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.OrangeRed;

        public TypographyStyle  TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public TypographyStyle  TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
