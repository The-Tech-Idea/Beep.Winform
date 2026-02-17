using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {

        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);


        public Color ButtonBackColor { get; set; } = Color.FromArgb(30, 30, 60);      // Deep space blue
        public Color ButtonForeColor { get; set; } = Color.FromArgb(230, 230, 250);   // Soft star white
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(72, 61, 139);   // Dark slate blue

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80);
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(240, 240, 255);
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(106, 90, 205);

        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(70, 20, 120);
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(245, 245, 255);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(123, 104, 238);

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(90, 30, 150);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(138, 43, 226);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(139, 0, 0);   // Dark red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 69, 0);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(20, 20, 40);
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(200, 200, 255);
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(65, 105, 225);
    }
}
