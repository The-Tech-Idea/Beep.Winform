using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public Font CardTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Font CardSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.Gray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 30)
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            TextColor = Color.FromArgb(70, 70, 70)
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            TextColor = Color.Gray,
            FontWeight = FontWeight.Normal
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(250, 250, 250);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
