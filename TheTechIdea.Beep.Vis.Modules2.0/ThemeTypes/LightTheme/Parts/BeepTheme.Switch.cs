using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle  SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.White;
        public Color SwitchBorderColor { get; set; } = Color.LightGray;
        public Color SwitchForeColor { get; set; } = Color.Black;
        public Color SwitchSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.Blue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.LightBlue;
        public Color SwitchHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.DarkBlue;
    }
}
