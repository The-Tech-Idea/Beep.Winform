using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ButtonHoverForeColor { get; set; } = Color.Black;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(180, 180, 180);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(240, 244, 248);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(211, 47, 47); // Red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(183, 28, 28);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color ButtonPressedForeColor { get; set; } = Color.Black;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
    }
}
