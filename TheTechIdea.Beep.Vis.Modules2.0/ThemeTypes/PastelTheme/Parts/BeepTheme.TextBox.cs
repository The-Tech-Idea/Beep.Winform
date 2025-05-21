using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(245, 235, 237);
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 235, 235);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(180, 80, 80);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(180, 80, 80);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(200, 120, 120);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(180, 80, 80);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 200, 200);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
    }
}