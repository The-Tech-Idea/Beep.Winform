using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // RadioButton properties with default Material Design values
        public Color RadioButtonBackColor { get; set; } = Color.Transparent;
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(66, 66, 66); // Dark Gray text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Light gray border
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue background when checked
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White; // White text when checked
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue border when checked
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light blue hover background
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue hover text
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(30, 136, 229); // Darker blue hover border
        public Font RadioButtonFont { get; set; } = new Font("Roboto", 11f, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Roboto", 11f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color RadioButtonSelectedBackColor { get; set; } = Color.Transparent;
    }
}
