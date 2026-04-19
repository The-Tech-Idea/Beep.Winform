using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Button Colors and Styles with default monochrome values
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(110, 110, 110);
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(90, 90, 90);

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(55, 55, 55);
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(245, 245, 245);

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(90, 90, 90);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(76, 76, 76);

        public Color ButtonErrorBackColor { get; set; } = Color.DarkRed;
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.Maroon;

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(60, 60, 60);
    }
}
