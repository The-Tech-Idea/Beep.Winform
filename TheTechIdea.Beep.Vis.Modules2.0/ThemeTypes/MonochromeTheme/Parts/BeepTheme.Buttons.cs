using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Button Colors and Styles with default monochrome values
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.DarkGray;
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.Gray;

        public Color ButtonSelectedBorderColor { get; set; } = Color.Black;
        public Color ButtonSelectedBackColor { get; set; } = Color.Black;
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.DimGray;
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.Black;

        public Color ButtonBackColor { get; set; } = Color.Gray;
        public Color ButtonForeColor { get; set; } = Color.Black;
        public Color ButtonBorderColor { get; set; } = Color.DarkGray;

        public Color ButtonErrorBackColor { get; set; } = Color.DarkRed;
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.Maroon;

        public Color ButtonPressedBackColor { get; set; } = Color.Black;
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.Black;
    }
}
