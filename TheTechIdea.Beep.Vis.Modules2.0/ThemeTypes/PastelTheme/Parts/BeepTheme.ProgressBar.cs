using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(180, 255, 180);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public Color ProgressBarInsideTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(255, 214, 229);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.FromArgb(80, 80, 80);
    }
}
