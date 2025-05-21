using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Textbox colors and Fonts with Material Design defaults
        public Color TextBoxBackColor { get; set; } = Color.White;
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(189, 189, 189); // Grey 400
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color TextBoxHoverBackColor { get; set; } = Color.White;
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600

        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color TextBoxErrorBackColor { get; set; } = Color.White;
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.White;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(229, 115, 115); // Red 300

        public Font TextBoxFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font TextBoxHoverFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font TextBoxSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
    }
}
