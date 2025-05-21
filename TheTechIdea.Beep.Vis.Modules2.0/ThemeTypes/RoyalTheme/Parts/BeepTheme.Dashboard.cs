using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 20,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(45, 45, 128);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}