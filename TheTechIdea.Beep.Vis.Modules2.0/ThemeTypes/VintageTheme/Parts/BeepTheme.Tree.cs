using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
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
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TreeForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}