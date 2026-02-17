using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(100, 100, 100);

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(0, 150, 255);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(0, 150, 255);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(70, 70, 70);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(176, 0, 32);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(220, 20, 60);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(90, 90, 90);
    }
}
