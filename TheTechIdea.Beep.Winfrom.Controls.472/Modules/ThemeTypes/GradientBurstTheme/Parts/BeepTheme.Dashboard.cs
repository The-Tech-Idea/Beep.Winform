using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Dashboard Colors & Fonts
        public TypographyStyle DashboardTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33), // Deep Gray
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
            TextColor = Color.FromArgb(66, 66, 66), // Slightly lighter gray
            LineHeight = 1.1f, // Added for consistency
            LetterSpacing = 0.3f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Light Gray
        public Color DashboardCardBackColor { get; set; } = Color.White;
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(245, 245, 255); // Soft Blue Tint

        public Color DashboardTitleForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Deep Gray
        public Color DashboardTitleBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Replaced Transparent with Light Gray

        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(33, 33, 33),
            LineHeight = 1.2f, // Added for consistency
            LetterSpacing = 0.5f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardSubTitleForeColor { get; set; } = Color.FromArgb(66, 66, 66);
        public Color DashboardSubTitleBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Replaced Transparent with Light Gray

        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(66, 66, 66),
            LineHeight = 1.1f, // Added for consistency
            LetterSpacing = 0.3f, // Added for consistency
            IsUnderlined = false, // Added for completeness
            IsStrikeout = false // Added for completeness
        };

        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(63, 81, 181); // Indigo
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(255, 87, 34); // Deep Orange
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(103, 58, 183); // Deep Purple
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}