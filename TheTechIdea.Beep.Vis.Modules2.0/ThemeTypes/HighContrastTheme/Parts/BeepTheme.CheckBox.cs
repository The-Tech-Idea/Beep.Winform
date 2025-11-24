using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.Black;
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.Yellow;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.Black;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.White;
        public Color CheckBoxHoverBackColor { get; set; } = Color.Gray;
        public Color CheckBoxHoverForeColor { get; set; } = Color.Yellow;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.Yellow;
        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
    }
}
