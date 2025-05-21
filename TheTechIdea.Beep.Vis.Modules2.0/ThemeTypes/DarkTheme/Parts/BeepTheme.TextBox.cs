using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public Color TextBoxForeColor { get; set; } = Color.LightGray;
        public Color TextBoxBorderColor { get; set; } = Color.Gray;
        public Color TextBoxHoverBorderColor { get; set; } = Color.CornflowerBlue;
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(60, 60, 65);
        public Color TextBoxHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 75);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.DarkGray;
        public Color TextBoxErrorBorderColor { get; set; } = Color.IndianRed;
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(70, 20, 20);
        public Color TextBoxErrorForeColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.LightCoral;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(90, 20, 20);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Firebrick;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.DarkRed;
        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
