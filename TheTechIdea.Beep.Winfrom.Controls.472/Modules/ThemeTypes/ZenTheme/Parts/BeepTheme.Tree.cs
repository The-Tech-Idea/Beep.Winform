using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(34, 34, 34),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(189, 189, 189),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TreeBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color TreeForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(189, 189, 189);
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(76, 175, 80);
    }
}