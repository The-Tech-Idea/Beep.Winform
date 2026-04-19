using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;
        public TypographyStyle ComboBoxItemFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle ComboBoxListFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
    }
}
