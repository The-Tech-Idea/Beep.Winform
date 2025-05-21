using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Card Colors & Fonts
//<<<<<<< HEAD
        public Font CardTitleFont { get; set; } = new Font("Roboto", 18f, FontStyle.Bold);
        public Color CardTextForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color CardBackColor { get; set; } = Color.White;
        public Color CardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Font CardSubTitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Color CardSubTitleForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600

        public TypographyStyle CardHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33)
        };

        public TypographyStyle CardparagraphStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(66, 66, 66)
        };

        public TypographyStyle CardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117)
        };

        public Color CardrGradiantStartColor { get; set; } = Color.White;
        public Color CardGradiantEndColor { get; set; } = Color.FromArgb(245, 245, 245); // Grey 100
        public Color CardGradiantMiddleColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public LinearGradientMode CardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
