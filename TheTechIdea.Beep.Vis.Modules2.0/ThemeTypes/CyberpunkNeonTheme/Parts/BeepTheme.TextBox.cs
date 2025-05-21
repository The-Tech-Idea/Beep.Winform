using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(18, 18, 32);               // Dark background
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255);              // Neon cyan text
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon magenta border

        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);       // Neon yellow hover border
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);         // Neon green hover background
        public Color TextBoxHoverForeColor { get; set; } = Color.White;                         // Bright hover text

        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);    // Neon green selected border
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);      // Neon magenta selected background
        public Color TextBoxSelectedForeColor { get; set; } = Color.White;                      // Selected text color

        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(100, 255, 255, 255);// Translucent neon cyan

        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(255, 0, 0);         // Bright red error border
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(50, 0, 0, 0);          // Dark red tinted background
        public Color TextBoxErrorForeColor { get; set; } = Color.White;                         // White error text
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(255, 100, 100);       // Soft red text
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(180, 255, 100, 100);// Soft red translucent placeholder
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 50, 50);      // Error input text background
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(255, 0, 0);  // Error input border
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 80, 80); // Error input hover

        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold | FontStyle.Italic);
    }
}
