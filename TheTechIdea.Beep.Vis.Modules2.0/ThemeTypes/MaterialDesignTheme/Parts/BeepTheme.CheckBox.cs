using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(66, 66, 66); // Grey 800
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Blue 50
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
    }
}
