using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ProgressBar properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for progress fill
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for progress chunks
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red for error state
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for success state
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text inside progress bar
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover fill
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for hover text inside
    }
}