using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(230, 240, 245);
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 245, 245);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 80, 80);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 80, 80);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(255, 120, 120);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 80, 80);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 120, 120);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 80, 120) };
    }
}