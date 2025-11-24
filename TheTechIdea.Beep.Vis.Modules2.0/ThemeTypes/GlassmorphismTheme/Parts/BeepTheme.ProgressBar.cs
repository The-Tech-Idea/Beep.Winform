using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ProgressBarForeColor { get; set; } = Color.SteelBlue;
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color ProgressBarChunkColor { get; set; } = Color.DeepSkyBlue;
        public Color ProgressBarErrorColor { get; set; } = Color.Red;
        public Color ProgressBarSuccessColor { get; set; } = Color.Green;

        public TypographyStyle  ProgressBarFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;

        public Color ProgressBarHoverBackColor { get; set; } = Color.LightBlue;
        public Color ProgressBarHoverForeColor { get; set; } = Color.Black;
        public Color ProgressBarHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}
