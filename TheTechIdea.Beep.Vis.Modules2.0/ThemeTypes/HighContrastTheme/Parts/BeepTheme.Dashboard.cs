using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f, // Added for consistency
            LetterSpacing = 0.5f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };
        public TypographyStyle DashboardSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Regular,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray,
            LineHeight = 1.1f, // Added for consistency
            LetterSpacing = 0.3f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardBackColor { get; set; } = Color.Black;
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);

        public Color DashboardTitleForeColor { get; set; } = Color.White;
        public Color DashboardTitleBackColor { get; set; } = Color.Black; // Replaced Transparent with Black

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            LineHeight = 1.2f, // Added for consistency
            LetterSpacing = 0.5f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Black; // Replaced Transparent with Black

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.LightGray,
            LineHeight = 1.1f, // Added for consistency
            LetterSpacing = 0.3f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.Black;
        public Color DashboardGradiantEndColor { get; set; } = Color.DimGray;
        public Color DashboardGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}