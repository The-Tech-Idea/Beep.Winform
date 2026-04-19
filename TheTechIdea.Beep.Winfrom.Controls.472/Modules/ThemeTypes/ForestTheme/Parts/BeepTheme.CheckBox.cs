using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(240, 255, 240); // light green background
        public Color CheckBoxForeColor { get; set; } = Color.DarkGreen;
        public Color CheckBoxBorderColor { get; set; } = Color.ForestGreen;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.ForestGreen;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.DarkGreen;
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(200, 255, 200);
        public Color CheckBoxHoverForeColor { get; set; } = Color.DarkOliveGreen;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.OliveDrab;
        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
