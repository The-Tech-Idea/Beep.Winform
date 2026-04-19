using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(45, 45, 48); // dark gray background
        public Color ProgressBarForeColor { get; set; } = Color.LightGray; // light gray foreground
        public Color ProgressBarBorderColor { get; set; } = Color.Gray;
        public Color ProgressBarChunkColor { get; set; } = Color.CornflowerBlue; // blue progress chunk
        public Color ProgressBarErrorColor { get; set; } = Color.Red;
        public Color ProgressBarSuccessColor { get; set; } = Color.LimeGreen;
        public TypographyStyle ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(60, 60, 65);
        public Color ProgressBarHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.LightBlue;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
