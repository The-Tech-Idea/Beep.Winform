using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Button Colors and Styles
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.LightGray;
        public Color ButtonHoverForeColor { get; set; } = Color.Black;
        public Color ButtonHoverBorderColor { get; set; } = Color.Gray;

        public Color ButtonSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color ButtonSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.RoyalBlue;
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.RoyalBlue;

        public Color ButtonBackColor { get; set; } = Color.FromArgb(238, 242, 247);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ButtonBorderColor { get; set; } = Color.Gray;

        public Color ButtonErrorBackColor { get; set; } = Color.LightCoral;
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.Red;

        public Color ButtonPressedBackColor { get; set; } = Color.DodgerBlue;
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.RoyalBlue;
    }
}
