using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // CheckBox properties
//<<<<<<< HEAD
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(120, 144, 156); // Blue Gray

        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(76, 175, 80);   // Green
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(56, 142, 60);   // Dark Green

        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(232, 240, 253); // Light Blue Hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210);  // Blue
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);  // Darker Blue

        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
    }
}
