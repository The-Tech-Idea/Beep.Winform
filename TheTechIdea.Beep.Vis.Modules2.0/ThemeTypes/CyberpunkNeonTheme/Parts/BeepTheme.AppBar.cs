using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // AppBar colors and styles

        // Deep purple/black background with magenta, cyan, neon blue, and yellow-green accents
        public Color AppBarBackColor { get; set; } = Color.FromArgb(24, 24, 48);              // Cyberpunk dark
        public Color AppBarForeColor { get; set; } = Color.FromArgb(0, 255, 255);             // Neon Cyan
        public Color AppBarButtonForeColor { get; set; } = Color.FromArgb(255, 20, 147);      // Neon Magenta (Deep Pink)
        public Color AppBarButtonBackColor { get; set; } = Color.FromArgb(34, 34, 68);        // Slightly lighter dark
        public Color AppBarTextBoxBackColor { get; set; } = Color.FromArgb(28, 28, 56);       // Almost black
        public Color AppBarTextBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255);      // Neon Cyan
        public Color AppBarLabelForeColor { get; set; } = Color.FromArgb(255, 255, 0);        // Neon Yellow
        public Color AppBarLabelBackColor { get; set; } = Color.FromArgb(24, 24, 48);         // Match AppBar
        public Color AppBarTitleForeColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta
        public Color AppBarTitleBackColor { get; set; } = Color.FromArgb(36, 0, 70);          // Dark Violet
        public Color AppBarSubTitleForeColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon Green
        public Color AppBarSubTitleBackColor { get; set; } = Color.FromArgb(24, 24, 48);      // Match AppBar
        public Color AppBarCloseButtonColor { get; set; } = Color.FromArgb(255, 64, 129);     // Neon Pink
        public Color AppBarMaxButtonColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan
        public Color AppBarMinButtonColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green

        // Typography styles: cyberpunk font style (use futuristic or monospace for effect)
        public TypographyStyle AppBarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas", // Consider a custom font like "Orbitron" or "Audiowide" if available
            FontSize = 15,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 0, 255), // Neon Magenta
            LetterSpacing = 1.5f,
            LineHeight = 1.1f
        };

        public TypographyStyle AppBarSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 12,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(0, 255, 128), // Neon Green
            LetterSpacing = 1.1f,
            LineHeight = 1.08f
        };

        public TypographyStyle AppBarTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(0, 255, 255), // Neon Cyan
            LetterSpacing = 1f,
            LineHeight = 1.05f
        };

        // Neon gradient: left cyan, center magenta, right blue
        public Color AppBarGradiantStartColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan
        public Color AppBarGradiantEndColor { get; set; } = Color.FromArgb(0, 102, 255);      // Neon Blue
        public Color AppBarGradiantMiddleColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon Magenta
        public LinearGradientMode AppBarGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
