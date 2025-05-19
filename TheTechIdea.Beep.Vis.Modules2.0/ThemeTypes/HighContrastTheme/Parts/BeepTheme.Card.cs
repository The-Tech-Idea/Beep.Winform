using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Card Colors & Fonts
<<<<<<< HEAD
        public Font CardTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.White;
        public Color CardBackColor { get; set; } = Color.Black;
        public Color CardTitleForeColor { get; set; } = Color.Yellow;
        public Font CardSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.LightGray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Bold,
            TextColor = Color.Yellow
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray
        };
        public Color CardrGradiantStartColor { get; set; } = Color.Black;
        public Color CardGradiantEndColor { get; set; } = Color.DarkSlateGray;
        public Color CardGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
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
