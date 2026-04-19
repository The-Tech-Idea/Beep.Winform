using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Textbox colors and Fonts

        public Color TextBoxBackColor { get; set; } = Color.FromArgb(255, 253, 194);           // Lemon Yellow
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(44, 62, 80);              // Navy
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(127, 255, 212);         // Mint

        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180);    // Candy Pink
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);      // Baby Blue
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);      // Candy Pink

        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(54, 162, 235);  // Soft Blue
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240);   // Mint
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(44, 62, 80);      // Navy

        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(206, 183, 255);    // Pastel Lavender

        // Error state: candy red and pink, still readable
        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 99, 132);     // Candy Red
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(255, 99, 132);       // Candy Red
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 99, 132);       // Candy Red
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 224, 235);   // Pastel Pink
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 99, 132); // Candy Red
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink

        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Italic);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Bold);
    }
}
