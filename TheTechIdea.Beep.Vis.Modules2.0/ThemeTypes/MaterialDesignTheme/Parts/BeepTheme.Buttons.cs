using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Button Colors and Styles
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(30, 136, 229); // Blue 600
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(13, 71, 161); // Blue 900
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(13, 71, 161); // Blue 900

        // Default: lighten fill for stronger contrast on light surfaces, darker text
        public Color ButtonBackColor { get; set; } = Color.FromArgb(100, 181, 246); // Blue 300
        public Color ButtonForeColor { get; set; } = Color.FromArgb(15, 40, 80);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(211, 47, 47); // Red 700
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(198, 40, 40); // Red 800

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(79, 140, 220); // between Blue 400/500
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(235, 242, 250);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
    }
}
