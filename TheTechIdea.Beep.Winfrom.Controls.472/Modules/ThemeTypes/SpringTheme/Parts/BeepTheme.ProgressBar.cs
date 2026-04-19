using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(60, 179, 113);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}