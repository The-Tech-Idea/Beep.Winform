using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // ProgressBar properties
        public Color ProgressBarBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color ProgressBarForeColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ProgressBarBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color ProgressBarChunkColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ProgressBarErrorColor { get; set; } = Color.FromArgb(204, 0, 0);
        public Color ProgressBarSuccessColor { get; set; } = Color.FromArgb(0, 128, 0);
        public TypographyStyle ProgressBarFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color ProgressBarInsideTextColor { get; set; } = Color.Black;
        public Color ProgressBarHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color ProgressBarHoverForeColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color ProgressBarHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public Color ProgressBarHoverInsideTextColor { get; set; } = Color.White;
    }
}