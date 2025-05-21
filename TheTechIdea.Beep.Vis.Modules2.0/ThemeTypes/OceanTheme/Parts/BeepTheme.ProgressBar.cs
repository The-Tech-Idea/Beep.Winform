using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(0, 200, 100);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(0, 200, 240);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}