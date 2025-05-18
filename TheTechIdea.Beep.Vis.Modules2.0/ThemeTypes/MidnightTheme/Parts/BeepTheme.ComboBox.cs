using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(40, 40, 40); // Dark gray background
        public Color ComboBoxForeColor { get; set; } = Color.WhiteSmoke;            // Light text
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(70, 70, 70); // Slightly lighter border
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60); // Hover dark gray
        public Color ComboBoxHoverForeColor { get; set; } = Color.White;            // Hover text white
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(120, 120, 120); // Hover border light gray
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 70); // Selected background
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;         // Selected text
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(180, 180, 180); // Selected border
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(100, 0, 0); // Dark red background for error
        public Color ComboBoxErrorForeColor { get; set; } = Color.FromArgb(255, 100, 100); // Light red text for error

        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font ComboBoxListFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(48, 63, 159); // Indigo 700
    }
}
