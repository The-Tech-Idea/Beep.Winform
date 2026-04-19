using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color RadioButtonForeColor { get; set; } = Color.White;
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color RadioButtonHoverForeColor { get; set; } = Color.White;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public TypographyStyle RadioButtonFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle RadioButtonCheckedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
    }
}
