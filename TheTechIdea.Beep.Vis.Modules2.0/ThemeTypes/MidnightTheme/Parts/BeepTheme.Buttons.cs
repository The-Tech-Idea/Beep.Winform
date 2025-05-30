using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Button Colors and Styles
//<<<<<<< HEAD
        public TypographyStyle  ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(60, 63, 65); // dark gray
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237); // cornflower blue

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(65, 105, 225); // royal blue
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(70, 130, 180); // steel blue
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(100, 149, 237); // cornflower blue
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(30, 144, 255); // dodger blue

        public Color ButtonBackColor { get; set; } = Color.FromArgb(45, 45, 48); // very dark gray
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(70, 70, 70); // medium dark gray

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34); // firebrick red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(139, 0, 0); // dark red

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(30, 30, 30); // very dark gray
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(65, 105, 225); // royal blue
    }
}
