using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
    }
}