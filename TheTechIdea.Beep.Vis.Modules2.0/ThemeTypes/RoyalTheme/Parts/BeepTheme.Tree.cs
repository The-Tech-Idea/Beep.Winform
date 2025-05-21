using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color TreeForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TreeBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
    }
}