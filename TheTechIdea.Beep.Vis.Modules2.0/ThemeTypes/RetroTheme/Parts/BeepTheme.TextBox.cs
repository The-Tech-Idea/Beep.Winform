using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color TextBoxForeColor { get; set; } = Color.White;
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TextBoxHoverForeColor { get; set; } = Color.White;
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(203, 75, 22);
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color TextBoxSelectedForeColor { get; set; } = Color.Black;
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(108, 123, 127);
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(204, 0, 0);
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 85, 85);
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 85, 85);
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(255, 128, 128);
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 85, 85);
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(204, 0, 0);
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 128, 128);
        public TypographyStyle TextBoxFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TextBoxHoverFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TextBoxSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
    }
}