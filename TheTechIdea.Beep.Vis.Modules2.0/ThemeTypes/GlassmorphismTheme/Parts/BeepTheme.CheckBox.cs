using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // CheckBox properties
//<<<<<<< HEAD
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color CheckBoxForeColor { get; set; } = Color.Black;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(180, 200, 220);

        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(180, 200, 230);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.Black;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(120, 140, 160);

        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(220, 230, 240);
        public Color CheckBoxHoverForeColor { get; set; } = Color.Black;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 180, 200);

        public TypographyStyle  CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
    }
}
