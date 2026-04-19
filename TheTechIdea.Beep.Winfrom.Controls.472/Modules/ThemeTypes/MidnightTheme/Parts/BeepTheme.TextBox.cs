using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {

        public Color TextBoxBackColor { get; set; } = Color.FromArgb(40, 40, 50);
        public Color TextBoxForeColor { get; set; } = Color.LightGray;
        public Color TextBoxBorderColor { get; set; } = Color.Gray;
        public Color TextBoxHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(55, 55, 70);
        public Color TextBoxHoverForeColor { get; set; } = Color.White;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(60, 60, 80);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.DarkGray;
        public Color TextBoxErrorBorderColor { get; set; } = Color.OrangeRed;
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(60, 0, 0);
        public Color TextBoxErrorForeColor { get; set; } = Color.OrangeRed;
        public Color TextBoxErrorTextColor { get; set; } = Color.OrangeRed;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.IndianRed;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(40, 0, 0);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.OrangeRed;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.Red;
        public TypographyStyle  TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

    }
}
