using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Card Colors & Fonts
<<<<<<< HEAD
        public Font CardTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(220, 220, 240); // Light lavender text
        public Color CardBackColor { get; set; } = Color.FromArgb(25, 25, 50); // Deep space blue
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(180, 180, 255); // Light blue-purple
        public Font CardSubTitleFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(160, 160, 210); // Medium lavender
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(200, 200, 255)
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(220, 220, 240)
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 160, 210)
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(20, 20, 40); // Darker blue
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(35, 35, 70); // Lighter blue
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(28, 28, 55); // Mid-tone blue
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.ForwardDiagonal;
=======
        public TypographyStyle CardTitleFont { get; set; }
        public Color CardTextForeColor { get; set; }
        public Color CardBackColor { get; set; }
        public Color CardTitleForeColor { get; set; }
        public TypographyStyle CardSubTitleFont { get; set; }
        public Color CardSubTitleForeColor { get; set; }
        public TypographyStyle CardHeaderStyle { get; set; }
        public TypographyStyle CardparagraphStyle { get; set; }
        public TypographyStyle CardSubTitleStyle { get; set; }
        public Color CardrGradiantStartColor { get; set; }
        public Color CardGradiantEndColor { get; set; }
        public Color CardGradiantMiddleColor { get; set; }
        public LinearGradientMode CardGradiantDirection { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}