using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // ProgressBar properties

        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(34, 34, 68);         // Cyberpunk Panel
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(0, 255, 255);        // Neon Cyan (filled bar)

        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(255, 0, 255);      // Neon Magenta (border)
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 255, 128);       // Neon Green (chunk/highlight)

        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 0, 0);         // Bright Red (error)
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow (success)

        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;

        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);   // Neon Green (hover BG)
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow (hover fill)
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon Magenta (hover border)
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(0, 255, 255); // Neon Cyan
    }
}
