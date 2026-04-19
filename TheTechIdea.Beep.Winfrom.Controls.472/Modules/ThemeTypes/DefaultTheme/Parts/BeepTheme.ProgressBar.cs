using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(230, 230, 230); // light gray background
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(0, 120, 215);   // accent blue foreground
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // subtle border
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 120, 215);  // accent blue chunk
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(220, 53, 69);  // bootstrap red error
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(40, 167, 69); // bootstrap green success
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(210, 210, 210); // slightly darker hover
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(0, 100, 180); // darker blue on hover
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
