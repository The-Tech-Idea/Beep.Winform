using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ComboBoxForeColor { get; set; } = Color.White;
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color ComboBoxHoverForeColor { get; set; } = Color.White;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
    }
}