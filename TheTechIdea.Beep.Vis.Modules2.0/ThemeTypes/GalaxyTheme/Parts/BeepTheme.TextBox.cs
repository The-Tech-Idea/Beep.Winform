using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TextBoxForeColor { get; set; } = Color.White;
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Accent hover
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover
        public Color TextBoxHoverForeColor { get; set; } = Color.White;

        public Color TextBoxSelectedBorderColor { get; set; } = Color.White;
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;

        public Color TextBoxPlaceholderColor { get; set; } = Color.Gray;

        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60); // ErrorColor
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(0x33, 0x00, 0x00);
        public Color TextBoxErrorForeColor { get; set; } = Color.White;
        public Color TextBoxErrorTextColor { get; set; } = Color.White;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.LightPink;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(0x55, 0x00, 0x00);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(0xAA, 0x33, 0x33);

        public Font TextBoxFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font TextBoxHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Font TextBoxSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
    }
}
