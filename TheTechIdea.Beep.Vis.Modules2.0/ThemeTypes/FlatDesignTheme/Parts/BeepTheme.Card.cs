using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Card Colors & Fonts
        public Font CardTitleFont { get; set; } = new Font(new FontFamily("Segoe UI"), 14, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Font CardSubTitleFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(100, 100, 100);
        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(33, 33, 33)
        };
        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(70, 70, 70)
        };
        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(120, 120, 120)
        };

        public Color CardrGradiantStartColor { get; set; } = Color.White;
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
