using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color CheckBoxForeColor { get; set; } = Color.White;
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color CheckBoxHoverForeColor { get; set; } = Color.White;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public TypographyStyle CheckBoxFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle CheckBoxCheckedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
    }
}