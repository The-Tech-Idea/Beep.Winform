using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color TreeBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color TreeForeColor { get; set; } = Color.White;
        public Color TreeBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color TreeNodeForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.Black;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.Black;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(181, 137, 0);
    }
}