using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(64, 255, 64);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(255, 192, 64);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}