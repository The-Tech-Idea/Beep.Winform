using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // CheckBox properties
//<<<<<<< HEAD
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.Black;
        public Color CheckBoxBorderColor { get; set; } = Color.Gray;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.SteelBlue;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.SteelBlue;
        public Color CheckBoxHoverBackColor { get; set; } = Color.LightSteelBlue;
        public Color CheckBoxHoverForeColor { get; set; } = Color.Black;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.SteelBlue;
        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
    }
}
