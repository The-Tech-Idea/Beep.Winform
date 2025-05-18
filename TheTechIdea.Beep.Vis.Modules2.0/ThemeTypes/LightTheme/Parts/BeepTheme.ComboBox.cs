using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.Black;
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(204, 204, 204); // Light Gray
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(240, 240, 240); // Very Light Gray
        public Color ComboBoxHoverForeColor { get; set; } = Color.Black;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(102, 175, 233); // Blue-ish border on hover
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(30, 136, 229); // Darker Blue
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 238); // Light Red
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(183, 28, 28); // Dark Red
        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 9);
        public Font ComboBoxListFont { get; set; } = new Font("Segoe UI", 9);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243);
    }
}
