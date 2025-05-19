using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ProgressBar properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for progress fill
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for progress chunks
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral for error state
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for success state
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text inside progress bar
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover fill
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for hover text inside
    }
}