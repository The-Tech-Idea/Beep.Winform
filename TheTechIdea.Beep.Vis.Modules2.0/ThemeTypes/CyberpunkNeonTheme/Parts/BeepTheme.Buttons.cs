using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Button Colors and Styles

        public Font ButtonFont { get; set; } = new Font("Consolas", 11f, FontStyle.Bold);
        public Font ButtonHoverFont { get; set; } = new Font("Consolas", 11f, FontStyle.Italic);
        public Font ButtonSelectedFont { get; set; } = new Font("Consolas", 11f, FontStyle.Bold | FontStyle.Underline);

        // Default: Neon cyan border, magenta text on deep purple
        public Color ButtonBackColor { get; set; } = Color.FromArgb(34, 34, 68);           // Cyberpunk Deep Dark
        public Color ButtonForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta

        // Hover: Neon blue border, magenta background, yellow text
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan

        // Selected: Neon green border, blue background, cyan text
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);    // Neon Green
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 102, 255);      // Neon Blue
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(0, 255, 255);      // Neon Cyan

        // Selected Hover: Neon pink border, magenta/blue background, yellow text
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(24, 24, 48);      // Dark background
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon Magenta

        // Error: Hot neon red background, black text, yellow border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 40, 80);        // Neon Hot Pink/Red
        public Color ButtonErrorForeColor { get; set; } = Color.Black;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow

        // Pressed: Neon blue background, magenta border, cyan text
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(0, 102, 255);      // Neon Blue
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(0, 255, 255);      // Neon Cyan
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(255, 0, 255);    // Neon Magenta
    }
}
