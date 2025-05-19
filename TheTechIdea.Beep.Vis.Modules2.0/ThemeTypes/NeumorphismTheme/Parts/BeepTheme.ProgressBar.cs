using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // ProgressBar properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(220, 220, 225); // Slightly darker gray for background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for progress fill
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for progress chunks
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red for error state
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for success state
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for text inside progress bar
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for hover
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover fill
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for hover text inside
    }
}