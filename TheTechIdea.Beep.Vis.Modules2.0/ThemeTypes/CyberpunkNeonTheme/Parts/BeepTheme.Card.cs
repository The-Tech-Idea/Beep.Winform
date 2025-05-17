using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Card Colors & Fonts

        public Font CardTitleFont { get; set; } = new Font("Consolas", 13f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(0, 255, 255);           // Neon Cyan
        public Color CardBackColor { get; set; } = Color.FromArgb(24, 24, 48);                // Cyberpunk Black

        public Color CardTitleForeColor { get; set; } = Color.FromArgb(255, 0, 255);          // Neon Magenta

        public Font CardSubTitleFont { get; set; } = new Font("Consolas", 11.5f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(0, 255, 128);       // Neon Green

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 0, 255), // Neon Magenta
            LetterSpacing = 1.3f
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(0, 255, 255), // Neon Cyan
            LineHeight = 1.07f
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 11.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(0, 255, 128), // Neon Green
            LineHeight = 1.05f
        };

        // Neon gradient: left magenta, center cyan, right blue
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(0, 102, 255);     // Neon Blue
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
