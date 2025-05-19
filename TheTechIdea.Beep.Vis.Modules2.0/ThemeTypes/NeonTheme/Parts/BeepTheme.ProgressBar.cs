using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // ProgressBar properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for progress fill
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for progress chunks
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red for error state
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for success state
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for text inside progress bar
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover fill
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for hover text inside
    }
}