using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color TreeForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(60, 100, 140);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(100, 149, 237);
    }
}