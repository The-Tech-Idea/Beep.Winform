using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color CardTextForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color CardBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color CardTitleForeColor { get; set; } = Color.White;
        public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}