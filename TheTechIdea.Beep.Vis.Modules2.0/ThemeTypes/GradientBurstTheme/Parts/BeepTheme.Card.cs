using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public Font CardTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(25, 118, 210);

        public Font CardSubTitleFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(66, 66, 66);

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 118, 210),
            LineHeight = 1.3f,
            LetterSpacing = 0.5f
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33),
            LineHeight = 1.5f,
            LetterSpacing = 0.2f
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(66, 66, 66),
            LineHeight = 1.2f,
            LetterSpacing = 0.3f
        };

        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(240, 248, 255); // AliceBlue
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(225, 245, 254); // Light Blue
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(232, 240, 253);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
