using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Button Colors and Styles
//<<<<<<< HEAD
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192); // Slightly darker

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(13, 71, 161); // Dark Blue
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(21, 101, 192);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(10, 70, 160);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(30, 136, 229);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(211, 47, 47);   // Red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(183, 28, 28);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(21, 101, 192);
    }
}
