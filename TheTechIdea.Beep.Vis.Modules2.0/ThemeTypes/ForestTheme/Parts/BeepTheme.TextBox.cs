using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(230, 245, 230); // Soft green background
        public Color TextBoxForeColor { get; set; } = Color.DarkGreen;
        public Color TextBoxBorderColor { get; set; } = Color.ForestGreen;
        public Color TextBoxHoverBorderColor { get; set; } = Color.SeaGreen;
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(240, 255, 240);
        public Color TextBoxHoverForeColor { get; set; } = Color.DarkGreen;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.OliveDrab;
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.DarkOliveGreen;
        public Color TextBoxErrorBorderColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorForeColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.IndianRed;
        public Font TextBoxFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TextBoxHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Italic);
        public Font TextBoxSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
