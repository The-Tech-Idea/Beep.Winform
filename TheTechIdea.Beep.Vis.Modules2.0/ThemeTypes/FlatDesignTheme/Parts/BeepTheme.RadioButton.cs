using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.Transparent;
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark gray text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(204, 204, 204); // Light gray border

        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 215); // Blue checked background
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);

        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(229, 241, 251); // Light blue hover background
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);

        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);

        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
    }
}
