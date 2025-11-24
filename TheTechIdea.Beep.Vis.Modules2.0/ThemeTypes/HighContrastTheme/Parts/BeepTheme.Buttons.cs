using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Button Colors and Styles
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.Black;
        public Color ButtonHoverForeColor { get; set; } = Color.Yellow;
        public Color ButtonHoverBorderColor { get; set; } = Color.Yellow;

        public Color ButtonSelectedBorderColor { get; set; } = Color.Lime;
        public Color ButtonSelectedBackColor { get; set; } = Color.Black;
        public Color ButtonSelectedForeColor { get; set; } = Color.Lime;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.Lime;
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.Black;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.Lime;

        public Color ButtonBackColor { get; set; } = Color.Black;
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.White;

        public Color ButtonErrorBackColor { get; set; } = Color.Black;
        public Color ButtonErrorForeColor { get; set; } = Color.Red;
        public Color ButtonErrorBorderColor { get; set; } = Color.Red;

        public Color ButtonPressedBackColor { get; set; } = Color.Gray;
        public Color ButtonPressedForeColor { get; set; } = Color.Black;
        public Color ButtonPressedBorderColor { get; set; } = Color.White;
    }
}
