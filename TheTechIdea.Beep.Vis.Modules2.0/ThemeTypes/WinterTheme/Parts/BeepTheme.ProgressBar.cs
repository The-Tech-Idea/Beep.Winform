using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(77, 182, 172);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}