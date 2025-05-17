using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // ComboBox Colors and Fonts

        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink

        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color ComboBoxSelectedForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue

        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(255, 99, 132);   // Candy Red
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;                    // White for contrast

        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Regular);
        public Font ComboBoxListFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);

        // In case ComboBox supports checkbox-multiselect items, themed as well:
        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
    }
}
