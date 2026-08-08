using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(178, 34, 34);
        public TypographyStyle TextBoxFont { get; set; }
        public TypographyStyle TextBoxHoverFont { get; set; }
        public TypographyStyle TextBoxSelectedFont { get; set; }
    }
}
