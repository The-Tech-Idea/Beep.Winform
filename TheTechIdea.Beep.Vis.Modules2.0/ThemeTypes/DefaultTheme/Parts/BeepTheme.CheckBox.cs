using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark text
        public Color CheckBoxBorderColor { get; set; } = Color.Gray;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(54, 162, 235);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237); // Cornflower Blue
        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
    }
}
