using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // ProgressBar properties

        // Background and border
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy (track text)
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Progress chunk: candy pink
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(240, 100, 180);     // Candy Pink

        // Error: candy red
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 99, 132);      // Candy Red

        // Success: mint
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(127, 255, 212);   // Mint

        // Main font and inside text
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 8f, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;

        // Hover state: baby blue track, pink chunk, candy pink text
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
    }
}
