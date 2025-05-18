using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Card Colors & Fonts
        public Font CardTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.WhiteSmoke;
        public Color CardBackColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color CardTitleForeColor { get; set; } = Color.White;
        public Font CardSubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);
        public Color CardSubTitleForeColor { get; set; } = Color.LightGray;
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.White
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.WhiteSmoke
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.LightGray
        };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(40, 40, 40);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
