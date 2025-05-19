using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253);          // Light blue hover
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243);          // Accent blue
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210);       // Darker blue for selected
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(21, 101, 192);    // Even darker on hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);

        public Color ButtonBackColor { get; set; } = Color.White;                                  // Default white
        public Color ButtonForeColor { get; set; } = Color.FromArgb(33, 150, 243);                 // Accent blue text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(244, 67, 54);            // Red error
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(211, 47, 47);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(21, 101, 192);          // Pressed state
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(21, 101, 192);
    }
}
