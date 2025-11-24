using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(20, 20, 40); // Deep space background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(200, 200, 255); // Light lavender text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(70, 70, 120); // Medium blue-purple
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(70, 20, 120); // Deep purple
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(230, 230, 255); // Bright white
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(100, 80, 180); // Purple-blue
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(40, 40, 70); // Medium space blue
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(220, 220, 255); // Bright lavender
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 100, 180); // Highlight blue
        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);

    }
}
