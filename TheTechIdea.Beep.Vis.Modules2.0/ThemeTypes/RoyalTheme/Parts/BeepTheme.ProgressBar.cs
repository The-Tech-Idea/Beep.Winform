using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(75, 192, 192);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color ProgressBarInsideTextColor { get; set; } = Color.White;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(255, 213, 27);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}