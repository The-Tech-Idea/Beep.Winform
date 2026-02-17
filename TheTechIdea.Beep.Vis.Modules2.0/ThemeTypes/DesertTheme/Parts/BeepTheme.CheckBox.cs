using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // CheckBox properties - Desert Style
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(255, 244, 214); // Light sand background
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(133, 94, 66); // Dark tan text
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(180, 132, 85); // Warm brown border

        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan checked background
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(111, 78, 55); // Deep brown text when checked
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna border when checked

        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(241, 208, 160); // Clay hover background
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(92, 64, 51); // Darker hover text
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(201, 144, 66); // Warm orange border on hover

        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
