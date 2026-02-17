using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Switch control Fonts & Colors
        public TypographyStyle SwitchTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);
        public TypographyStyle SwitchSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle SwitchUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color SwitchBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color SwitchBorderColor { get; set; } = Color.DarkGreen;
        public Color SwitchForeColor { get; set; } = Color.White;
        public Color SwitchSelectedBackColor { get; set; } = Color.SeaGreen;
        public Color SwitchSelectedBorderColor { get; set; } = Color.LimeGreen;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;
        public Color SwitchHoverBackColor { get; set; } = Color.MediumSeaGreen;
        public Color SwitchHoverBorderColor { get; set; } = Color.LightGreen;
        public Color SwitchHoverForeColor { get; set; } = Color.WhiteSmoke;
    }
}
