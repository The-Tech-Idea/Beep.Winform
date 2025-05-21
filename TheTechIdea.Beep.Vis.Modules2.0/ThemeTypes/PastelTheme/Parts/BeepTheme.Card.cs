using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Card Colors & Fonts
        public TypographyStyle CardTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color CardTextForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color CardBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle CardSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CardrGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}