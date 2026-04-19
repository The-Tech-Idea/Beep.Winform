using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CheckBoxForeColor { get; set; } = Color.LightGray;
        public Color CheckBoxBorderColor { get; set; } = Color.Gray;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.Cyan;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.Black;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.Cyan;
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.Cyan;
        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
