using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}