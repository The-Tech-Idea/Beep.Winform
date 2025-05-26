using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Button Colors and Styles
//<<<<<<< HEAD
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(220, 230, 240);
        public Color ButtonForeColor { get; set; } = Color.Black;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(180, 200, 220);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(200, 210, 225);
        public Color ButtonHoverForeColor { get; set; } = Color.Black;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(160, 180, 200);

        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color ButtonSelectedForeColor { get; set; } = Color.Black;
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(140, 160, 180);

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(160, 180, 200);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.Black;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(120, 140, 160);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 200, 200); // Soft red
        public Color ButtonErrorForeColor { get; set; } = Color.DarkRed;
        public Color ButtonErrorBorderColor { get; set; } = Color.Red;

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(150, 170, 190);
        public Color ButtonPressedForeColor { get; set; } = Color.Black;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(120, 140, 160);
    }
}
