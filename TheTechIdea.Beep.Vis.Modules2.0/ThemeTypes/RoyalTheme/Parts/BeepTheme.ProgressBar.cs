using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(178, 34, 34); // Crimson
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(0, 128, 0); // Emerald
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(100, 100, 180); // Light indigo
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}