using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Textbox colors and Fonts
//<<<<<<< HEAD
        public Color TextBoxBackColor { get; set; } = Color.White;
        public Color TextBoxForeColor { get; set; } = Color.Black;
        public Color TextBoxBorderColor { get; set; } = Color.LightGray;
        public Color TextBoxHoverBorderColor { get; set; } = Color.SkyBlue;
        public Color TextBoxHoverBackColor { get; set; } = Color.WhiteSmoke;
        public Color TextBoxHoverForeColor { get; set; } = Color.Black;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TextBoxSelectedBackColor { get; set; } = Color.White;
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.Gray;
        public Color TextBoxErrorBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorForeColor { get; set; } = Color.Black;
        public Color TextBoxErrorTextColor { get; set; } = Color.Red;
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.DarkRed;
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.MistyRose;
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.Red;
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.IndianRed;

        public Font TextBoxFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TextBoxHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TextBoxSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
    }
}
