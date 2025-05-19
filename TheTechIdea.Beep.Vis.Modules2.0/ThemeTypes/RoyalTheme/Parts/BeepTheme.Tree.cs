using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
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
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color TreeForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(75, 192, 192);
    }
}