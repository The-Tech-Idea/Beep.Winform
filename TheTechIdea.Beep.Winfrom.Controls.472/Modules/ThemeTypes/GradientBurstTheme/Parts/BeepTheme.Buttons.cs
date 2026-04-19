using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Button Colors and Styles
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192); // Slightly darker

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(13, 71, 161); // Dark Blue
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(20, 110, 205);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(15, 90, 180);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(10, 70, 160);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(245, 248, 252); // light plate for contrast
        public Color ButtonForeColor { get; set; } = Color.FromArgb(25, 70, 130);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(30, 136, 229);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(211, 47, 47);   // Red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(183, 28, 28);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(215, 230, 247);
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(20, 60, 110);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(21, 101, 192);
    }
}
