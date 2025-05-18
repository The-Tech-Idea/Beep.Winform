using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ComboBoxForeColor { get; set; } = Color.Black;
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(180, 200, 220);

        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color ComboBoxHoverForeColor { get; set; } = Color.Black;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 180, 200);

        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color ComboBoxSelectedForeColor { get; set; } = Color.Black;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(140, 160, 180);

        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 220, 220);
        public Color ComboBoxErrorForeColor { get; set; } = Color.DarkRed;

        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font ComboBoxListFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CheckBoxSelectedForeColor { get; set; } = Color.Black;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(180, 200, 230);
    }
}
